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
    public class EquialarmlogsService : BaseService<tn_dts_equialarmlogs>, IEquialarmlogsService
    {
        public EquialarmlogsService(SessionInfo session) : base(session)
        {

        }

        #region 按cn_s_equialarmlogs_name、cn_s_equialarmlogs_errcode、cn_s_equialarmlogs_errmsg、cn_t_equialarmlogs_timestamp分页模糊查询
        /// <summary>
        /// 按cn_s_equialarmlogs_name、cn_s_equialarmlogs_errcode、cn_s_equialarmlogs_errmsg、cn_t_equialarmlogs_timestamp分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equialarmlogs> GetListPages(PageParm parm)
        {
            string cn_s_equialarmlogs_name = parm.Parms["cn_s_equialarmlogs_name"].ObjToString();
            string cn_s_equialarmlogs_errcode = parm.Parms["cn_s_equialarmlogs_errcode"].ObjToString();
            string cn_s_equialarmlogs_errmsg = parm.Parms["cn_s_equialarmlogs_errmsg"].ObjToString();
            string cn_s_equialarmlogs_starttime = parm.Parms["cn_s_equialarmlogs_starttime"].ObjToString();
            string cn_s_equialarmlogs_endtime = parm.Parms["cn_s_equialarmlogs_endtime"].ObjToString();
            return Db.Queryable<tn_dts_equialarmlogs>()
            .WhereIF(!String.IsNullOrEmpty(cn_s_equialarmlogs_name), it => it.cn_s_equialarmlogs_name == cn_s_equialarmlogs_name)
            .WhereIF(!String.IsNullOrEmpty(cn_s_equialarmlogs_errcode), it => it.cn_s_equialarmlogs_errcode == cn_s_equialarmlogs_errcode)
            .WhereIF(!String.IsNullOrEmpty(cn_s_equialarmlogs_errmsg), it => it.cn_s_equialarmlogs_errmsg == cn_s_equialarmlogs_errmsg)
            .WhereIF((!String.IsNullOrEmpty(cn_s_equialarmlogs_starttime)) && (!String.IsNullOrEmpty(cn_s_equialarmlogs_endtime)),
            it => (it.cn_t_equialarmlogs_timestamp >= DateTime.Parse(cn_s_equialarmlogs_starttime)) && (it.cn_t_equialarmlogs_timestamp <= DateTime.Parse(cn_s_equialarmlogs_endtime)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_equialarmlogs_timestamp desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 批量删除设备异常信息历史记录
        /// <summary>
        /// 批量删除设备异常信息历史记录
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage Delete(string[] guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_equialarmlogs> alarmlogList = new List<tn_dts_equialarmlogs>();
            foreach (var guid in guidList)
            {
                tn_dts_equialarmlogs alarmlogGuid = Db.Queryable<tn_dts_equialarmlogs>().Where(it => it.cn_guid == guid).First();
                if(alarmlogGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的异常信息记录中有异常记录的唯一标识在数据库中不存在，删除失败！";
                    return returnMessage;
                }
                alarmlogList.Add(alarmlogGuid);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equialarmlogs>(alarmlogList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "10.2设备（Equialarmlogs）异常信息管理批量删除接口删除异常信息记录失败，详细信息为：" + res.Message);
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
        public ApiResult BatchAdd(List<tn_dts_equialarmlogs> listModel)
        {
            return UseTransaction(trans =>
            {
                trans.Insertable<tn_dts_equialarmlogs>(listModel).ExecuteCommand();
            });
        }
        #endregion
    }
}
