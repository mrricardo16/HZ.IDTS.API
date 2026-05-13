using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class CameraService : BaseService<tn_dts_camera>, ICameraService
    {
        public CameraService(SessionInfo session) : base(session)
        {

        }

        #region 新增相机
        /// <summary>
        /// 新增相机
        /// </summary>
        /// <param name="tn_Dts_Camera"></param>
        /// <returns></returns>
        public ReturnMessage AddCamera(tn_dts_camera tn_Dts_Camera)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            if (!(Db.Queryable<tn_dts_camera>().Where(it => it.cn_s_camera_code == tn_Dts_Camera.cn_s_camera_code).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "相机编号不能重复，请重试!";
                return returnMessage;
            }

            tn_dts_camera camera = new tn_dts_camera();
            camera.cn_guid = Guid.NewGuid().ToString();
            camera.cn_s_camera_area_guid = tn_Dts_Camera.cn_s_camera_area_guid;
            camera.cn_s_camera_serial = tn_Dts_Camera.cn_s_camera_serial;
            camera.cn_s_camera_code = tn_Dts_Camera.cn_s_camera_code;
            camera.cn_s_camera_name = tn_Dts_Camera.cn_s_camera_name;
            camera.cn_s_camera_enabledset = tn_Dts_Camera.cn_s_camera_enabledset;
            camera.cn_s_camera_enabledmaxwin = tn_Dts_Camera.cn_s_camera_enabledmaxwin;
            camera.cn_s_camera_posX = tn_Dts_Camera.cn_s_camera_posX;
            camera.cn_s_camera_posY = tn_Dts_Camera.cn_s_camera_posY;
            camera.cn_s_camera_posZ = tn_Dts_Camera.cn_s_camera_posZ;
            camera.cn_s_camera_angleX = tn_Dts_Camera.cn_s_camera_angleX;
            camera.cn_s_camera_angleY = tn_Dts_Camera.cn_s_camera_angleY;
            camera.cn_s_camera_angleZ = tn_Dts_Camera.cn_s_camera_angleZ;
            camera.cn_s_camera_remarks = tn_Dts_Camera.cn_s_camera_remarks;
            camera.cn_s_creator = user.UserCode;
            camera.cn_s_creator_by = user.UserName;
            camera.cn_t_create = DateTime.Now;
            int res = Db.Insertable(camera).ExecuteCommand();
            if (res <= 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "新增tn_dts_camera表失败！";
                return returnMessage;
            }

            returnMessage.IsSuccess = true;
            returnMessage.Message = "新增相机成功！";
            return returnMessage;
        }
        #endregion

        #region 按相机名称分页模糊查询
        /// <summary>
        /// 按相机名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_camera> GetListPages(PageParm parm)
        {
            //string cn_s_camera_name = parm.Parms["cn_s_camera_name"].ObjToString();
            //PagedInfo<tn_dts_camera> pagedCameraBefore = Db.Queryable<tn_dts_camera>()
            //.WhereIF(!string.IsNullOrEmpty(cn_s_camera_name), (s => s.cn_s_camera_name.Contains(cn_s_camera_name)))
            //.OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            //.ToPage(parm.PageIndex, parm.PageSize);
            //PagedInfo<tn_dts_camera> pagedCamera = new PagedInfo<tn_dts_camera>();
            //pagedCamera.PageIndex = pagedCameraBefore.PageIndex;
            //pagedCamera.PageSize = pagedCameraBefore.PageSize;
            //pagedCamera.TotalCount = pagedCameraBefore.TotalCount;
            //pagedCamera.TotalPages = pagedCameraBefore.TotalPages;
            //List<tn_dts_camera> cameraList = new List<tn_dts_camera>();
            //foreach (var camera in pagedCameraBefore.DataSource)
            //{
            //    string areacode = Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == camera.cn_s_camera_area_guid).Select(it => it.cn_s_camera_areacode).First();
            //    string areaname = Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == camera.cn_s_camera_area_guid).Select(it => it.cn_s_camera_areaname).First();
            //    camera.cn_s_camera_area_guid = areaname + "(" + areacode + ")";
            //    cameraList.Add(camera);
            //}
            //pagedCamera.DataSource = cameraList;
            //return pagedCamera;

            string cn_s_camera_name = parm.Parms["cn_s_camera_name"].ObjToString();
            return Db.Queryable<tn_dts_camera>()
            .LeftJoin<tn_dts_camera_area>((c, ca) => c.cn_s_camera_area_guid == ca.cn_guid)
            .Select((c, ca) => new tn_dts_camera
            {
                cn_guid = c.cn_guid,
                cn_s_camera_area_guid = SqlFunc.MergeString(ca.cn_s_camera_areaname, "(", ca.cn_s_camera_areacode, ")"),
                cn_s_camera_serial = c.cn_s_camera_serial,
                cn_s_camera_code = c.cn_s_camera_code,
                cn_s_camera_name = c.cn_s_camera_name,
                cn_s_camera_enabledset = c.cn_s_camera_enabledset,
                cn_s_camera_enabledmaxwin = c.cn_s_camera_enabledmaxwin,
                cn_s_camera_posX = c.cn_s_camera_posX,
                cn_s_camera_posY = c.cn_s_camera_posY,
                cn_s_camera_posZ = c.cn_s_camera_posZ,
                cn_s_camera_angleX = c.cn_s_camera_angleX,
                cn_s_camera_angleY = c.cn_s_camera_angleY,
                cn_s_camera_angleZ = c.cn_s_camera_angleZ,
                cn_s_camera_remarks = c.cn_s_camera_remarks,
                cn_s_modify = c.cn_s_modify,
                cn_s_modify_by = c.cn_s_modify_by,
                cn_t_modify = c.cn_t_modify,
                cn_s_creator = c.cn_s_creator,
                cn_s_creator_by = c.cn_s_creator_by,
                cn_t_create = c.cn_t_create
            })
            .WhereIF(!string.IsNullOrEmpty(cn_s_camera_name), (c => c.cn_s_camera_name.Contains(cn_s_camera_name)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 修改相机
        /// <summary>
        /// 修改相机
        /// </summary>
        /// <param name="tn_Dts_Camera"></param>
        /// <returns></returns>
        public ReturnMessage UpdateCamera(tn_dts_camera tn_Dts_Camera)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_camera itemGuid = Db.Queryable<tn_dts_camera>().Where(it => it.cn_guid == tn_Dts_Camera.cn_guid).First();
            if (itemGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "数据库中不存在唯一标识为：" + tn_Dts_Camera.cn_guid + "的记录!";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_camera>().Where(it => it.cn_s_camera_code == tn_Dts_Camera.cn_s_camera_code && it.cn_guid != tn_Dts_Camera.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "数据库中已经存在相机编号为：" + tn_Dts_Camera.cn_s_camera_code + "的相机，请重试!";
                return returnMessage;
            }

            itemGuid.cn_s_camera_area_guid = tn_Dts_Camera.cn_s_camera_area_guid;
            itemGuid.cn_s_camera_serial = tn_Dts_Camera.cn_s_camera_serial;
            itemGuid.cn_s_camera_code = tn_Dts_Camera.cn_s_camera_code;
            itemGuid.cn_s_camera_name = tn_Dts_Camera.cn_s_camera_name;
            itemGuid.cn_s_camera_enabledset = tn_Dts_Camera.cn_s_camera_enabledset;
            itemGuid.cn_s_camera_enabledmaxwin = tn_Dts_Camera.cn_s_camera_enabledmaxwin;
            itemGuid.cn_s_camera_posX = tn_Dts_Camera.cn_s_camera_posX;
            itemGuid.cn_s_camera_posY = tn_Dts_Camera.cn_s_camera_posY;
            itemGuid.cn_s_camera_posZ = tn_Dts_Camera.cn_s_camera_posZ;
            itemGuid.cn_s_camera_angleX = tn_Dts_Camera.cn_s_camera_angleX;
            itemGuid.cn_s_camera_angleY = tn_Dts_Camera.cn_s_camera_angleY;
            itemGuid.cn_s_camera_angleZ = tn_Dts_Camera.cn_s_camera_angleZ;
            itemGuid.cn_s_camera_remarks = tn_Dts_Camera.cn_s_camera_remarks;
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = Db.Updateable(itemGuid).ExecuteCommand();
            if (res <= 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "更新tn_dts_camera表失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改相机成功！";
            return returnMessage;
        }
        #endregion

        #region 批量删除相机
        /// <summary>
        /// 批量删除相机
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteCamera(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            foreach (var guid in guidList)
            {
                if (Db.Queryable<tn_dts_camera>().Where(it => it.cn_guid == guid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的相机中有相机的唯一标识不存在!";
                    return returnMessage;
                }
            }

            ApiResult res = UseTransaction(dbTran =>
            {
                foreach (string guid in guidList)
                {
                    dbTran.Deleteable<tn_dts_camera>().Where(it => it.cn_guid == guid).ExecuteCommand();
                }
            });
            if (!res.IsSuccess)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除相机失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除相机成功！";
            return returnMessage;
        }
        #endregion

        ///// <summary>
        ///// 删除相机
        ///// </summary>
        ///// <param name="tn_Dts_Camera"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteCamera(tn_dts_camera tn_Dts_Camera)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    if (Db.Queryable<tn_dts_camera>().Where(it => it.cn_guid == tn_Dts_Camera.cn_guid).First() is null)
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "数据库中不存在唯一标识为：" + tn_Dts_Camera.cn_guid + "的记录!";
        //        return returnMessage;
        //    }
        //    int res = Db.Deleteable<tn_dts_camera>().Where(it => it.cn_guid == tn_Dts_Camera.cn_guid).ExecuteCommand();
        //    if (res <= 0)
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "删除tn_dts_camera表失败！";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "删除相机成功！";
        //    return returnMessage;
        //}

        #region 获取所有区域的所有虚拟相机信息
        /// <summary>
        /// 获取所有区域的所有虚拟相机信息
        /// </summary>
        /// <returns></returns>
        public ReturnVirtualCamera GetAllVirtualCamera()
        {
            ReturnVirtualCamera returnVirtualCamera = new ReturnVirtualCamera();
            VirCamera vircamera = new VirCamera();
            string iss = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == "isShowSet").Select(it => it.cn_s_setting_keyvalue).First();
            if (String.IsNullOrEmpty(iss))
            {
                vircamera.isShowSet = null;
            }
            else
            {
                vircamera.isShowSet = bool.Parse(iss);
            }
            List<AreaMember> areaMemberList = new List<AreaMember>();
            List<string> areacodeList = Db.Queryable<tn_dts_camera_area>().OrderBy(it => it.cn_t_modify, OrderByType.Desc).Select(it => it.cn_s_camera_areacode).ToList();
            foreach (var areacode in areacodeList)
            {
                tn_dts_camera_area cameraarea = Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_s_camera_areacode == areacode).First();
                AreaMember areaMember = new AreaMember();
                areaMember.areacode = cameraarea.cn_s_camera_areacode;
                areaMember.areaname = cameraarea.cn_s_camera_areaname;
                if(cameraarea.cn_s_camera_areaimages is null)
                {
                    areaMember.areaimagesurl = "";
                }
                else
                {
                    areaMember.areaimagesurl = cameraarea.cn_s_camera_areaimages;
                }            
                List<CameraObjectMember> cameraObjectList = new List<CameraObjectMember>();
                //List<string> cameracodeList = Db.Queryable<tn_dts_camera>().Where(it => it.cn_s_camera_area_guid == cameraarea.cn_guid).OrderBy(it => it.cn_t_modify, OrderByType.Desc).Select(it => it.cn_s_camera_code).ToList();
                List<string> cameracodeList = Db.Queryable<tn_dts_camera>().Where(it => it.cn_s_camera_area_guid == cameraarea.cn_guid).OrderBy(it => it.cn_s_camera_serial).Select(it => it.cn_s_camera_code).ToList();
                foreach (var cameracode in cameracodeList)
                {
                    tn_dts_camera camera = Db.Queryable<tn_dts_camera>().Where(it => it.cn_s_camera_code == cameracode).First();
                    CameraObjectMember cameraObjectMember = new CameraObjectMember();
                    cameraObjectMember.serial = camera.cn_s_camera_serial;
                    cameraObjectMember.code = camera.cn_s_camera_code;
                    cameraObjectMember.name = camera.cn_s_camera_name;
                    cameraObjectMember.enabledset = camera.cn_s_camera_enabledset;
                    cameraObjectMember.enabledmaxwin = camera.cn_s_camera_enabledmaxwin;
                    cameraObjectMember.posX = camera.cn_s_camera_posX;
                    cameraObjectMember.posY = camera.cn_s_camera_posY;
                    cameraObjectMember.posZ = camera.cn_s_camera_posZ;
                    cameraObjectMember.angleX = camera.cn_s_camera_angleX;
                    cameraObjectMember.angleY = camera.cn_s_camera_angleY;
                    cameraObjectMember.angleZ = camera.cn_s_camera_angleZ;
                    cameraObjectList.Add(cameraObjectMember);
                }
                areaMember.cameraobject = cameraObjectList;
                areaMemberList.Add(areaMember);
            }
            vircamera.areas = areaMemberList;
            returnVirtualCamera.vircamera = vircamera;
            return returnVirtualCamera;
        }
        #endregion
    }
}
