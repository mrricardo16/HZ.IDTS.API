using HZ.CommonUtil.Model;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService;
using HZ.IDTSCore.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HZ.IDTSCore.Api.Authorization
{
    /// <summary>
    /// 登录验证过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public AuthorizationAttribute() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            #region 判断是否登录
            var _tokenManager = ServiceLocator.GetService<ITokenService>(new DbHelper.SessionInfo());
            if (!_tokenManager.IsAuthenticated())
            {
                ApiResult response = new ApiResult
                {
                    StatusCode = (int)StatusCodeType.Unauthorized,
                    //Message = StatusCodeType.Unauthorized.GetEnumText()
                    Message = StatusCodeType.Unauthorized.ToString()//EnumDescription.GetFieldText(StatusCodeType.Unauthorized)
                };

                context.Result = new JsonResult(response) { StatusCode = (int)StatusCodeType.Unauthorized };
                return;
            }

            #endregion
        }
    }
}