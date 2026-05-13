using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.location
{
    /// <summary>
    /// 立库3D结构表
    ///</summary>
    [SugarTable("tn_dts_stock3d")]
    public class tn_dts_stock3d
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_location_stockcode")]
        public string cn_s_location_stockcode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_location_stockname")]
        public string cn_s_location_stockname { get; set; }
        /// <summary>
        ///  区域编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_areacode")]
        public string cn_s_location_areacode { get; set; }
        /// <summary>
        ///  区域名换
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_areaname")]
        public string cn_s_location_areaname { get; set; }
        /// <summary>
        ///  是否显示样例
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_isshow")]
        public int? cn_s_location_isshow { get; set; }
        /// <summary>
        ///  排数
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_row")]
        public int? cn_s_location_row { get; set; }
        /// <summary>
        ///  列数
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_col")]
        public int? cn_s_location_col { get; set; }
        /// <summary>
        ///  层数
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_layer")]
        public int? cn_s_location_layer { get; set; }
        /// <summary>
        ///  货位长度
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_length")]
        public decimal? cn_s_location_length { get; set; }
        /// <summary>
        ///  货位高度
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_height")]
        public decimal? cn_s_location_height { get; set; }
        /// <summary>
        ///  货位宽度
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_width")]
        public decimal? cn_s_location_width { get; set; }
        /// <summary>
        ///  X坐标位置
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_xpos")]
        public decimal? cn_s_location_xpos { get; set; }
        /// <summary>
        ///  Y坐标位置
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_ypos")]
        public decimal? cn_s_location_ypos { get; set; }
        /// <summary>
        ///  报废货位
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_nullify")]
        public string cn_s_location_nullify { get; set; }
        /// <summary>
        ///  每排间隔
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_gap")]
        public string cn_s_location_gap { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_location_remarks")]
        public string cn_s_location_remarks { get; set; }
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
    /// 根据选择的仓库名获取全部库区名及其最大排列行（新增调用）方法传入数据类
    /// </summary>
    public class GetReturnStock
    {
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string cn_s_location_stockcode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string cn_s_location_stockname { get; set; }
    }

    #region 获取某区域最大排列层数据类
    /// <summary>
    /// 获取某区域最大排列层数据类
    /// </summary>
    public class GetAreaMaxRowColLayer
    {
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string stockCode { get; set; }

        /// <summary>
        /// 库区编码
        /// </summary>
        public string areaCode { get; set; }
    }
    #endregion

    #region 最大排列层数据类
    /// <summary>
    /// 最大排列层数据类
    /// </summary>
    public class MaxRowColLayer
    {
        /// <summary>
        /// 最大排
        /// </summary>
        public string MaxRow { get; set; }

        /// <summary>
        /// 最大列
        /// </summary>
        public string MaxCol { get; set; }

        /// <summary>
        /// 最大层
        /// </summary>
        public string MaxLayer { get; set; }
    }
    #endregion


    /// <summary>
    /// 根据选择的仓库名获取全部库区名及其最大排列行（新增调用）方法返回数据类
    /// </summary>
    public class ReturnArea
    {
        /// <summary>
        /// 库区编码
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 库区名称
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 最大排
        /// </summary>
        public string max_row { get; set; }
        /// <summary>
        /// 最大列
        /// </summary>
        public string max_col { get; set; }
        /// <summary>
        /// 最大层
        /// </summary>
        public string max_floor { get; set; }
    }

    public class GetUpdate
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string cn_guid { get; set; }
    }

    /// <summary>
    /// 执行增加更新删除方法传入数据类
    /// </summary>
    public class ExecuteAddUpdateDelete
    {
        /// <summary>
        /// 用户点击的选项
        /// </summary>
        public string button { get; set; }
        /// <summary>
        /// 立库列表
        /// </summary>
        public List<tn_dts_stock3d> stock3dList { get; set; }
    }

    public class GetScrapList
    {
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string location_stockcode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string location_stockname { get; set; }
        /// <summary>
        /// 库区编码
        /// </summary>
        public string location_area_code { get; set; }
        /// <summary>
        /// 库区名称
        /// </summary>
        public string location_area_name { get; set; }
    }

    public class ReturnScrap
    {
        /// <summary>
        /// 货位编号
        /// </summary>
        public string location_code { get; set; }
        /// <summary>
        /// 所属排
        /// </summary>
        public string row { get; set; }
        /// <summary>
        /// 所属列
        /// </summary>
        public string col { get; set; }
        /// <summary>
        /// 所属层
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 当前状态
        /// </summary>
        public string location_state { get; set; }
    }
}
