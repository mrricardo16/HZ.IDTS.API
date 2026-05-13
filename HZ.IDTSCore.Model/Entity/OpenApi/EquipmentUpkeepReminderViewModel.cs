using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class EquipmentUpkeepReminderViewModel
    {
        public List<EQUpkeepCollect> eqUpkeepCollect = new List<EQUpkeepCollect>();
    }

    public class EQUpkeepCollect
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string deviceNo { get; set; }
        /// <summary>
        /// 保养事项总数
        /// </summary>
        public string upkeepItemCount { get; set; }
        /// <summary>
        /// 保养项
        /// </summary>
        public List<UpkeepItemModel> upkeepItem = new List<UpkeepItemModel>();
    }

    public class UpkeepItemModel
    {
        /// <summary>
        /// 下次保养时间（上次保养时间+保养周期）
        /// </summary>
        public string upkeepTime { get; set; }
        /// <summary>
        /// 保养事项名称
        /// </summary>
        public string upkeepItemName { get; set; }
        /// <summary>
        /// 上次保养时间
        /// </summary>
        public string lastUpkeepTime { get; set; }
        /// <summary>
        /// 保养周期（天）
        /// </summary>
        public string defentperiod { get; set; }
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
