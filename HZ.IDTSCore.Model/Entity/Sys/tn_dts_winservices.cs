using SqlSugar;
using System;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    /// <summary>
    /// 后台服务管理表
    ///</summary>
    [SugarTable("tn_dts_winservices")]
    public class tn_dts_winservices
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  服务名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_winservices_name")]
        public string cn_s_winservices_name { get; set; }
        /// <summary>
        ///  服务描述
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_winservices_describe")]
        public string cn_s_winservices_describe { get; set; }
        /// <summary>
        ///  服务路径
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_winservices_fullpath")]
        public string cn_s_winservices_fullpath { get; set; }
        /// <summary>
        ///  服务状态
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_winservices_status")]
        public string cn_s_winservices_status { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_winservices_remarks")]
        public string cn_s_winservices_remarks { get; set; }
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

    /// <summary>
    /// 保存服务数据类
    /// </summary>
    public class SaveWinservices
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 服务描述
        /// </summary>
        public string describe { get; set; }
    }

    /// <summary>
    /// 操作服务参数类
    /// </summary>
    public class OperateServiceParameter
    {
        /// <summary>
        /// 服务操作（start/启动，stop/停止，restart/重启）
        /// </summary>
        public string operation { get; set; }
        /// <summary>
        /// 服务名称
        /// </summary>
        public string serviceName { get; set; }
    }
}
