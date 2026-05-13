using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 版本信息表
    /// <summary>
    /// 版本信息表
    ///</summary>
    [SugarTable("tn_dts_version")]
    public class tn_dts_version
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  产品版本号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_number")]
        public string cn_s_ver_number { get; set; }
        /// <summary>
        ///  更新包版本
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_packagedate")]
        public string cn_s_ver_packagedate { get; set; }
        /// <summary>
        ///  包类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_packagetype")]
        public string cn_s_ver_packagetype { get; set; }
        /// <summary>
        ///  是否已更新
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_isupdated")]
        public int? cn_s_ver_isupdated { get; set; }
        /// <summary>
        ///  更新内容
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_updatecontent")]
        public string cn_s_ver_updatecontent { get; set; }
        /// <summary>
        ///  更新时间
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_update")]
        public DateTime? cn_s_ver_update { get; set; }
        /// <summary>
        ///  更新人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_updateman")]
        public string cn_s_ver_updateman { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_ver_remarks")]
        public string cn_s_ver_remarks { get; set; }
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
        /// <summary>
        ///  备份文件名
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_var_backupfilename")]
        public string cn_s_var_backupfilename { get; set; }
        /// <summary>
        ///  文件类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_var_backupfiletype")]
        public string cn_s_var_backupfiletype { get; set; }
        /// <summary>
        ///  备份路径
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_var_backupfilepath")]
        public string cn_s_var_backupfilepath { get; set; }
    }
    #endregion

    /// <summary>
    /// 更新包数据类
    /// </summary>
    public class UpdatePackage
    {
        /// <summary>
        /// 包版本
        /// </summary>
        public string PackageVersion { get; set; }
        /// <summary>
        /// 更新内容
        /// </summary>
        public string UpdateContent { get; set; }
    }

    #region 备份更新包类
    /// <summary>
    /// 备份更新包类
    /// </summary>
    public class BackupsPackage
    {
        /// <summary>
        /// 产品版本号
        /// </summary>
        public string packageDate { get; set; }
        /// <summary>
        /// 备份文件名
        /// </summary>
        public string backupfilename { get; set; }
        /// <summary>
        /// 备份文件类型
        /// </summary>
        public string backupfiletype { get; set; }
        /// <summary>
        /// 备份路径
        /// </summary>
        public string backupfilepath { get; set; }
    }
    #endregion

    #region MyRegion
    /// <summary>
    /// 更新更新包类
    /// </summary>
    public class ExecuteUpdatePackage
    {
        /// <summary>
        /// 产品版本号
        /// </summary>
        public string PackageDate { get; set; }
        /// <summary>
        /// 是否已更新
        /// </summary>
        public int IsUpdated { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateMan { get; set; }
    }
    #endregion

    #region 上传更新包类
    /// <summary>
    /// 上传更新包类
    /// </summary>
    public class UploadPackage
    {
        /// <summary>
        /// 是否有UI更新包
        /// </summary>
        public bool HasUIPackage { get; set; }
        /// <summary>
        /// 是否有API更新包
        /// </summary>
        public bool HasAPIPackage { get; set; }
        /// <summary>
        /// 是否有数据库脚本
        /// </summary>
        public bool HasSqlScript { get; set; }
    }
    #endregion

}
