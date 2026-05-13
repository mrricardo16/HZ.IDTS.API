using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class FilesController : BaseController
    {
        private IFilesService _IFilesService;
        private ICameraAreaService _ICameraAreaService;
        private static readonly string UploadsFolder = "uploads";


        public FilesController()
        {
            _IFilesService = ServiceLocator.GetService<IFilesService>(HttpContextSession());
            _ICameraAreaService = ServiceLocator.GetService<ICameraAreaService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 按cn_s_files_source_guid和cn_s_files_source_name分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IFilesService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([FromBody] tn_dts_files model)
        {
            var first = _IFilesService.GetFirst(x => x.cn_s_files_source_guid == model.cn_s_files_source_guid);
            if (first != null)
            {
                return toResponse(StatusCodeType.AppMessage, "关键字不能重复！");
            }
            else
            {
                UserSession user = GetSessionInfo();
                model.cn_s_creator = user.UserCode;
                model.cn_s_creator_by = user.UserName;
                model.cn_t_create = DateTime.Now;
                var res = _IFilesService.Add(model);
                return toResponse(res);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Update([FromBody] tn_dts_files model)
        {
            tn_dts_files itemGuid = _IFilesService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));


            tn_dts_files itemSourceguid = _IFilesService.GetFirst(o => o.cn_s_files_source_guid == model.cn_s_files_source_guid && o.cn_guid != model.cn_guid);
            if (itemSourceguid != null)
            {
                return toResponse(StatusCodeType.ParameterError, "来源Guid不能重复！");
            }

            tn_dts_files itemName = _IFilesService.GetFirst(o => o.cn_s_files_name == model.cn_s_files_name && o.cn_guid != model.cn_guid);
            if (itemName != null)
            {
                return toResponse(StatusCodeType.ParameterError, "文件名称不能重复！");
            }
            itemGuid.cn_s_files_source_guid = model.cn_s_files_source_guid;
            itemGuid.cn_s_files_source_module = model.cn_s_files_source_module;
            itemGuid.cn_s_files_type = model.cn_s_files_type;
            itemGuid.cn_s_files_name = model.cn_s_files_name;
            itemGuid.cn_s_files_extenname = model.cn_s_files_extenname;
            itemGuid.cn_s_files_size = model.cn_s_files_size;
            itemGuid.cn_s_files_rootpath = model.cn_s_files_rootpath;
            itemGuid.cn_s_files_oppopath = model.cn_s_files_oppopath;
            itemGuid.cn_s_files_abspath = model.cn_s_files_abspath;
            itemGuid.cn_s_files_remarks = model.cn_s_files_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _IFilesService.Update(itemGuid);
            ApiResult result = new ApiResult();
            if (res > 0)
            {
                result.IsSuccess = true;
                result.StatusCode = 200;
            }
            else
            {
                result.IsSuccess = false;
                result.StatusCode = 500;
                result.Message = "无影响行数";
            }

            return new JsonResult(result);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(string[] cn_s_guid)
        {
            ApiResult result = _IFilesService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        //#region 附件上传接口
        ///// <summary>
        ///// 附件上传接口
        ///// </summary>
        ///// <param name="sourcemodule"></param>
        ///// <param name="sourceguid"></param>
        ///// <param name="addOrModify"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> UploadFiles(string sourcemodule, string sourceguid,string addOrModify)
        ////public async Task<IActionResult> UploadFiles(string sourcemodule, string sourceguid)
        //{
        //    ApiResult resultReturn = new ApiResult();
        //    UserSession user = GetSessionInfo();
        //    HttpRequest httpRequest = Request;
        //    if (httpRequest.ContentLength <= 0)
        //    {
        //        resultReturn.IsSuccess = false;
        //        resultReturn.StatusCode = (int)StatusCodeType.Error;
        //        resultReturn.Message = "请求接口参数不对，请重试！";
        //        return toResponse(resultReturn);
        //    }
        //    var files = Request.Form.Files;
        //    if (files == null || files.Count == 0)
        //    {
        //        resultReturn.IsSuccess = false;
        //        resultReturn.StatusCode = (int)StatusCodeType.Error;
        //        resultReturn.Message = "没有选择要上传的文件，请重试！";
        //        return toResponse(resultReturn);
        //    }

        //    // 创建上传文件保存的文件夹（如果不存在）
        //    var directoryInfo = Directory.CreateDirectory(UploadsFolder);

        //    var uploadedFiles = new List<string>();

        //    string guid = Guid.NewGuid().ToString();
            
            
        //    foreach (var file in files)
        //    {
        //        var fileName = guid + "_" + file.FileName;
        //        var filePath = Path.Combine(UploadsFolder, fileName);

        //        if (sourcemodule == "虚拟相机模块")
        //        {
        //            if (addOrModify == "add")
        //            {
        //                tn_dts_camera_area area = _ICameraAreaService.GetFirst(it => it.cn_guid == sourceguid);
        //                if (area is null)
        //                {
        //                    resultReturn.IsSuccess = false;
        //                    resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                    resultReturn.Message = "tn_dts_camera_area表中找不到唯一标识为" + sourceguid + "记录！";
        //                    return toResponse(resultReturn);
        //                }

        //                if (!(_IFilesService.GetFirst(it => it.cn_s_files_source_guid == sourceguid && it.cn_guid != guid) is null))
        //                {
        //                    resultReturn.IsSuccess = false;
        //                    resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                    resultReturn.Message = "一个相机区域只能有一个区域静态图";
        //                    return toResponse(resultReturn);
        //                }
        //                string ipAddress = Request.Host.Host;
        //                int? port = Request.Host.Port;
        //                string request = "http://" + ipAddress + ":" + port + "/api/Files/DownloadFile?file=" + fileName;
        //                area.cn_s_camera_areaimages = request;
        //                _ICameraAreaService.Update(area);
        //            }
        //            else if (addOrModify == "modify")
        //            {
        //                tn_dts_camera_area area = _ICameraAreaService.GetFirst(it => it.cn_guid == sourceguid);
        //                if (area is null)
        //                {
        //                    resultReturn.IsSuccess = false;
        //                    resultReturn.StatusCode = (int)StatusCodeType.Error;
        //                    resultReturn.Message = "tn_dts_camera_area表中找不到唯一标识为" + sourceguid + "记录！";
        //                    return toResponse(resultReturn);
        //                }
        //                if(area.cn_s_camera_areaimages is null)//为没有静态图的相机区域新增区域
        //                {
                            
        //                }
        //                else//为已有静态图的相机区域更换静态图
        //                {

        //                }
        //                string ipAddress = Request.Host.Host;
        //                int? port = Request.Host.Port;
        //                string request = "http://" + ipAddress + ":" + port + "/api/Files/DownloadFile?file=" + fileName;
        //                area.cn_s_camera_areaimages = request;
        //                _ICameraAreaService.Update(area);
        //            }
        //        }
        //        // 检查文件是否已经部分上传
        //        var fileExists = System.IO.File.Exists(filePath);
        //        if (fileExists)
        //        {
        //            // 如果文件已经存在，获取已上传的字节数
        //            var existingFileSize = new FileInfo(filePath).Length;

        //            // 如果已上传的字节数等于文件总大小，说明文件已完整上传，直接跳过
        //            if (existingFileSize == file.Length)
        //            {
        //                uploadedFiles.Add(fileName);
        //                continue;
        //            }

        //            // 如果已上传的字节数小于文件总大小，说明文件部分上传，进行断点续传
        //            using (var fileStream = new FileStream(filePath, FileMode.Append))
        //            {
        //                // 设置文件指针位置，从已上传的字节数开始
        //                fileStream.Seek(existingFileSize, SeekOrigin.Begin);

        //                await file.CopyToAsync(fileStream);
        //            }
        //        }
        //        else
        //        {
        //            // 如果文件不存在，直接创建新文件并上传
        //            using (var fileStream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(fileStream);
        //            }
        //        }

        //        uploadedFiles.Add(fileName);
        //        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        //        IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);            
        //        tn_dts_files fileInfo = new tn_dts_files();
        //        fileInfo.cn_guid = guid;
        //        fileInfo.cn_s_files_source_guid = sourceguid;
        //        fileInfo.cn_s_files_source_module = sourcemodule;
        //        fileInfo.cn_s_files_type = fileName.Split('.')[1];
        //        fileInfo.cn_s_files_name = file.FileName;
        //        fileInfo.cn_s_files_extenname = fileInfo.cn_s_files_type;
        //        fileInfo.cn_s_files_size = file.Length;
        //        if (ipaddress is null)
        //        {
        //            fileInfo.cn_s_files_rootpath = null;
        //        }
        //        else
        //        {
        //            fileInfo.cn_s_files_rootpath = ipaddress.ToString();
        //        }
        //        fileInfo.cn_s_files_oppopath = filePath;
        //        fileInfo.cn_s_files_abspath = Path.Combine(directoryInfo.FullName, fileName);
        //        fileInfo.cn_s_creator = user.UserCode;
        //        fileInfo.cn_s_creator_by = user.UserName;
        //        fileInfo.cn_t_create = DateTime.Now;
        //        _IFilesService.Add(fileInfo);          
        //    }

        //    //StringBuilder sb = new StringBuilder(100);
        //    //await
        //    string message = String.Empty;

        //    foreach (var uploadedFile in uploadedFiles)
        //    {
        //        message = message + "文件" + uploadedFile + "上传完成 " + Environment.NewLine;
        //        //sb.Append("文件" + uploadedFile + "上传完成！\n");
        //    }
        //    resultReturn.IsSuccess = true;
        //    resultReturn.StatusCode = (int)StatusCodeType.Success;
        //    resultReturn.Message = message;
        //    return toResponse(resultReturn);
        //    //return Ok(uploadedFiles);
        //}
        //#endregion

        #region 附件上传接口
        /// <summary>
        /// 附件上传接口
        /// </summary>
        /// <param name="sourcemodule"></param>
        /// <param name="sourceguid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadFiles(string sourcemodule, string sourceguid)
        //public async Task<IActionResult> UploadFiles(string sourcemodule, string sourceguid)
        {
            ApiResult resultReturn = new ApiResult();
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

            var uploadedFiles = new List<string>();

            string guid = Guid.NewGuid().ToString();


            foreach (var file in files)
            {
                var fileName = guid + "_" + file.FileName;
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
                        uploadedFiles.Add(fileName);
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

                uploadedFiles.Add(fileName);
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress? ipaddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                tn_dts_files fileInfo = new tn_dts_files();
                fileInfo.cn_guid = guid;
                fileInfo.cn_s_files_source_guid = sourceguid;
                fileInfo.cn_s_files_source_module = sourcemodule;
                fileInfo.cn_s_files_type = fileName.Split('.')[1];
                fileInfo.cn_s_files_name = file.FileName.Substring(file.FileName.IndexOf('_') + 1).Split('.')[0];
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

            //StringBuilder sb = new StringBuilder(100);
            //await
            string message = String.Empty;

            foreach (var uploadedFile in uploadedFiles)
            {
                message = message + "文件" + uploadedFile + "上传完成 " + Environment.NewLine;
                //sb.Append("文件" + uploadedFile + "上传完成！\n");
            }
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = message;
            return toResponse(resultReturn);
            //return Ok(uploadedFiles);
        }
        #endregion

        #region 附件下载接口
        /// <summary>
        /// 附件下载接口
        /// </summary>
        /// <param name="file">唯一标识_文件名称</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DownloadFile(string file)
        {
            var filePath = Path.Combine(UploadsFolder, file);

            if (!System.IO.File.Exists(filePath))
            {
                ApiResult resultReturn = new ApiResult();
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "服务器上传文件夹中找不到" + file + "文件";
                return toResponse(resultReturn);
            }

            var fileProvider = new FileExtensionContentTypeProvider();
            if (!fileProvider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, file);
        }
        #endregion

        [HttpGet]
        public IActionResult DownloadFileByPath(string relativePath)
        {
            var filePath = relativePath;
            var file = relativePath.Split('\\')[0];


            if (!System.IO.File.Exists(filePath))
            {
                ApiResult resultReturn = new ApiResult();
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "服务器上传文件夹中找不到" + file + "文件";
                return toResponse(resultReturn);
            }

            var fileProvider = new FileExtensionContentTypeProvider();
            if (!fileProvider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);

            return File(fileBytes, contentType, file);
        }

        #region 附件删除接口
        /// <summary>
        /// 附件删除接口
        /// </summary>
        /// <param name="file">唯一标识_文件名</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteFile(string file)
        {
            
            var filePath = Path.Combine(UploadsFolder, file);
            ApiResult resultReturn = new ApiResult();
            if (!System.IO.File.Exists(filePath))
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "服务器上传文件夹中找不到" + file + "文件";
                return toResponse(resultReturn);
            }

            string fileguid = file.Split('_')[0];
            string filename = file.Substring(file.IndexOf('_') + 1).Split('.')[0];
            
            int res = _IFilesService.Delete(it => it.cn_guid == fileguid && it.cn_s_files_name == filename);
            if (res <= 0)
            {
                resultReturn.IsSuccess = true;
                resultReturn.StatusCode = (int)StatusCodeType.Success;
                resultReturn.Message = "删除tn_dts_files表记录失败";
                return toResponse(resultReturn);
            }
            System.IO.File.Delete(filePath);
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "删除成功";
            return toResponse(resultReturn);

        }
        #endregion
    }
}
