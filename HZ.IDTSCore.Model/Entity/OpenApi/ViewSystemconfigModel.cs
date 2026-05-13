using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class ViewSystemconfigModel
    //public class ViewSystemconfigModel<T>
    {
        /// <summary>
        /// 立库/Location表信息
        /// </summary>
        public List<Region> Regions { get; set; }
        /// <summary>
        /// 地堆/Siteinfo表信息
        /// </summary>
        public Ground Ground { get; set; }
        /// <summary>
        /// AGV/Equiobject和Equiobjectattr表信息
        /// </summary>
        public List<AGVMember> AGV { get; set; }
        //public List<T> AGV { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ChargeMember> charge { get; set; }
        /// <summary>
        /// AutoView
        /// </summary>
        public AutoView AutoView { get; set; }
        /// <summary>
        /// Position
        /// </summary>
        public Position Position { get; set; }
        /// <summary>
        /// SysConfig
        /// </summary>
        public SysConfig SysConfig { get; set; }
        /// <summary>
        /// SysAuthority
        /// </summary>
        public SysAuthority SysAuthority { get; set; }
    }

    public class Region
    {
        /// <summary>
        /// 区域名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否显示货位
        /// </summary>
        public int? showGoods { get; set; }
        /// <summary>
        /// 列数
        /// </summary>
        public int? colNum { get; set; }
        /// <summary>
        /// 层数
        /// </summary>
        public int? levelNum { get; set; }
        /// <summary>
        /// 排数
        /// </summary>
        public int? rowNum { get; set; }
        /// <summary>
        /// 货物长度
        /// </summary>
        public decimal? sizeX { get; set; }
        /// <summary>
        /// 货物高度
        /// </summary>
        public decimal? sizeY { get; set; }
        /// <summary>
        /// 货物宽度
        /// </summary>
        public decimal? sizeZ { get; set; }
        /// <summary>
        /// 每排间隔
        /// </summary>
        public List<double> rowGap { get; set; }
        /// <summary>
        /// X坐标位置
        /// </summary>
        public decimal? origionPointX { get; set; }
        /// <summary>
        /// Y坐标位置
        /// </summary>
        public decimal? origionPointY { get; set; }
        /// <summary>
        /// 报废货位
        /// </summary>
        public List<string> invalid { get; set; }
    }

    public class DataMember
    {
        /// <summary>
        /// 是否显示地堆
        /// </summary>
        public int? showGoods { get; set; }
        /// <summary>
        /// 区域编号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// X坐标位置
        /// </summary>
        public decimal? positionX { get; set; }
        /// <summary>
        /// Y坐标位置
        /// </summary>
        public decimal? positionY { get; set; }
        /// <summary>
        /// 货物长度
        /// </summary>
        public decimal? sizeX { get; set; }
        /// <summary>
        /// 货物高度
        /// </summary>
        public decimal? sizeY { get; set; }
        /// <summary>
        /// 货物宽度
        /// </summary>
        public decimal? sizeZ { get; set; }
        /// <summary>
        /// 规划的角度
        /// </summary>
        public double angle { get; set; }
        /// <summary>
        /// 地堆模型
        /// </summary>
        public string pileofland { get; set; }
        /// <summary>
        /// 货物信息
        /// </summary>
        private RackInfoViewModel _rackInfo = new RackInfoViewModel();
        public RackInfoViewModel rackInfo
        {
            get
            {
                return _rackInfo;
            }
            set
            {
                if (value is null)
                {
                    _rackInfo = new RackInfoViewModel();
                }
                else
                {
                    _rackInfo = value;
                }
            }
        }
    }

    public class Ground
    {
        /// <summary>
        /// 地堆数据
        /// </summary>
        public List<DataMember> data = new List<DataMember>();
    }


    public class DynamicsObjectClass
    {
        public string objectKeyName { get; set; }//对象名

        public object objectValue { get; set; }//对应对象的值
    }


    public class AGVMember
    {
        /// <summary>
        /// AGV设备名称/Equiobjectattr表
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// AGV移动速度/Equiobjectattr表
        /// </summary>
        public double? Speed { get; set; }
        /// <summary>
        /// AGV设备编号/Equiobjectattr表
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// AGV旋转速度/Equiobjectattr表
        /// </summary>
        public double? RotateSpeed { get; set; }
        /// <summary>
        /// AGV初始位置X坐标/Equiobjectattr表
        /// </summary>
        public double? InitPosX { get; set; }
        /// <summary>
        /// AGV初始位置Y坐标/Equiobjectattr表
        /// </summary>
        public double? InitPosY { get; set; }
    }

    public class ChargeMember
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// charge初始X坐标
        /// </summary>
        public double? PosX { get; set; }
        /// <summary>
        /// charge初始Y坐标
        /// </summary>
        public double? PosY { get; set; }
    }

    public class AutoView
    {
        /// <summary>
        /// 自动旋转视角时间间隔
        /// </summary>
        public double? interval { get; set; }
    }

    public class Position
    {
        /// <summary>
        /// 原点X坐标
        /// </summary>
        public double? originX { get; set; }
        /// <summary>
        /// 原点Y坐标
        /// </summary>
        public double? originY { get; set; }
    }

    public class SysConfig
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        public string systemName { get; set; }
        /// <summary>
        /// 系统Logo图片地址
        /// </summary>
        public string systemLogoURL { get; set; }
        /// <summary>
        /// WebSocket服务器地址
        /// </summary>
        public string webSocketServer { get; set; }
    }

    public class SysAuthority
    {
        /// <summary>
        /// 是否启用实时监控菜单
        /// </summary>
        public bool? enabledRealMonitorMenu { get; set; }
        /// <summary>
        /// 是否启用设备数据菜单
        /// </summary>
        public bool? enabledDeviceDataMenu { get; set; }
        /// <summary>
        /// 是否启用业务监控菜单
        /// </summary>
        public bool? enabledBussMonitorMenu { get; set; }
        /// <summary>
        /// 总线监视器菜单设置
        /// </summary>
        public BussMonitorMenuSett bussMonitorMenuSett { get; set; }
        /// <summary>
        /// 是否启用虚拟视频菜单
        /// </summary>
        public bool? enabledVirtualVideoMenu { get; set; }
        /// <summary>
        /// 是否启用计划监控展示模块
        /// </summary>
        public bool? enabledPlannedModule { get; set; }
        /// <summary>
        /// 是否启用首页设备列表模块默认折叠
        /// </summary>
        public bool? homeDeviceListDefaultFold { get; set; }
        /// <summary>
        /// 是否启用首页设备查询列表模块
        /// </summary>
        public bool? enabledHomeDeviceQueryList { get; set; }
        /// <summary>
        /// 是否启用实时采集展示模块
        /// </summary>
        public bool? enabledRealTimeModule { get; set; }
        /// <summary>
        /// 是否启用维修统计展示模块
        /// </summary>
        public bool? enabledRepairCountShowModule { get; set; }
        /// <summary>
        /// 是否启用保养统计展示模块
        /// </summary>
        public bool? enabledUpkeepCountShowModule { get; set; }
        /// <summary>
        /// 是否启用异常统计展示模块
        /// </summary>
        public bool? enabledErrorsCountShowModule { get; set; }
        /// <summary>
        /// 是否启用物资查询模块
        /// </summary>
        public bool? enabledMaterialQueryModule { get; set; }
    }

    public class BussMonitorMenuSett
    {
        /// <summary>
        /// 是否自定义菜单
        /// </summary>
        public bool? isCustomMenu { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string menuName { get; set; }
        /// <summary>
        /// 菜单http网址
        /// </summary>
        public string menuUrl { get; set; }
    }
}
