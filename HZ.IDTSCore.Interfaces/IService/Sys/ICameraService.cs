using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.IService.Sys
{
    public interface ICameraService : IBaseService<tn_dts_camera>
    {
        /// <summary>
        /// 新增相机
        /// </summary>
        /// <param name="tn_Dts_Camera"></param>
        /// <returns></returns>
        public ReturnMessage AddCamera(tn_dts_camera tn_Dts_Camera);

        /// <summary>
        /// 按相机名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_camera> GetListPages(PageParm parm);

        /// <summary>
        /// 修改相机
        /// </summary>
        /// <param name="tn_Dts_Camera"></param>
        /// <returns></returns>
        public ReturnMessage UpdateCamera(tn_dts_camera tn_Dts_Camera);

        /// <summary>
        /// 批量删除相机
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteCamera(List<string> guidList);

        /// <summary>
        /// 获取所有区域的所有虚拟相机信息
        /// </summary>
        /// <returns></returns>
        public ReturnVirtualCamera GetAllVirtualCamera();
    }
}
