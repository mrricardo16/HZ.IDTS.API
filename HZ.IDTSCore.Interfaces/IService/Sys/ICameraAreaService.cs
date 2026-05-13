using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface ICameraAreaService : IBaseService<tn_dts_camera_area>
    {
        /// <summary>
        /// 新增相机区域
        /// </summary>
        /// <param name="cameraAreaIncrease"></param>
        /// <returns></returns>
        public ReturnMessage AddCameraArea(CameraAreaIncrease cameraAreaIncrease);

        /// <summary>
        /// 按区域名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_camera_area> GetListPages(PageParm parm);

        /// <summary>
        /// 修改相机区域
        /// </summary>
        /// <param name="tn_Dts_Camera"></param>
        /// <returns></returns>
        public ReturnMessage UpdateCameraArea(tn_dts_camera_area tn_Dts_Camera_Area);

        /// <summary>
        /// 删除相机区域
        /// </summary>
        /// <param name="tn_Dts_Camera"></param>
        /// <returns></returns>
        public ReturnMessage DeleteCameraArea(List<string> guidList);

        /// <summary>
        /// 获取所有相机区域坐标系
        /// </summary>
        /// <returns></returns>
        public List<CameraAreaPoint> GetAllPoint();

        /// <summary>
        /// 匹配指定点所在相机区域
        /// </summary>
        /// <param name="cameraAreaPointList">最新相机区域坐标系列表</param>
        /// <param name="matchPoint">匹配点二维坐标</param>
        /// <returns></returns>
        public List<ReturnCameraAreaPoint> MatchArea(List<CameraAreaPoint> cameraAreaPointList, MatchPoint matchPoint);
    }
}
