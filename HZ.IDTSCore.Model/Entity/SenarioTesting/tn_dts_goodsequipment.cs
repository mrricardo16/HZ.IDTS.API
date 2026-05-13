using HZ.IDTSCore.Model.Entity.OpenApi;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 货位设备表
    /// <summary>
    ///货位设备表
    /// </summary>
    [SugarTable("tn_dts_goodsequipment")]
    public class tn_dts_goodsequipment
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 货位设备编码
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_no")]
        public string cn_s_goodsequipment_no { get; set; }
        /// <summary>
        /// 货位设备名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_name")]
        public string cn_s_goodsequipment_name { get; set; }
        /// <summary>
        ///  货位设备类型(立库/地堆）
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_type")]
        public string cn_s_goodsequipment_type { get; set; }
        ///// <summary>
        /////  货位站点唯一标识
        /////</summary>
        //[SugarColumn(ColumnName = "cn_s_goodsequipment_stocksiteguid")]
        //public string cn_s_goodsequipment_stocksiteguid { get; set; }
        /// <summary>
        ///  仓库编码
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_stockcode")]
        public string cn_s_goodsequipment_stockcode { get; set; }
        /// <summary>
        ///  仓库名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_stockname")]
        public string cn_s_goodsequipment_stockname { get; set; }
        /// <summary>
        ///  区域编码
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_areacode")]
        public string cn_s_goodsequipment_areacode { get; set; }
        /// <summary>
        ///  区域名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_areaname")]
        public string cn_s_goodsequipment_areaname { get; set; }
        /// <summary>
        ///  站点编码
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_locationcode")]
        public string cn_s_goodsequipment_locationcode { get; set; }
        /// <summary>
        ///  站点名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_locationname")]
        public string cn_s_goodsequipment_locationname { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodsequipment_remarks")]
        public string cn_s_goodsequipmemt_remarks { get; set; }
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

    #region 货位指令信息
    /// <summary>
    /// 货位指令信息
    /// </summary>
    public class GoodscommandInformation
    {
        /// <summary>
        /// 货位指令唯一标识
        /// </summary>
        public string GoodscommandGuid { get; set; }
        /// <summary>
        /// 货位指令编码
        /// </summary>
        public string GoodscommandNo { get; set; }
        /// <summary>
        /// 货位指令名称
        /// </summary>
        public string GoodscommandName { get; set; }
    }
    #endregion

    #region 货位设备信息
    /// <summary>
    /// 货位设备信息
    /// </summary>
    public class GoodsequipmentInformation
    {
        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        public string GoodsequipmentGuid { get; set; }
        /// <summary>
        /// 货位设备编码
        /// </summary>
        public string GoodsequipmentNo { get; set; }
        /// <summary>
        /// 货位设备名称
        /// </summary>
        public string GoodsequipmentName { get; set; }
    }
    #endregion

    #region 获取指定仓库库区的站点信息数据类
    /// <summary>
    /// 获取指定仓库库区的站点信息数据类
    /// </summary>
    public class GetReturnSite
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string StockCode { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string AreaCode { get; set; }
    }
    #endregion

    #region 站点信息返回数据类
    /// <summary>
    /// 站点信息返回数据类
    /// </summary>
    public class ReturnSite
    {
        /// <summary>
        /// 地堆编码
        /// </summary>
        public string SiteCode { get; set; }
        /// <summary>
        /// 地堆名称
        /// </summary>
        public string SiteName { get; set; }
    }
    #endregion

    #region 保存货位设备数据类
    /// <summary>
    /// 保存货位设备数据类
    /// </summary>
    public class SaveGoodsequipment
    {
        /// <summary>
        /// 新增修改标识
        /// </summary>
        public string AddOrModify { get; set; }
        /// <summary>
        /// 货位设备
        /// </summary>
        public tn_dts_goodsequipment goodsequipment { get; set; }
    }
    #endregion

    #region 刷新库区数据项
    /// <summary>
    /// 刷新库区数据项
    /// </summary>
    public class RefreshStock
    {
        /// <summary>
        /// 指令来源：初始化/业务
        /// </summary>
        public string commandSource { get; set; }

        /// <summary>
        /// 业务唯一标识
        /// </summary>
        public string busGuid { get; set; }

        /// <summary>
        /// 货位物料
        /// </summary>
        public StockViewModel stockViewModel { get; set; }
    }
    #endregion

}
