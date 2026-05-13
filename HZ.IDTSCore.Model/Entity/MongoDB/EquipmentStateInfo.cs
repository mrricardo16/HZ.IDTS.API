using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 设备状态中间表
    /// </summary>
   public class EquipmentStateInfo
    {
        public ObjectId _id { get; set; }

        /// <summary>
        /// 设备编号：数字 有几台就写数字几
        /// </summary>
        public string deviceCode { get; set; }
        /// <summary>
        /// 设备类型：堆垛机/KIVA/四向车/提升机 这四类
        /// </summary>
        public string deviceType { get; set; }
        /// <summary>
        /// X坐标
        /// </summary>
        public int pontX { get; set; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public int pontY { get; set; }
        /// <summary>
        /// 这里四向车写Z坐标，提升机写楼层数，没有默认0
        /// </summary>
        public int layerZ { get; set; }
        /// <summary>
        /// 正常或故障
        /// </summary>
        public string deviceState { get; set; }
        /// <summary>
        /// 最后一次更新时间
        /// </summary>
        public string lastUpdateTime { get; set; }

    }
}
