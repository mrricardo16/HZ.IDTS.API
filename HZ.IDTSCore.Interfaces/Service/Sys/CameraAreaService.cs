using HZ.IDTSCore.Model.Entity.Sys;
using HZ.IDTSCore.Interfaces.IService.Sys;
using System;
using System.Collections.Generic;
using System.Text;
using HZ.DbHelper;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.CommonUtil.Model;
using SqlSugar.Extensions;
using System.IO;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using HZ.CommonUtil.Helpers;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class CameraAreaService : BaseService<tn_dts_camera_area>, ICameraAreaService
    {
        public CameraAreaService(SessionInfo session) : base(session)
        {

        }

        #region 新增相机区域
        /// <summary>
        /// 新增相机区域
        /// </summary>
        /// <param name="cameraAreaIncrease"></param>
        /// <returns></returns>
        public ReturnMessage AddCameraArea(CameraAreaIncrease cameraAreaIncrease)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            tn_dts_camera_area newCameraArea = cameraAreaIncrease.NewCameraArea;
            if (!(Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_s_camera_areacode == newCameraArea.cn_s_camera_areacode).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "区域编号不能重复，请重试!";
                return returnMessage;
            }

            tn_dts_camera_area cameraarea = new tn_dts_camera_area();
            tn_dts_files file = new tn_dts_files();
            string images = string.Empty;
            string guidArea = Guid.NewGuid().ToString();
            string guidFile = cameraAreaIncrease.FileGuid;
            if (cameraAreaIncrease.HasPicture)
            {             
                string fileName = guidFile + '.' + cameraAreaIncrease.FileName.Split('.')[1];
                images = "http://" + cameraAreaIncrease.IpAddress + ":" + cameraAreaIncrease.Port + "/api/Files/DownloadFile?file=" + fileName;
            }
            else
            {
                images = null;
            }
            cameraarea.cn_guid = guidArea;
            cameraarea.cn_s_camera_areacode = newCameraArea.cn_s_camera_areacode;
            cameraarea.cn_s_camera_areaname = newCameraArea.cn_s_camera_areaname;
            cameraarea.cn_s_camera_areaimages = images;
            cameraarea.cn_s_camera_arearemarks = newCameraArea.cn_s_camera_arearemarks;
            cameraarea.cn_s_camera_points = newCameraArea.cn_s_camera_points;
            cameraarea.cn_s_creator = user.UserCode;
            cameraarea.cn_s_creator_by = user.UserName;
            cameraarea.cn_t_create = DateTime.Now;
            tn_dts_logs cameraAreaLog = new tn_dts_logs();
            cameraAreaLog.cn_guid = Guid.NewGuid().ToString();
            cameraAreaLog.cn_s_logs_type = "操作";
            cameraAreaLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_camera_area表中新增一条虚拟相机区域记录，详细信息为" + JsonConvert.SerializeObject(cameraarea);
            cameraAreaLog.cn_t_create = DateTime.Now;
            logList.Add(cameraAreaLog);
            if(cameraAreaIncrease.HasPicture)
            {
                file.cn_guid = guidFile;
                file.cn_s_files_source_guid = guidArea;
                file.cn_s_files_source_module = "虚拟相机模块";
                file.cn_s_files_type = cameraAreaIncrease.FileName.Split('.')[1];
                file.cn_s_files_name = cameraAreaIncrease.FileName.Split('.')[0];
                file.cn_s_files_extenname = cameraAreaIncrease.FileName.Split('.')[1];
                file.cn_s_files_size = cameraAreaIncrease.Size;
                file.cn_s_files_rootpath = cameraAreaIncrease.IpAddress;
                file.cn_s_files_oppopath = cameraAreaIncrease.OppoPath;
                file.cn_s_files_abspath = cameraAreaIncrease.AbsPath;
                file.cn_s_creator = user.UserCode;
                file.cn_s_creator_by = user.UserName;
                file.cn_t_create = DateTime.Now;
                tn_dts_logs fileLog = new tn_dts_logs();
                fileLog.cn_guid = Guid.NewGuid().ToString();
                fileLog.cn_s_logs_type = "操作";
                fileLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_files表中新增一条区域静态图记录，详细信息为" + JsonConvert.SerializeObject(file);
                fileLog.cn_t_create = DateTime.Now;
                logList.Add(fileLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_camera_area>(cameraarea).ExecuteCommand();
                if(cameraAreaIncrease.HasPicture)
                {
                    dbTran.Insertable<tn_dts_files>(file).ExecuteCommand();
                }
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "12.5相机（cameraarea）区域管理（带文件上传功能）新增接口新增相机区域失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "新增失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "新增成功！";
            return returnMessage;
        }
        #endregion

        //#region 新增相机区域
        ///// <summary>
        ///// 新增相机区域
        ///// </summary>
        ///// <param name="cameraAreaIncrease"></param>
        ///// <returns></returns>
        //public ReturnMessage AddCameraArea(CameraAreaIncrease cameraAreaIncrease)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    List<tn_dts_logs> logList = new List<tn_dts_logs>();
        //    tn_dts_camera_area newCameraArea = cameraAreaIncrease.NewCameraArea;
        //    if (!(Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_s_camera_areacode == newCameraArea.cn_s_camera_areacode).First() is null))
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "区域编号不能重复，请重试!";
        //        return returnMessage;
        //    }

        //    tn_dts_camera_area cameraarea = new tn_dts_camera_area();
        //    tn_dts_files file = new tn_dts_files();
        //    string images = string.Empty;
        //    string guidArea = Guid.NewGuid().ToString();
        //    string guidFile = cameraAreaIncrease.FileGuid;
        //    string fileFullName = guidFile + "_" + cameraAreaIncrease.FileName;
        //    if (cameraAreaIncrease.HasPicture)
        //    {
        //        images = "http://" + cameraAreaIncrease.IpAddress + ":" + cameraAreaIncrease.Port + "/api/Files/DownloadFile?file=" + fileFullName;
        //    }
        //    else
        //    {
        //        images = null;
        //    }
        //    cameraarea.cn_guid = guidArea;
        //    cameraarea.cn_s_camera_areacode = newCameraArea.cn_s_camera_areacode;
        //    cameraarea.cn_s_camera_areaname = newCameraArea.cn_s_camera_areaname;
        //    cameraarea.cn_s_camera_areaimages = images;
        //    cameraarea.cn_s_camera_arearemarks = newCameraArea.cn_s_camera_arearemarks;
        //    cameraarea.cn_s_camera_points = newCameraArea.cn_s_camera_points;
        //    cameraarea.cn_s_creator = user.UserCode;
        //    cameraarea.cn_s_creator_by = user.UserName;
        //    cameraarea.cn_t_create = DateTime.Now;
        //    tn_dts_logs cameraAreaLog = new tn_dts_logs();
        //    cameraAreaLog.cn_guid = Guid.NewGuid().ToString();
        //    cameraAreaLog.cn_s_logs_type = "操作";
        //    cameraAreaLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_camera_area表中新增一条虚拟相机区域记录，详细信息为" + JsonConvert.SerializeObject(cameraarea);
        //    cameraAreaLog.cn_t_create = DateTime.Now;
        //    logList.Add(cameraAreaLog);
        //    if (cameraAreaIncrease.HasPicture)
        //    {
        //        file.cn_guid = guidFile;
        //        file.cn_s_files_source_guid = guidArea;
        //        file.cn_s_files_source_module = "虚拟相机模块";
        //        file.cn_s_files_type = cameraAreaIncrease.FileName.Split('.')[1];
        //        file.cn_s_files_name = cameraAreaIncrease.FileName.Split('.')[0];
        //        file.cn_s_files_extenname = cameraAreaIncrease.FileName.Split('.')[1];
        //        file.cn_s_files_size = cameraAreaIncrease.Size;
        //        file.cn_s_files_rootpath = cameraAreaIncrease.IpAddress;
        //        file.cn_s_files_oppopath = cameraAreaIncrease.OppoPath;
        //        file.cn_s_files_abspath = cameraAreaIncrease.AbsPath;
        //        file.cn_s_creator = user.UserCode;
        //        file.cn_s_creator_by = user.UserName;
        //        file.cn_t_create = DateTime.Now;
        //        tn_dts_logs fileLog = new tn_dts_logs();
        //        fileLog.cn_guid = Guid.NewGuid().ToString();
        //        fileLog.cn_s_logs_type = "操作";
        //        fileLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_files表中新增一条区域静态图记录，详细信息为" + JsonConvert.SerializeObject(file);
        //        fileLog.cn_t_create = DateTime.Now;
        //        logList.Add(fileLog);
        //    }
        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        dbTran.Insertable<tn_dts_camera_area>(cameraarea).ExecuteCommand();
        //        if (cameraAreaIncrease.HasPicture)
        //        {
        //            dbTran.Insertable<tn_dts_files>(file).ExecuteCommand();
        //        }
        //        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        LogHelper.Info(DateTime.Now.ToString() + "12.5相机（cameraarea）区域管理（带文件上传功能）新增接口新增相机区域失败，详细信息为：" + res.Message);
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "新增失败!";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "新增成功！";
        //    return returnMessage;
        //}
        //#endregion

        #region 按区域名称分页模糊查询
        /// <summary>
        /// 按区域名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_camera_area> GetListPages(PageParm parm)
        {
            string cn_s_camera_areaname = parm.Parms["cn_s_camera_areaname"].ObjToString();
            return Db.Queryable<tn_dts_camera_area>()
            .WhereIF(!string.IsNullOrEmpty(cn_s_camera_areaname), (s => s.cn_s_camera_areaname.Contains(cn_s_camera_areaname)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 修改相机区域
        /// <summary>
        /// 修改相机区域
        /// </summary>
        /// <param name="tn_Dts_Camera"></param>
        /// <returns></returns>
        public ReturnMessage UpdateCameraArea(tn_dts_camera_area tn_Dts_Camera_Area)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_camera_area itemGuid = Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == tn_Dts_Camera_Area.cn_guid).First();
            if (itemGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "数据库中不存在唯一标识为：" + tn_Dts_Camera_Area.cn_guid + "的记录!";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_s_camera_areacode == tn_Dts_Camera_Area.cn_s_camera_areacode && it.cn_guid != tn_Dts_Camera_Area.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "数据库中已经存在区域编号为：" + tn_Dts_Camera_Area.cn_s_camera_areacode + "的相机区域，请重试!";
                return returnMessage;
            }
            itemGuid.cn_s_camera_areacode = tn_Dts_Camera_Area.cn_s_camera_areacode;
            itemGuid.cn_s_camera_areaname = tn_Dts_Camera_Area.cn_s_camera_areaname;
            if (tn_Dts_Camera_Area.cn_s_camera_areaimages == "")
            {
                itemGuid.cn_s_camera_areaimages = null;
            }
            itemGuid.cn_s_camera_points = tn_Dts_Camera_Area.cn_s_camera_points;
            itemGuid.cn_s_camera_arearemarks = tn_Dts_Camera_Area.cn_s_camera_arearemarks;
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = Db.Updateable(itemGuid).ExecuteCommand();
            if (res <= 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "更新tn_dts_camera_area表失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改相机区域成功！";
            return returnMessage;
        }
        #endregion

        //#region 修改相机区域
        ///// <summary>
        ///// 修改相机区域
        ///// </summary>
        ///// <param name="tn_Dts_Camera"></param>
        ///// <returns></returns>
        //public ReturnMessage UpdateCameraArea(tn_dts_camera_area tn_Dts_Camera_Area)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    tn_dts_camera_area itemGuid = Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == tn_Dts_Camera_Area.cn_guid).First();
        //    if (itemGuid is null)
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "数据库中不存在唯一标识为：" + tn_Dts_Camera_Area.cn_guid + "的记录!";
        //        return returnMessage;
        //    }
        //    if (!(Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_s_camera_areacode == tn_Dts_Camera_Area.cn_s_camera_areacode && it.cn_guid != tn_Dts_Camera_Area.cn_guid).First() is null))
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "数据库中已经存在区域编号为：" + tn_Dts_Camera_Area.cn_s_camera_areacode + "的相机区域，请重试!";
        //        return returnMessage;
        //    }
        //    itemGuid.cn_s_camera_areacode = tn_Dts_Camera_Area.cn_s_camera_areacode;
        //    itemGuid.cn_s_camera_areaname = tn_Dts_Camera_Area.cn_s_camera_areaname;
        //    if (tn_Dts_Camera_Area.cn_s_camera_areaimages == "")
        //    {
        //        itemGuid.cn_s_camera_areaimages = null;
        //    }
        //    itemGuid.cn_s_camera_points = tn_Dts_Camera_Area.cn_s_camera_points;
        //    itemGuid.cn_s_camera_arearemarks = tn_Dts_Camera_Area.cn_s_camera_arearemarks;
        //    itemGuid.cn_s_modify = user.UserCode;
        //    itemGuid.cn_s_modify_by = user.UserName;
        //    itemGuid.cn_t_modify = DateTime.Now;
        //    int res = Db.Updateable(itemGuid).ExecuteCommand();
        //    if (res <= 0)
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "更新tn_dts_camera_area表失败！";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "修改相机区域成功！";
        //    return returnMessage;
        //}
        //#endregion

        #region 批量删除相机区域
        /// <summary>
        /// 批量删除相机区域
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteCameraArea(List<string> guidList)
        {
            string UploadsFolder = "uploads";
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            foreach (string guid in guidList)
            {
                if (Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == guid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的相机区域中有相机区域的唯一标识不存在!";
                    return returnMessage;
                }
                if (!(Db.Queryable<tn_dts_camera>().Where(it => it.cn_s_camera_area_guid == guid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的相机区域中有相机区域有相机区域下有虚拟相机，请先删除虚拟相机！";
                    return returnMessage;
                }

            }

            List<tn_dts_camera_area> areaMapList = new List<tn_dts_camera_area>();
            foreach (string guid in guidList)
            {
                tn_dts_camera_area area = Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == guid).First();
                if (!string.IsNullOrEmpty(area.cn_s_camera_areaimages))
                {
                    areaMapList.Add(area);
                }
            }

            foreach (tn_dts_camera_area areaMap in areaMapList)
            {
                string file = areaMap.cn_s_camera_areaimages.Split('=')[1];
                var filePath = Path.Combine(UploadsFolder, file);

                if (!System.IO.File.Exists(filePath))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的有区域静态图的相机区域中,有不在服务器上传文件夹中的区域静态图！";
                    return returnMessage;
                }
            }

            ApiResult res = UseTransaction(dbTran =>
            {
                foreach (string guid in guidList)
                {
                    dbTran.Deleteable<tn_dts_camera_area>().Where(it => it.cn_guid == guid).ExecuteCommand();
                }

                foreach (tn_dts_camera_area areaMap in areaMapList)
                {
                    dbTran.Deleteable<tn_dts_files>().Where(it => it.cn_s_files_source_guid == areaMap.cn_guid).ExecuteCommand();
                }
            });
            if (!res.IsSuccess)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除相机区域失败!";
                return returnMessage;
            }

            foreach (tn_dts_camera_area areaMap in areaMapList)
            {
                string file = areaMap.cn_s_camera_areaimages.Split('=')[1];
                var filePath = Path.Combine(UploadsFolder, file);             
                System.IO.File.Delete(filePath);
            }

            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除相机区域成功！";
            return returnMessage;
        }
        #endregion

        //#region 批量删除相机区域
        ///// <summary>
        ///// 批量删除相机区域
        ///// </summary>
        ///// <param name="guidList"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteCameraArea(List<string> guidList)
        //{
        //    string UploadsFolder = "uploads";
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    foreach (string guid in guidList)
        //    {
        //        if (Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == guid).First() is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "选中的相机区域中有相机区域的唯一标识不存在!";
        //            return returnMessage;
        //        }
        //        if (!(Db.Queryable<tn_dts_camera>().Where(it => it.cn_s_camera_area_guid == guid).First() is null))
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "选中的相机区域中有相机区域有相机区域下有虚拟相机，请先删除虚拟相机！";
        //            return returnMessage;
        //        }

        //    }

        //    List<tn_dts_camera_area> areaMapList = new List<tn_dts_camera_area>();
        //    foreach (string guid in guidList)
        //    {
        //        tn_dts_camera_area area = Db.Queryable<tn_dts_camera_area>().Where(it => it.cn_guid == guid).First();
        //        if (!string.IsNullOrEmpty(area.cn_s_camera_areaimages))
        //        {
        //            areaMapList.Add(area);
        //        }
        //    }

        //    foreach (tn_dts_camera_area areaMap in areaMapList)
        //    {
        //        string file = areaMap.cn_s_camera_areaimages.Split('=')[1];
        //        var filePath = Path.Combine(UploadsFolder, file);

        //        if (!System.IO.File.Exists(filePath))
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "选中的有区域静态图的相机区域中,有不在服务器上传文件夹中的区域静态图！";
        //            return returnMessage;
        //        }
        //    }

        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        foreach (string guid in guidList)
        //        {
        //            dbTran.Deleteable<tn_dts_camera_area>().Where(it => it.cn_guid == guid).ExecuteCommand();
        //        }

        //        foreach (tn_dts_camera_area areaMap in areaMapList)
        //        {
        //            dbTran.Deleteable<tn_dts_files>().Where(it => it.cn_s_files_source_guid == areaMap.cn_guid).ExecuteCommand();
        //        }
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "批量删除相机区域失败!";
        //        return returnMessage;
        //    }

        //    foreach (tn_dts_camera_area areaMap in areaMapList)
        //    {
        //        string file = areaMap.cn_s_camera_areaimages.Split('=')[1];
        //        var filePath = Path.Combine(UploadsFolder, file);
        //        System.IO.File.Delete(filePath);
        //    }

        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "删除相机区域成功！";
        //    return returnMessage;
        //}
        //#endregion

        #region 获取所有相机区域坐标系
        /// <summary>
        /// 获取所有相机区域坐标系
        /// </summary>
        /// <returns></returns>
        public List<CameraAreaPoint> GetAllPoint()
        {
            List<CameraAreaPoint> cameraAreaPointList = new List<CameraAreaPoint>();
            try
            {
                List<tn_dts_camera_area> cameraAreaList = Db.Queryable<tn_dts_camera_area>().ToList();
                foreach (var cameraArea in cameraAreaList)
                {
                    if (!string.IsNullOrEmpty(cameraArea.cn_s_camera_points))
                    {
                        CameraAreaPoint cameraAreaPoint = new CameraAreaPoint();
                        cameraAreaPoint.AreaCode = cameraArea.cn_s_camera_areacode;
                        cameraAreaPoint.AreaName = cameraArea.cn_s_camera_areaname;
                        cameraAreaPoint.Points = cameraArea.cn_s_camera_points;
                        cameraAreaPointList.Add(cameraAreaPoint);
                    }
                }
            }
            catch { }
            return cameraAreaPointList;
        }
        #endregion

        #region 匹配指定点所在相机区域
        /// <summary>
        /// 匹配指定点所在相机区域
        /// </summary>
        /// <param name="cameraAreaPointList">最新相机区域坐标系列表</param>
        /// <param name="matchPoint">匹配点二维坐标</param>
        /// <returns></returns>
        public List<ReturnCameraAreaPoint> MatchArea(List<CameraAreaPoint> cameraAreaPointList, MatchPoint matchPoint)
        {
            PointF point = new PointF(Convert.ToSingle(matchPoint.PosX), Convert.ToSingle(matchPoint.PosY));
            List<ReturnCameraAreaPoint> returnCameraAreaPointList = new List<ReturnCameraAreaPoint>();
            foreach (var cameraAreaPoint in cameraAreaPointList)
            {
                List<PointF> pointfList = JsonConvert.DeserializeObject<List<PointF>>(cameraAreaPoint.Points);   
                if (pointfList != null && pointfList.Count != 0 && pointfList.Count != 1 && pointfList.Count != 2)
                {
                    Region region = new Region();
                    GraphicsPath graphicsPath = new GraphicsPath();
                    graphicsPath.Reset();
                    graphicsPath.AddPolygon(pointfList.ToArray());
                    region.MakeEmpty();
                    region.Union(graphicsPath);
                    bool isvisible = region.IsVisible(point);
                    if (isvisible)
                    {
                        ReturnCameraAreaPoint returnCameraAreaPoint = new ReturnCameraAreaPoint();
                        returnCameraAreaPoint.AreaCode = cameraAreaPoint.AreaCode;
                        returnCameraAreaPoint.AreaName = cameraAreaPoint.AreaName;
                        returnCameraAreaPointList.Add(returnCameraAreaPoint);
                    }
                }
            }        
            return returnCameraAreaPointList;
        }
        #endregion
    }
}
