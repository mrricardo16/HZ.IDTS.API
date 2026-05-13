using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 业务流程设备指令子表
    /// <summary>
    /// 业务流程设备指令子表
    /// </summary>
    [SugarTable("tn_dts_chiequipment")]
    public class tn_dts_chiequipment
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 设备档案唯一标识
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_chiequipment_equiguid")]
        public string cn_s_chiequipment_equiguid { get; set; }
        /// <summary>
        ///  指令唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiequipment_comguid")]
        public string cn_s_chiequipment_comguid { get; set; }
        /// <summary>
        ///  指令次序
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_chiequipment_sequence")]
        public int cn_n_chiequipment_sequence { get; set; }
        /// <summary>
        ///  指令间隔
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_chiequipment_interval")]
        public int cn_n_chiequipment_interval { get; set; }
        /// <summary>
        ///  指令类别(设备/货位）
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiequipment_category")]
        public string cn_s_chiequipment_category { get; set; }
        /// <summary>
        /// 指令类型（设备指令管理/货位指令管理/流程设备管理）
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_chiequipment_type")]
        public string cn_s_chiequipment_type { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiequipment_remarks")]
        public string cn_s_chiequipment_remarks { get; set; }
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

    #region 设备子表信息（包括指令信息）
    /// <summary>
    /// 设备子表信息（包括指令信息）
    /// </summary>
    public class ChiequipmentInformation
    {
        /// <summary>
        /// 设备-指令关系唯一标识
        /// </summary>
        public string ChiequipmentGuid { get; set; }
        /// <summary>
        /// 指令唯一标识
        /// </summary>
        public string CommandGuid { get; set; }
        /// <summary>
        /// 指令编码
        /// </summary>
        public string CommandNo { get; set; }
        /// <summary>
        /// 指令名称
        /// </summary>
        public string CommandName { get; set; }
        /// <summary>
        /// 指令类别(设备/货位）
        /// </summary>
        public string ChiequipmentCategory { get; set; }
        /// <summary>
        /// 指令次序
        /// </summary>
        public int ChiequipmentSequence { get; set; }
        /// <summary>
        /// 指令间隔/ms
        /// </summary>
        public int ChiequipmentInterval { get; set; }
    }
    #endregion

    #region 保存设备数据类
    /// <summary>
    /// 保存设备数据类
    /// </summary>
    public class SaveEquipmentDate
    {
        ///// <summary>
        ///// 新增修改标识
        ///// </summary>
        //public string AddOrModify { get; set; }
        /// <summary>
        /// 删除设备-指令关系唯一标识列表
        /// </summary>
        public List<string> DeleteChiequipmentGuidList { get; set; }
        ///// <summary>
        ///// 设备档案唯一标识
        ///// </summary>
        //public string EquipmentGuid { get; set; }
        /// <summary>
        /// 流程子表唯一标识
        /// </summary>
        public string ChiprocedureGuid { get; set; }
        /// <summary>
        /// 设备所包含指令信息列表
        /// </summary>
        public List<ChiequipmentInformation> ChiequipmentInformationList { get; set; }
    }
    #endregion

    #region 设备子表信息（包括指令信息和修改创建信息）
    /// <summary>
    /// 设备子表信息（包括指令信息和修改创建信息）
    /// </summary>
    public class ChiequipmentInformationPlus
    {
        /// <summary>
        /// 设备-指令关系唯一标识
        /// </summary>
        public string ChiequipmentGuid { get; set; }
        /// <summary>
        /// 指令唯一标识
        /// </summary>
        public string CommandGuid { get; set; }
        /// <summary>
        /// 指令编码
        /// </summary>
        public string CommandNo { get; set; }
        /// <summary>
        /// 指令名称
        /// </summary>
        public string CommandName { get; set; }
        /// <summary>
        /// 指令类别(设备/货位）
        /// </summary>
        public string ChiequipmentCategory { get; set; }
        /// <summary>
        /// 指令Json
        /// </summary>
        public string CommandJson { get; set; }
        /// <summary>
        /// 有无通配符
        /// </summary>
        public int HasWildcard { get; set; }
        /// <summary>
        /// 指令次序
        /// </summary>
        public int ChiequipmentSequence { get; set; }
        /// <summary>
        /// 指令间隔/ms
        /// </summary>
        public int ChiequipmentInterval { get; set; }
        /// <summary>
        /// 修改人编码
        /// </summary>
        public string ModifyNo { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        public string ModifyName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        /// <summary>
        /// 创建人编码
        /// </summary>
        public string CreateNo { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
    #endregion
}
