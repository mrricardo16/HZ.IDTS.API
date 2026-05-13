using HZ.IDTSCore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace HZ.IDTSCore.Api.Global
{
    public class GlobalActionMonitor : Attribute, IActionFilter
    {
        public GlobalActionMonitor()
        {
        }

        private const string ResponseTimeKey = "ResponseTimeKey";
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Stopwatch stopwatch = (Stopwatch)context.HttpContext.Items[ResponseTimeKey];
            // Calculate the time elapsed   
            var timeElapsed = stopwatch.Elapsed;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Start the timer   
            context.HttpContext.Items[ResponseTimeKey] = Stopwatch.StartNew();

            //#region 模型验证   暂时去掉
            //if (context.ModelState.IsValid) return;

            //ApiResult response = new ApiResult();
            //response.StatusCode = (int)StatusCodeType.ParameterError;

            //foreach (var item in context.ModelState.Values)
            //{
            //    foreach (var error in item.Errors)
            //    {
            //        if (!string.IsNullOrEmpty(response.Message))
            //        {
            //            response.Message += " | ";
            //        }

            //        response.Message += error.ErrorMessage;
            //    }
            //}

            //context.Result = new JsonResult(response);
            //#endregion
        }

        //private static LogEventInfo LogEventInfoBuild(string message, string elapsed)
        //{
        //    var eventInfo = new LogEventInfo();
        //    eventInfo.Message = message;
        //    eventInfo.Properties["Elapsed"] = elapsed;
        //    return eventInfo;
        //}
    }
}
