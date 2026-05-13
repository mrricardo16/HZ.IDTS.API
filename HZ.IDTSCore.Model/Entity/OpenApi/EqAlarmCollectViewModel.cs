using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class EqAlarmCollectViewModel
    {
        public List<EqAlarmCollectModel> eqAlarmCollect = new List<EqAlarmCollectModel>();
    }

    public class EqAlarmCollectModel
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
        /// 警报总数
        /// </summary>
        public int alarmItemCount { get; set; }

        /// <summary>
        /// 警报明细
        /// </summary>
        public List<AlarmItems> alarmItem = new List<AlarmItems>();
    }

    public class AlarmItems
    {
        /// <summary>
        /// 警报时间
        /// </summary>
        public string alarmTime { get; set; }

        /// <summary>
        /// 警报代码
        /// </summary>
        public string alarmCode { get; set; }

        /// <summary>
        /// 警报详情
        /// </summary>
        public string alarmName { get; set; }
        public string ext1 { get; set; }
        public string ext2 { get; set; }
    }
}
