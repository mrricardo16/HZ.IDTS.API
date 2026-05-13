using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using CSRedis;
using HZ.iWCS.MData.Core;
using MongoDB.Driver;
using HZ.IDTSCore.Model.Entity.OpenApi;

namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class SettingController : BaseController
    {
        private ISettingService _ISettingService;
        private ILogsService _ILogsService;
        private IBackupsService _IBackupsService;

        public SettingController()
        {
            _ISettingService = ServiceLocator.GetService<ISettingService>(HttpContextSession());
            _ILogsService = ServiceLocator.GetService<ILogsService>(HttpContextSession());
            _IBackupsService = ServiceLocator.GetService<IBackupsService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 按cn_s_setting_class、cn_s_setting_keycode和cn_s_setting_keyname分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var pResult = _ISettingService.GetListPages(param);
            return toResponse(pResult);
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([FromBody] tn_dts_setting model)
        {
            var first = _ISettingService.GetFirst(x => x.cn_s_setting_keycode == model.cn_s_setting_keycode || x.cn_s_setting_keyname == model.cn_s_setting_keyname);
            if (first != null)
            {
                return toResponse(StatusCodeType.AppMessage, "关键字编码或名称不能重复！");
            }
            else
            {
                UserSession user = GetSessionInfo();
                model.cn_s_creator = user.UserCode;
                model.cn_s_creatorBy = user.UserName;
                model.cn_t_create = DateTime.Now;
                var res = _ISettingService.Add(model);
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
        public IActionResult Update([FromBody] tn_dts_setting model)
        {
            tn_dts_setting itemGuid = _ISettingService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));


            tn_dts_setting itemKeycode = _ISettingService.GetFirst(o => o.cn_s_setting_keycode == model.cn_s_setting_keycode && o.cn_guid != model.cn_guid);
            if (itemKeycode != null)
            {
                return toResponse(StatusCodeType.ParameterError, "关键字编码不能重复！");
            }

            tn_dts_setting itemKeyname = _ISettingService.GetFirst(o => o.cn_s_setting_keyname == model.cn_s_setting_keyname && o.cn_guid != model.cn_guid);
            if (itemKeyname != null)
            {
                return toResponse(StatusCodeType.ParameterError, "关键字名称不能重复！");
            }
            itemGuid.cn_s_setting_class = model.cn_s_setting_class;
            itemGuid.cn_s_setting_keycode = model.cn_s_setting_keycode;
            itemGuid.cn_s_setting_keyname = model.cn_s_setting_keyname;
            itemGuid.cn_s_setting_keyvalue = model.cn_s_setting_keyvalue;
            itemGuid.cn_s_setting_remarks = model.cn_s_setting_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modifyBy = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _ISettingService.Update(itemGuid);
            ApiResult result = new ApiResult();
            if (res > 0)
            {
                result.IsSuccess = true;
                result.StatusCode = (int)StatusCodeType.Success;
            }
            else
            {
                result.IsSuccess = false;
                result.StatusCode = (int)StatusCodeType.Error;
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
            ApiResult result = _ISettingService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion


        //[HttpPost]
        //public IActionResult GroupByTest()
        //{
        //    var result = _ISettingService.GroupByTest();

        //    return toResponse(result);
        //}

        ///// <summary>
        ///// 分页获取全部设置信息
        ///// </summary>
        ///// <param name="parm"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult GetAllListPages(PageParm parm)
        //{
        //    var pResult = _ISettingService.GetAllListPages(parm);
        //    List<tn_dts_setting> settings = new List<tn_dts_setting>();
        //    foreach (var p in pResult.DataSource)
        //    {
        //        settings.Add(p);
        //    }
        //    return new JsonResult(settings);
        //}

        #region 保存设置
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveSetting(SettingSavety ss)
        {
            //请求MDG新增单条日志接口/AddLog
            string token = HttpContextSession().token;
            MDGAddLog mDGAddLog = new MDGAddLog();
            mDGAddLog.logType = "修改";
            mDGAddLog.appCode = "MDG";
            UserSession user = GetSessionInfo();
            string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
            string savetyJoin = "关键字编码为" + ss.cn_s_setting_keycode + "的" + ss.cn_s_setting_keyname;
            mDGAddLog.logDesc = userJoin + "修改了" + savetyJoin;
            mDGAddLog.ip = HZ.DbHelper.HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string data = JsonConvert.SerializeObject(mDGAddLog);
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult apiRes = new ApiResult();
            string str = WebApiManager.HttpPost(mdg, "/api/LogWork/AddLog", data, ref apiRes, token);

            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "接口";
            log.cn_s_logs_clientip = HZ.DbHelper.HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            log.cn_s_logs_requesturl = mdg + "/api/LogWork/AddLog";
            log.cn_s_logs_requestpram = data;
            log.cn_s_logs_requestresult = JsonConvert.SerializeObject(apiRes);
            log.cn_s_creator = user.UserCode;
            log.cn_s_creator_by = user.UserName;
            log.cn_t_create = DateTime.Now;
            int failResult = _ILogsService.Add(log);
            ApiResult result = new ApiResult();
            if (failResult <= 0)
            {
                LogHelper.Info(DateTime.Now.ToString() + "1.2系统（Setting）参数设置保存接口请求MDG新增日志接口存入tn_dts_logs表失败。");
            }

            tn_dts_setting model = _ISettingService.GetWhere(it => it.cn_s_setting_keycode == ss.cn_s_setting_keycode && it.cn_s_setting_keyname == ss.cn_s_setting_keyname)[0];
            if (model is null)
            {
                result.IsSuccess = false;
                result.StatusCode = (int)StatusCodeType.Error;
                result.Message = "系统中找不到该关键字编码和名称的数据";
                return new JsonResult(result);
            }
            if (ss.cn_s_setting_keycode == "IsAutomatic" && ss.cn_s_setting_keycode == "BackupsSpan" && ss.cn_s_setting_keycode == "BackupsTime" && ss.cn_s_setting_keycode == "BackupsDiretory")
            {
                BackupsDriver.Instance.DBackups = _IBackupsService.GetLatestSaveBackups();
                BackupsDriver.Instance.LastBackups = DateTime.Now;
            }
            ReturnMessage res = _ISettingService.SaveSetting(ss);
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

        #region 测试关系数据库设置向导连通性
        /// <summary>
        /// 测试关系数据库设置向导连通性
        /// </summary>
        /// <param name="wizardDateBase"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestRelationDateBaseConnection(WizardDateBase wizardDateBase)
        {
            ApiResult apiResult = new ApiResult();
            string connectionString = "PORT=" + wizardDateBase.Port + ";DATABASE=" + wizardDateBase.DateBaseName + ";HOST=" + wizardDateBase.IPAddress + ";PASSWORD=" + wizardDateBase.DateBaseUserPassward + ";USER ID=" + wizardDateBase.DateBaseUserName;
            ConnectionConfig connectionConfig = new ConnectionConfig();
            connectionConfig.ConnectionString = connectionString;
            switch (wizardDateBase.DateBaseType)
            {
                case "MySql":
                    connectionConfig.DbType = DbType.MySql;
                    break;
                case "SqlServer":
                    connectionConfig.DbType = DbType.SqlServer;
                    break;
                case "Sqlite":
                    connectionConfig.DbType = DbType.Sqlite;
                    break;
                case "Oracle":
                    connectionConfig.DbType = DbType.Oracle;
                    break;
                case "PostgreSQL":
                    connectionConfig.DbType = DbType.PostgreSQL;
                    break;
                case "Dm":
                    connectionConfig.DbType = DbType.Dm;
                    break;
                case "Kdbndp":
                    connectionConfig.DbType = DbType.Kdbndp;
                    break;
                case "Oscar":
                    connectionConfig.DbType = DbType.Oscar;
                    break;
                case "MySqlConnector":
                    connectionConfig.DbType = DbType.MySqlConnector;
                    break;
                case "Access":
                    connectionConfig.DbType = DbType.Access;
                    break;
                case "OpenGauss":
                    connectionConfig.DbType = DbType.OpenGauss;
                    break;
                case "Custom":
                    connectionConfig.DbType = DbType.Custom;
                    break;
                default:
                    connectionConfig.DbType = DbType.PostgreSQL;
                    break;
            }
            connectionConfig.IsAutoCloseConnection = true;
            SqlSugarClient db = new SqlSugarClient(connectionConfig);
            //bool isConnected = false;
            //try
            //{
            //    db.Open();
            //    isConnected = true;
            //}
            //catch (Exception ex)
            //{
            //}
            //if (isConnected)
            //{
            //    apiResult.IsSuccess = true;
            //    apiResult.StatusCode = (int)StatusCodeType.Success;
            //    apiResult.Message = "连接成功！";
            //    apiResult.Data = true;
            //}
            //else
            //{
            //    apiResult.IsSuccess = false;
            //    apiResult.StatusCode = (int)StatusCodeType.Success;
            //    apiResult.Message = "连接失败！";
            //    apiResult.Data = false;
            //}
            if (db.Ado.IsValidConnection())
            {
                apiResult.IsSuccess = true;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接成功！";
                apiResult.Data = true;
            }
            else
            {
                apiResult.IsSuccess = false;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接失败！";
                apiResult.Data = false;
            }
            return toResponse(apiResult);
        }
        #endregion

        #region 测试WebSocket连通性
        /// <summary>
        /// 测试WebSocket连通性（是否可以创建WebSocket服务端，而非测试WebSocket服务端连通性）
        /// </summary>
        /// <param name="wsURL"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestWebSocketConnection([FromBody] string wsURL)
        {
            //ClientWebSocket clientWebSocket = new ClientWebSocket();
            string ipPort = wsURL.Substring(5);
            HttpListener httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://" + ipPort + "/");
            ApiResult apiResult = new ApiResult();
            bool isConnect = false;
            try
            {
                //clientWebSocket.ConnectAsync(new Uri(wsURL), new System.Threading.CancellationToken()).Wait();
                httpListener.Start();
                isConnect = true;
            }
            catch (Exception ex)
            {
            }
            if (isConnect)
            {
                apiResult.IsSuccess = true;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接成功！";
                apiResult.Data = true;
            }
            else
            {
                apiResult.IsSuccess = false;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接失败！";
                apiResult.Data = false;
            }
            return toResponse(apiResult);
        }
        #endregion

        #region 测试MDGAPI连通性
        /// <summary>
        /// 测试MDGAPI连通性
        /// </summary>
        /// <param name="mdgURL"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestMDGAPIConnection([FromBody] string mdgURL)
        {
            ApiResult apiResult = new ApiResult();
            bool isConnect = false;
            try
            {
                //string ipPortString = mdgURL.Substring(mdgURL.IndexOf(':') + 1);
                //if(ipPortString.Length == 4)
                //{
                //    ipPortString = mdgURL;
                //}
                var data = new { userCode = "", userPwd = "", appCode = "MDG", ip = mdgURL };
                ApiResult apiRes = new ApiResult();
                string dataLogin = JsonConvert.SerializeObject(data);
                string str = WebApiManager.HttpPost(mdgURL, "/api/Account/Login", dataLogin, ref apiRes);
                if (apiRes.IsSuccess)
                {
                    isConnect = true;
                }
            }
            catch
            {
            }
            if (isConnect)
            {
                apiResult.IsSuccess = true;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接成功！";
                apiResult.Data = true;
            }
            else
            {
                apiResult.IsSuccess = false;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接失败！";
                apiResult.Data = false;
            }
            return toResponse(apiResult);
        }
        #endregion

        #region 测试3D大屏URL连通性
        /// <summary>
        /// 测试3D大屏URL连通性
        /// </summary>
        /// <param name="threeDimensionURL"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestThreeDimensionScreenConnection([FromBody] string threeDimensionURL)
        {
            ApiResult apiResult = new ApiResult();
            bool isConnect = false;
            try
            {
                String ipPortString = String.Empty;
                if (threeDimensionURL.Contains("http://"))
                {
                    ipPortString = threeDimensionURL.Replace("http://", "");
                }
                if (threeDimensionURL.Contains("https://"))
                {
                    ipPortString = threeDimensionURL.Replace("https://", "");
                }
                string host = ipPortString.Split(':')[0];
                string port = ipPortString.Split(':')[1];
                int portInt = int.Parse(port);
                TcpClient tcpClient = new TcpClient(host, portInt);
                if (tcpClient.Connected)
                {
                    isConnect = true;
                }
            }
            catch
            {
            }
            if (isConnect)
            {
                apiResult.IsSuccess = true;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接成功！";
                apiResult.Data = true;
            }
            else
            {
                apiResult.IsSuccess = false;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接失败！";
                apiResult.Data = false;
            }
            return toResponse(apiResult);
        }
        #endregion

        #region 测试MDG_Redis连通性
        /// <summary>
        /// 测试MDG_Redis连通性
        /// </summary>
        /// <param name="redisURL"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestRedisConnection([FromBody] string redisURL)
        {
            ApiResult apiResult = new ApiResult();
            bool isConnect = false;
            try
            {
                CSRedisClient cSRedisClient = new CSRedisClient(redisURL);
                RedisHelper.Initialization(cSRedisClient);
                cSRedisClient.Ping("key");
                isConnect = true;
            }
            catch
            {
            }
            if (isConnect)
            {
                apiResult.IsSuccess = true;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接成功！";
                apiResult.Data = true;
            }
            else
            {
                apiResult.IsSuccess = false;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接失败！";
                apiResult.Data = false;
            }
            return toResponse(apiResult);
        }
        #endregion

        #region 测试MongoDB连通性
        /// <summary>
        /// 测试MongoDB连通性
        /// </summary>
        /// <param name="mongoDBConnection"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult TestMongoDbConnection([FromBody] MongoDBConnection mongoDBConnection)
        {
            ApiResult apiResult = new ApiResult();
            bool isConnect = false;
            try
            {
                string ipAddress = mongoDBConnection.IpAddress;
                string port = mongoDBConnection.Port;
                string mongoDatabase = mongoDBConnection.DateBaseName;
                string userName = mongoDBConnection.UserName;
                string passWord = mongoDBConnection.Passward;
                if (String.IsNullOrEmpty(ipAddress) && String.IsNullOrEmpty(port) && String.IsNullOrEmpty(mongoDatabase) && String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(passWord))
                {
                    throw new Exception();
                }
                string connectionString = "mongodb://" + ipAddress + ":" + port;
                MongoClientSettings mongoClientSetting = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
                mongoClientSetting.Credential = MongoCredential.CreateCredential(mongoDatabase, userName, passWord);
                MongoClient mongoClient = new MongoClient(mongoClientSetting);
                mongoClient.ListDatabaseNames();
                isConnect = true;
            }
            catch
            {
            }
            if (isConnect)
            {
                apiResult.IsSuccess = true;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接成功！";
                apiResult.Data = true;
            }
            else
            {
                apiResult.IsSuccess = false;
                apiResult.StatusCode = (int)StatusCodeType.Success;
                apiResult.Message = "连接失败！";
                apiResult.Data = false;
            }
            return toResponse(apiResult);
        }
        #endregion

        #region 下一步接口
        /// <summary>
        /// 下一步接口
        /// </summary>
        /// <param name="settingItemList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult NextStep([FromBody] List<SettingItem> settingItemList)
        {
            ReturnMessage res = _ISettingService.NextStep(settingItemList);
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

        /// <summary>
        /// 重启服务、退出当前应用程序
        /// </summary>
        [HttpPost]
        public IActionResult RestartApp()
        {
            ApiResult resultReturn = new ApiResult();
            try
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly();
                var pathToExe = assembly.Location;
                var pathToDll = pathToExe + ".dll";
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = pathToExe;
                process.StartInfo.Arguments = pathToDll;
                process.Start();
                Environment.Exit(0); // 退出当前应用程序实例

                resultReturn.IsSuccess = true;
            }
            catch (Exception ex)
            {
                resultReturn.IsSuccess = false;
                resultReturn.Message = ex.Message;
            }
            return toResponse(resultReturn);
        }

        #endregion

        #region 根据关键字编码读取设置值
        /// <summary>
        /// 根据关键字编码读取设置值
        /// </summary>
        /// <param name="keycode"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetKeyvalueByKeycode([FromBody] string keycode)
        {
            string keyValue = _ISettingService.GetKeyvalueByKeycode(keycode);
            return toResponse(keyValue);
        }
        #endregion

        ///// <summary>
        ///// 测试MongoDB连通性
        ///// </summary>
        ///// <param name="mongoDBConnection"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult TestMongoDbConnection([FromBody] MongoDBConnection mongoDBConnection)
        //{
        //    ApiResult apiResult = new ApiResult();
        //    bool isConnect = false;
        //    try
        //    {
        //        string ipAddress = mongoDBConnection.IpAddress;
        //        string port = mongoDBConnection.Port;
        //        string mongoDatabase = mongoDBConnection.DateBaseName;
        //        string userName = mongoDBConnection.UserName;
        //        string passWord = mongoDBConnection.Passward;
        //        if (String.IsNullOrEmpty(ipAddress) && String.IsNullOrEmpty(port) && String.IsNullOrEmpty(mongoDatabase) && String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(passWord))
        //        {
        //            throw new Exception();
        //        }
        //        string connectionString = "mongodb://" + ipAddress + ":" + port;
        //        MongoClientSettings mongoClientSetting = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        //        mongoClientSetting.Credential = MongoCredential.CreateCredential(mongoDatabase, userName, passWord);
        //        MongoClient mongoClient = new MongoClient(mongoClientSetting);
        //        mongoClient.ListDatabaseNames();
        //        mongoClient.GetDatabase("string").GetCollection<LocationRealMonitorViewModel>("LocationRealMonitorViewModel").Ag
        //        isConnect = true;
        //    }
        //    catch
        //    {
        //    }
        //    if (isConnect)
        //    {
        //        apiResult.IsSuccess = true;
        //        apiResult.StatusCode = (int)StatusCodeType.Success;
        //        apiResult.Message = "连接成功！";
        //        apiResult.Data = true;
        //    }
        //    else
        //    {
        //        apiResult.IsSuccess = false;
        //        apiResult.StatusCode = (int)StatusCodeType.Success;
        //        apiResult.Message = "连接失败！";
        //        apiResult.Data = false;
        //    }
        //    return toResponse(apiResult);
        //}
    }
}
