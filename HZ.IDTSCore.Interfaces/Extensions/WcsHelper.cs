using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Common.Const;
using HZ.IDTSCore.Model.Entity.Basic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Extensions
{
    /// <summary>
    /// WCS对接请求类
    /// </summary>
    public class WcsHelper
    {
        /// <summary>
        /// 是否生产环境调用WCS
        /// </summary>
        public readonly static bool WCS_PRODUCE = AppSettings.GetValue<bool>("ProjectSet:WCS_PRODUCE_SCCH");    //test测试环境，produce生产环境

        private static string APP_SECRET()
        {
            return "test";//WCS_TEST ? "test" : "produce";
        }

        /// <summary>
        /// 请求数据
        /// </summary>
        /// <param name="func"></param>
        /// <param name="content"></param>
        public static WcsResult Post(string func, string content, string token, int wcsLine = 1)
        {
            if (!WCS_PRODUCE)
                return new WcsResult { code = 200, msg = "", desc = string.Empty };

            string ip = wcsLine == 1 ? AppSettings.GetValue<string>("SysInterface:Wcs") : AppSettings.GetValue<string>("SysInterface:Wcs2"); //从配置文件取
            if (string.IsNullOrEmpty(ip))
                return new WcsResult { code = 500, msg = "WMS配置文件未找到Mes API地址！" };

            string path = "/wcs/butt";
            ApiResult res = ApiResult.Success();

            var sign = Sign(func, content);
            var data = JsonConvert.SerializeObject(new
            {
                api = func,
                data = content,
                sign
            });

            LogHelper.Info($"对接WCS参数：{ip}，{path},{data}");
            switch (func)
            {
                case SysKeyword.WCS_ITEM_BUTT:
                case SysKeyword.WCS_RECEIPT_BUTT:
                case SysKeyword.WCS_SHIPMENT_BUTT:
                case SysKeyword.WCS_CYCLECOUNT_BUTT:
                    //添加到接口log里
                    tn_wms_interface_log log = new tn_wms_interface_log()
                    {
                        cn_s_data_type = "JSON",
                        cn_s_guid = Guid.NewGuid().ToString(),
                        cn_s_mode = "POST",
                        cn_t_create = DateTime.Now,
                        cn_s_url = $"http://{ip}" + path,
                        cn_s_state = "待执行",
                        cn_s_infcode = func,
                        cn_s_data = data
                    };
                    //AutoCallService.GetInstance(new DbHelper.SessionInfo() { token = token}).Add(log);
                    //LogHelper.Info($"自动推送WCS结果：{JsonConvert.SerializeObject(log)}");
                    return new WcsResult { code = 0, msg = res.Message, desc = string.Empty };
                default:
                    string result = WebApiManager.HttpPost(ip, path, data, ref res);
                    //LogHelper.Info($"直接调WCS结果：{result}");
                    if (res.IsSuccess)
                        return JsonConvert.DeserializeObject<WcsResult>(result);
                    return new WcsResult { code = 500, msg = res.Message, desc = string.Empty };
            }
        }

        /// <summary>
        /// 按仓库查询WCS产线
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns></returns>
        public static int GetWcsLine(string stockCode)
        {
            return "Y009".Equals(stockCode) ? 2 : 1;
        }

        /// <summary>
        /// 生成检验码
        /// </summary>
        /// <param name="api"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Sign(string api, string data)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("api", api);
            param.Add("data", data);
            //var obj = new { 
            //    api, data
            //};
            StringBuilder sb = new StringBuilder(APP_SECRET());
            foreach (var dict in param)
            {
                sb.Append(dict.Key);
                sb.Append(dict.Value);
            }

            sb.Append(APP_SECRET());

            var json = sb.ToString();
            var md5String = TO32MD5(json);
            //var result = Convert.ToBase64String(Encoding.UTF8.GetBytes(md5String));
            return md5String;
        }

        /// <summary>
        /// 32位小加密
        /// </summary>
        /// <param name="srcstr"></param>
        /// <returns></returns>
        public static string TO32MD5(string srcstr)
        {
            MD5 md5 = MD5.Create();
            string md5str = "";//加密后的string
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(srcstr));
            for (int i = 0; i < s.Length; i++)
            {
                string btos = s[i].ToString("x2");//.ToString("x");
                //if (btos.Length == 1)
                //{
                //    //每次转换得到的都是2位
                //    btos = "0" + btos;
                //}
                md5str = md5str + btos;//转换成十六进制
            }
            return md5str;
        }


    }

    /// <summary>
    /// WCS反馈结果
    /// </summary>
    public class WcsResult
    {
        /// <summary>
        /// code 为0代表请求成功
        /// </summary>
        public int code { get; set; }

        public string msg { get; set; }

        public string desc { get; set; }

        public object data { get; set; }

        public WcsPageInfo pageInfo { get; set; }
    }

    public class WcsPageInfo
    {
        public int total { get; set; }

        public int pageSize { get; set; }

        public int current { get; set; }
    }
}
