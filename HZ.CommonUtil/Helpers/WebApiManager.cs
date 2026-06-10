using HZ.CommonUtil.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HZ.CommonUtil.Helpers
{
    public class WebApiManager
    {
        private const int DefaultHttpTimeoutSeconds = 50;

        /// <summary>
        /// 共享HttpClient。
        /// 时间：2026-06-03
        /// 优化内容：避免每次请求都new HttpClient，减少高并发下端口耗尽、TIME_WAIT堆积和TCP重复建连成本。
        /// 注意：不要把token、splitDb等动态请求头写到DefaultRequestHeaders，动态头统一放到HttpRequestMessage。
        /// </summary>
        private static readonly HttpClient SharedHttpClient = CreateHttpClient();

        private class WebApiResponse
        {
            public bool IsSuccessStatusCode { get; set; }

            public HttpStatusCode StatusCode { get; set; }

            public string Content { get; set; }
        }

        private static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                // 保留老代码忽略证书校验的行为；生产环境建议改为校验证书或只对指定域名放行。
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };

            return new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(GetHttpTimeoutSeconds())
            };
        }

        private static int GetHttpTimeoutSeconds()
        {
            int timeoutSeconds = AppSettings.GetValue<int>("AppSettings:HttpClientTimeoutSeconds");
            if (timeoutSeconds <= 0)
            {
                timeoutSeconds = AppSettings.GetValue<int>("WebApiManager:TimeoutSeconds");
            }
            return timeoutSeconds > 0 ? timeoutSeconds : DefaultHttpTimeoutSeconds;
        }

        private static string NormalizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return "";
            }
            return url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : "http://" + url;
        }

        private static string CombineUrl(string ipPort, string path)
        {
            return NormalizeUrl(ipPort).TrimEnd('/') + "/" + (path ?? "").TrimStart('/');
        }

        private static string AppendQueryString(string url, string paramStr)
        {
            if (string.IsNullOrWhiteSpace(paramStr))
            {
                return url;
            }
            return url + (url.Contains("?") ? "&" : "?") + paramStr.TrimStart('?');
        }

        private static string GetAcceptLanguage()
        {
            try
            {
                return HelperHttpContext.Current?.Request?.Headers["Accept-Language"].ToString();
            }
            catch
            {
                return "";
            }
        }

        private static void AddRequestHeaders(HttpRequestMessage request, string token = "", string splitDbCode = "", Dictionary<string, string> headers = null)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string acceptLanguage = GetAcceptLanguage();
            if (!string.IsNullOrWhiteSpace(acceptLanguage))
            {
                request.Headers.TryAddWithoutValidation("Accept-Language", acceptLanguage);
            }

            if (!string.IsNullOrWhiteSpace(splitDbCode))
            {
                request.Headers.TryAddWithoutValidation("splitDb", splitDbCode);
            }

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.TryAddWithoutValidation("token", token);
            }

            if (headers == null)
            {
                return;
            }

            foreach (var header in headers)
            {
                if (string.IsNullOrWhiteSpace(header.Key))
                {
                    continue;
                }

                if (string.Equals(header.Key, "Content-Type", StringComparison.OrdinalIgnoreCase) && request.Content != null)
                {
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value);
                    continue;
                }

                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        private static async Task<WebApiResponse> SendAsync(HttpMethod method, string url, string data = "", string token = "", string splitDbCode = "", Dictionary<string, string> headers = null)
        {
            using (var request = new HttpRequestMessage(method, url))
            {
                if (method != HttpMethod.Get && data != null)
                {
                    request.Content = new StringContent(data, Encoding.UTF8, "application/json");
                }

                AddRequestHeaders(request, token, splitDbCode, headers);

                using (var response = await SharedHttpClient.SendAsync(request).ConfigureAwait(false))
                {
                    return new WebApiResponse()
                    {
                        IsSuccessStatusCode = response.IsSuccessStatusCode,
                        StatusCode = response.StatusCode,
                        Content = await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                    };
                }
            }
        }

        private static string SendForApiResult(HttpMethod method, string url, string data, ref ApiResult res, string token = "", string splitDbCode = "", Dictionary<string, string> headers = null)
        {
            try
            {
                WebApiResponse response = SendAsync(method, url, data, token, splitDbCode, headers).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    res = ApiResult.Success();
                    return response.Content;
                }

                res = ApiResult.Error(response.StatusCode.ToString(), 13010001);
                return "";
            }
            catch (Exception ex)
            {
                LogHelper.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
                res = ApiResult.Error(ex.Message);
                return "";
            }
        }

        private static string CleanResponseString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Replace("\\", "").TrimEnd('"').TrimStart('"');
        }

        #region Web Request
        /// <summary>
        /// Get请求，ip端口和路由分开传参
        /// </summary>
        /// <remark>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-08-19 14:04
        /// </remark>
        /// <param name="ipPort">ip端口,支持带http和不带</param>
        /// <param name="path">路由地址，地址前缀带/和不带/</param>
        /// <param name="res">接口传输层的响应</param>
        /// <param name="paramStr">参数，默认空</param>
        /// <param name="token">token，默认空</param>
        /// <returns></returns>
        public static string HttpGet(string ipPort, string path, ref ApiResult res, string paramStr = "", string token = "",Dictionary<string,string> headers=null)
        {
            string webUrl = AppendQueryString(CombineUrl(ipPort, path), paramStr);
            return SendForApiResult(HttpMethod.Get, webUrl, null, ref res, token, "", headers);
        }

        /// <summary>
        /// Post请求，ip端口和路由分开传参
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-08-19 14:13
        /// </remarks>
        /// <param name="ipPort">ip端口,支持带http和不带</param>
        /// <param name="path">路由地址，地址前缀带/和不带/</param>
        /// <param name="data">入参</param>
        /// <param name="res">接口传输层的响应</param>
        /// <param name="token">token，默认空</param>
        /// <param name="splitDbCode">分库标识，默认空</param>
        /// <returns></returns>
        public static string HttpPost(string ipPort, string path, string data, ref ApiResult res, string token = "", string splitDbCode = "", Dictionary<string, string> headers = null)
        {
            string url = CombineUrl(ipPort, path);
            return SendForApiResult(HttpMethod.Post, url, data, ref res, token, splitDbCode, headers);
        }
        #endregion

        /// <summary>
        /// Get请求，ip端口和路由合并
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-08-19 14:16
        /// </remarks>
        /// <param name="url">接口地址(包含ip和路由),支持带http和不带</param>
        /// <param name="res">接口传输层的响应</param>
        /// <param name="token">token，默认空</param>
        /// <param name="splitDbCode">分库标识，默认空</param>
        /// <returns></returns>
        public static string HttpGet(string url, ref ApiResult res, string token = "", string splitDbCode = "", Dictionary<string, string> headers = null)
        {
            string webUrl = NormalizeUrl(url);
            return SendForApiResult(HttpMethod.Get, webUrl, null, ref res, token, splitDbCode, headers);
        }

        /// <summary>
        /// Post请求，ip端口和路由合并，支持传请求头Headers
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-08-19 14:18
        /// </remarks>
        /// <param name="url">接口地址(包含ip和路由),支持带http和不带</param>
        /// <param name="data">入参</param>
        /// <param name="res">接口传输层的响应</param>
        /// <param name="headers">Headers头数据，默认null</param>
        /// <returns></returns>
        public static string HttpPost(string url, string data, ref ApiResult res, Dictionary<string, string> headers = null)
        {
            url = NormalizeUrl(url);
            return SendForApiResult(HttpMethod.Post, url, data, ref res, "", "", headers);
        }

        /// <summary>
        /// 带Bsic验证的Post请求，ip端口和路由合并，支持传请求头Headers
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-08-19 14:26
        /// </remarks>
        /// <param name="url">接口地址(包含ip和路由),支持带http和不带</param>
        /// <param name="data">入参</param>
        /// <param name="res">接口传输层的响应</param>
        /// <param name="headers">Headers头数据，默认null</param>
        /// <param name="userCode">账号</param>
        /// <param name="userPwd">密码</param>
        /// <returns></returns>
        public static string HttpPostBasicVerify(string url, string data, ref ApiResult res, Dictionary<string, string> headers = null, string userCode = "", string userPwd = "")
        {
            try
            {
                url = NormalizeUrl(url);
                Dictionary<string, string> requestHeaders = headers == null
                    ? new Dictionary<string, string>()
                    : new Dictionary<string, string>(headers);

                if (!string.IsNullOrWhiteSpace(userCode) || !string.IsNullOrWhiteSpace(userPwd))
                {
                    string basicToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userCode}:{userPwd}"));
                    requestHeaders["Authorization"] = "Basic " + basicToken;
                }

                string returnStr = SendForApiResult(HttpMethod.Post, url, data, ref res, "", "", requestHeaders);
                return CleanResponseString(returnStr);
            }
            catch (WebException ex)
            {
                res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
                return "";
            }
            catch (Exception ex)
            {
                res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
                return "";
            }
        }

        #region 微信_HttpGet请求
        /// <summary>
        /// Http GET
        /// </summary>
        /// <param name="webUrl">微信开放平台接口地址</param>
        /// <param name="paramStr"></param>
        /// <returns></returns>
        public static string HttpWechat_Get(string webUrl, string paramStr = "")
        {
            try
            {
                string dataUrl = AppendQueryString(NormalizeUrl(webUrl), paramStr);
                WebApiResponse response = SendAsync(HttpMethod.Get, dataUrl).GetAwaiter().GetResult();
                return response.IsSuccessStatusCode ? CleanResponseString(response.Content) : "";
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region 微信_HttpPost请求
        /// <summary>
        /// HttpPost请求
        /// </summary>
        /// <param name="weburl">微信开放平台接口地址</param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HttpWechat_Post(string weburl, string data)
        {
            try
            {
                weburl = NormalizeUrl(weburl);
                WebApiResponse response = SendAsync(HttpMethod.Post, weburl, data).GetAwaiter().GetResult();
                return response.IsSuccessStatusCode ? CleanResponseString(response.Content) : "";
            }
            catch
            {
                return "";
            }
        }
        #endregion

        public static async Task<string> HttpClientGetAsync(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                string ipPort = NormalizeUrl(url);
                WebApiResponse response = await SendAsync(HttpMethod.Get, ipPort, null, "", "", headers).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return response.Content;
                }
                return JsonConvert.SerializeObject(ApiResult.Error($"(13010001)访问[{url}]异常[{response.IsSuccessStatusCode}]", 13010001));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]"));
            }
        }

        public static async Task<string> HttpClientPutAsync(string url, string data, Dictionary<string, string> headers = null)
        {
            try
            {
                string ipPort = NormalizeUrl(url);
                WebApiResponse response = await SendAsync(HttpMethod.Put, ipPort, data, "", "", headers).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return response.Content;
                }
                return JsonConvert.SerializeObject(ApiResult.Error($"(13010001)访问[{url}]异常[{response.IsSuccessStatusCode}]"));
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]"));
            }
        }
    }
}
