using HZ.IDTSCore.Common;
using HZ.IDTSCore.Common.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using HZ.CommonUtil.ExceptionExtend;
using HZ.CommonUtil.Helpers;
using System.Data.Common;

namespace HZ.IDTSCore.Api.Global
{
    public class GlobalExceptionFilter: IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DbException)
            {
                // 处理数据库连接异常，重定向到指定的错误页面
                context.Result = new RedirectToActionResult("Error", "Home", null);
            }
            else
            {
                // 处理其他异常，可以选择跳转到通用的错误页面或返回自定义错误信息
                context.Result = new ContentResult
                {
                    Content = "<h1>发生了未知错误。</h1>",
                    StatusCode = 500,
                    ContentType = "text/html"
                };
            }
        }
    }
}
