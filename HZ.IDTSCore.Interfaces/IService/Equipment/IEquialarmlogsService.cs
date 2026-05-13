using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquialarmlogsService : IBaseService<tn_dts_equialarmlogs>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equialarmlogs> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除设备异常信息历史记录
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage Delete(string[] guidList);

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public ApiResult BatchAdd(List<tn_dts_equialarmlogs> listModel);
    }
}
