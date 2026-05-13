using HZ.IDTSCore.Model.Entity.OpenApi;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.SenarioTesting
{
    #region 货位指令表
    /// <summary>
    /// 货位指令表
    /// </summary>
    [SugarTable("tn_dts_goodscommand")]
    public class tn_dts_goodscommand
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        ///// <summary>
        ///// 虚拟货位设备编码
        ///// </summary>
        //[SugarColumn(ColumnName = "cn_s_goodscommand_virequi")]
        //public string cn_s_goodscommand_virequi { get; set; }
        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_goodscommand_goodsequipguid")]
        public string cn_s_goodscommand_goodsequipguid { get; set; }
        /// <summary>
        /// 指令编码
        /// </summary>
        [SugarColumn(ColumnName = "cn_s_goodscommand_no")]
        public string cn_s_goodscommand_no { get; set; }
        /// <summary>
        ///  指令名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodscommand_name")]
        public string cn_s_goodscommand_name { get; set; }
        /// <summary>
        ///  指令类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodscommand_type")]
        public string cn_s_goodscommand_type { get; set; }
        ///// <summary>
        /////  货位排列层
        /////</summary>
        //[SugarColumn(ColumnName = "cn_s_goodscommand_rowcollayer")]
        //public string cn_s_goodscommand_rowcollayer { get; set; }
        ///// <summary>
        /////  货位状态
        /////</summary>
        //[SugarColumn(ColumnName = "cn_s_goodscommand_storagestate")]
        //public string cn_s_goodscommand_storagestate { get; set; }
        /// <summary>
        ///  有无通配符
        ///</summary>
        [SugarColumn(ColumnName = "cn_n_goodscommand_haswildcard")]
        public int cn_n_goodscommand_haswildcard { get; set; }
        /// <summary>
        ///  指令Json
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodscommand_json")]
        public string cn_s_goodscommand_json { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_goodscommand_remarks")]
        public string cn_s_goodscommand_remarks { get; set; }
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

    #region 货位指令保存数据类
    /// <summary>
    /// 货位指令保存数据类
    /// </summary>
    public class SaveGoodscommand
    {
        /// <summary>
        /// 新增修改
        /// </summary>
        public string AddOrModify { get; set; }
        /// <summary>
        /// 货位指令
        /// </summary>
        public tn_dts_goodscommand NewGoodscommand { get; set; }
    }
    #endregion

    #region 批量添加货位设备指令数据类
    /// <summary>
    /// 批量添加货位设备指令数据类
    /// </summary>
    public class BatchAddGoodscommandDate
    {
        /// <summary>
        /// 用户已选货位设备唯一标识数组
        /// </summary>
        public List<string> SelectiveGoodsequipmentGuidList { get; set; }
        ///// <summary>
        ///// 指令编码
        ///// </summary>
        //public string GoodscommandNo { get; set; }
        /// <summary>
        /// 指令名称
        /// </summary>
        public string GoodscommandName { get; set; }
        /// <summary>
        /// 指令类型
        /// </summary>
        public string GoodscommandType { get; set; }
        /// <summary>
        /// 有无通配符
        /// </summary>
        public int Haswildcard { get; set; }
        /// <summary>
        /// 指令Json
        /// </summary>
        public string Json { get; set; }
    }
    #endregion

    #region 编辑货位指令JSON模版数据类
    /// <summary>
    /// 编辑货位指令JSON模版数据类
    /// </summary>
    public class EditGoodscommandDate
    {
        /// <summary>
        /// 货位指令唯一标识
        /// </summary>
        public string GoodscommandGuid { get; set; }
        /// <summary>
        /// 有无通配符（0：无；1：有）
        /// </summary>
        public int Haswildcard { get; set; }
        /// <summary>
        /// 指令Json
        /// </summary>
        public string Json { get; set; }
    }
    #endregion

    #region 复制货位指令数据类
    /// <summary>
    /// 复制货位指令数据类
    /// </summary>
    public class CopyGoodscommandDate
    {
        /// <summary>
        /// 复制的货位指令唯一标识数组
        /// </summary>
        public List<string> GoodscommandGuidList { get; set; }
        /// <summary>
        /// 粘贴的货位设备唯一标识数组
        /// </summary>
        public List<string> GoodsequipmentGuidList { get; set; }
    }
    #endregion

    #region 批量增加库位数据类
    /// <summary>
    /// 批量增加库位数据类
    /// </summary>
    public class BatchAddLocationDate
    {
        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        public string GoodsequipmentGuid { get; set; }

        /// <summary>
        /// 批量增加模式（整排、整列、整层和单个）
        /// </summary>
        public string AddMode { get; set; }

        /// <summary>
        /// 货位信息
        /// </summary>
        public StockViewModel StockViewModel { get; set; }
    }
    #endregion

    #region 修改库位数据类
    /// <summary>
    /// 修改库位数据类
    /// </summary>
    public class ModifyLocationDate
    {
        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        public string GoodsequipmentGuid { get; set; }

        /// <summary>
        /// 货位信息
        /// </summary>
        public StockViewModel StockViewModel { get; set; }
    }
    #endregion

    #region 删除库位数据类
    /// <summary>
    /// 删除库位数据类
    /// </summary>
    public class DeleteLocationDate
    {
        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        public string GoodsequipmentGuid { get; set; }

        /// <summary>
        /// 货位编码（排列层）列表
        /// </summary>
        public List<string> LocationCodeList { get; set; }
    }
    #endregion

    #region 排列层
    /// <summary>
    /// 排列层
    /// </summary>
    public class RowColLayer
    {
        /// <summary>
        /// 排
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// 列
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// 层
        /// </summary>
        public int Layer { get; set; }
    }
    #endregion

    #region 货位设备信息（含货位设备编码）
    /// <summary>
    /// 货位设备信息（含货位设备编码）
    /// </summary>
    public class GoodscommandPlus
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        public string cn_guid { get; set; }
        /// <summary>
        /// 货位设备编码
        /// </summary>
        public string cn_s_goodscommand_goodsequipmentno { get; set; }
        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        public string cn_s_goodscommand_goodsequipguid { get; set; }
        /// <summary>
        /// 指令编码
        /// </summary>
        public string cn_s_goodscommand_no { get; set; }
        /// <summary>
        ///  指令名称
        ///</summary>
        public string cn_s_goodscommand_name { get; set; }
        /// <summary>
        ///  指令类型
        ///</summary>
        public string cn_s_goodscommand_type { get; set; }
        /// <summary>
        ///  有无通配符
        ///</summary>
        public int cn_n_goodscommand_haswildcard { get; set; }
        /// <summary>
        ///  指令Json
        ///</summary>
        public string cn_s_goodscommand_json { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        public string cn_s_goodscommand_remarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  创建人
        ///</summary>
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  创建人名称
        ///</summary>
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        public DateTime? cn_t_create { get; set; }
    }
    #endregion
}
