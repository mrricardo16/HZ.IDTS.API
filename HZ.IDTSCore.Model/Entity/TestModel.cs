using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity
{
    #region 货位状态数据（WMS)
    /// <summary>
    /// 货位状态数据（WMS)
    /// </summary>
    public class LocationStateInfo
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
        /// 货位号（地面货位编码）
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
        /// 移动货位
        /// </summary>
        public MoveLocationStateInfo moveLocations { get; set; }
    }

    public class MoveLocationStateInfo
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
        /// 料箱号
        /// </summary>
        public string trayCode { get; set; }

        /// <summary>
        /// 货位状态（正常、报废）
        /// </summary>
        public string locationState { get; set; }

        /// <summary>
        /// 使用状态（空、满、空托盘、不满）
        /// </summary>
        public string useState { get; set; }
    }
    #endregion

    public class DeviceStateInfo
    {
        /// <summary>
        /// 输送线列表
        /// </summary>
        public List<Line> line { get; set; }
        /// <summary>
        /// 提升机列表
        /// </summary>
        public List<Hoister> hoister { get; set; }
        /// <summary>
        /// 堆垛机列表
        /// </summary>
        public List<Stacker> stacker { get; set; }
    }

    public class Line
    {
        /// <summary>
        /// 输送线编码（P1001）
        /// </summary>
        public string deviceCode { get; set; }
        /// <summary>
        /// 输送线名称
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 异常码
        /// </summary>
        public string errCode { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string errMsg { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 光电信号
        /// </summary>
        public bool signal { get; set; }
        /// <summary>
        /// 任务号
        /// </summary>
        public string taskNo { get; set; }
        /// <summary>
        /// 料箱号
        /// </summary>
        public string trayCode { get; set; }
        /// <summary>
        /// 货位使用状态
        /// </summary>
        public string trayUsed { get; set; }
    }

    public class Hoister
    {
        /// <summary>
        /// 提升机编码
        /// </summary>
        public string  deviceCode { get; set; }
        /// <summary>
        /// 提升机名称
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 异常码
        /// </summary>
        public string errCode { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string errMsg { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 任务号
        /// </summary>
        public string taskNo { get; set; }
        /// <summary>
        /// 料箱编码
        /// </summary>
        public string trayCode { get; set; }
        /// <summary>
        /// 料箱使用状态
        /// </summary>
        public string trayUsed { get; set; }
        /// <summary>
        /// 层
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 高度/mm
        /// </summary>
        public double runHeight { get; set; }
    }

    public class Stacker
    {
        /// <summary>
        /// 堆垛机编码
        /// </summary>
        public string deviceCode { get; set; }
        /// <summary>
        /// 堆垛机名称
        /// </summary>
        public string deviceName { get; set; }
        /// <summary>
        /// 异常码
        /// </summary>
        public string errCode { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string errMsg { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 任务号
        /// </summary>
        public string taskNo { get; set; }
        /// <summary>
        /// 终点
        /// </summary>
        public string toStation { get; set; }
        /// <summary>
        /// 料箱号
        /// </summary>
        public string trayCode { get; set; }
        /// <summary>
        /// 料箱使用状态
        /// </summary>
        public string trayUsed { get; set; }
        /// <summary>
        /// 行走
        /// </summary>
        public double posX { get; set; }
        /// <summary>
        /// 举升
        /// </summary>
        public double posY { get; set; }
        /// <summary>
        /// 货叉
        /// </summary>
        public double posZ { get; set; }
    }

    public class TrayItemInfo
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
        public List<Item> items { get; set; }
        /// <summary>
        /// 移动料架里的料箱物料信息
        /// </summary>
        public List<MoveLocationTrayItem> moveLocations { get; set; }
    }

    public class MoveLocationTrayItem
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
        public List<Item> items { get; set; }

    }

    public class Item
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
