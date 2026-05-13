using HZ.IDTSCore.Model.Entity.OpenApi;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 货位数据同步项
    /// </summary>
    public class StockItemInformation
    {
        public ObjectId _id { get; set; }

        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        public string goodsequipmentGuid { get; set; }

        /// <summary>
        /// 指令来源：初始化/业务
        /// </summary>
        public string commandSource { get; set; }

        /// <summary>
        /// 业务唯一标识
        /// </summary>
        public string busGuid { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public string stockCode { get; set; }

        /// <summary>
        /// 库区编号
        /// </summary>
        public string areaCode { get; set; }

        /// <summary>
        /// 货位编码
        /// </summary>
        public string locationCode { get; set; }

        /// <summary>
        /// 货位类型：立库、地堆
        /// </summary>
        public string locationType { get; set; }

        /// <summary>
        /// 库位状态(正常、报废、预入库锁定、预出库锁定)
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 储位状态(空、满、空托盘、不满)
        /// </summary>
        public string storageState { get; set; }

        /// <summary>
        /// 货位详情列表
        /// </summary>
        //public List<ItemRowViewModel> itemRow = new List<ItemRowViewModel>();
        public List<ItemRowInformation> itemRow { get; set; }
    }

    /// <summary>
    /// 物料信息同步项
    /// </summary>
    public class ItemRowInformation
    {
        public ObjectId _id { get; set; }

        /// <summary>
        /// 货位设备唯一标识
        /// </summary>
        public string goodsequipmentGuid { get; set; }

        /// <summary>
        /// 排列层
        /// </summary>
        public string rowColLayer { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// 托盘码
        /// </summary>
        public string trayCode { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 拓展字段1
        /// </summary>
        public string ext1 { get; set; }

        /// <summary>
        /// 拓展字段2
        /// </summary>
        public string ext2 { get; set; }
    }


}
