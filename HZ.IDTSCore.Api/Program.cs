using Autofac.Extensions.DependencyInjection;
using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.IDTSCore.WebSocketServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using SuperSocket.WebSocket.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
//1
namespace HZ.IDTSCore.Api
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            logger.Debug("MainDebugger-0");
            try
            {
                // EnableSenarioTesting=false 时不初始化场景测试线程，避免 WebSocket 客户端连接后触发高频测试数据推送。
                bool enableSenarioTesting = AppSettings.GetValue<bool>("AppSettings:Thread:EnableSenarioTesting");
                SenarioTestingThread.Instance.Initialize(enableSenarioTesting);

                List<Model.Entity.Sys.tn_dts_setting> SysSetList = new Interfaces.Service.Sys.SettingService(new DbHelper.SessionInfo()
                {
                    token = "",
                    splitDbCode = ""
                }).GetAll(); ;

                SystemDriver.Instance.SysSetList = SysSetList;

                string webSocketServer = SysSetList.Where(it => it.cn_s_setting_keycode == "WebSocketServer").Select(it => it.cn_s_setting_keyvalue).First();
                string ipPort = webSocketServer.Substring(5);
                string ip =  ipPort.Split(':')[0];
                string port = ipPort.Split(':')[1];
                //ip = "192.168.8.43"; port = "4040";
                var host = WebSocketHostBuilder.Create(args)
              .UseWebSocketMessageHandler(async (session, message) =>
              {
                  try
                  {
                      //echo message back to the client
                      Global.WebSocketReceiveFilter.Receive(session, message.Message);
                  }
                  catch(Exception exception)
                  {
                      LogHelper.Error(DateTime.Now.ToString() + "接受测试指令并响应发生错误，详细信息为：" + exception.Message);
                  }
                 
              }).UseSession<WebSession>()
              // 2026-06-10优化：持续高频推送时，permessage-deflate 压缩可能与部分客户端解压实现不兼容，
              // 客户端会出现 invalid block type / invalid stored block lengths 后主动断开连接。
              // 这里默认关闭 WebSocket 压缩，优先保证长连接稳定；如客户端确认完全兼容后再开启。
              //.UsePerMessageCompression()
              .ConfigureLogging((hostCtx, loggingBuilder) =>
              {
                  // register your logging library here
                  loggingBuilder.AddConsole();
              })
              .ConfigureAppConfiguration((hostCtx, configApp) =>
              {
                  configApp.AddInMemoryCollection(new Dictionary<string, string>
                  {
                      { "serverOptions:name", "HZWebSocketServer" },
                      { "serverOptions:listeners:0:ip", ip },
                      { "serverOptions:listeners:0:port", port }//修改为从tn_dts_setting表中读取
                  });
              }).Build();
                _ = host.RunAsync();
                logger.Debug("MainDebugger-1");
            }
            catch (Exception ex)
            {
            }


            try
            {
                CreateHostBuilder(args).Build().Run();
                logger.Debug("MainDebugger-2");
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message);
                logger.Error(exception.StackTrace);
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
                logger.Debug("MainDebugger-3");
            }
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = long.MaxValue;
                });
            })
              .ConfigureLogging(logging =>
              {
                  logging.ClearProviders();
                  logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
              })
              .UseNLog();
    }
}
