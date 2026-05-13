using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备保养表
    /// <summary>
    /// 设备保养表
    ///</summary>
    [SugarTable("tn_dts_equiupkeep")]
    public class tn_dts_equiupkeep
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_no")]
        public string cn_s_equiupkeep_no { get; set; }
        /// <summary>
        ///  设备名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_name")]
        public string cn_s_equiupkeep_name { get; set; }
        /// <summary>
        ///  保养项目
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_item")]
        public string cn_s_equiupkeep_item { get; set; }
        /// <summary>
        ///  保养类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_cause")]
        public string cn_s_equiupkeep_cause { get; set; }
        /// <summary>
        ///  是否首保
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_equiupkeep_isfirst")]
        public string cn_n_equiupkeep_isfirst { get; set; }
        /// <summary>
        ///  下次保养时间
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_equiupkeep_nextdate")]
        public DateTime? cn_t_equiupkeep_nextdate { get; set; }
        /// <summary>
        ///  保养总费用
        ///</summary>
        [SugarColumn(ColumnName = "cn_d_equiupkeep_cost")]
        public decimal? cn_d_equiupkeep_cost { get; set; }
        /// <summary>
        ///  保养原料内容
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_material")]
        public string cn_s_equiupkeep_material { get; set; }
        /// <summary>
        ///  保养人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_man")]
        public string cn_s_equiupkeep_man { get; set; }
        /// <summary>
        ///  保养人电话
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_phone")]
        public string cn_s_equiupkeep_phone { get; set; }
        /// <summary>
        ///  保养时间
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_equiupkeep_date")]
        public DateTime? cn_t_equiupkeep_date { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equiupkeep_remarks")]
        public string cn_s_equiupkeep_remarks { get; set; }
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

    #region 保存设备保养传入数据类
    /// <summary>
    /// 保存设备保养传入数据类
    /// </summary>
    public class SaveData_Upkeep
    {
        /// <summary>
        /// 新增修改
        /// </summary>
        public string add_or_modify { get; set; }
        /// <summary>
        /// 保养记录
        /// </summary>
        public tn_dts_equiupkeep equiupkeep { get; set; }
    }
    #endregion

    /// <summary>
    /// 获取所有整机零部件编号、购买日期和质保周期递归模型
    /// </summary>
    public class UpkeepRecrusionModel
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string cn_s_equi_no { get; set; }
        /// <summary>
        /// 首保日期
        /// </summary>
        public DateTime? cn_s_equi_firstdate { get; set; }
        /// <summary>
        /// 保养周期
        /// </summary>
        public int? cn_s_equi_defentperiod { get; set; }
    }
}
