using SqlSugar;
using System;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备层级关系表
    /// <summary>
    /// 设备层级关系表
    ///</summary>
    [SugarTable("tn_dts_equibom")]
    public class tn_dts_equibom
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  父项设备编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equibom_parentno")]
        public string cn_s_equibom_parentno { get; set; }
        /// <summary>
        /// 父项设备名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equibom_parentname")]
        public string cn_s_equibom_parentname { get; set; }
        /// <summary>
        /// 子项设备编号
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equibom_childno")]
        public string cn_s_equibom_childno { get; set; }
        /// <summary>
        /// 子项设备名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_equibom_childname")]
        public string cn_s_equibom_childname { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        [SugarColumn(ColumnName = "cn_t_equibom_effectuatetime")]
        public DateTime? cn_t_equibom_effectuatetime { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        [SugarColumn(ColumnName = "cn_t_equibom_lapsetime")]
        public DateTime? cn_t_equibom_lapsetime { get; set; }
        /// <summary>
        ///  关系状态
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equibom_status")]
        public string cn_s_equibom_status { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_equibom_remarks")]
        public string cn_s_equibom_remarks { get; set; }
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
