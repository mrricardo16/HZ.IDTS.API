using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 系统设置表
    /// <summary>
    /// 系统设置表
    ///</summary>
    [SugarTable("tn_dts_setting")]
    public class tn_dts_setting
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  关键字分类
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_setting_class")]
        public string cn_s_setting_class { get; set; }
        /// <summary>
        ///  关键字编码
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_setting_keycode")]
        public string cn_s_setting_keycode { get; set; }
        /// <summary>
        ///  关键字名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_setting_keyname")]
        public string cn_s_setting_keyname { get; set; }
        /// <summary>
        ///  设置值
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_setting_keyvalue")]
        public string cn_s_setting_keyvalue { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_setting_remarks")]
        public string cn_s_setting_remarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modifyBy")]
        public string cn_s_modifyBy { get; set; }
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
        [SugarColumn(ColumnName = "cn_s_creatorBy")]
        public string cn_s_creatorBy { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
        /// <summary>
        ///  关键字类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_setting_valuetype")]
        public string cn_s_setting_valuetype { get; set; }
    }
    #endregion

    #region 系统保存数据类
    /// <summary>
    /// 系统保存数据类
    /// </summary>
    public class SettingSavety
    {
        /// <summary>
        /// 关键字编码
        /// </summary>
        public string cn_s_setting_keycode { get; set; }
        /// <summary>
        /// 关键字名称
        /// </summary>
        public string cn_s_setting_keyname { get; set; }
        /// <summary>
        /// 设置值
        /// </summary>
        public string keyValue { get; set; }
        /// <summary>
        /// 设置描述
        /// </summary>
        public string describe { get; set; }
    }
    #endregion

    public class MDGAddLog
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public string logType { get; set; }
        /// <summary>
        /// 应用编码
        /// </summary>
        public string appCode { get; set; }
        /// <summary>
        /// 日志描述
        /// </summary>
        public string logDesc { get; set; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string ip { get; set; }
    }

    #region 数据库设置向导类
    /// <summary>
    /// 数据库设置向导类
    /// </summary>
    public class WizardDateBase
    {
        /// <summary>
        /// 数据库主机IP地址
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DateBaseName { get; set; }
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string DateBaseUserName { get; set; }
        /// <summary>
        /// 数据库密码
        /// </summary>
        public string DateBaseUserPassward { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DateBaseType { get; set; }
    }
    #endregion

    #region MongoDB连接数据类
    /// <summary>
    /// MongoDB连接数据类
    /// </summary>
    public class MongoDBConnection
    {
        /// <summary>
        /// MongoDB的连接IP地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DateBaseName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Passward { get; set; }
    }
    #endregion

    #region 设置项类
    /// <summary>
    /// 设置项类
    /// </summary>
    public class SettingItem
    {
        /// <summary>
        /// 关键字编码
        /// </summary>
        public string SettingKeyCode { get; set; }
        /// <summary>
        /// 设置值
        /// </summary>
        public string SettingKeyValue { get; set; }
    }
    #endregion

}
