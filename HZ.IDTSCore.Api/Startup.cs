using Autofac;
using Autofac.Extras.DynamicProxy;
using HZ.CommonUtil.Helpers;
using HZ.DbHelper;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Global;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Api.Middleware;
using HZ.IDTSCore.Common.Const;
using HZ.IDTSCore.Common.Helpers;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using MapsterMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace HZ.IDTSCore.Api
{
    public class Startup
    {
        public List<Model.Entity.Sys.tn_dts_setting> SysSetList;
        public List<tn_dts_goodsequipment> GoodsequipmentList;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
            SysSetList = new List<tn_dts_setting>();
            GoodsequipmentList = new List<tn_dts_goodsequipment>();
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.1
        public void ConfigureServices(IServiceCollection services)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            logger.Debug("StartupDebugger-0");
            //日志初始化
            LogHelper.Configure();
            logger.Debug("StartupDebugger-1");
            bool dbConnectioned = false;//数据库链接状态
            try
            {
                SysSetList = new Interfaces.Service.Sys.SettingService(new DbHelper.SessionInfo()
                {
                    token = "",
                    splitDbCode = ""
                }).GetAll();
                GoodsequipmentList = new Interfaces.Service.SenarioTesting.GoodsequipmentService(new DbHelper.SessionInfo()
                {
                    token = "",
                    splitDbCode = ""
                }).GetAll();
                dbConnectioned = true;

                SystemDriver.Instance.SysSetList = SysSetList;

                logger.Debug("StartupDebugger-2");
            }
            catch (Exception exception)
            {
                string s = exception.Message;
            }


            #region 跨域设置
            bool iscors = false;
            var corsValue = Configuration["Startup:CorsIPs"];
            string[] CorsIPs;
            var CorsIPsModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "CorsIPs");
            if (CorsIPsModel != null && !string.IsNullOrEmpty(CorsIPsModel.cn_s_setting_keyvalue))
            {
                CorsIPs = CorsIPsModel.cn_s_setting_keyvalue.Split(',');
                if (CorsIPsModel.cn_s_setting_keyvalue != corsValue)
                {
                    //第一次安装部署，配置全局*,满足配置向导调用接口
                    iscors = true;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(corsValue))
                {
                    CorsIPs = corsValue.Split(',');
                }
                else
                {
                    CorsIPs = new string[1] { "http://127.0.0.1" };
                }
            }
            logger.Debug("StartupDebugger-3");
            services.AddCors(c =>
            {
                if (iscors)
                {
                    c.AddPolicy("LimitRequests", policy =>
                    {
                        policy
                        .WithOrigins(new string[1] { "*" })
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithMethods("GET", "POST", "PUT", "DELETE");
                    });
                    logger.Debug("StartupDebugger-LimitRequests-1");
                }
                else
                {
                    c.AddPolicy("LimitRequests", policy =>
                    {
                        policy
                        .WithOrigins(CorsIPs)
                        .AllowAnyHeader()//Ensures that the policy allows any header.
                        .AllowAnyMethod()
                        .WithMethods("GET", "POST", "PUT", "DELETE")//请求方法添加到策略
                        .AllowCredentials();
                    });
                    logger.Debug("StartupDebugger-LimitRequests-2");
                }
            });
            #endregion

            #region 自动映射
            services.AddScoped<IMapper, ServiceMapper>();
            #endregion

            #region Api文档说明
            string ApiName = "";
            var ApiNameModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "ApiName");
            if (ApiNameModel != null && !string.IsNullOrEmpty(ApiNameModel.cn_s_setting_keyvalue)) ApiName = ApiNameModel.cn_s_setting_keyvalue;
            else ApiName = AppSettings.GetValue<string>("Startup:ApiName");
            if (string.IsNullOrEmpty(ApiName)) ApiName = "IDTSCoreApi";
            services.AddSwaggerGen(c =>
                {

                    c.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = $"{ApiName} 接口文档",
                        Description = $"{ApiName} HTTP API "
                    });

                    try
                    {
                        //就是这里
                        var xmlPath = Path.Combine(AppContext.BaseDirectory, "HZ.IDTSCore.Api.xml");//这个就是刚刚配置的xml文件名
                        c.IncludeXmlComments(xmlPath, true);//默认的第二个参数是false，这个是controller的注释，记得修改

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Xml 文件丢失，请检查并拷贝。\n{ex.Message}");
                    }

                    // 开启加权小锁
                    c.OperationFilter<AppendAuthorizeFilter>();

                });
            #endregion

            #region 配置Json格式
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                // 忽略循环引用
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                // 不使用驼峰
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                // 设置时间格式
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 如字段为null值，该字段不会返回到前端
                //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });
            #endregion

            #region 获取客户端 IP
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            #endregion

            string redis = Configuration["RedisServer:Mdg"];
            var redisModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "RedisMDG");
            if (redisModel != null && !string.IsNullOrEmpty(redisModel.cn_s_setting_keyvalue))
            {
                redis = redisModel.cn_s_setting_keyvalue;
            }
            if (!string.IsNullOrEmpty(redis))
            {
                HZ.Redis.HashOperator.Instance.Initialize(redis);
            }
            logger.Debug("StartupDebugger-4");
            string ipAddress = Configuration["mongodb:ipAddress"];
            string mongoDatabase = Configuration["mongodb:databaseName"];
            string port = Configuration["mongodb:port"];
            string userName = Configuration["mongodb:userName"];
            string passWord = Configuration["mongodb:passWord"];

            var MIPAddressModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MIPAddress");
            if (MIPAddressModel != null && !string.IsNullOrEmpty(MIPAddressModel.cn_s_setting_keyvalue)) ipAddress = MIPAddressModel.cn_s_setting_keyvalue;
            var MMDatabaseNameModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MDatabaseName");
            if (MMDatabaseNameModel != null && !string.IsNullOrEmpty(MMDatabaseNameModel.cn_s_setting_keyvalue)) mongoDatabase = MMDatabaseNameModel.cn_s_setting_keyvalue;
            var MPortModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MPort");
            if (MPortModel != null && !string.IsNullOrEmpty(MPortModel.cn_s_setting_keyvalue)) port = MPortModel.cn_s_setting_keyvalue;
            var MUserNameModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MUserName");
            if (MUserNameModel != null && !string.IsNullOrEmpty(MUserNameModel.cn_s_setting_keyvalue)) userName = MUserNameModel.cn_s_setting_keyvalue;
            var MPassWordModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MPassWord");
            if (MPassWordModel != null && !string.IsNullOrEmpty(MPassWordModel.cn_s_setting_keyvalue)) passWord = MPassWordModel.cn_s_setting_keyvalue;
            //ipAddress = "192.168.8.159";
            //mongoDatabase = "fyyj";
            //port = "27017";
            //userName = "fyyj";
            //passWord = "123456";


            if (!string.IsNullOrEmpty(ipAddress) && !string.IsNullOrEmpty(mongoDatabase)
                && !string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(passWord)
                )
            {
                MongoDBSingleton.Instance.InitMongoDB(ipAddress, Convert.ToInt32(port), mongoDatabase, userName, passWord);
            }
            logger.Debug("StartupDebugger-5");

            SysConst.MDGApi = AppSettings.GetValue<string>("SysInterface:Mdg");
            var MDGApiModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "MDGApi");
            if (MDGApiModel != null && !string.IsNullOrEmpty(MDGApiModel.cn_s_setting_keyvalue)) SysConst.MDGApi = MDGApiModel.cn_s_setting_keyvalue;
            #region 加载项
            logger.Debug("StartupDebugger-6");
            // 捕获 Request.Body 允许同步读取IO流
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            //注入缓存
            services.AddMemoryCache();

            //注入 HTTPCONTEXT
            services.AddHttpContextAccessor();
            
            //注入 TokenManager
            //services.AddScoped<TokenManager>();

            //注入全局异常过滤
            services.AddControllers(options =>
            {
                //全局异常过滤
                options.Filters.Add<GlobalExceptions>();
                //全局日志
                options.Filters.Add<GlobalActionMonitor>();

            })
            .ConfigureApiBehaviorOptions(options =>
            {
                //抑制系统自带模型验证
                options.SuppressModelStateInvalidFilter = true;
            });
            //初始化常量
            SysConst.Initalize();

            //注册REDIS 服务
            //RedisServer.Initalize();
            #endregion

            // 增加Http组件
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            HelperHttpContext.serviceCollection = services;

            //将最新的自动备份信息写入全局变量
            try
            {
                BackupsDriver.Instance.DBackups = new Interfaces.Service.Sys.BackupsService(new DbHelper.SessionInfo()
                {
                    token = "",
                    splitDbCode = ""
                }).GetLatestSaveBackups();
            }
            catch { }
            BackupsDriver.Instance.LastDBackups = BackupsDriver.Instance.DBackups;
            BackupsDriver.Instance.LastBackups = DateTime.Now;
            BackupsDriver.Instance.IsFirst = true;
            logger.Debug("StartupDebugger-7");
            //将最新的相机区域坐标系信息写入全局变量
            try
            {
                CameraAreaDriver.Instance.LatestCameraAreaPointList = new Interfaces.Service.Sys.CameraAreaService(new DbHelper.SessionInfo()
                {
                    token = "",
                    splitDbCode = ""
                }).GetAllPoint();
            }
            catch { }

            #region 优化项
            // 时间：2026-05-29
            // 原逻辑：逐个执行 GoodscommandDriver.Instance.RefreshStockItemInformation。
            // 问题：货位设备数量较多时串行执行耗时长，会拖慢 API 站点启动。
            try
            {
                foreach (var goodsequipment in GoodsequipmentList)
                {
                    GoodscommandDriver.Instance.RefreshStockItemInformationV2(goodsequipment.cn_guid);
                }
            }
            catch { }
            #endregion

            if (dbConnectioned)
            {
                //添加托管线程
                services.AddHostedService<DeviceThread>();
                if (AppSettings.GetValue<bool>("AppSettings:Thread:EnableRepair"))
                {
                    services.AddHostedService<RepairThread>();
                }
                if (AppSettings.GetValue<bool>("AppSettings:Thread:EnalbeUpkeep"))
                {
                    services.AddHostedService<UpkeepThread>();
                }
                services.AddHostedService<BackupsThread>();
                services.AddHostedService<LogsThread>();

            }

            //WMS Redis 初始化
            WMSRedisServer.Initalize();
            ////注入 Redis 订阅后台服务
            services.AddHostedService<RedisSubscribeHostedService>();
            logger.Debug("StartupDebugger-8");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            logger.Debug("StartupDebugger-9");
            app.UsePerformanceLog();

            #region 开发错误提示

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            };
            #endregion

            logger.Debug("StartupDebugger-10");

            #region 跨域设置
            app.UseCors("LimitRequests");
            #endregion

            #region Api文档说明
            if (AppSettings.GetValue<bool>("AppSettings:UseSwagger"))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    string ApiName = "";
                    var ApiNameModel = SysSetList.FirstOrDefault(p => p.cn_s_setting_keycode == "ApiName");
                    if (ApiNameModel != null && !string.IsNullOrEmpty(ApiNameModel.cn_s_setting_keyvalue)) ApiName = ApiNameModel.cn_s_setting_keyvalue;
                    else ApiName = AppSettings.GetValue<string>("Startup:ApiName");
                    if (string.IsNullOrEmpty(ApiName)) ApiName = "IDTSCoreApi";
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiName);
                    c.RoutePrefix = ""; //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉，如果你想换一个路径，直接写名字即可，比如直接写c.RoutePrefix = "doc";
            });
            }
            #endregion
            logger.Debug("StartupDebugger-11");
            #region 加载项

            // 请求日志监控   暂时去掉，以后对于对外开放的接口可以加入该监听
            //app.UseMiddleware<RequestMiddleware>();

            //httpcontext
            app.UseStaticHttpContext();
            // 使用静态文件
            app.UseForwardedHeaders();
            // 使用静态文件
            app.UseStaticFiles();
            // 使用cookie
            app.UseCookiePolicy();
            // 使用Routing
            app.UseRouting();

            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #endregion
            logger.Debug("StartupDebugger-12");

            app.UseStaticHttpContext();

            logger.Debug("StartupDebugger-13");
        }

        #region 自动注入服务
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();
            logger.Debug("StartupDebugger-14");
            var assemblysServices = Assembly.Load("HZ.IDTSCore.Interfaces");
            //var assemblysServices2 = Assembly.Load("HZ.WMSCore.AI");
            builder.RegisterAssemblyTypes(assemblysServices)
                .InstancePerDependency()//瞬时单例
               .AsImplementedInterfaces()////自动以其实现的所有接口类型暴露（包括IDisposable接口）
               .EnableInterfaceInterceptors(); //引用Autofac.Extras.DynamicProxy;

            builder.RegisterBuildCallback(lifetimeScope =>
            {
                ServiceLocator.SetContainer(lifetimeScope as IContainer);
            });

            logger.Debug("StartupDebugger-15");
        }
        #endregion

    }
}
