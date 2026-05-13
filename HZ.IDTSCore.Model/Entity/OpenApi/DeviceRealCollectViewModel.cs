using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{

    /// <summary>
    /// 设备实时采集Model
    /// </summary>
    public class DeviceRealCollectViewModel
    {
        public List<EQRealCollectModel> eqRealCollect = new List<EQRealCollectModel>();
    }

    public class EQRealCollectModel
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceNo { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string deviceType { get; set; }

        /// <summary>
        /// 在线状态
        /// </summary>
        public string onlineStatus { get; set; }

        /// <summary>
        /// 采集事项总数
        /// </summary>
        public string collectItemCount { get; set; }

        /// <summary>
        /// 采集明细
        /// </summary>
        public List<CollectItemModel> collectItem = new List<CollectItemModel>();
    }

    public class CollectItemModel
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public string collectTime { get; set; }

        /// <summary>
        /// 采集事项名称
        /// </summary>
        public string collectItemName { get; set; }

        /// <summary>
        /// 采集对象名称
        /// </summary>
        public string collectObjectName { get; set; }

        /// <summary>
        /// 采集值
        /// </summary>
        public string collectObjectVal { get; set; }

        /// <summary>
        /// 采集值对应的单位
        /// </summary>
        public string collectObjectUnit { get; set; }
    }
}
