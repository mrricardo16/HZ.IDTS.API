using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    /// <summary>
    /// 充电机状态Model
    /// </summary>
    public class BatteryChargerMonitorViewModel
    {
        public List<ChargerViewModel> Charger = new List<ChargerViewModel>();
    }

    public class ChargerViewModel
    {
        /// <summary>
        /// 设备的名称
        /// </summary>
        public string deviceName { get; set; }

        /// <summary>
        /// 工作状态：空闲、充电、故障
        /// </summary>
        public string deviceState { get; set; }

        /// <summary>
        /// 在线状态(在线、离线)
        /// </summary>
        public string onlineState { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string errMsg { get; set; }

        /// <summary>
        /// 充电机详细信息
        /// </summary>

        public InfoViewModel info = new InfoViewModel();
    }

    public class InfoViewModel
    {
        /// <summary>
        /// 电量
        /// </summary>
        public string power { get; set; }

        /// <summary>
        /// 电流
        /// </summary>
        public string current { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public string voltage { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public string temperature { get; set; }

        /// <summary>
        /// 其他非标信息，用$隔开
        /// </summary>
        public string other { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string ext1 { get; set; }

        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string ext2 { get; set; }
    }
}
