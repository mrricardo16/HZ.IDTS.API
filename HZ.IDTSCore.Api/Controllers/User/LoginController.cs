using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Logic;
using HZ.IDTSCore.Model.SerializeEntity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HZ.IDTSCore.Api.Controllers.User
{
    /// <summary>
    /// login相关
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : BaseController
    {
        private readonly ISystemService _ISystemService;
        private readonly IUserService _IUserService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public LoginController(IHostingEnvironment hostingEnvironment)
        {
            _ISystemService = ServiceLocator.GetService<ISystemService>(HttpContextSession());
            _IUserService = ServiceLocator.GetService<IUserService>(HttpContextSession());
            _hostingEnvironment = hostingEnvironment;
        }

        #region 获取菜单
        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="sourceTerminal"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllMenus(string sourceTerminal)
        {
            var res = _ISystemService.GetAllMenus(sourceTerminal);
            return toResponse(res);
        }
        #endregion

        #region 获取按钮
        /// <summary>
        /// 获取按钮
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetButton(string pageName)
        {
            var res = _ISystemService.GetButton(pageName);
            return toResponse(res);
        }
        #endregion

        [HttpPost]
        public IActionResult UpdateMenus(sys_menu mode)
        {
            var res = _ISystemService.UpdateMenus(mode);
            return toResponse(res);
        }

        [HttpGet]
        public IActionResult IsCollection(string menuurlcode)
        {
            var res = _ISystemService.IsCollection(menuurlcode);
            return new JsonResult(res);
        }
        
        [HttpPost]
        public IActionResult AddMenus(sys_menu mode)
        {
            var res = _ISystemService.AddMenus(mode);
            return toResponse(res);
        }
        [HttpGet]
        public IActionResult GetMenus(int type)
        {
            var res = _ISystemService.GetMenus(type);
            return toResponse(res);
        }
        [HttpGet]
        public IActionResult GetPDAMenus()
        {
            var res = _ISystemService.GetPDAMenus();
            return toResponse(res);
        }

        #region 登录
        [HttpPost]
        public IActionResult Login(SYS_USER user)
        {
            LogHelper.Info($"Login：" + Thread.CurrentThread.ManagedThreadId.ToString());
            ApiResult result = new ApiResult();
            AccountEntity account = _IUserService.Login(user.UserCode, user.Password);
            if (account == null)
            {
                return toResponse(StatusCodeType.AppMessage, "请检查MDG登录接口！");
            }
            if (account.Code == "0")
            {
                if (!string.IsNullOrEmpty(user.PhoneID))
                    _IUserService.AddUser(account.Login, account.UserName, user.PhoneID);

                user.TokenId = account.Token;
                user.AuthorizeStock = account.AuthorizeStock;
                user.UserName = account.UserName;

                UserSession userSeesion = new UserSession()
                {
                    OrgCode = account.userExt.OrgCode,
                    OrgFlag = account.userExt.OrgFlag,
                    OrgName = account.userExt.OrgName,
                    TokenId = account.Token,
                    SplitDbCode=account.SplitDbCode,
                    UserCode = account.Login,
                    UserName = account.UserName,
                    AuthorizeStock = user.AuthorizeStock,
                     
                };

                result.StatusCode = (int)StatusCodeType.Success;
                
                user.TokenId = account.Token;
                user.SplitDbCode = account.SplitDbCode;
                user.SplitDbList = account.SplitDbList;
                return toResponse(user);
            }
            else
            {
                return toResponse(StatusCodeType.AppMessage, account.Data);
            }
        }
        #endregion

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditPassword(JObject jObject)
        {
            var userInfo = jObject["userInfo"].ToString();
            var newPassWord = jObject["newPassWord"].ToString();
            var oldPassWrod = jObject["oldPassWrod"].ToString();

            if (string.IsNullOrEmpty(userInfo))
                return new JsonResult(ApiResult.Error("用户名不能为空！"));
            if (string.IsNullOrEmpty(newPassWord))
                return new JsonResult(ApiResult.Error("新密码不能为空！"));
            if (string.IsNullOrEmpty(oldPassWrod))
                return new JsonResult(ApiResult.Error("原密码不能为空！"));

            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult res = new ApiResult();
            var result = WebApiManager.HttpPost(mdg, "api/Account/EditPassWord", JsonConvert.SerializeObject(new
            {
                userInfo,
                newPassWord,
                oldPassWrod
            }),ref res);

            if (!res.IsSuccess)
                return new JsonResult(result);

            return new JsonResult(result);
        }

        #region 版本更新
        [HttpGet]
        public IActionResult GetApkUpdateInfo()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var PdaServerVersionName = string.Empty;
            string path = contentRootPath + "\\Program";
            DirectoryInfo folder = new DirectoryInfo(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (FileInfo file in folder.GetFiles("*.apk").OrderByDescending(b => b.CreationTime))
            {
                PdaServerVersionName = file.Name;
                break;
            }
            try
            {
                string pdaUpdateUrl = AppSettings.GetValue<string>("FileUrl:ApkUrl");
                UpdateInfoEntity updateEntity = new UpdateInfoEntity();
                if (string.IsNullOrEmpty(pdaUpdateUrl))
                {
                    return new JsonResult(ApiResult.Error("请检查web.config中是否配置pda下载更新地址"));
                }
                updateEntity.serverVersionName = PdaServerVersionName;
                updateEntity.updateUrl = pdaUpdateUrl;
                updateEntity.upGradeInfo = "系统有新版本，是否需要更新";
                return new JsonResult(ApiResult.Success("", updateEntity));
            }
            catch (Exception ex)
            {
                return new JsonResult(ApiResult.Error("GetApkUpdateInfo方法异常" + ex.Message));
            }
        }

        /// <summary>
        /// APP版本自动更新
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetApkUpdateInfoByApp()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            var PdaServerVersionName = string.Empty;
            string path = contentRootPath + "\\Program\\app";
            DirectoryInfo folder = new DirectoryInfo(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            foreach (FileInfo file in folder.GetFiles("*.apk").OrderByDescending(b => b.CreationTime))
            {
                PdaServerVersionName = file.Name;
                break;
            }
            try
            {
                string pdaUpdateUrl = AppSettings.GetValue<string>("FileUrl:ApkAppUrl");
                UpdateInfoEntity updateEntity = new UpdateInfoEntity();
                if (string.IsNullOrEmpty(pdaUpdateUrl))
                {
                    return new JsonResult(ApiResult.Error("请检查web.config中是否配置pda下载更新地址"));
                }
                updateEntity.serverVersionName = PdaServerVersionName;
                updateEntity.updateUrl = pdaUpdateUrl+ PdaServerVersionName;
                updateEntity.upGradeInfo = "系统有新版本，是否需要更新";
                return new JsonResult(ApiResult.Success("", updateEntity));
            }
            catch (Exception ex)
            {
                return new JsonResult(ApiResult.Error("GetApkUpdateInfo方法异常" + ex.Message));
            }
        }
        #endregion

        #region 获取待办事项汇总数量
        [HttpGet]
        public IActionResult GetTodoWaitItem()
        {
            var res = _IUserService.GetTodoWaitItem();
            return new JsonResult(res);
        }
        #endregion

        #region 获取待办事项明细
        [HttpPost]
        public IActionResult GetFlowItem(PageParm param)
        {
            var res = _IUserService.GetFlowItem(param);
            return new JsonResult(res);
        }
        #endregion

        #region 获取本机ip地址
        [HttpPost]
        public IActionResult GetLocalIp()
        {
            //获取本地的IP地址
            string AddressIP = string.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return toResponse(AddressIP);
        }
        #endregion


        [HttpGet]
        public string Test()
        {
            return "";
        }

        [HttpGet]
        public IActionResult GetConfig()
        {
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            return toResponse(new {
                mdgApi = mdg
            });
        }
    }
}
