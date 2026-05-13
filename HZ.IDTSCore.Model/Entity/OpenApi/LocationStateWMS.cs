using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    /// <summary>
    /// 货位状态数据（WMS)
    /// </summary>
    public class LocationStateWMS
    {
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string stockCode { get; set; }

        /// <summary>
        /// 库区编码
        /// </summary>
        public string areaCode { get; set; }

        /// <summary>
        /// 库区结构（立库/移动料架）
        /// </summary>
        public string structMode { get; set; }

        /// <summary>
        /// 固定货位（地面货位编码）
        /// </summary>
        public string fixLocationCode { get; set; }

        /// <summary>
        /// 货位状态（正常/报废）
        /// </summary>
        public string locationState { get; set; }

        /// <summary>
        /// 使用状态（空/满）
        /// </summary>
        public string useState { get; set; }

        /// <summary>
        /// 托盘是否有货
        /// </summary>
        public bool trayHasItem { get; set; }

        /// <summary>
        /// 移动货位
        /// </summary>
        public MoveLocation moveLocations { get; set; }
    }

    public class MoveLocation
    {
        /// <summary>
        /// 移动料架号
        /// </summary>
        public string rackCode { get; set; }

        /// <summary>
        /// 面号（货架角度）
        /// </summary>
        public string face { get; set; }

        /// <summary>
        /// 料箱列表
        /// </summary>
        public List<Container> containers = new List<Container>();
    }

    public class Container
    {
        /// <summary>
        /// 移动货位号
        /// </summary>
        public string moveLocationCode { get; set; }

        /// <summary>
        /// 货位状态（空、满、空托盘、不满）
        /// </summary>
        public string useState { get; set; }
    }
}
