using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTS.SimulateService
{
    /// <summary>
    /// AGV状态接口
    /// </summary>
    public class AgvRealMonitorViewModel
    {
        public List<AgvModel> Agv = new List<AgvModel>();
    }

    public class AgvModel
    {
        /// <summary>
        /// AGV编号
        /// </summary>
        public string carCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string carName { get; set; }

        /// <summary>
        /// 设备的运行状态，需要提供定义（执行状态：运行中、取货中、卸货中、故障、充电、空闲）
        /// </summary>
        public string carState { get; set; }

        /// <summary>
        /// 在线状态(在线、离线)
        /// </summary>
        public string onlineState { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string errMsg { get; set; }

        /// <summary>
        /// 设备的当前角度
        /// </summary>
        public string angle { get; set; }

        /// <summary>
        /// 设备位置x坐标
        /// </summary>
        public string x { get; set; }

        /// <summary>
        /// 设备位置y坐标
        /// </summary>
        public string y { get; set; }

        /// <summary>
        /// 速度  mm/s
        /// </summary>
        public string speed { get; set; }

        /// <summary>
        /// 举升高度 mm
        /// </summary>
        public string liftHeight { get; set; }

        /// <summary>
        /// 载货状态  0:放下 1:举升
        /// </summary>
        public string loadStatus { get; set; }

        /// <summary>
        /// 电量
        /// </summary>
        public int power { get; set; }

        /// <summary>
        /// 是否有货物
        /// </summary>
        public int hasGoods { get; set; }

        /// <summary>
        /// 货物信息
        /// </summary>
        public GoodsInfoViewModel goodsInfo = new GoodsInfoViewModel();
    }

    public class GoodsInfoViewModel
    {
        /// <summary>
        /// 任务号
        /// </summary>
        public string taskNo { get; set; }

        /// <summary>
        /// 起点
        /// </summary>
        public string startBit { get; set; }

        /// <summary>
        ///  终点
        /// </summary>
        public string endBit { get; set; }
        
        /// <summary>
        /// 搬运的货位内容描述
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }
    }
}
