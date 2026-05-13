using SqlSugar;
using System;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备实时采集记录表
    /// <summary>
    /// 设备实时采集记录表
    ///</summary>
    [SugarTable("tn_dts_equireallogs")]
    public class tn_dts_equireallogs
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_no")]
        public string cn_s_equireallogs_no { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_name")]
        public string cn_s_equireallogs_name { get; set; }
        /// <summary>
        /// 采集事项名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_itemname")]
        public string cn_s_equireallogs_itemname { get; set; }
        /// <summary>
        /// 采集对象名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_objectname")]
        public string cn_s_equireallogs_objectname { get; set; }
        /// <summary>
        /// 采集值
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_objectval")]
        public string cn_s_equireallogs_objectval { get; set; }
        /// <summary>
        /// 采集值单位
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_objectvalunit")]
        public string cn_s_equireallogs_objectvalunit { get; set; }
        /// <summary>
        ///  采集时间
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_equireallogs_timestamp")]
        public DateTime? cn_t_equireallogs_timestamp { get; set; }
        /// <summary>
        ///  拓展字段1
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_ext1")]
        public string cn_s_equireallogs_ext1 { get; set; }
        /// <summary>
        ///  拓展字段2
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equireallogs_ext2")]
        public string cn_s_equireallogs_ext2 { get; set; }
    }
    #endregion
}
