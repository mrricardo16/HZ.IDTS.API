using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Simulate;
using HZ.IDTSCore.Model.Entity.Simulate;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Simulate
{
    public class AgvTaskTrackingService : BaseService<tn_dts_agvtasktracking>, IAgvTaskTrackingService
    {
        public AgvTaskTrackingService(SessionInfo session) : base(session)
        {

        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_agvtasktracking> GetListPages(PageParm parm)
        {
            string taskNo = parm.Parms["taskNo"].ObjToString();
            string starttime = parm.Parms["starttime"].ObjToString();
            string endtime = parm.Parms["endtime"].ObjToString();
            return Db.Queryable<tn_dts_agvtasktracking>()
            .WhereIF(!String.IsNullOrEmpty(taskNo), it => it.cn_t_tracking_taskno == taskNo)
            .WhereIF((!String.IsNullOrEmpty(starttime)) && (!String.IsNullOrEmpty(endtime)),
            it => (it.cn_t_tracking_timestamp >= DateTime.Parse(starttime)) && (it.cn_t_tracking_timestamp <= DateTime.Parse(endtime)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_tracking_timestamp asc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
    }
}
