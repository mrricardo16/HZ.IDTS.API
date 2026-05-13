using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface ICacheService : IBaseService<tn_dts_cache>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_cache> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 调用递归算法实现获取一组父项编号的全部子类字典
        /// </summary>
        /// <param name="parentnameList"></param>
        /// <returns></returns>
        public List<DataDictionary> GetAllDict(List<DataDictionary> parentnameList);
    }
}
