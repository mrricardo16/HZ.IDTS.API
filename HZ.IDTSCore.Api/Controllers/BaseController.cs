using HZ.CommonUtil.ExceptionExtend;
using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.Redis.RedisUserModel;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.SerializeEntity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace HZ.IDTSCore.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 获取 Session 内容
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        private T GetSessionItem<T>(string key)
        {
            string GetSysToken = this.HttpContext.Request.Headers["token"];
            if (!RedisServer.Session.Exists(GetSysToken))
            {
                throw new Exception("请重新登录系统！");
            }
            return RedisServer.Session.HGet<T>(GetSysToken, key);
        }

        [HttpGet]
        protected DbHelper.SessionInfo HttpContextSession()
        {
            return new DbHelper.SessionInfo()
            {
                token = HelperHttpContext.Current.Request.Headers["token"].ToString(),
                splitDbCode = HelperHttpContext.Current.Request.Headers["splitDb"].ToString()
            };
            //return this.HttpContext.Request.Headers["splitDb"];
        }

        [HttpGet]
        public UserSession GetSessionInfo()
        {
            string GetSysToken = this.HttpContext.Request.Headers["token"];
            RedisMsgEntity user = HZ.Redis.RedisUserinfoHelper.GetUserInfo(GetSysToken);
            if (user.Success == false || user.Msg == "未获得token值" || user.Msg == "未获得对应的用户信息")
                throw new PowerException("请重新登录！");
            return new UserSession()
            {
                AuthorizeStock = user.userInfo.stockCode == null ? new List<string>() : new List<string>(user.userInfo.stockCode.Split(',')),
                OrgCode = user.userInfo.orgCode,
                OrgFlag = user.userInfo.orgFlag,
                OrgName = user.userInfo.orgName,
                TokenId = user.userInfo.token,
                UserCode = user.userInfo.userCode,
                UserName = user.userInfo.userName,
            };
        }

        #region 统一返回封装

        /// <summary>
        /// 返回封装
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public static JsonResult toResponse(StatusCodeType statusCode)
        {
            ApiResult response = new ApiResult();
            response.StatusCode = (int)statusCode;
            //response.Message = statusCode.GetEnumText();
            response.Message = statusCode.ToString();

            return new JsonResult(response);
        }

        /// <summary>
        /// 返回封装
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="retMessage"></param>
        /// <returns></returns>
        public static JsonResult toResponse(StatusCodeType statusCode, string retMessage)
        {
            ApiResult response = new ApiResult();
            response.StatusCode = (int)statusCode;
            response.Message = retMessage;

            return new JsonResult(response);
        }

        /// <summary>
        /// 返回封装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JsonResult toResponse<T>(T data)
        {
            if (data is ApiResult)
                return new JsonResult(data);
            ApiResult<T> response = new ApiResult<T>();
            response.StatusCode = (int)StatusCodeType.Success;
            response.IsSuccess = true;
            response.Message = StatusCodeType.Success.ToString();// StatusCodeType.Success.GetEnumText();
            response.Data = data;
            return new JsonResult(response);
        }

        #endregion

        #region 常用方法函数
        public static string GetGUID
        {
            get
            {
               return Guid.NewGuid().ToString().ToUpper();
            }
        }

        #endregion

        //[HttpGet]
        //public string LoginToken()
        //{
        //    IUserService _IUserService = ServiceLocator.GetService<IUserService>();

        //    AccountEntity account = _IUserService.Login("hz", "123456");
        //    if (account == null)
        //    {
        //        throw new Exception("请检查MDG登录接口！");
        //    }
        //    if (account.Code == "0")
        //    {
        //        UserSession userSeesion = new UserSession()
        //        {
        //            OrgCode = account.userExt.OrgCode,
        //            OrgFlag = account.userExt.OrgFlag,
        //            OrgName = account.userExt.OrgName,
        //            TokenId = account.Token,
        //            UserCode = account.Login,
        //            UserName = account.UserName
        //        };
        //        ITokenService _ITokenService = ServiceLocator.GetService<ITokenService>(account.Token);

        //        bool newMdg = AppSettings.GetValue<bool>("AppSet:NewMdg");
        //        if (newMdg)
        //            return account.Token;
        //        else
        //        {
        //            var token = _ITokenService.CreateSession(userSeesion, "HZ.WMSCore");
        //            return token;
        //        }
        //    }
        //    throw new PowerException(account.Data);
        //    return "0";
        //}
    }
}