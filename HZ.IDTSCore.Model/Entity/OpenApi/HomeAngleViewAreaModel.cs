using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    #region 首页视角区域名称定义项
    /// <summary>
    /// 首页视角区域名称定义项
    /// </summary>
    public class HomeAngleViewAreaModel
    {
        /// <summary>
        /// 首页视角项
        /// </summary>
        public HomeAngle HomeAngle { get; set; }
    }
    #endregion

    #region 首页视角项
    /// <summary>
    /// 首页视角项
    /// </summary>
    public class HomeAngle
    {
        /// <summary>
        /// 是否启用自动轮播
        /// </summary>
        public bool EnabledAutoView { get; set; }
        /// <summary>
        /// 自动轮播时间（秒）
        /// </summary>
        public int AutoViewInterval { get; set; }
        /// <summary>
        /// 视角信息列表
        /// </summary>
        public List<AngleView> AngleViewInfo { get; set; }
    }
    #endregion

    #region 视角信息项
    /// <summary>
    /// 视角信息项
    /// </summary>
    public class AngleView
    {
        /// <summary>
        /// 视角区域编号
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 视角区域名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 相机X坐标
        /// </summary>
        public float PosX { get; set; }
        /// <summary>
        /// 相机Y坐标
        /// </summary>
        public float PosY { get; set; }
        /// <summary>
        /// 相机Z坐标
        /// </summary>
        public float PosZ { get; set; }
        /// <summary>
        /// 角度X
        /// </summary>
        public float AngleX { get; set; }
        /// <summary>
        /// 角度Y
        /// </summary>
        public float AngleY { get; set; }
        /// <summary>
        /// 角度Z
        /// </summary>
        public float AngleZ { get; set; }
    }
    #endregion
}
