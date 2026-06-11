using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquireallogsService : IBaseService<tn_dts_equireallogs>
    {
        /// <summary>
        /// 按cn_s_equireallogs_name和cn_s_equireallogs_timestamp分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equireallogs> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除实时采集记录
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage Delete(string[] guidList);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public ApiResult BatchAdd(List<tn_dts_equireallogs> listModel);

        /// <summary>
        /// 批量新增或更新采集最新值V2。
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public ApiResult BatchUpsertLatestV2(List<tn_dts_equireallogs> listModel);
    }
}
