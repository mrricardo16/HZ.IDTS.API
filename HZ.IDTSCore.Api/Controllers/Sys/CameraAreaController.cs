using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class CameraAreaController : BaseController
    {
        private ICameraAreaService _ICameraAreaService;
        private ICameraService _ICameraService;
        private IFilesService _IFilesService;

        public CameraAreaController()
        {
            _ICameraService = ServiceLocator.GetService<ICameraService>(HttpContextSession());
            _ICameraAreaService = ServiceLocator.GetService<ICameraAreaService>(HttpContextSession());
            _IFilesService = ServiceLocator.GetService<IFilesService>(HttpContextSession());
        }

        #region 新增相机区域
        /// <summary>
        /// 新增相机区域
        /// </summary>
        /// <param name="areacode">区域编号</param>
        /// <param name="areaname">区域名称</param>
        /// <param name="arearemarks">备注</param>
        /// <param name="areapoints">区域坐标系</param>
        /// <param name="haspicture">用户新增时是否上传区域静态图</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddCameraArea(string areacode, string areaname, string arearemarks, string areapoints, bool haspicture)
        {
            string UploadsFolder = "uploads";
            ApiResult resultReturn = new ApiResult();
            CameraAreaIncrease cameraAreaIncrease = new CameraAreaIncrease();
            tn_dts_camera_area newCameraArea = new tn_dts_camera_area()
            {
                cn_s_camera_areacode = areacode,
                cn_s_camera_areaname = areaname,
                cn_s_camera_arearemarks = arearemarks,
                cn_s_camera_points = areapoints
            };
            cameraAreaIncrease.NewCameraArea = newCameraArea;
            cameraAreaIncrease.HasPicture = haspicture;
            if (haspicture == true)
            {
                HttpRequest httpRequest = Request;
                if (httpRequest.ContentLength <= 0)
                {
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "请求接口参数不对，请重试！";
                    return toResponse(resultReturn);
                }
                var files = Request.Form.Files;
                if (files == null || files.Count == 0)
                {
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "没有选择要上传的文件，请重试！";
                    return toResponse(resultReturn);
                }
                // 创建上传文件保存的文件夹（如果不存在）
                var directoryInfo = Directory.CreateDirectory(UploadsFolder);
                foreach (var file in files)
                {
                    string fileGuid = Guid.NewGuid().ToString();
                    string extension = file.FileName.Split('.')[1];
                    string fileName = fileGuid + "." + extension;
                    var filePath = Path.Combine(UploadsFolder, fileName);
                    // 检查文件是否已经部分上传
                    var fileExists = System.IO.File.Exists(filePath);
                    if (fileExists)
                    {
                        // 如果文件已经存在，获取已上传的字节数
                        var existingFileSize = new FileInfo(filePath).Length;
                        // 如果已上传的字节数等于文件总大小，说明文件已完整上传，直接跳过
                        if (existingFileSize == file.Length)
                        {
                            continue;
                        }
                        // 如果已上传的字节数小于文件总大小，说明文件部分上传，进行断点续传
                        using (var fileStream = new FileStream(filePath, FileMode.Append))
                        {
                            // 设置文件指针位置，从已上传的字节数开始
                            fileStream.Seek(existingFileSize, SeekOrigin.Begin);
                            await file.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        // 如果文件不存在，直接创建新文件并上传
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }

                    IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if(ipaddress is null)
                    {
                        cameraAreaIncrease.IpAddress = null;
                    }
                    else
                    {
                        cameraAreaIncrease.IpAddress = ipaddress.ToString();
                    }
                    cameraAreaIncrease.Port = Request.Host.Port.ToString();
                    cameraAreaIncrease.FileName = file.FileName;
                    cameraAreaIncrease.Size = file.Length;
                    cameraAreaIncrease.OppoPath = Path.Combine(UploadsFolder, fileName);
                    cameraAreaIncrease.AbsPath = Path.Combine(directoryInfo.FullName, fileName);
                    cameraAreaIncrease.FileGuid = fileGuid;
                }
            }

            ReturnMessage res = _ICameraAreaService.AddCameraArea(cameraAreaIncrease);
            if (res.IsSuccess)
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Success;
                resultReturn.Message = res.Message;
            }
            else
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = res.Message;
            }
            Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
            CameraAreaDriver.Instance.LatestCameraAreaPointList = _ICameraAreaService.GetAllPoint();
            return toResponse(resultReturn);
        }
        #endregion

        //#region 新增相机区域
        ///// <summary>
        ///// 新增相机区域
        ///// </summary>
        ///// <param name="areacode">区域编号</param>
        ///// <param name="areaname">区域名称</param>
        ///// <param name="arearemarks">备注</param>
        ///// <param name="areapoints">区域坐标系</param>
        ///// <param name="haspicture">用户新增时是否上传区域静态图</param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> AddCameraArea(string areacode, string areaname, string arearemarks, string areapoints, bool haspicture)
        //{
        //    string UploadsFolder = "uploads";
        //    ApiResult resultReturn = new ApiResult();
        //    CameraAreaIncrease cameraAreaIncrease = new CameraAreaIncrease();
        //    tn_dts_camera_area newCameraArea = new tn_dts_camera_area()
        //    {
        //        cn_s_camera_areacode = areacode,
        //        cn_s_camera_areaname = areaname,
        //        cn_s_camera_arearemarks = arearemarks,
        //        cn_s_camera_points = areapoints
        //    };
        //    cameraAreaIncrease.NewCameraArea = newCameraArea;
        //    cameraAreaIncrease.HasPicture = haspicture;
        //    if (haspicture == true)
        //    {
        //        HttpRequest httpRequest = Request;
        //        if (httpRequest.ContentLength <= 0)
        //        {
        //            resultReturn.IsSuccess = false;
        //            resultReturn.StatusCode = (int)StatusCodeType.Error;
        //            resultReturn.Message = "请求接口参数不对，请重试！";
        //            return toResponse(resultReturn);
        //        }
        //        var files = Request.Form.Files;
        //        if (files == null || files.Count == 0)
        //        {
        //            resultReturn.IsSuccess = false;
        //            resultReturn.StatusCode = (int)StatusCodeType.Error;
        //            resultReturn.Message = "没有选择要上传的文件，请重试！";
        //            return toResponse(resultReturn);
        //        }
        //        // 创建上传文件保存的文件夹（如果不存在）
        //        var directoryInfo = Directory.CreateDirectory(UploadsFolder);
        //        foreach (var file in files)
        //        {
        //            string fileGuid = Guid.NewGuid().ToString();
        //            string fileName = fileGuid + "_" + file.FileName;
        //            var filePath = Path.Combine(UploadsFolder, fileName);
        //            // 检查文件是否已经部分上传
        //            var fileExists = System.IO.File.Exists(filePath);
        //            if (fileExists)
        //            {
        //                // 如果文件已经存在，获取已上传的字节数
        //                var existingFileSize = new FileInfo(filePath).Length;
        //                // 如果已上传的字节数等于文件总大小，说明文件已完整上传，直接跳过
        //                if (existingFileSize == file.Length)
        //                {
        //                    continue;
        //                }
        //                // 如果已上传的字节数小于文件总大小，说明文件部分上传，进行断点续传
        //                using (var fileStream = new FileStream(filePath, FileMode.Append))
        //                {
        //                    // 设置文件指针位置，从已上传的字节数开始
        //                    fileStream.Seek(existingFileSize, SeekOrigin.Begin);
        //                    await file.CopyToAsync(fileStream);
        //                }
        //            }
        //            else
        //            {
        //                // 如果文件不存在，直接创建新文件并上传
        //                using (var fileStream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(fileStream);
        //                }
        //            }

        //            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        //            IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        //            if (ipaddress is null)
        //            {
        //                cameraAreaIncrease.IpAddress = null;
        //            }
        //            else
        //            {
        //                cameraAreaIncrease.IpAddress = ipaddress.ToString();
        //            }
        //            cameraAreaIncrease.Port = Request.Host.Port.ToString();
        //            cameraAreaIncrease.FileName = file.FileName;
        //            cameraAreaIncrease.Size = file.Length;
        //            cameraAreaIncrease.OppoPath = Path.Combine(UploadsFolder, fileName);
        //            cameraAreaIncrease.AbsPath = Path.Combine(directoryInfo.FullName, fileName);
        //            cameraAreaIncrease.FileGuid = fileGuid;
        //        }
        //    }

        //    ReturnMessage res = _ICameraAreaService.AddCameraArea(cameraAreaIncrease);
        //    if (res.IsSuccess)
        //    {
        //        resultReturn.IsSuccess = res.IsSuccess;
        //        resultReturn.StatusCode = (int)StatusCodeType.Success;
        //        resultReturn.Message = res.Message;
        //    }
        //    else
        //    {
        //        resultReturn.IsSuccess = res.IsSuccess;
        //        resultReturn.StatusCode = (int)StatusCodeType.Error;
        //        resultReturn.Message = res.Message;
        //    }
        //    Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
        //    CameraAreaDriver.Instance.LatestCameraAreaPointList = _ICameraAreaService.GetAllPoint();
        //    return toResponse(resultReturn);
        //}
        //#endregion

        #region 按相机名称分页模糊查询
        /// <summary>
        /// 按相机名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm parm)
        {
            var res = _ICameraAreaService.GetListPages(parm);
            return toResponse(res);
        }
        #endregion

        #region 修改相机区域
        /// <summary>
        /// 修改相机区域
        /// </summary>
        /// <param name="guid">唯一标识</param>
        /// <param name="areacode">区域编号</param>
        /// <param name="areaname">区域名称</param>
        /// <param name="arearemarks">备注</param>
        /// <param name="areapoints">区域坐标系</param>
        /// <param name="isDelete">是否删除原有静态图</param>
        /// <param name="hasFromdate">fromdate里有无图片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCameraArea(string guid, string areacode, string areaname, string arearemarks, string areapoints, bool isDelete, bool hasFromdate)
        {
            string UploadsFolder = "uploads";
            tn_dts_camera_area model = new tn_dts_camera_area()
            {
                cn_guid = guid,
                cn_s_camera_areacode = areacode,
                cn_s_camera_areaname = areaname,
                cn_s_camera_arearemarks = arearemarks,
                cn_s_camera_points = areapoints
            };
            ReturnMessage res = _ICameraAreaService.UpdateCameraArea(model);
            ApiResult resultReturn = new ApiResult();
            if (!res.IsSuccess)
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = res.Message;
                return toResponse(resultReturn);
            }
            tn_dts_camera_area area = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
            if (hasFromdate)
            {
                if (area.cn_s_camera_areaimages is null)//用户为原本没有静态图的区域，新增静态图
                {
                    HttpRequest httpRequest = Request;
                    UserSession user = GetSessionInfo();

                    if (httpRequest.ContentLength <= 0)
                    {
                        resultReturn.IsSuccess = false;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = "请求接口参数不对，请重试！";
                        return toResponse(resultReturn);
                    }
                    var files = Request.Form.Files;
                    if (files == null || files.Count == 0)
                    {
                        resultReturn.IsSuccess = false;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = "没有选择要上传的文件，请重试！";
                        return toResponse(resultReturn);
                    }

                    // 创建上传文件保存的文件夹（如果不存在）
                    var directoryInfo = Directory.CreateDirectory(UploadsFolder);

                    if (!(_IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid) is null))
                    {
                        resultReturn.IsSuccess = false;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = "一个相机区域只能有一个区域静态图";
                        return toResponse(resultReturn);
                    }


                    foreach (var file in files)
                    {
                        string fileguid = Guid.NewGuid().ToString();
                        string fileName = fileguid + "." + file.FileName.Split('.')[1];
                        var filePath = Path.Combine(UploadsFolder, fileName);

                        // 检查文件是否已经部分上传
                        var fileExists = System.IO.File.Exists(filePath);
                        if (fileExists)
                        {
                            // 如果文件已经存在，获取已上传的字节数
                            var existingFileSize = new FileInfo(filePath).Length;

                            // 如果已上传的字节数等于文件总大小，说明文件已完整上传，直接跳过
                            if (existingFileSize == file.Length)
                            {
                                continue;
                            }

                            // 如果已上传的字节数小于文件总大小，说明文件部分上传，进行断点续传
                            using (var fileStream = new FileStream(filePath, FileMode.Append))
                            {
                                // 设置文件指针位置，从已上传的字节数开始
                                fileStream.Seek(existingFileSize, SeekOrigin.Begin);

                                await file.CopyToAsync(fileStream);
                            }
                        }
                        else
                        {
                            // 如果文件不存在，直接创建新文件并上传
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                        }

                        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                        IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                        tn_dts_files fileInfo = new tn_dts_files();
                        fileInfo.cn_guid = fileguid;
                        fileInfo.cn_s_files_source_guid = guid;
                        fileInfo.cn_s_files_source_module = "虚拟相机管理";
                        fileInfo.cn_s_files_type = fileName.Split('.')[1];
                        fileInfo.cn_s_files_name = file.FileName.Split('.')[0];
                        fileInfo.cn_s_files_extenname = fileInfo.cn_s_files_type;
                        fileInfo.cn_s_files_size = file.Length; 
                        if (ipaddress is null)
                        {
                            fileInfo.cn_s_files_rootpath = null;
                        }
                        else
                        {
                            fileInfo.cn_s_files_rootpath = ipaddress.ToString();
                        }
                        fileInfo.cn_s_files_oppopath = filePath;
                        fileInfo.cn_s_files_abspath = Path.Combine(directoryInfo.FullName, fileName);
                        fileInfo.cn_s_creator = user.UserCode;
                        fileInfo.cn_s_creator_by = user.UserName;
                        fileInfo.cn_t_create = DateTime.Now;
                        _IFilesService.Add(fileInfo);
                    }

                    tn_dts_camera_area areaGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
                    string ipAddress = Request.Host.Host;
                    int? port = Request.Host.Port;
                    string fileFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
                    string request = "http://" + ipAddress + ":" + port + "/api/Files/DownloadFile?file=" + fileFullName;
                    areaGuid.cn_s_camera_areaimages = request;
                    _ICameraAreaService.Update(areaGuid);
                }
                else//用户为原本有静态图的区域更换静态图
                {
                    string fileOldFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
                    var fileOldPath = Path.Combine(UploadsFolder, fileOldFullName);
                    string fileOldGuid = fileOldFullName.Split('.')[0];
                    //string fileOldGuid = fileOldFullName.Substring(fileOldFullName.IndexOf('_') + 1);
                    //string fileOldName = fileOldFullName.Split('_')[1];
                    int resFile = _IFilesService.Delete(it => it.cn_guid == fileOldGuid);
                    if (resFile <= 0)
                    {
                        resultReturn.IsSuccess = true;
                        resultReturn.StatusCode = (int)StatusCodeType.Success;
                        resultReturn.Message = "删除tn_dts_files表记录失败";
                        return toResponse(resultReturn);
                    }
                    System.IO.File.Delete(fileOldPath);

                    tn_dts_camera_area areaOldGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
                    areaOldGuid.cn_s_camera_areaimages = "";
                    _ICameraAreaService.Update(areaOldGuid);

                    HttpRequest httpRequest = Request;
                    UserSession user = GetSessionInfo();

                    if (httpRequest.ContentLength <= 0)
                    {
                        resultReturn.IsSuccess = false;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = "请求接口参数不对，请重试！";
                        return toResponse(resultReturn);
                    }
                    var files = Request.Form.Files;
                    if (files == null || files.Count == 0)
                    {
                        resultReturn.IsSuccess = false;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = "没有选择要上传的文件，请重试！";
                        return toResponse(resultReturn);
                    }

                    // 创建上传文件保存的文件夹（如果不存在）
                    var directoryInfo = Directory.CreateDirectory(UploadsFolder);

                    if (!(_IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid) is null))
                    {
                        resultReturn.IsSuccess = false;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = "一个相机区域只能有一个区域静态图";
                        return toResponse(resultReturn);
                    }


                    foreach (var file in files)
                    {
                        string fileNewGuid = Guid.NewGuid().ToString();
                        string fileNewFullName = fileNewGuid + "." + file.FileName.Split('.')[1];
                        var fileNewPath = Path.Combine(UploadsFolder, fileNewFullName);

                        // 检查文件是否已经部分上传
                        var fileExists = System.IO.File.Exists(fileNewPath);
                        if (fileExists)
                        {
                            // 如果文件已经存在，获取已上传的字节数
                            var existingFileSize = new FileInfo(fileNewPath).Length;

                            // 如果已上传的字节数等于文件总大小，说明文件已完整上传，直接跳过
                            if (existingFileSize == file.Length)
                            {
                                continue;
                            }

                            // 如果已上传的字节数小于文件总大小，说明文件部分上传，进行断点续传
                            using (var fileStream = new FileStream(fileNewPath, FileMode.Append))
                            {
                                // 设置文件指针位置，从已上传的字节数开始
                                fileStream.Seek(existingFileSize, SeekOrigin.Begin);

                                await file.CopyToAsync(fileStream);
                            }
                        }
                        else
                        {
                            // 如果文件不存在，直接创建新文件并上传
                            using (var fileStream = new FileStream(fileNewPath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                        }

                        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                        IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                        tn_dts_files fileInfo = new tn_dts_files();
                        fileInfo.cn_guid = fileNewGuid;
                        fileInfo.cn_s_files_source_guid = guid;
                        fileInfo.cn_s_files_source_module = "虚拟相机管理";
                        fileInfo.cn_s_files_type = fileNewFullName.Split('.')[1];
                        fileInfo.cn_s_files_name = file.FileName.Split('.')[0];
                        fileInfo.cn_s_files_extenname = fileInfo.cn_s_files_type;
                        fileInfo.cn_s_files_size = file.Length;
                        if (ipaddress is null)
                        {
                            fileInfo.cn_s_files_rootpath = null;
                        }
                        else
                        {
                            fileInfo.cn_s_files_rootpath = ipaddress.ToString();
                        }
                        fileInfo.cn_s_files_oppopath = fileNewPath;
                        fileInfo.cn_s_files_abspath = Path.Combine(directoryInfo.FullName, fileNewFullName);
                        fileInfo.cn_s_creator = user.UserCode;
                        fileInfo.cn_s_creator_by = user.UserName;
                        fileInfo.cn_t_create = DateTime.Now;
                        _IFilesService.Add(fileInfo);
                    }

                    tn_dts_camera_area areaNewGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
                    string ipAddress = Request.Host.Host;
                    int? port = Request.Host.Port;
                    string fileFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
                    string request = "http://" + ipAddress + ":" + port + "/api/Files/DownloadFile?file=" + fileFullName;
                    areaNewGuid.cn_s_camera_areaimages = request;
                    _ICameraAreaService.Update(areaNewGuid);
                }
            }

            if (!(area.cn_s_camera_areaimages is null) && isDelete)//用户将已有静态图区域删除静态图
            {
                string fileFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
                var filePath = Path.Combine(UploadsFolder, fileFullName);
                string fileGuid = fileFullName.Split('.')[0];
                //string fileName = fileFullName.Split('_')[1];
                int resFile = _IFilesService.Delete(it => it.cn_guid == fileGuid);
                if (resFile <= 0)
                {
                    resultReturn.IsSuccess = true;
                    resultReturn.StatusCode = (int)StatusCodeType.Success;
                    resultReturn.Message = "删除tn_dts_files表记录失败";
                    return toResponse(resultReturn);
                }
                System.IO.File.Delete(filePath);        

                tn_dts_camera_area areaOldGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
                areaOldGuid.cn_s_camera_areaimages = null;
                _ICameraAreaService.Update(areaOldGuid);
            }

            //ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
            //string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
            //await WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
            CameraAreaDriver.Instance.LatestCameraAreaPointList = _ICameraAreaService.GetAllPoint();
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "修改成功";
            return toResponse(resultReturn);
        }
        #endregion

        //#region 修改相机区域
        ///// <summary>
        ///// 修改相机区域
        ///// </summary>
        ///// <param name="guid">唯一标识</param>
        ///// <param name="areacode">区域编号</param>
        ///// <param name="areaname">区域名称</param>
        ///// <param name="arearemarks">备注</param>
        ///// <param name="areapoints">区域坐标系</param>
        ///// <param name="isDelete">是否删除原有静态图</param>
        ///// <param name="hasFromdate">fromdate里有无图片</param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> UpdateCameraArea(string guid, string areacode, string areaname, string arearemarks, string areapoints, bool isDelete, bool hasFromdate)
        //{
        //    string UploadsFolder = "uploads";
        //    tn_dts_camera_area model = new tn_dts_camera_area()
        //    {
        //        cn_guid = guid,
        //        cn_s_camera_areacode = areacode,
        //        cn_s_camera_areaname = areaname,
        //        cn_s_camera_arearemarks = arearemarks,
        //        cn_s_camera_points = areapoints
        //    };
        //    ReturnMessage res = _ICameraAreaService.UpdateCameraArea(model);
        //    ApiResult resultReturn = new ApiResult();
        //    if (!res.IsSuccess)
        //    {
        //        resultReturn.IsSuccess = res.IsSuccess;
        //        resultReturn.StatusCode = (int)StatusCodeType.Error;
        //        resultReturn.Message = res.Message;
        //        return toResponse(resultReturn);
        //    }
        //    tn_dts_camera_area area = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
        //    if (hasFromdate)
        //    {
        //        if (area.cn_s_camera_areaimages is null)//用户为原本没有静态图的区域，新增静态图
        //        {
        //            HttpRequest httpRequest = Request;
        //            UserSession user = GetSessionInfo();

        //            if (httpRequest.ContentLength <= 0)
        //            {
        //                resultReturn.IsSuccess = false;
        //                resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                resultReturn.Message = "请求接口参数不对，请重试！";
        //                return toResponse(resultReturn);
        //            }
        //            var files = Request.Form.Files;
        //            if (files == null || files.Count == 0)
        //            {
        //                resultReturn.IsSuccess = false;
        //                resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                resultReturn.Message = "没有选择要上传的文件，请重试！";
        //                return toResponse(resultReturn);
        //            }

        //            // 创建上传文件保存的文件夹（如果不存在）
        //            var directoryInfo = Directory.CreateDirectory(UploadsFolder);

        //            if (!(_IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid) is null))
        //            {
        //                resultReturn.IsSuccess = false;
        //                resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                resultReturn.Message = "一个相机区域只能有一个区域静态图";
        //                return toResponse(resultReturn);
        //            }


        //            foreach (var file in files)
        //            {
        //                string fileguid = Guid.NewGuid().ToString();
        //                string fileName = fileguid + "_" + file.FileName;
        //                var filePath = Path.Combine(UploadsFolder, fileName);

        //                // 检查文件是否已经部分上传
        //                var fileExists = System.IO.File.Exists(filePath);
        //                if (fileExists)
        //                {
        //                    // 如果文件已经存在，获取已上传的字节数
        //                    var existingFileSize = new FileInfo(filePath).Length;

        //                    // 如果已上传的字节数等于文件总大小，说明文件已完整上传，直接跳过
        //                    if (existingFileSize == file.Length)
        //                    {
        //                        continue;
        //                    }

        //                    // 如果已上传的字节数小于文件总大小，说明文件部分上传，进行断点续传
        //                    using (var fileStream = new FileStream(filePath, FileMode.Append))
        //                    {
        //                        // 设置文件指针位置，从已上传的字节数开始
        //                        fileStream.Seek(existingFileSize, SeekOrigin.Begin);

        //                        await file.CopyToAsync(fileStream);
        //                    }
        //                }
        //                else
        //                {
        //                    // 如果文件不存在，直接创建新文件并上传
        //                    using (var fileStream = new FileStream(filePath, FileMode.Create))
        //                    {
        //                        await file.CopyToAsync(fileStream);
        //                    }
        //                }

        //                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        //                IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        //                tn_dts_files fileInfo = new tn_dts_files();
        //                fileInfo.cn_guid = fileguid;
        //                fileInfo.cn_s_files_source_guid = guid;
        //                fileInfo.cn_s_files_source_module = "虚拟相机管理";
        //                fileInfo.cn_s_files_type = fileName.Split('.')[1];
        //                fileInfo.cn_s_files_name = file.FileName.Split('.')[0];
        //                fileInfo.cn_s_files_extenname = fileInfo.cn_s_files_type;
        //                fileInfo.cn_s_files_size = file.Length;
        //                if (ipaddress is null)
        //                {
        //                    fileInfo.cn_s_files_rootpath = null;
        //                }
        //                else
        //                {
        //                    fileInfo.cn_s_files_rootpath = ipaddress.ToString();
        //                }
        //                fileInfo.cn_s_files_oppopath = filePath;
        //                fileInfo.cn_s_files_abspath = Path.Combine(directoryInfo.FullName, fileName);
        //                fileInfo.cn_s_creator = user.UserCode;
        //                fileInfo.cn_s_creator_by = user.UserName;
        //                fileInfo.cn_t_create = DateTime.Now;
        //                _IFilesService.Add(fileInfo);
        //            }

        //            tn_dts_camera_area areaGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
        //            string ipAddress = Request.Host.Host;
        //            int? port = Request.Host.Port;
        //            string fileFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
        //            string request = "http://" + ipAddress + ":" + port + "/api/Files/DownloadFile?file=" + fileFullName;
        //            areaGuid.cn_s_camera_areaimages = request;
        //            _ICameraAreaService.Update(areaGuid);
        //        }
        //        else//用户为原本有静态图的区域更换静态图
        //        {
        //            string fileOldFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
        //            var fileOldPath = Path.Combine(UploadsFolder, fileOldFullName);
        //            string fileOldGuid = fileOldFullName.Split('_')[0];
        //            //string fileOldGuid = fileOldFullName.Substring(fileOldFullName.IndexOf('_') + 1);
        //            //string fileOldName = fileOldFullName.Split('_')[1];
        //            string fileOldName = fileOldFullName.Substring(fileOldFullName.IndexOf('_') + 1).Split('.')[0];
        //            int resFile = _IFilesService.Delete(it => it.cn_guid == fileOldGuid && it.cn_s_files_name == fileOldName);
        //            if (resFile <= 0)
        //            {
        //                resultReturn.IsSuccess = true;
        //                resultReturn.StatusCode = (int)StatusCodeType.Success;
        //                resultReturn.Message = "删除tn_dts_files表记录失败";
        //                return toResponse(resultReturn);
        //            }
        //            System.IO.File.Delete(fileOldPath);

        //            tn_dts_camera_area areaOldGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
        //            areaOldGuid.cn_s_camera_areaimages = "";
        //            _ICameraAreaService.Update(areaOldGuid);

        //            HttpRequest httpRequest = Request;
        //            UserSession user = GetSessionInfo();

        //            if (httpRequest.ContentLength <= 0)
        //            {
        //                resultReturn.IsSuccess = false;
        //                resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                resultReturn.Message = "请求接口参数不对，请重试！";
        //                return toResponse(resultReturn);
        //            }
        //            var files = Request.Form.Files;
        //            if (files == null || files.Count == 0)
        //            {
        //                resultReturn.IsSuccess = false;
        //                resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                resultReturn.Message = "没有选择要上传的文件，请重试！";
        //                return toResponse(resultReturn);
        //            }

        //            // 创建上传文件保存的文件夹（如果不存在）
        //            var directoryInfo = Directory.CreateDirectory(UploadsFolder);

        //            if (!(_IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid) is null))
        //            {
        //                resultReturn.IsSuccess = false;
        //                resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                resultReturn.Message = "一个相机区域只能有一个区域静态图";
        //                return toResponse(resultReturn);
        //            }


        //            foreach (var file in files)
        //            {
        //                string fileNewGuid = Guid.NewGuid().ToString();
        //                string fileNewFullName = fileNewGuid + "_" + file.FileName;
        //                var fileNewPath = Path.Combine(UploadsFolder, fileNewFullName);

        //                // 检查文件是否已经部分上传
        //                var fileExists = System.IO.File.Exists(fileNewPath);
        //                if (fileExists)
        //                {
        //                    // 如果文件已经存在，获取已上传的字节数
        //                    var existingFileSize = new FileInfo(fileNewPath).Length;

        //                    // 如果已上传的字节数等于文件总大小，说明文件已完整上传，直接跳过
        //                    if (existingFileSize == file.Length)
        //                    {
        //                        continue;
        //                    }

        //                    // 如果已上传的字节数小于文件总大小，说明文件部分上传，进行断点续传
        //                    using (var fileStream = new FileStream(fileNewPath, FileMode.Append))
        //                    {
        //                        // 设置文件指针位置，从已上传的字节数开始
        //                        fileStream.Seek(existingFileSize, SeekOrigin.Begin);

        //                        await file.CopyToAsync(fileStream);
        //                    }
        //                }
        //                else
        //                {
        //                    // 如果文件不存在，直接创建新文件并上传
        //                    using (var fileStream = new FileStream(fileNewPath, FileMode.Create))
        //                    {
        //                        await file.CopyToAsync(fileStream);
        //                    }
        //                }

        //                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        //                IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        //                tn_dts_files fileInfo = new tn_dts_files();
        //                fileInfo.cn_guid = fileNewGuid;
        //                fileInfo.cn_s_files_source_guid = guid;
        //                fileInfo.cn_s_files_source_module = "虚拟相机管理";
        //                fileInfo.cn_s_files_type = fileNewFullName.Split('.')[1];
        //                fileInfo.cn_s_files_name = file.FileName.Split('.')[0];
        //                fileInfo.cn_s_files_extenname = fileInfo.cn_s_files_type;
        //                fileInfo.cn_s_files_size = file.Length;
        //                if (ipaddress is null)
        //                {
        //                    fileInfo.cn_s_files_rootpath = null;
        //                }
        //                else
        //                {
        //                    fileInfo.cn_s_files_rootpath = ipaddress.ToString();
        //                }
        //                fileInfo.cn_s_files_oppopath = fileNewPath;
        //                fileInfo.cn_s_files_abspath = Path.Combine(directoryInfo.FullName, fileNewFullName);
        //                fileInfo.cn_s_creator = user.UserCode;
        //                fileInfo.cn_s_creator_by = user.UserName;
        //                fileInfo.cn_t_create = DateTime.Now;
        //                _IFilesService.Add(fileInfo);
        //            }

        //            tn_dts_camera_area areaNewGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
        //            string ipAddress = Request.Host.Host;
        //            int? port = Request.Host.Port;
        //            string fileFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
        //            string request = "http://" + ipAddress + ":" + port + "/api/Files/DownloadFile?file=" + fileFullName;
        //            areaNewGuid.cn_s_camera_areaimages = request;
        //            _ICameraAreaService.Update(areaNewGuid);
        //        }
        //    }

        //    if (!(area.cn_s_camera_areaimages is null) && isDelete)//用户将已有静态图区域删除静态图
        //    {
        //        string fileFullName = _IFilesService.GetFirst(it => it.cn_s_files_source_guid == guid).cn_s_files_oppopath.Split('\\')[1];
        //        var filePath = Path.Combine(UploadsFolder, fileFullName);
        //        string fileGuid = fileFullName.Split('_')[0];
        //        //string fileName = fileFullName.Split('_')[1];
        //        string fileName = fileFullName.Substring(fileFullName.IndexOf('_') + 1).Split('.')[0];
        //        int resFile = _IFilesService.Delete(it => it.cn_guid == fileGuid && it.cn_s_files_name == fileName);
        //        if (resFile <= 0)
        //        {
        //            resultReturn.IsSuccess = true;
        //            resultReturn.StatusCode = (int)StatusCodeType.Success;
        //            resultReturn.Message = "删除tn_dts_files表记录失败";
        //            return toResponse(resultReturn);
        //        }
        //        System.IO.File.Delete(filePath);

        //        tn_dts_camera_area areaOldGuid = _ICameraAreaService.GetFirst(it => it.cn_guid == guid);
        //        areaOldGuid.cn_s_camera_areaimages = null;
        //        _ICameraAreaService.Update(areaOldGuid);
        //    }

        //    ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
        //    string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
        //    await WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
        //    CameraAreaDriver.Instance.LatestCameraAreaPointList = _ICameraAreaService.GetAllPoint();
        //    resultReturn.IsSuccess = true;
        //    resultReturn.StatusCode = (int)StatusCodeType.Success;
        //    resultReturn.Message = "修改成功";
        //    return toResponse(resultReturn);
        //}
        //#endregion

        #region 删除相机区域
        /// <summary>
        /// 删除相机区域
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteCameraArea([FromBody] List<string> model)
        {
            ReturnMessage res = _ICameraAreaService.DeleteCameraArea(model);
            ApiResult resultReturn = new ApiResult();
            if (res.IsSuccess)
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Success;
                resultReturn.Message = res.Message;
            }
            else
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = res.Message;
            }
            //ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
            //string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
            //WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
            CameraAreaDriver.Instance.LatestCameraAreaPointList = _ICameraAreaService.GetAllPoint();
            return toResponse(resultReturn);
        }
        #endregion
        ///// <summary>
        ///// 删除相机区域
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult DeleteCameraArea([FromBody] tn_dts_camera_area model)
        //{
        //    string UploadsFolder = "uploads";
        //    ApiResult resultReturn = new ApiResult();
        //    tn_dts_camera_area area = _ICameraAreaService.GetFirst(it => it.cn_guid == model.cn_guid);
        //    ReturnMessage res = _ICameraAreaService.DeleteCameraArea(model);
        //    if (!res.IsSuccess)
        //    {
        //        resultReturn.IsSuccess = res.IsSuccess;
        //        resultReturn.StatusCode = (int)StatusCodeType.Error;
        //        resultReturn.Message = res.Message;
        //        return toResponse(resultReturn);
        //    }
        //    if (!string.IsNullOrEmpty(area.cn_s_camera_areaimages))
        //    {
        //        string file = area.cn_s_camera_areaimages.Split('=')[1];
        //        var filePath = Path.Combine(UploadsFolder, file);

        //        if (!System.IO.File.Exists(filePath))
        //        {
        //            resultReturn.IsSuccess = false;
        //            resultReturn.StatusCode = (int)StatusCodeType.Error;
        //            resultReturn.Message = "服务器上传文件夹中找不到" + file + "文件";
        //            return toResponse(resultReturn);
        //        }

        //        string fileguid = file.Split('_')[0];
        //        string filename = file.Substring(file.IndexOf('_') + 1).Split('.')[0];

        //        int resFile = _IFilesService.Delete(it => it.cn_guid == fileguid && it.cn_s_files_name == filename);
        //        if (resFile <= 0)
        //        {
        //            resultReturn.IsSuccess = true;
        //            resultReturn.StatusCode = (int)StatusCodeType.Success;
        //            resultReturn.Message = "删除tn_dts_files表记录失败";
        //            return toResponse(resultReturn);
        //        }
        //        System.IO.File.Delete(filePath);
        //    }

        //    resultReturn.IsSuccess = true;
        //    resultReturn.StatusCode = (int)StatusCodeType.Success;
        //    resultReturn.Message = "删除成功";
        //    ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
        //    string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
        //    WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
        //    return toResponse(resultReturn);
        //}

        #region 匹配指定点所在坐标区域
        /// <summary>
        /// 匹配指定点所在坐标区域
        /// </summary>
        /// <param name="matchPoint"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult MatchCameraArea([FromBody] MatchPoint matchPoint)
        {
            List<CameraAreaPoint> cameraAreaPointList = CameraAreaDriver.Instance.LatestCameraAreaPointList;
            var res = _ICameraAreaService.MatchArea(cameraAreaPointList, matchPoint);
            return toResponse(res);
        }
        #endregion
    }
}
