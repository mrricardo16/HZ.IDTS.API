using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquiObjectService : IBaseService<tn_dts_equiobject>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equiobject> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 获取对象树
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<ObjectTree> GetObjectTree();

        /// <summary>
        /// 新增对象
        /// </summary>
        /// <param name="equiobject"></param>
        /// <returns></returns>
        public ReturnMessage AddObject(tn_dts_equiobject equiobject);

        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="equiobject"></param>
        /// <returns></returns>
        public ReturnMessage UpdateObject(tn_dts_equiobject equiobject);

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="equiobject"></param>
        /// <returns></returns>
        public ReturnMessage DeleteObject(tn_dts_equiobject equiobject);

        /// <summary>
        /// 粘贴对象接口
        /// </summary>
        /// <param name="pasteObject"></param>
        /// <returns></returns>
        public ReturnMessage PasteObject(PasteObject pasteObject);
    }
}
