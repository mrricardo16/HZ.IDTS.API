using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 设备指令表
    /// <summary>
    /// 设备指令表
    /// </summary>
    [SugarTable("tn_dts_equicommand")]
    public class tn_dts_equicommand
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 指令编码
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equicommand_no")]
        public string cn_s_equicommand_no { get; set; }
        /// <summary>
        ///  指令名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equicommand_name")]
        public string cn_s_equicommand_name { get; set; }
        /// <summary>
        ///  指令类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equicommand_type")]
        public string cn_s_equicommand_type { get; set; }
        /// <summary>
        ///  有无通配符
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_equicommand_haswildcard")]
        public int cn_n_equicommand_haswildcard { get; set; }
        /// <summary>
        ///  指令Json
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equicommand_json")]
        public string cn_s_equicommand_json { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equicommand_remarks")]
        public string cn_s_equicommand_remarks { get; set; }
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

    #region 批量添加设备指令数据类
    /// <summary>
    /// 批量添加设备指令数据类
    /// </summary>
    public class BatchAddEquicommandDate
    {
        /// <summary>
        /// 用户已选设备唯一标识数组
        /// </summary>
        public List<string> SelectiveEquipmentGuidList { get; set; }
        ///// <summary>
        ///// 指令编码
        ///// </summary>
        //public string EquicommandNo { get; set; }
        /// <summary>
        /// 指令名称
        /// </summary>
        public string EquicommandName { get; set; }
        /// <summary>
        /// 指令类型
        /// </summary>
        public string EquicommandType { get; set; }
        /// <summary>
        /// 有无通配符
        /// </summary>
        public int Haswildcard { get; set; }
        /// <summary>
        /// 指令Json
        /// </summary>
        public string Json { get; set; }
    }
    #endregion

    #region 编辑设备指令JSON模版数据类
    /// <summary>
    /// 编辑设备指令JSON模版数据类
    /// </summary>
    public class EditEquicommandDate
    {
        /// <summary>
        /// 设备指令唯一标识
        /// </summary>
        public string EquicommandGuid { get; set; }
        /// <summary>
        /// 有无通配符（0：无；1：有）
        /// </summary>
        public int Haswildcard { get; set; }
        /// <summary>
        /// 指令Json
        /// </summary>
        public string Json { get; set; }
    }
    #endregion

    #region 设备指令信息
    /// <summary>
    /// 设备指令信息
    /// </summary>
    public class EquicommandInformation
    {
        /// <summary>
        /// 设备指令唯一标识
        /// </summary>
        public string EquicommandGuid { get; set; }
        /// <summary>
        /// 设备指令编码
        /// </summary>
        public string EquicommandNo { get; set; }
        /// <summary>
        /// 设备指令名称
        /// </summary>
        public string EquicommandName { get; set; }
    }
    #endregion

    #region 复制设备指令数据类
    /// <summary>
    /// 复制设备指令数据类
    /// </summary>
    public class CopyEquicommandDate
    {
        /// <summary>
        /// 复制的指令唯一标识数组
        /// </summary>
        public List<string> EquicommandGuidList { get; set; }
        /// <summary>
        /// 粘贴的设备唯一标识数组
        /// </summary>
        public List<string> EquipmentGuidList { get; set; }
    }
    #endregion

    /// <summary>
    /// 设备指令信息(带设备编码）
    /// </summary> 
    public class EquicommandPlus
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        public string cn_guid { get; set; }
        /// <summary>
        /// 设备编码
        /// </summary>
        public string cn_s_equicommand_equipmentno { get; set; }
        /// <summary>
        /// 指令编码
        /// </summary>
        public string cn_s_equicommand_no { get; set; }
        /// <summary>
        ///  指令名称
        ///</summary>
        public string cn_s_equicommand_name { get; set; }
        /// <summary>
        ///  指令类型
        ///</summary>
        public string cn_s_equicommand_type { get; set; }
        /// <summary>
        ///  有无通配符
        ///</summary>
        public int cn_n_equicommand_haswildcard { get; set; }
        /// <summary>
        ///  指令Json
        ///</summary>
        public string cn_s_equicommand_json { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        public string cn_s_equicommand_remarks { get; set; }
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
    }
}
