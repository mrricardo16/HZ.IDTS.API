using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace HZ.IDTSCore.Interfaces.IService.Equipment
{
    public interface IEquipmentService : IBaseService<tn_dts_equipment>
    {
        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        PagedInfo<tn_dts_equipment> GetListPages(PageParm parm);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid);

        /// <summary>
        /// 获取列表数据（树状展示，按设备编号和设备名称混合模糊）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<CompletemachineTree> GetCompletemachineTreeList(PageParm param);

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        public ReturnMessage SaveData(SaveData saveData);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public ReturnMessage DeleteData(tn_dts_equipment equipment);

        /// <summary>
        /// 复制整机
        /// </summary>
        /// <param name="completemachineTree"></param>
        /// <param name="needCopyComponents"></param>
        /// <returns></returns>
        public ApiResult CopyEquipment(CompletemachineTree completemachineTree, bool needCopyComponents);

        /// <summary>
        /// 获取设备类型-设备树信息（按设备编号和设备名称混合模糊）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<EquipmentTypeTree> GetEquipmentTypeTree(PageParm param);

        /// <summary>
        /// 获取一个设备类型下所有设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<EquipmentInformation> GetEquipmentInformationByEquitype(PageParm param);

        /// <summary>
        /// 按设备类型、设备编码和设备名称混合模糊分页查询设备信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<EquipmentInformation> GetEquipment(PageParm parm);
    }
}
