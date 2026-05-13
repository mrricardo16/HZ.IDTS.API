using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Simulate;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Simulate
{
    public interface IAgvTimeTrackingService : IBaseService<tn_dts_agvtimetracking>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_agvtimetracking> GetListPages(PageParm parm);
    }
}
