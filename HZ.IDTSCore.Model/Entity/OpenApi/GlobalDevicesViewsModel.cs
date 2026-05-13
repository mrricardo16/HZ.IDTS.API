using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class GlobalRealCollectViewModel: DeviceRealCollectViewModel
    {

    }

    public class GlobalDevicesViewsModel
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
        /// 设备类型
        /// </summary>
        public string deviceType { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public string deviceState { get; set; }

        /// <summary>
        /// 异常代码
        /// </summary>
        public string errCode { get; set; }

        /// <summary>
        /// 异常说明
        /// </summary>
        public string errMsg { get; set; }

        /// <summary>
        /// 更新时间戳
        /// </summary>
        public DateTime publishTimestamp { get; set; }
    }
}
