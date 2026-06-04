using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace HZ.CommonUtil.Helpers
{
    /// <summary>
    /// appsettings.json操作类
    /// </summary>
    public class AppSettings
    {
        private static IConfiguration Configuration { get; set; }
        static AppSettings()
        {
            //ReloadOnChange = true 当appsettings.json被修改时重新加载            
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                //.AddJsonFile("appsettings.SIT.json", optional: true, reloadOnChange: true)
            .Build();
            //Configuration.AddEnvironmentVariables();

            //var env = hostingContext.HostingEnvironment;
            //config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            //config.AddEnvironmentVariables();
        }
        /// <summary>
        /// 获得配置文件的对象值
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetJson(string jsonPath, string key)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile(jsonPath).Build(); //json文件地址
            string s = config.GetSection(key).Value; //json某个对象
            return s;
        }

        public static T GetValue<T>(string key)
        {
            object o = AppSettings.Configuration[key];
            if (o == null && typeof(T) == typeof(bool))
                return (T)Convert.ChangeType(false, typeof(T));
            else if (o == null && typeof(T) == typeof(string))
                return (T)Convert.ChangeType("", typeof(T));
            else if (o == null && typeof(T) == typeof(int))
                return (T)Convert.ChangeType(0, typeof(T));
            return (T)Convert.ChangeType(o, typeof(T));
        }
    }
}
