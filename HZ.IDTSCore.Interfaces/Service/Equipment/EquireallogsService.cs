using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
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
            return UseTransaction(trans =>
            {
                trans.Insertable<tn_dts_equireallogs>(listModel).ExecuteCommand();
            });
        }
        #endregion
    }
}
