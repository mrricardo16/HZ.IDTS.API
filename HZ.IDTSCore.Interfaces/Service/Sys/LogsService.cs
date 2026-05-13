using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class LogsService : BaseService<tn_dts_logs>, ILogsService
    {
        public LogsService(SessionInfo session) : base(session)
        {

        }

        #region 按cn_s_logs_type和cn_t_create分页模糊查询
        /// <summary>
        /// 按cn_s_logs_type和cn_t_create分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_logs> GetListPages(PageParm parm)
        {
            string cn_s_logs_type = parm.Parms["cn_s_logs_type"].ObjToString();
            string cn_s_create_starttime = parm.Parms["cn_s_create_starttime"].ObjToString();
            string cn_s_create_endtime = parm.Parms["cn_s_create_endtime"].ObjToString();
            return Db.Queryable<tn_dts_logs>().WhereIF(!string.IsNullOrEmpty(cn_s_logs_type), (s => s.cn_s_logs_type.Contains(cn_s_logs_type)))
            .WhereIF((!String.IsNullOrEmpty(cn_s_create_starttime)) && (!String.IsNullOrEmpty(cn_s_create_endtime)), it => (it.cn_t_create >= DateTime.Parse(cn_s_create_starttime)) && (it.cn_t_create <= DateTime.Parse(cn_s_create_endtime)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 批量删除日志
        /// <summary>
        /// 批量删除日志
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ReturnMessage Delete(string[] guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var guid in guidList)
            {
                tn_dts_logs logGuid = Db.Queryable<tn_dts_logs>().Where(it => it.cn_guid == guid).First();
                if (logGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的日志中有日志的唯一标识在数据库中不存在！";
                    return returnMessage;
                }
                logList.Add(logGuid);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "8.2日志（Logs）管理批量删除接口删除日志记录失败，详细信息为：" + res.Message);
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
        public ApiResult BatchAdd(List<tn_dts_logs> listModel)
        {
            return UseTransaction(trans =>
            {
                trans.Insertable<tn_dts_logs>(listModel).ExecuteCommand();
            });
        }
        #endregion
    }
}
