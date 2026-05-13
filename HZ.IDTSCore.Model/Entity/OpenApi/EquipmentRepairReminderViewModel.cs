using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class EquipmentRepairReminderViewModel
    {
        public List<EQRepairCollect> eqRepairCollect = new List<EQRepairCollect>();
    }

    public class EQRepairCollect
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceNo { get; set; }
        /// <summary>
        /// 维修事项总数
        /// </summary>
        public string repairItemCount { get; set; }
        /// <summary>
        /// 维修项
        /// </summary>
        public List<RepairItemModel> repairItem = new List<RepairItemModel>();
    }

    public class RepairItemModel
    {
        /// <summary>
        /// 维修时间
        /// </summary>
        public string repairTime { get; set; }
        /// <summary>
        /// 维修事项名称
        /// </summary>
        public string repairItemName { get; set; }
        /// <summary>
        /// 维修人
        /// </summary>
        public string repairMain { get; set; }
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
