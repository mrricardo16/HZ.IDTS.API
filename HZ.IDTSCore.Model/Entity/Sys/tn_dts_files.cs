using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 设备附件表
    /// <summary>
    /// 设备附件表
    ///</summary>
    [SugarTable("tn_dts_files")]
    public class tn_dts_files
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  来源Guid
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_source_guid")]
        public string cn_s_files_source_guid { get; set; }
        /// <summary>
        ///  来源模块
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_source_module")]
        public string cn_s_files_source_module { get; set; }
        /// <summary>
        ///  文件类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_type")]
        public string cn_s_files_type { get; set; }
        /// <summary>
        ///  文件名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_name")]
        public string cn_s_files_name { get; set; }
        /// <summary>
        ///  文件拓展名
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_extenname")]
        public string cn_s_files_extenname { get; set; }
        /// <summary>
        ///  文件大小
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_size")]
        public decimal? cn_s_files_size { get; set; }
        /// <summary>
        ///  域服务器
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_rootpath")]
        public string cn_s_files_rootpath { get; set; }
        /// <summary>
        ///  相对路径
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_oppopath")]
        public string cn_s_files_oppopath { get; set; }
        /// <summary>
        ///  绝对路径
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_abspath")]
        public string cn_s_files_abspath { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_files_remarks")]
        public string cn_s_files_remarks { get; set; }
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
