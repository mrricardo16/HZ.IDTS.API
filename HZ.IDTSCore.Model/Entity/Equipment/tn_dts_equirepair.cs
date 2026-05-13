using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备维修表
    /// <summary>
    /// 设备维修表
    ///</summary>
    [SugarTable("tn_dts_equirepair")]
    public class tn_dts_equirepair
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_no")]
        public string cn_s_equirepair_no { get; set; }
        /// <summary>
        ///  设备名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_name")]
        public string cn_s_equirepair_name { get; set; }
        /// <summary>
        /// 维修类别（维修/更换）
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_category")]
        public string cn_s_equirepair_category { get; set; }
        /// <summary>
        ///  维修项目
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_item")]
        public string cn_s_equirepair_item { get; set; }
        /// <summary>
        ///  故障原因
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_cause")]
        public string cn_s_equirepair_cause { get; set; }
        /// <summary>
        /// 解决方案
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_solution")]
        public string cn_s_equirepair_solution { get; set; }
        /// <summary>
        ///  维修人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_man")]
        public string cn_s_equirepair_man { get; set; }
        /// <summary>
        ///  维修人电话
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_phone")]
        public string cn_s_equirepair_phone { get; set; }
        /// <summary>
        ///  维修结果
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_result")]
        public string cn_s_equirepair_result { get; set; }
        /// <summary>
        ///  维修时间
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_date")]
        public DateTime? cn_s_equirepair_date { get; set; }
        /// <summary>
        ///  维修总费用
        ///</summary>
        [SugarColumn(ColumnName = "cn_d_equirepair_cost")]
        public decimal? cn_d_equirepair_cost { get; set; }
        /// <summary>
        ///  维修原料内容
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_material")]
        public string cn_s_equirepair_material { get; set; }
        /// <summary>
        /// 更换后设备编号
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_change_no")]
        public string cn_s_equirepair_change_no { get; set; }
        /// <summary>
        /// 更换后设备名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_change_name")]
        public string cn_s_equirepair_change_name { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equirepair_remarks")]
        public string cn_s_equirepair_remarks { get; set; }
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

    #region 维修保存数据类
    /// <summary>
    /// 维修保存数据类
    /// </summary>
    public class SaveData_Repair
    {
        /// <summary>
        /// 新增修改
        /// </summary>
        public string add_or_modify { get; set; }
        /// <summary>
        /// 维修记录
        /// </summary>
        public tn_dts_equirepair equirepair { get; set; }
        /// <summary>
        /// 新设备
        /// </summary>
        public tn_dts_equipment newequipment { get; set; }
    }
    #endregion

    public class MatterInfo
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string cn_guid { get; set; }
        /// <summary>
        /// 事项名称
        /// </summary>
        public string mattername { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? createtime { get; set; }
    }

    public class ReasonInfo
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string cn_guid { get; set; }
        /// <summary>
        /// 故障原因
        /// </summary>
        public string fault { get; set; }
        /// <summary>
        /// 解决方案
        /// </summary>
        public string solution { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string creator { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? createtime { get; set; }
    }
}
