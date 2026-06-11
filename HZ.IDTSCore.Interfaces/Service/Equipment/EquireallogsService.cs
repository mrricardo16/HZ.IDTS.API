using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Equipment
{
    public class EquireallogsService : BaseService<tn_dts_equireallogs>, IEquireallogsService
    {
        public EquireallogsService(SessionInfo session) : base(session)
        {
                
        }

        #region 按cn_s_equireallogs_name和cn_s_equireallogs_timestamp分页模糊查询
        /// <summary>
        /// 按cn_s_equireallogs_name和cn_s_equireallogs_timestamp分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equireallogs> GetListPages(PageParm parm)
        {
            string cn_s_equireallogs_name = parm.Parms["cn_s_equireallogs_name"].ObjToString();
            string cn_s_equireallogs_starttime = parm.Parms["cn_s_equireallogs_starttime"].ObjToString();
            string cn_s_equireallogs_endtime = parm.Parms["cn_s_equireallogs_endtime"].ObjToString();
            return Db.Queryable<tn_dts_equireallogs>()
            .WhereIF(!String.IsNullOrEmpty(cn_s_equireallogs_name), it => it.cn_s_equireallogs_name == cn_s_equireallogs_name)
            .WhereIF((!String.IsNullOrEmpty(cn_s_equireallogs_starttime)) && (!String.IsNullOrEmpty(cn_s_equireallogs_endtime)), it => (it.cn_t_equireallogs_timestamp >= DateTime.Parse(cn_s_equireallogs_starttime)) && (it.cn_t_equireallogs_timestamp <= DateTime.Parse(cn_s_equireallogs_endtime)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_equireallogs_timestamp desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 批量删除实时采集记录
        /// <summary>
        /// 批量删除实时采集记录
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage Delete(string[] guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_equireallogs> reallogList = new List<tn_dts_equireallogs>();
            foreach (var guid in guidList)
            {
                tn_dts_equireallogs reallogGuid = Db.Queryable<tn_dts_equireallogs>().Where(it => it.cn_guid == guid).First();
                if(reallogGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的实时采集记录中有采集记录的唯一标识在数据库中不存在，删除失败！";
                    return returnMessage;
                }
                reallogList.Add(reallogGuid);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equireallogs>(reallogList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "9.2设备（Equireallogs）实时采集管理批量删除接口删除实时采集记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        #region 批量添加
        /// <summary>
        /// 批量增加
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ApiResult BatchAdd(List<tn_dts_equireallogs> listModel)
        {
            return UseTransactionV2(trans =>
            {
                trans.Insertable<tn_dts_equireallogs>(listModel).ExecuteCommand();
            });
        }
        #endregion

        #region 批量新增或更新采集最新值V2
        /// <summary>
        /// 批量新增或更新采集最新值V2。
        /// 时间：2026-06-11
        /// 优化内容：
        /// 1、把原来的历史明细新增改为“设备编号+采集事项+采集对象”维度的最新值Upsert，避免tn_dts_equireallogs单表持续膨胀到千万级。
        /// 2、每批最多500条拼成一次UPDATE+INSERT SQL，减少逐条循环更新造成的大量数据库往返。
        /// 3、PostgreSQL下使用事务级 advisory lock，避免高并发同时判断不存在后重复插入同一个采集点。
        /// 注意：如果历史表已存在重复采集点，需要先做一次历史重复数据清理，再加唯一索引，性能会更稳。
        /// </summary>
        /// <param name="listModel">采集日志集合</param>
        /// <returns>执行结果</returns>
        public ApiResult BatchUpsertLatestV2(List<tn_dts_equireallogs> listModel)
        {
            if (listModel == null || listModel.Count == 0)
            {
                return ApiResult.Success("无采集日志需要写入");
            }

            List<tn_dts_equireallogs> latestListV2 = listModel
                .Where(it => it != null)
                .GroupBy(it => new
                {
                    No = NormalizeKeyV2(it.cn_s_equireallogs_no),
                    ItemName = NormalizeKeyV2(it.cn_s_equireallogs_itemname),
                    ObjectName = NormalizeKeyV2(it.cn_s_equireallogs_objectname)
                })
                .Select(group => group
                    .OrderByDescending(it => it.cn_t_equireallogs_timestamp ?? DateTime.MinValue)
                    .First())
                .ToList();

            if (latestListV2.Count == 0)
            {
                return ApiResult.Success("无有效采集日志需要写入");
            }

            return UseTransaction(trans =>
            {
                if (trans.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                {
                    // 2026-06-11优化：没有唯一索引前，用事务级锁保护“先更新、再插入”的判断窗口，避免并发调用产生重复最新值。
                    trans.Ado.ExecuteCommand("SELECT pg_advisory_xact_lock(hashtext('tn_dts_equireallogs_latest_v2'));");
                }

                const int batchSizeV2 = 500;
                for (int i = 0; i < latestListV2.Count; i += batchSizeV2)
                {
                    List<tn_dts_equireallogs> batchListV2 = latestListV2.Skip(i).Take(batchSizeV2).ToList();
                    if (trans.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                    {
                        ExecutePostgreSqlLatestUpsertBatchV2(trans, batchListV2);
                    }
                    else
                    {
                        // 2026-06-11兼容：当前项目主库为PostgreSQL；其它数据库类型先走SqlSugar Saveable，避免新方法在非PG环境不可用。
                        trans.Storageable(batchListV2).ExecuteCommand();
                    }
                }
            });
        }

        /// <summary>
        /// PostgreSQL批量Upsert最新采集值。
        /// 时间：2026-06-11
        /// 优化内容：一批数据只执行一次批量UPDATE和一次批量INSERT，替代逐条查询/逐条更新。
        /// </summary>
        private static void ExecutePostgreSqlLatestUpsertBatchV2(SqlSugarClient trans, List<tn_dts_equireallogs> batchList)
        {
            if (batchList == null || batchList.Count == 0)
            {
                return;
            }

            StringBuilder valuesSqlV2 = new StringBuilder();
            List<SugarParameter> parametersV2 = new List<SugarParameter>();

            for (int i = 0; i < batchList.Count; i++)
            {
                tn_dts_equireallogs itemV2 = batchList[i];
                if (i > 0)
                {
                    valuesSqlV2.Append(",");
                }

                valuesSqlV2.Append("(")
                    .Append("CAST(@guid").Append(i).Append(" AS text),")
                    .Append("CAST(@no").Append(i).Append(" AS text),")
                    .Append("CAST(@name").Append(i).Append(" AS text),")
                    .Append("CAST(@item").Append(i).Append(" AS text),")
                    .Append("CAST(@object").Append(i).Append(" AS text),")
                    .Append("CAST(@val").Append(i).Append(" AS text),")
                    .Append("CAST(@unit").Append(i).Append(" AS text),")
                    .Append("CAST(@time").Append(i).Append(" AS timestamp)")
                    .Append(")");

                parametersV2.Add(new SugarParameter("@guid" + i, string.IsNullOrEmpty(itemV2.cn_guid) ? Guid.NewGuid().ToString() : itemV2.cn_guid));
                parametersV2.Add(new SugarParameter("@no" + i, NormalizeKeyV2(itemV2.cn_s_equireallogs_no)));
                parametersV2.Add(new SugarParameter("@name" + i, itemV2.cn_s_equireallogs_name ?? string.Empty));
                parametersV2.Add(new SugarParameter("@item" + i, NormalizeKeyV2(itemV2.cn_s_equireallogs_itemname)));
                parametersV2.Add(new SugarParameter("@object" + i, NormalizeKeyV2(itemV2.cn_s_equireallogs_objectname)));
                parametersV2.Add(new SugarParameter("@val" + i, itemV2.cn_s_equireallogs_objectval ?? string.Empty));
                parametersV2.Add(new SugarParameter("@unit" + i, itemV2.cn_s_equireallogs_objectvalunit ?? string.Empty));
                parametersV2.Add(new SugarParameter("@time" + i, itemV2.cn_t_equireallogs_timestamp ?? DateTime.Now));
            }

            string inputSqlV2 = @"
WITH input(
    cn_guid,
    cn_s_equireallogs_no,
    cn_s_equireallogs_name,
    cn_s_equireallogs_itemname,
    cn_s_equireallogs_objectname,
    cn_s_equireallogs_objectval,
    cn_s_equireallogs_objectvalunit,
    cn_t_equireallogs_timestamp
) AS (VALUES " + valuesSqlV2 + @")";

            string sqlV2 = inputSqlV2 + @"
UPDATE tn_dts_equireallogs AS target
SET
    cn_s_equireallogs_name = input.cn_s_equireallogs_name,
    cn_s_equireallogs_objectval = input.cn_s_equireallogs_objectval,
    cn_s_equireallogs_objectvalunit = input.cn_s_equireallogs_objectvalunit,
    cn_t_equireallogs_timestamp = input.cn_t_equireallogs_timestamp
FROM input
WHERE target.cn_s_equireallogs_no = input.cn_s_equireallogs_no
    AND target.cn_s_equireallogs_itemname = input.cn_s_equireallogs_itemname
    AND target.cn_s_equireallogs_objectname = input.cn_s_equireallogs_objectname;
"
            + inputSqlV2 + @"
INSERT INTO tn_dts_equireallogs(
    cn_guid,
    cn_s_equireallogs_no,
    cn_s_equireallogs_name,
    cn_s_equireallogs_itemname,
    cn_s_equireallogs_objectname,
    cn_s_equireallogs_objectval,
    cn_s_equireallogs_objectvalunit,
    cn_t_equireallogs_timestamp
)
SELECT
    input.cn_guid,
    input.cn_s_equireallogs_no,
    input.cn_s_equireallogs_name,
    input.cn_s_equireallogs_itemname,
    input.cn_s_equireallogs_objectname,
    input.cn_s_equireallogs_objectval,
    input.cn_s_equireallogs_objectvalunit,
    input.cn_t_equireallogs_timestamp
FROM input
WHERE NOT EXISTS (
    SELECT 1
    FROM tn_dts_equireallogs AS target
    WHERE target.cn_s_equireallogs_no = input.cn_s_equireallogs_no
        AND target.cn_s_equireallogs_itemname = input.cn_s_equireallogs_itemname
        AND target.cn_s_equireallogs_objectname = input.cn_s_equireallogs_objectname
);";

            trans.Ado.ExecuteCommand(sqlV2, parametersV2.ToArray());
        }

        /// <summary>
        /// 采集最新值的业务键归一化。
        /// </summary>
        private static string NormalizeKeyV2(string value)
        {
            return value ?? string.Empty;
        }
        #endregion
    }
}
