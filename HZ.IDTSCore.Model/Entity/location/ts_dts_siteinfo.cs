using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.location
{
    /// <summary>
    /// 地堆货位表
    ///</summary>
    [SugarTable("tn_dts_siteinfo")]
    public class tn_dts_siteinfo
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  站点编码
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_code")]
        public string cn_s_siteinfo_code { get; set; }
        /// <summary>
        ///  站点名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_name")]
        public string cn_s_siteinfo_name { get; set; }
        /// <summary>
        ///  X坐标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_xpos")]
        public decimal? cn_s_siteinfo_xpos { get; set; }
        /// <summary>
        ///  Y坐标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_ypos")]
        public decimal? cn_s_siteinfo_ypos { get; set; }
        /// <summary>
        ///  是否显示样例（1表示是，0和其他数字以及空都表示否）
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_isshow")]
        public int? cn_s_siteinfo_isshow { get; set; }
        /// <summary>
        ///  规划长度
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_lenght")]
        public decimal? cn_s_siteinfo_lenght { get; set; }
        /// <summary>
        ///  规划宽度
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_width")]
        public decimal? cn_s_siteinfo_width { get; set; }
        /// <summary>
        ///  规划高度
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_height")]
        public decimal? cn_s_siteinfo_height { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_remarks")]
        public string cn_s_siteinfo_remarks { get; set; }
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
        /// 创建人
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
        /// <summary>
        ///  修改日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  规划角度
        ///</summary>
        [SugarColumn(ColumnName = "cn_d_siteinfo_angle")]
        public double cn_d_siteinfo_angle { get; set; }
        /// <summary>
        /// 地堆模型编码
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_siteinfo_pileofland")]
        public string cn_s_siteinfo_pileofland { get; set; }
    }

    /// <summary>
    /// 执行修改、删除、批量设置地堆数据类
    /// </summary>
    public class ExecuteUpdateDeleteBatchsetting
    {
        /// <summary>
        /// 用户点击的选项
        /// </summary>
        public string button { get; set; }
        /// <summary>
        /// 地堆列表
        /// </summary>
        public List<tn_dts_siteinfo> siteinfoList { get; set; }
    }
}
