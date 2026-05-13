using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface ISiteinfoService : IBaseService<tn_dts_siteinfo>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_siteinfo> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 新增地堆
        /// </summary>
        /// <param name="locationSite">同步缓存到MongoDB中的LocationSiteInformation表中的MDG站点信息</param>
        /// <returns></returns>
        public ReturnMessage AddSiteinfo(LocationSiteInformation locationSite);

        /// <summary>
        /// 修改地堆
        /// </summary>
        /// <param name="siteinfo"></param>
        /// <returns></returns>
        public ReturnMessage UpdateSiteinfo(tn_dts_siteinfo siteinfo);

        /// <summary>
        /// 删除地堆
        /// </summary>
        /// <param name="siteinfoList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteSiteinfo(List<tn_dts_siteinfo> siteinfoList);

        /// <summary>
        /// 批量设置地堆
        /// </summary>
        /// <param name="siteinfoList"></param>
        /// <returns></returns>
        public ReturnMessage BatchsettingSiteinfo(List<tn_dts_siteinfo> siteinfoList);
    }
}
