using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备表
    /// <summary>
    /// 设备表
    ///</summary>
    [SugarTable("tn_dts_equipment")]
    public class tn_dts_equipment
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 部件类型
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equi_parttype")]
        public string cn_s_equi_parttype { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_no")]
        public string cn_s_equi_no { get; set; }
        /// <summary>
        ///  设备名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_name")]
        public string cn_s_equi_name { get; set; }
        /// <summary>
        ///  设备类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_type")]
        public string cn_s_equi_type { get; set; }
        /// <summary>
        ///  设备型号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_model")]
        public string cn_s_equi_model { get; set; }
        /// <summary>
        ///  设备状态
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_status")]
        public string cn_s_equi_status { get; set; }
        /// <summary>
        ///  购买日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_buydate")]
        public DateTime? cn_s_equi_buydate { get; set; }
        /// <summary>
        ///  质保日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_qadate")]
        public DateTime? cn_s_equi_qadate { get; set; }
        /// <summary>
        ///  首保日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_firstdate")]
        public DateTime? cn_s_equi_firstdate { get; set; }
        /// <summary>
        ///  保养周期
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_defentperiod")]
        public int? cn_s_equi_defentperiod { get; set; }
        /// <summary>
        ///  所属部门
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_dept")]
        public string cn_s_equi_dept { get; set; }
        /// <summary>
        ///  设备负责人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_dutyman")]
        public string cn_s_equi_dutyman { get; set; }
        /// <summary>
        ///  设备负责人电话
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_dutyphone")]
        public string cn_s_equi_dutyphone { get; set; }
        /// <summary>
        ///  合同编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_contractno")]
        public string cn_s_equi_contractno { get; set; }
        /// <summary>
        ///  所属产线
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_beltline")]
        public string cn_s_equi_beltline { get; set; }
        /// <summary>
        ///  x坐标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_xpos")]
        public decimal? cn_s_equi_xpos { get; set; }
        /// <summary>
        ///  y坐标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_ypos")]
        public decimal? cn_s_equi_ypos { get; set; }
        /// <summary>
        /// z坐标
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equi_zpos")]
        public decimal? cn_s_equi_zpos { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equi_remarks")]
        public string cn_s_equi_remarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify_by")]
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  创建人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator")]
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  创建人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator_by")]
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
    }
    #endregion

    #region 设备树（设备整机和零部件关系）数据类
    /// <summary>
    /// 设备树（设备整机和零部件关系）数据类
    /// </summary>
    public class CompletemachineTree
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        public string cn_guid { get; set; }
        /// <summary>
        /// 部件类型
        /// </summary>
        public string cn_s_equi_parttype { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        public string cn_s_equi_no { get; set; }
        /// <summary>
        ///  设备名称
        ///</summary>
        public string cn_s_equi_name { get; set; }
        /// <summary>
        ///  设备类型
        ///</summary>
        public string cn_s_equi_type { get; set; }
        /// <summary>
        ///  设备型号
        ///</summary>
        public string cn_s_equi_model { get; set; }
        /// <summary>
        ///  设备状态
        ///</summary>
        public string cn_s_equi_status { get; set; }
        /// <summary>
        ///  购买日期
        ///</summary>
        public DateTime? cn_s_equi_buydate { get; set; }
        /// <summary>
        ///  质保日期
        ///</summary>
        public DateTime? cn_s_equi_qadate { get; set; }
        /// <summary>
        ///  首保日期
        ///</summary>
        public DateTime? cn_s_equi_firstdate { get; set; }
        /// <summary>
        ///  保养周期
        ///</summary>
        public int? cn_s_equi_defentperiod { get; set; }
        /// <summary>
        ///  所属部门
        ///</summary>
        public string cn_s_equi_dept { get; set; }
        /// <summary>
        ///  设备负责人
        ///</summary>
        public string cn_s_equi_dutyman { get; set; }
        /// <summary>
        ///  设备负责人电话
        ///</summary>
        public string cn_s_equi_dutyphone { get; set; }
        /// <summary>
        ///  所属合同编号
        ///</summary>
        public string cn_s_equi_contractno { get; set; }
        /// <summary>
        ///  所属产线
        ///</summary>
        public string cn_s_equi_beltline { get; set; }
        /// <summary>
        ///  X坐标
        ///</summary>
        public decimal? cn_s_equi_xpos { get; set; }
        /// <summary>
        ///  Y坐标
        ///</summary>
        public decimal? cn_s_equi_ypos { get; set; }
        /// <summary>
        /// Z坐标
        /// </summary>
        public decimal? cn_s_equi_zpos { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        public string cn_s_equi_remarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  创建人
        ///</summary>
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  创建人名称
        ///</summary>
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        public DateTime? cn_t_create { get; set; }
        /// <summary>
        /// 子项列表
        /// </summary>
        public List<CompletemachineTree> children { get; set; }
    }
    #endregion

    #region 设备档案保存数据类
    /// <summary>
    /// 设备档案保存数据类
    /// </summary>
    public class SaveData
    {
        /// <summary>
        /// 新增修改
        /// </summary>
        public string add_or_modify { get; set; }
        /// <summary>
        /// 整机零部件
        /// </summary>
        public string completemachine_and_part { get; set; }
        /// <summary>
        /// 父项整机/零部件（仅新增零部件这项有值）
        /// </summary>
        public tn_dts_equipment parent_equipment { get; set; }
        /// <summary>
        /// 子项零部件
        /// </summary>
        public tn_dts_equipment child_equipment { get; set; }
    }
    #endregion

    #region 通用返回消息类
    /// <summary>
    /// 通用返回消息类
    /// </summary>
    public class ReturnMessage
    {
        /// <summary>
        /// 是否执行成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }
    #endregion

    #region 设备类型树数据类
    /// <summary>
    /// 设备类型树数据类
    /// </summary>
    public class EquipmentTypeTree
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        public string EquiType { get; set; }
        /// <summary>
        /// 设备信息列表
        /// </summary>
        public List<EquipmentInformation> EquipmentInformationList { get; set; }
    }
    #endregion

    #region 设备信息
    /// <summary>
    /// 设备信息
    /// </summary>
    public class EquipmentInformation
    {
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        public string EquiGuid { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string EquiNo { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquiName { get; set; }
    }
    #endregion

}
