using HZ.CommonUtil.Model;
using HZ.IDTSCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace HZ.IDTSCore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : BaseController
    {
        public Interfaces.IService.ISystemService _ISystemService;

        public HomeController()
        {
            _ISystemService = ServiceLocator.GetService<Interfaces.IService.ISystemService>(HttpContextSession());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DbBackupTester()
        {
            ApiResult result = new ApiResult();
            
            return new JsonResult(result);
        }

        /// <summary>
        /// 安装配置向导
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="dbHostIP"></param>
        /// <param name="dbHostPort"></param>
        /// <param name="dbName"></param>
        /// <param name="dbUser"></param>
        /// <param name="dbPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Install(string dbType,string dbHostIP,string dbHostPort,string dbName,string dbUser,string dbPassword)
        {
            ApiResult result = new ApiResult();

            string connectionString = "";
            if (dbType == "PGSQL") connectionString = string.Format("PORT={0};DATABASE={1};HOST={2};PASSWORD={3};USER ID={4}", dbHostPort, dbName, dbHostIP, dbPassword, dbUser);

            AppSettingsModifier.UpdateNestedAppSettings("DbConnection", "ConnectionString_", connectionString);
            AppSettingsModifier.UpdateNestedAppSettings("DbConnection", "DataType", dbType);
            //重启站点服务代码
            //返回适当的响应
            result.IsSuccess = true;
            result.Message = "Application restarting...";
            return new JsonResult(result);
        }

        ///// <summary>
        ///// 重启指定服务名称
        ///// </summary>
        ///// <param name="serviceName"></param>
        //public void RestartService(string serviceName)
        //{
        //    Process process = new Process();
        //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        //    {
        //        process.StartInfo.FileName = "cmd.exe";
        //        process.StartInfo.Arguments = $"/c sc stop {serviceName} && sc start {serviceName}";
        //    }
        //    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        //    {
        //        process.StartInfo.FileName = "/bin/bash";
        //        process.StartInfo.Arguments = $"-c \"sudo systemctl restart {serviceName}\"";
        //    }
        //    else
        //    {
        //        throw new Exception("Unsupported operating system");
        //    }
        //    process.StartInfo.RedirectStandardOutput = true;
        //    process.StartInfo.UseShellExecute = false;
        //    process.Start();
        //    process.WaitForExit();
        //}
    }
}
