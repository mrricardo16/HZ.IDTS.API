using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface ILogsService : IBaseService<tn_dts_logs>
    {
        /// <summary>
        /// 按cn_s_logs_type和cn_t_create分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_logs> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除日志
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ReturnMessage Delete(string[] guidList);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public ApiResult BatchAdd(List<tn_dts_logs> listModel);
    }
}
