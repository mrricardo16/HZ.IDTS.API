using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquiObjectAttrService : IBaseService<tn_dts_equiobjectattr>
    {
        /// <summary>
        /// 按对象名、属性名（模糊）分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equiobjectattr> GetListPages(PageParm parm);

        /// <summary>
        /// 新增对象属性
        /// </summary>
        /// <param name="equiobjectattr"></param>
        /// <returns></returns>
        public ReturnMessage AddObjectAttr(tn_dts_equiobjectattr equiobjectattr);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 修改对象属性
        /// </summary>
        /// <param name="equiobjectattr"></param>
        /// <returns></returns>
        public ReturnMessage UpdateObjectAttr(tn_dts_equiobjectattr equiobjectattr);

        /// <summary>
        /// 批量删除对象属性
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteObjectAttr(List<string> guidList);
    }
}
