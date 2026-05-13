using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 业务流程设备子表
    /// <summary>
    /// 业务流程设备子表
    /// </summary>
    [SugarTable("tn_dts_chiprocedure")]
    public class tn_dts_chiprocedure
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 业务流程主表唯一标识
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_parproguid")]
        public string cn_s_chiprocedure_parproguid { get; set; }
        /// <summary>
        ///  设备档案唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_equiguid")]
        public string cn_s_chiprocedure_equiguid { get; set; }
        /// <summary>
        ///  起点唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_startguid")]
        public string cn_s_chiprocedure_startguid { get; set; }
        /// <summary>
        ///  起点类别（地堆/立库）
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_startcategory")]
        public string cn_s_chiprocedure_startcategory { get; set; }
        /// <summary>
        ///  起点排列层
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_startrcl")]
        public string cn_s_chiprocedure_startrcl { get; set; }
        /// <summary>
        ///  终点唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_endguid")]
        public string cn_s_chiprocedure_endguid { get; set; }
        /// <summary>
        ///  终点类别（地堆/立库）
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_endcategory")]
        public string cn_s_chiprocedure_endcategory { get; set; }
        /// <summary>
        ///  终点排列层
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_endrcl")]
        public string cn_s_chiprocedure_endrcl { get; set; }
        /// <summary>
        ///  设备次序
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_chiprocedure_sequence")]
        public int cn_n_chiprocedure_sequence { get; set; }
        /// <summary>
        ///  设备间隔
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_chiprocedure_interval")]
        public int cn_n_chiprocedure_interval { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_chiprocedure_remarks")]
        public string cn_s_chiprocedure_remarks { get; set; }
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

    #region 流程子表信息（包含设备信息、起点信息和终点信息）
    /// <summary>
    /// 流程子表信息（包含设备信息、起点信息和终点信息）
    /// </summary>
    public class ChiprocedureInformation
    {
        /// <summary>
        /// 流程-设备关系唯一标识
        /// </summary>
        public string ChiprocedureGuid { get; set; }
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        public string EquipmentGuid { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentNo { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
        /// <summary>
        /// 起点唯一标识
        /// </summary>
        public string StartGuid { get; set; }
        /// <summary>
        /// 起点类型
        /// </summary>
        public string StartType { get; set; }
        /// <summary>
        /// 起点编码
        /// </summary>
        public string StartNo { get; set; }
        /// <summary>
        /// 起点名称
        /// </summary>
        public string StartName { get; set; }
        /// <summary>
        /// 终点唯一标识
        /// </summary>
        public string EndGuid { get; set; }
        /// <summary>
        /// 终点类型
        /// </summary>
        public string EndType { get; set; }
        /// <summary>
        /// 终点编码
        /// </summary>
        public string EndNo { get; set; }
        /// <summary>
        /// 终点名称
        /// </summary>
        public string EndName { get; set; }
        /// <summary>
        /// 设备次序
        /// </summary>
        public int ChiprocedureSequence { get; set; }
        /// <summary>
        /// 设备间隔/ms
        /// </summary>
        public int ChiprocedureInterval { get; set; }
    }
    #endregion

    #region 流程子表信息（包含设备信息、起点信息、终点信息和修改创建信息）
    /// <summary>
    /// 流程子表信息（包含设备信息、起点信息、终点信息和修改创建信息）
    /// </summary>
    public class ChiprocedureInformationPlus
    {
        /// <summary>
        /// 流程-设备关系唯一标识
        /// </summary>
        public string ChiprocedureGuid { get; set; }
        /// <summary>
        /// 设备唯一标识
        /// </summary>
        public string EquipmentGuid { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string EquipmentNo { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string EquipmentName { get; set; }
        /// <summary>
        /// 起点编码
        /// </summary>
        public string StartNo { get; set; }
        /// <summary>
        /// 起点名称
        /// </summary>
        public string StartName { get; set; }
        /// <summary>
        /// 终点编码
        /// </summary>
        public string EndNo { get; set; }
        /// <summary>
        /// 终点名称
        /// </summary>
        public string EndName { get; set; }
        /// <summary>
        /// 设备次序
        /// </summary>
        public int ChiprocedureSequence { get; set; }
        /// <summary>
        /// 设备间隔/ms
        /// </summary>
        public int ChiprocedureInterval { get; set; }
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
