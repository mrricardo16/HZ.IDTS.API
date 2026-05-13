using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 故障原因表
    /// <summary>
    /// 故障原因表
    ///</summary>
    [SugarTable("tn_dts_reason")]
    public class tn_dts_reason
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  来源ID
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_reason_sourceid")]
        public string cn_s_reason_sourceid { get; set; }
        /// <summary>
        ///  故障原因
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_reason_fault")]
        public string cn_s_reason_fault { get; set; }
        /// <summary>
        ///  解决方案
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_reason_solution")]
        public string cn_s_reason_solution { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_reason_remarks")]
        public string cn_s_reason_remarks { get; set; }
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
