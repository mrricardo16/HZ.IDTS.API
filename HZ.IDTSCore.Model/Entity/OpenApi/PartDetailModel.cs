using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class PartDetail
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceCode { get; set; }
    }

    public class ReturnPartDetail
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
        /// 设备零整类型（1：整机；2：零部件）
        /// </summary>
        public string parttype { get; set; }
        /// <summary>
        /// 设备零部件详情数组
        /// </summary>
        public List<PartDetailModel> partdetail { get; set; }
    }

    public class PartDetailModel
    {
        /// <summary>
        /// 零部件设备编号
        /// </summary>
        public string deviceCode { get; set; }
        /// <summary>
        /// 零部件设备名称
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 爆炸图保养提醒灯颜色（1：绿色；2：黄色；3：红色）
        /// </summary>
        public int lightReminder { get; set; }
    }
}
