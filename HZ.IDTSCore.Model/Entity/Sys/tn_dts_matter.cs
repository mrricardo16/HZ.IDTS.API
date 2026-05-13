using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 事项表
    /// <summary>
    /// 事项表
    ///</summary>
    [SugarTable("tn_dts_matter")]
    public class tn_dts_matter
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  事项类型（保养/维修）
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_matter_type")]
        public string cn_s_matter_type { get; set; }
        /// <summary>
        ///  事项来源ID
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_matter_sourceid")]
        public string cn_s_matter_sourceid { get; set; }
        /// <summary>
        ///  事项名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_matter_name")]
        public string cn_s_matter_name { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_matter_remarks")]
        public string cn_s_matter_remarks { get; set; }
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
}


