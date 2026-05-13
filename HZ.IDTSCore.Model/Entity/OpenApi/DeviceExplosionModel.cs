using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    /// <summary>
    /// 设备爆炸首页当前推送的设备信息
    /// </summary>
    public class DeviceExplosionModel
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string deviceName { get; set; }

        /// <summary>
        /// 设备类型(1:整机 2:零部件)
        /// </summary>
        public string deviceType { get; set; }
    }
}
