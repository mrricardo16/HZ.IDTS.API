using SqlSugar;
using System;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备异常信息表
    /// <summary>
    /// 设备异常信息表
    ///</summary>
    [SugarTable("tn_dts_equialarmlogs")]
    public class tn_dts_equialarmlogs
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equialarmlogs_no")]
        public string cn_s_equialarmlogs_no { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equialarmlogs_name")]
        public string cn_s_equialarmlogs_name { get; set; }
        /// <summary>
        /// 警报代码
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equialarmlogs_errcode")]
        public string cn_s_equialarmlogs_errcode { get; set; }
        /// <summary>
        /// 警报名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equialarmlogs_errmsg")]
        public string cn_s_equialarmlogs_errmsg { get; set; }
        /// <summary>
        ///  警报时间
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_equialarmlogs_timestamp")]
        public DateTime? cn_t_equialarmlogs_timestamp { get; set; }
        /// <summary>
        ///  拓展字段1
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equialarmlogs_ext1")]
        public string cn_s_equialarmlogs_ext1 { get; set; }
        /// <summary>
        ///  拓展字段2
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equialarmlogs_ext2")]
        public string cn_s_equialarmlogs_ext2 { get; set; }
    }
    #endregion
}
