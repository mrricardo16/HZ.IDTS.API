using SqlSugar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 相机区域表
    /// <summary>
    /// 相机区域表
    ///</summary>
    [SugarTable("tn_dts_camera_area")]
    public class tn_dts_camera_area
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  区域编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_areacode")]
        public string cn_s_camera_areacode { get; set; }
        /// <summary>
        ///  区域名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_areaname")]
        public string cn_s_camera_areaname { get; set; }
        /// <summary>
        ///  区域静态图
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_areaimages")]
        public string cn_s_camera_areaimages { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_arearemarks")]
        public string cn_s_camera_arearemarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify_by")]
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  创建人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator")]
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  创建人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator_by")]
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
        /// <summary>
        ///  区域坐标系
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_points")]
        public string cn_s_camera_points { get; set; }
    }
    #endregion

    public class ReturnVirtualCamera
    {
        /// <summary>
        /// 虚拟相机
        /// </summary>
        public VirCamera vircamera { get; set; }
    }

    public class VirCamera
    {
        /// <summary>
        /// 首页是否显示设置按钮
        /// </summary>
        public bool? isShowSet { get; set; }
        /// <summary>
        /// 所有相机区域及其相机信息
        /// </summary>
        public List<AreaMember> areas { get; set; }
    }

    public class AreaMember
    {
        /// <summary>
        /// 区域编号
        /// </summary>
        public string areacode { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string areaname { get; set; }
        /// <summary>
        /// 区域静态缩列图
        /// </summary>
        public string areaimagesurl { get; set; }
        /// <summary>
        /// 区域内所有相机信息
        /// </summary>
        public List<CameraObjectMember> cameraobject { get; set; }
    }

    public class CameraObjectMember
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int? serial { get; set; }
        /// <summary>
        /// 相机编号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 相机名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 是否启用设置功能
        /// </summary>
        public bool? enabledset { get; set; }
        /// <summary>
        /// 是否启用最大化预览
        /// </summary>
        public bool? enabledmaxwin { get; set; }
        /// <summary>
        /// 相机X坐标
        /// </summary>
        public float? posX { get; set; }
        /// <summary>
        /// 相机Y坐标
        /// </summary>
        public float? posY { get; set; }
        /// <summary>
        /// 相机Z坐标
        /// </summary>
        public float? posZ { get; set; }
        /// <summary>
        /// 角度X
        /// </summary>
        public float? angleX { get; set; }
        /// <summary>
        /// 角度Y
        /// </summary>
        public float? angleY { get; set; }
        /// <summary>
        /// 角度Z
        /// </summary>
        public float? angleZ { get; set; }
    }

    #region 相机区域坐标系类
    /// <summary>
    /// 相机区域坐标系类
    /// </summary>
    public class CameraAreaPoint
    {
        /// <summary>
        /// 区域编号
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 区域坐标系
        /// </summary>
        public string Points { get; set; }
    }
    #endregion

    //#region 匹配传入数据类
    ///// <summary>
    ///// 匹配传入数据类
    ///// </summary>
    //public class MatchArea
    //{
    //    /// <summary>
    //    /// 相机区域二维坐标系
    //    /// </summary>
    //    public CameraAreaPoint CameraAreaPoint { get; set; }
    //    /// <summary>
    //    /// 匹配点坐标
    //    /// </summary>
    //    public MatchPoint Point { get; set; }

    //}
    //#endregion

    #region 匹配区域返回数据类
    /// <summary>
    /// 匹配区域返回数据类
    /// </summary>
    public class ReturnCameraAreaPoint
    {
        /// <summary>
        /// 区域编号
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaName { get; set; }
    }
    #endregion
    /// <summary>
    /// 匹配点数据类
    /// </summary>
    public class MatchPoint
    {
        /// <summary>
        /// 匹配点X坐标
        /// </summary>
        public string PosX { get; set; }
        /// <summary>
        /// 匹配点Y坐标
        /// </summary>
        public string PosY { get; set; }
    }

    #region 新增相机区域数据类
    /// <summary>
    /// 新增相机区域数据类
    /// </summary>
    public class CameraAreaIncrease
    {
        /// <summary>
        /// 新增的相机区域是否有静态图
        /// </summary>
        public bool HasPicture { get; set; }
        /// <summary>
        /// 新增的相机区域
        /// </summary>
        public tn_dts_camera_area NewCameraArea { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 区域静态图名称（含文件拓展名）
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 区域静态图大小
        /// </summary>
        public decimal Size { get; set; }
        /// <summary>
        /// 相对路径
        /// </summary>
        public string OppoPath { get; set; }
        /// <summary>
        /// 绝对路径
        /// </summary>
        public string AbsPath { get; set; }
        /// <summary>
        /// 文件唯一标识
        /// </summary>
        public string FileGuid { get; set; }
    }
    #endregion

    public class CameraAreaUpdate
    {
        /// <summary>
        /// fromdate里有无图片
        /// </summary>
        public bool HasFromdate { get; set; }
        /// <summary>
        /// 是否删除原有静态图
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 修改后的相机区域
        /// </summary>
        public tn_dts_camera_area NewCameraArea { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// 区域静态图名称（含文件拓展名）
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 区域静态图大小
        /// </summary>
        public decimal Size { get; set; }
        /// <summary>
        /// 相对路径
        /// </summary>
        public string OppoPath { get; set; }
        /// <summary>
        /// 绝对路径
        /// </summary>
        public string AbsPath { get; set; }
        /// <summary>
        /// 文件唯一标识
        /// </summary>
        public string FileGuid { get; set; }
    }
}
