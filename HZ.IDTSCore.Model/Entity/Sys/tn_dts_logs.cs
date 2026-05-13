using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 日志信息表
    /// <summary>
    /// 日志信息表
    ///</summary>
    [SugarTable("tn_dts_logs")]
    public class tn_dts_logs
    {
        /// <summary>
        /// 唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 日志类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_type")]
        public string cn_s_logs_type { get; set; }
        /// <summary>
        /// 客户端IP
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_clientip")]
        public string cn_s_logs_clientip { get; set; }
        /// <summary>
        /// 接收地址
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_receiveurl")]
        public string cn_s_logs_receiveurl { get; set; }
        /// <summary>
        /// 接收参数
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_receivepram")]
        public string cn_s_logs_receivepram { get; set; }
        /// <summary>
        /// 接收返回结果
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_logs_receiveresult")]
        public string cn_s_logs_receiveresult { get; set; }
        /// <summary>
        /// 请求地址
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_requesturl")]
        public string cn_s_logs_requesturl { get; set; }
        /// <summary>
        /// 请求参数
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_requestpram")]
        public string cn_s_logs_requestpram { get; set; }
        /// <summary>
        /// 请求反馈结果
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_requestresult")]
        public string cn_s_logs_requestresult { get; set; }
        /// <summary>
        /// 操作路径
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_optionpath")]
        public string cn_s_logs_optionpath { get; set; }
        /// <summary>
        /// 异常信息
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_errorsinfo")]
        public string cn_s_logs_errorsinfo { get; set; }
        /// <summary>
        /// 备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_logs_remarks")]
        public string cn_s_logs_remarks { get; set; }
        /// <summary>
        /// 修改人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        /// 修改人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify_by")]
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        /// 创建人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator")]
        public string cn_s_creator { get; set; }
        /// <summary>
        /// 创建人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator_by")]
        public string cn_s_creator_by { get; set; }
        /// <summary>
        /// 创建日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
    }
    #endregion

    public class OpenLog
    {
        /// <summary>
        ///  日志类型
        ///</summary>
        public string logtype { get; set; }
        /// <summary>
        ///  客户端IP
        ///</summary>
        public string clientip { get; set; }
        /// <summary>
        ///  接收地址
        ///</summary>
        public string receiveurl { get; set; }
        /// <summary>
        ///  接收参数
        ///</summary>
        public string receivepram { get; set; }
        /// <summary>
        ///  接收返回结果
        /// </summary>
        public string receiveresult { get; set; }
        /// <summary>
        ///  请求地址
        ///</summary>
        public string requesturl { get; set; }
        /// <summary>
        ///  请求参数
        ///</summary>
        public string requestpram { get; set; }
        /// <summary>
        ///  请求反馈结果
        ///</summary>
        public string requestresult { get; set; }
        /// <summary>
        ///  操作路径
        ///</summary>
        public string optionpath { get; set; }
        /// <summary>
        ///  异常信息
        ///</summary>
        public string errorsinfo { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        public string remarks { get; set; }
    }
}
