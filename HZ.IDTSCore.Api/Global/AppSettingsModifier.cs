using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.IO;

namespace HZ.IDTSCore.Api
{
    public class AppSettingsModifier
    {
        public static void UpdateAppSettings(string key, string value)
        {
            try
            {
                // 获取appsettings.json的完整路径
                string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

                // 读取文件到JObject
                var jsonObj = JObject.Parse(File.ReadAllText(appSettingsPath));

                // 修改你想修改的值
                jsonObj[key] = value;

                // 将修改后的对象重新写入appsettings.json文件
                File.WriteAllText(appSettingsPath, jsonObj.ToString());
            }
            catch { }
        }

        public static void UpdateNestedAppSettings(string parentKey, string childKey, string value)
        {
            try
            {
                // 获取appsettings.json的完整路径
                string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

                // 读取文件到JObject
                var jsonObj = JObject.Parse(File.ReadAllText(appSettingsPath));

                // 修改你想修改的值
                ((JObject)jsonObj[parentKey])[childKey] = value;

                // 将修改后的对象重新写入appsettings.json文件
                File.WriteAllText(appSettingsPath, jsonObj.ToString());
            }
            catch { }
        }
    }
}
