using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("tn_dts_cache")]
    public class tn_dts_cache
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_cache_code")]
        public string cn_s_cache_code { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_cache_name")]
        public string cn_s_cache_name { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_cache_type")]
        public string cn_s_cache_type { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_cache_remarks")]
        public string cn_s_cache_remarks { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify_by")]
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator")]
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator_by")]
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
    }

    public class CacheSynchronousness
    {
        /// <summary>
        /// 缓存名称
        /// </summary>
        public string cn_s_cache_code { get; set; }
        /// <summary>
        /// 缓存类型
        /// </summary>
        public string cn_s_cache_type { get; set; }
    }

    public class MDGGetStock
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string stockName { get; set; }
    }

    public class MDGGetArea
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string areaCode { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string areaName { get; set; }
    }

    //public class MDGGetLocationSite
    //{
    //    /// <summary>
    //    /// 货位/站点编码
    //    /// </summary>
    //    public string code { get; set; }
    //}

    public class MDGGetLocation
    {
        /// <summary>
        /// 货位编码
        /// </summary>
        public string locationCode { get; set; }
    }

    public class MDGGetSite
    {
        /// <summary>
        /// 站点编码
        /// </summary>
        public string siteCode { get; set; }
    }
}
