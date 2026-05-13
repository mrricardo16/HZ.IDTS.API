using SqlSugar;
using System;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("tn_dts_dbops")]
    public class tn_dts_dbops
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_dbops_category")]
        public string cn_s_dbops_category { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_dbops_type")]
        public string cn_s_dbops_type { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_dbops_filename")]
        public string cn_s_dbops_filename { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_dbops_filesize")]
        public float? cn_s_dbops_filesize { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_dbops_fullpath")]
        public string cn_s_dbops_fullpath { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_dbops_result")]
        public string cn_s_dbops_result { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_dbops_remarks")]
        public string cn_s_dbops_remarks { get; set; }
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

    public class SaveBackups
    {
        /// <summary>
        /// 是否自动
        /// </summary>
        public bool? IsAutomatic { get; set; }
        /// <summary>
        /// 自动备份周期
        /// </summary>
        public Span? Span { get; set; }
        /// <summary>
        /// 时/日/周
        /// </summary>
        public string HourDayWeek { get; set; }
        /// <summary>
        /// 备份目录
        /// </summary>
        public string BackupsDiretory { get; set; }
    }

    public enum Span
    {
        Day,
        Week,
        Month
    }

    //public class WeekInformation
    //{
    //    /// <summary>
    //    /// 年
    //    /// </summary>
    //    public int Year { get; set; }

    //    /// <summary>
    //    /// 月
    //    /// </summary>
    //    public int Month { get; set; }

    //    /// <summary>
    //    /// 星期
    //    /// </summary>
    //    public DayOfWeek Week { get; set; }
    //}

    public class WeekDayOfYearRange
    {
        /// <summary>
        /// 所在周的周一在全年的DayOfYear
        /// </summary>
        public int ThisWeekMonday { get; set; }
        /// <summary>
        /// 所在周的周日在全年的DayOfYear
        /// </summary>
        public int ThisWeekSunday { get; set; }

        /// <summary>
        /// 日期状态：0：不在该年第一周或最后一周；1：在该年第一周；2：在该年最后一周。
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        ///该年最后一天的DayOfWeek(只有在状态为2时，该属性有值)
        /// </summary>
        public DayOfWeek LastWeek { get; set; }

        /// <summary>
        /// 最后一周所在年总共有多少天(只有在状态为2时，该属性有值）
        /// </summary>
        public int AllYearDays { get; set; }

        /// <summary>
        /// 该周所在年份
        /// </summary>
        public int Year { get; set; }
    }
}
