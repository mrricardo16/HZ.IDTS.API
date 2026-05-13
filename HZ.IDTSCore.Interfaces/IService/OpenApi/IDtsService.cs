using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;

using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.OpenApi
{
    public interface IDtsService
    {
        /// <summary>
        /// 通用初始化接口V2
        /// </summary>
        /// <returns>通用初始化信息</returns>
        public ViewSystemconfigModel GetSystemconfigV2();

        /// <summary>
        /// 通用初始化接口
        /// </summary>
        /// <param name="locationRealMonitorViewModel">WMS料架信息</param>
        /// <returns></returns>
        public Newtonsoft.Json.Linq.JObject GetSystemconfig(LocationRealMonitorViewModel locationRealMonitorViewModel);

        /// <summary>
        /// 获取零部件（只查一级）
        /// </summary>
        /// <param name="partDetail"></param>
        /// <returns></returns>
        public ReturnPartDetail GetPartDetail(PartDetail partDetail);

        /// <summary>
        /// 通用新增日志接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ReturnMessage UniversallyAddLog(OpenLog model);

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <returns></returns>
        public MenuAuthorityModel GetMenu();

        /// <summary>
        /// 获取首页视角区域
        /// </summary>
        /// <returns></returns>
        public HomeAngleViewAreaModel GetHomeAngle();
    }
}
