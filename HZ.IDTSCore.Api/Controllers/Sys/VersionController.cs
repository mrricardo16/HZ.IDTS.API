using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NPOI.SS.UserModel;
using ICSharpCode.SharpZipLib.Checksum;

namespace HZ.IDTSCore.Api.Controllers.Info
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class VersionController : BaseController
    {
        private IVersionService _IVersionService;

        public VersionController()
        {
            _IVersionService = ServiceLocator.GetService<IVersionService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 按cn_s_ver_number和cn_s_ver_packagedate分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IVersionService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_version model)
        {
            var first = _IVersionService.GetFirst(x => x.cn_s_ver_number == model.cn_s_ver_number);
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
                var res = _IVersionService.Add(model);
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
        public IActionResult Update([FromBody] tn_dts_version model)
        {
            tn_dts_version itemGuid = _IVersionService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));


            tn_dts_version itemNumber = _IVersionService.GetFirst(o => o.cn_s_ver_number == model.cn_s_ver_number && o.cn_guid != model.cn_guid);
            if (itemNumber != null)
            {
                return toResponse(StatusCodeType.ParameterError, "产品版本号不能重复！");
            }
            itemGuid.cn_s_ver_number = model.cn_s_ver_number;
            itemGuid.cn_s_ver_packagedate = model.cn_s_ver_packagedate;
            itemGuid.cn_s_ver_packagetype = model.cn_s_ver_packagetype;
            itemGuid.cn_s_ver_isupdated = model.cn_s_ver_isupdated;
            itemGuid.cn_s_ver_updatecontent = model.cn_s_ver_updatecontent;
            itemGuid.cn_s_ver_update = model.cn_s_ver_update;
            itemGuid.cn_s_ver_updateman = model.cn_s_ver_updateman;
            itemGuid.cn_s_ver_remarks = model.cn_s_ver_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _IVersionService.Update(itemGuid);
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
            ApiResult result = _IVersionService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        #region 上传更新包
        /// <summary>
        /// 上传更新包
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadUpdate()
        //public void UploadUpdate()
        {
            string UploadsFolder = "updatePackages";
            ApiResult resultReturn = new ApiResult();
            HttpRequest httpRequest = Request;
            UserSession user = GetSessionInfo();
            string absPath = string.Empty;

            if (httpRequest.ContentLength <= 0)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "请求接口参数不对，请重试！";
                return toResponse(resultReturn);
            }
            var files = Request.Form.Files;
            //Microsoft.Extensions.Primitives.StringValues files = "";

            //Request.Form.TryGetValue("Files", out files);
            if (files == null || files.Count == 0)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "没有选择要上传的文件，请重试！";
                return toResponse(resultReturn);
            }

            // 创建上传文件保存的文件夹（如果不存在）
            var directoryInfo = Directory.CreateDirectory(UploadsFolder);
            absPath = directoryInfo.FullName;

            List<string> updateList = new List<string>();

            foreach (var file in files)
            {
                string guid = Guid.NewGuid().ToString();
                var updateName = guid + "_" + file.FileName;
                updateList.Add(updateName);
                var filePath = Path.Combine(UploadsFolder, updateName);

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
            }

            List<tn_dts_version> versionList = new List<tn_dts_version>();
            string folderAbsPath = string.Empty;
            UploadPackage uploadPackage = new UploadPackage();
            foreach (var updateName in updateList)
            {
                var filePath = Path.Combine(UploadsFolder, updateName);
                if (!System.IO.File.Exists(filePath))
                {
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "服务器上传文件夹中找不到" + updateName + "文件";
                    return toResponse(resultReturn);
                }

                if (updateName.Split(".")[1] != "zip")
                {
                    System.IO.File.Delete(filePath);
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "上传的更新包格式不为zip无法解压，请重试！";
                    return toResponse(resultReturn);
                }

                string decompressionFolder = updateName.Substring(updateName.IndexOf('_') + 1).Split('.')[0];
                folderAbsPath = Path.Combine(absPath, decompressionFolder);
                if (Directory.Exists(folderAbsPath))
                {
                    System.IO.File.Delete(filePath);
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "已上传同名更新包不能重名，请重试！";
                    return toResponse(resultReturn);
                }

                Dictionary<string, string> packageName = new Dictionary<string, string>();
                using (ZipInputStream zipInputStream = new ZipInputStream(System.IO.File.OpenRead(filePath)))
                {
                    ZipEntry theEntry;
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var gbk = Encoding.GetEncoding("GBK");
                    ZipStrings.CodePage = gbk.CodePage;
                    while ((theEntry = zipInputStream.GetNextEntry()) != null)
                    {
                        string fileName = Path.GetFileName(theEntry.Name);
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string[] levelPathList = theEntry.Name.Split("/");
                        Regex regUI = new Regex("idts_ui");
                        Regex regApi = new Regex("idts_api");
                        Regex regPgsql = new Regex("idts_pgsql");
                        Regex regUpdate = new Regex("更新说明");
                        if (regUI.IsMatch(theEntry.Name) && String.IsNullOrEmpty(fileName) && levelPathList.Length == 3 && levelPathList[1] != "")
                        {
                            packageName["idts_ui"] = theEntry.Name.Split("/")[1];
                            uploadPackage.HasUIPackage = true;
                            continue;
                        }
                        if (regApi.IsMatch(theEntry.Name) && String.IsNullOrEmpty(fileName) && levelPathList.Length == 3 && levelPathList[1] != "")
                        {
                            packageName["idts_api"] = theEntry.Name.Split("/")[1];
                            uploadPackage.HasAPIPackage = true;
                            continue;
                        }
                        if (regPgsql.IsMatch(theEntry.Name) && String.IsNullOrEmpty(fileName) && levelPathList.Length == 3 && levelPathList[1] != "")
                        {
                            packageName["idts_pgsql"] = theEntry.Name.Split("/")[1];
                            uploadPackage.HasSqlScript = true;
                            continue;
                        }
                        if (regUpdate.IsMatch(theEntry.Name) && !String.IsNullOrEmpty(fileName) && levelPathList.Length == 2 && levelPathList[1] != "")
                        {
                            packageName["update"] = theEntry.Name.Split("/")[1];
                            continue;
                        }
                        if (String.IsNullOrEmpty(fileName) && levelPathList.Length == 3 && levelPathList[1] != "")
                        {
                            resultReturn.IsSuccess = false;
                            resultReturn.StatusCode = (int)StatusCodeType.Error;
                            resultReturn.Message = "上传的更新包有子文件夹名称不为“idts_ui”、“idts_api”或“idts_pgsql”，请重试！";
                            return toResponse(resultReturn);
                        }
                    }
                }
                if (!packageName.ContainsKey("update"))
                {
                    System.IO.File.Delete(filePath);
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "上传的更新包中不含更新说明，请重试！";
                    return toResponse(resultReturn);
                }
                if (packageName["update"].Split('.')[1] != "xls" && packageName["update"].Split('.')[1] != "xlsx")
                {
                    System.IO.File.Delete(filePath);
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "系统仅支持xls和xlsx格式的更新说明，请重试！";
                    return toResponse(resultReturn);
                }

                using (ZipInputStream zipInputStreamChecked = new ZipInputStream(System.IO.File.OpenRead(filePath)))
                {
                    ZipEntry theEntry;
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var gbk = Encoding.GetEncoding("GBK");
                    ZipStrings.CodePage = gbk.CodePage;
                    while ((theEntry = zipInputStreamChecked.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName != String.Empty)
                        {
                            string newDir = Path.Combine(UploadsFolder, directoryName);
                            Directory.CreateDirectory(newDir);
                        }
                        if (fileName != String.Empty)
                        {
                            string newFileDir = Path.Combine(UploadsFolder, theEntry.Name);
                            FileStream streamWriter = System.IO.File.Create(newFileDir);
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = zipInputStreamChecked.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            streamWriter.Close();
                        }
                    }
                }
                //System.IO.File.Delete(filePath);
                string updatePath = Path.Combine(UploadsFolder, decompressionFolder, packageName["update"]);
                List<UpdatePackage> updatePackageList = new List<UpdatePackage>();

                //using (StreamReader sr = new StreamReader(updatePath))
                //{
                //    string line;
                //    while ((line = sr.ReadLine()) != null)
                //    {
                //        UpdatePackage updatePackage = new UpdatePackage();
                //        string packageVersion = line.Split('@')[1].Trim();
                //        updatePackage.PackageVersion = packageVersion.Substring(packageVersion.IndexOf('：') + 1);
                //        string updateContent = line.Split('@')[2].Trim();
                //        updatePackage.UpdateContent = updateContent.Substring(updateContent.IndexOf('：') + 1);
                //        updatePackageList.Add(updatePackage);
                //    }
                //}
                //ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                //using (ExcelPackage package = new ExcelPackage(new FileInfo(updatePath)))
                //{
                //    int sheetCount = package.Workbook.Worksheets.Count;
                //    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                //    DataTable dt = new DataTable(worksheet.Name);
                //}
                //using (FileStream fileStream = new FileStream(updatePath, FileMode.Open))
                //{
                //    Workbook workbook = Workbook.Load(fileStream);
                //    Worksheet worksheet = workbook.Worksheets[1];
                //    for (int i = 1; i <= worksheet.Cells.LastRowIndex; i++)
                //    {
                //        UpdatePackage updatePackage = new UpdatePackage();
                //        updatePackage.PackageVersion = worksheet.Cells[i, 5].Value.ToString();
                //        updatePackage.UpdateContent = worksheet.Cells[i, 4].Value.ToString();
                //        updatePackageList.Add(updatePackage);
                //    }
                //}
                using (FileStream fileStream = new FileStream(updatePath, FileMode.Open))
                {
                    IWorkbook workbook = WorkbookFactory.Create(fileStream);
                    ISheet sheet = null;
                    sheet = workbook.GetSheetAt(1);
                    if (sheet != null)
                    {
                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            if (String.IsNullOrEmpty(sheet.GetRow(i).GetCell(5).StringCellValue) || String.IsNullOrEmpty(sheet.GetRow(i).GetCell(4).StringCellValue))
                            {
                                continue;
                            }
                            UpdatePackage updatePackage = new UpdatePackage();
                            updatePackage.PackageVersion = sheet.GetRow(i).GetCell(5).StringCellValue;
                            updatePackage.UpdateContent = sheet.GetRow(i).GetCell(4).StringCellValue;
                            updatePackageList.Add(updatePackage);
                        }
                    }
                }

                foreach (var updatePackage in updatePackageList)
                {
                    if (packageName.Values.Where(it => it == updatePackage.PackageVersion).ToArray().Length == 0)
                    {
                        resultReturn.IsSuccess = false;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = "存在“idts_ui”、“idts_api”和“idts_pgsql”更新包不包含更新说明或者更新包版本号不对应，请重试！";
                        return toResponse(resultReturn);
                    }
                }

                foreach (var key in packageName.Keys)
                {
                    if (key == "update")
                    {
                        continue;
                    }
                    tn_dts_version version = new tn_dts_version();
                    version.cn_guid = Guid.NewGuid().ToString();
                    version.cn_s_ver_number = decompressionFolder;
                    version.cn_s_ver_packagedate = packageName[key];
                    if(key == "idts_ui")
                    {
                        version.cn_s_ver_packagetype = "前端更新";
                    }
                    else if(key == "idts_api")
                    {
                        version.cn_s_ver_packagetype = "后端更新";
                    }
                    else if(key == "idts_pgsql")
                    {
                        version.cn_s_ver_packagetype = "脚本更新";
                    }
                    version.cn_s_ver_isupdated = 0;
                    version.cn_s_ver_updatecontent = updatePackageList.Where(it => it.PackageVersion == packageName[key]).First().UpdateContent;
                    version.cn_s_ver_update = DateTime.Now;
                    version.cn_s_ver_updateman = user.UserCode;
                    version.cn_s_creator = user.UserCode;
                    version.cn_s_creator_by = user.UserName;
                    version.cn_t_create = DateTime.Now;
                    versionList.Add(version);
                }
            }

            ReturnMessage res = _IVersionService.AddPackages(versionList);
            if (res.IsSuccess)
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Success;
                resultReturn.Message = res.Message;
            }
            else
            {
                //Directory.Delete(folderAbsPath, true);
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = res.Message;
                return toResponse(resultReturn);
            }

            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "上传成功";
            resultReturn.Data = uploadPackage;
            return toResponse(resultReturn);
        }
        #endregion

        #region 解压缩
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="file">待解压文件名(包含物理路径)</param>
        /// <param name="dir"> 解压到哪个目录中(包含物理路径)</param>
        [HttpPost]
        public IActionResult UnpackFiles(string file, string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (Path.GetFileName(file).Split(".")[1] != "zip")
                {
                    return toResponse(false);
                }
                ZipInputStream s = new ZipInputStream(System.IO.File.OpenRead(file));
                ZipEntry theEntry;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var gbk = Encoding.GetEncoding("GBK");
                ZipStrings.CodePage = gbk.CodePage;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName != String.Empty)
                    {
                        string newDir = Path.Combine(dir, directoryName);
                        Directory.CreateDirectory(newDir);
                    }
                    if (fileName != String.Empty)
                    {
                        string newFileDir = Path.Combine(dir, theEntry.Name);
                        FileStream streamWriter = System.IO.File.Create(newFileDir);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                }
                s.Close();
                return toResponse(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        //[HttpPost]
        //public IActionResult ZipFileDictory(string FolderToZip, ZipOutputStream s, string ParentFolderName)
        //{
        //    bool res = true;
        //    string[] folders, filenames;
        //    ZipEntry entry = null;
        //    FileStream fs = null;
        //    Crc32 crc = new Crc32();
        //    try
        //    {
        //        entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/"));
        //        s.PutNextEntry(entry);
        //        s.Flush();
        //        filenames = Directory.GetFiles(FolderToZip);
        //        foreach (string file in filenames)
        //        {
        //            fs = File.OpenRead(file);
        //            byte[] buffer = new byte[fs.Length];
        //            fs.Read(buffer, 0, buffer.Length);
        //            entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/" + Path.GetFileName(file)));
        //            entry.DateTime = DateTime.Now;
        //            entry.Size = fs.Length;
        //            fs.Close();
        //            crc.Reset();
        //            crc.Update(buffer);
        //            entry.Crc = crc.Value;
        //            s.PutNextEntry(entry);
        //            s.Write(buffer, 0, buffer.Length);
        //        }
        //    }
        //    catch
        //    {
        //        res = false;
        //    }
        //    finally
        //    {
        //        if (fs != null)
        //        {
        //            fs.Close();
        //            fs = null;
        //        }
        //        if (entry != null)
        //        {
        //            entry = null;
        //        }
        //        GC.Collect();
        //        GC.Collect(1);
        //    }
        //    folders = Directory.GetDirectories(FolderToZip);
        //    foreach (string folder in folders)
        //    {
        //        if (!ZipFileDictory(folder, s, Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip))))
        //        {
        //            return false;
        //        }
        //    }
        //    return res;
        //}

        #region 删除指定文件夹中所有项
        /// <summary>
        /// 删除指定文件夹中所有项
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteFolder([FromBody] string dir)
        {
            ApiResult resultReturn = new ApiResult();
            if (!Directory.Exists(dir))
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "指定路径不存在，请重试！";
                return toResponse(resultReturn);
            }
            Directory.Delete(dir, true);
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "级联删除文件夹成功！";
            return toResponse(resultReturn);
        }
        #endregion

        #region 读取一个产品版本号里所有的更新包版本
        /// <summary>
        /// 读取一个产品版本号里所有的更新包版本
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllPackageDateByNumber([FromBody] string number)
        {
            return new JsonResult(_IVersionService.GetAllPackageDateByNumber(number));
        }
        #endregion

        #region 修改指定更新包版本的备份信息
        /// <summary>
        /// 修改指定更新包版本的备份信息
        /// </summary>
        /// <param name="backupsPackage"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BackUpPackage(BackupsPackage backupsPackage)
        {
            ReturnMessage res = _IVersionService.BackUpPackage(backupsPackage);
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
            return toResponse(resultReturn);
        }
        #endregion

        #region 修改指定更新包版本的更新信息
        /// <summary>
        /// 修改指定更新包版本的更新信息
        /// </summary>
        /// <param name="executeUpdatePackage"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ExecuteUpdatePackage(ExecuteUpdatePackage executeUpdatePackage)
        {
            ReturnMessage res = _IVersionService.ExecuteUpdatePackage(executeUpdatePackage);
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
            return toResponse(resultReturn);
        }
        #endregion

        #region 获取最新前端更新版本
        /// <summary>
        /// 获取最新前端更新版本
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetLatestUIVersion()
        {
            string res = _IVersionService.GetLatestUIVersion();
            return toResponse(res);
        }
        #endregion

        #region 获取最新后端更新版本
        /// <summary>
        /// 获取最新后端更新版本
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetLatestAPIVersion()
        {
            string res = _IVersionService.GetLatestAPIVersion();
            return toResponse(res);
        }
        #endregion

        #region 获取最新产品版本号
        /// <summary>
        ///  获取最新产品版本号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetLatestNumberVersion()
        {
            string res = _IVersionService.GetLatestNumberVersion();
            return toResponse(res);
        }
        #endregion

        #region 删除一个产品版本号里所有的更新包版本
        /// <summary>
        /// 删除一个产品版本号里所有的更新包版本
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteAllPackageDateByNumber([FromBody] string number)
        {
            ReturnMessage res = _IVersionService.DeleteAllPackageDateByNumber(number);
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
            return toResponse(resultReturn);
        }
        #endregion

    }
}
