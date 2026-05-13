using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class TrayItemInfoWMS
    {
        /// <summary>
        /// 托盘号
        /// </summary>
        public string trayCode { get; set; }
        /// <summary>
        /// 托盘/料箱/移动料架
        /// </summary>
        public string trayClass { get; set; }
        /// <summary>
        /// 料箱里的物料信息
        /// </summary>
        public List<ItemWMS> items { get; set; }
        /// <summary>
        /// 移动料架里的料箱物料信息
        /// </summary>
        public List<MoveLocationTrayItemWMS> moveLocations { get; set; }
    }

    public class MoveLocationTrayItemWMS
    {
        /// <summary>
        /// 托盘号（料箱号）
        /// </summary>
        public string trayCode { get; set; }
        /// <summary>
        /// 托盘/料箱/移动料架
        /// </summary>
        public string trayClass { get; set; }
        /// <summary>
        /// 物料列表
        /// </summary>
        public List<ItemWMS> items { get; set; }

    }

    public class ItemWMS
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemCode { get; set; }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemName { get; set; }
        /// <summary>
        /// 物料规格
        /// </summary>
        public string itemModel { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public double qty { get; set; }
        /// <summary>
        /// 数量单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string lotCode { get; set; }
    }
}
