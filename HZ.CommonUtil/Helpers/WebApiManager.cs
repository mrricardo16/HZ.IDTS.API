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
            string webUrl = "";
            try
            {
                if (ipPort.StartsWith("http"))
                    webUrl = ipPort + "/" + path.TrimStart('/');
                else
                    webUrl = "http://" + ipPort + "/" + path.TrimStart('/');

                string Data = webUrl + (paramStr == "" ? "" : "?") + paramStr;

                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                using var client = new HttpClient();

                // 添加 Token 到请求头
                client.DefaultRequestHeaders.Add("token", token);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Content-Type", "application/json");
                if (HelperHttpContext.Current != null)
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(HelperHttpContext.Current.Request.Headers["Accept-Language"].ToString());
                try
                {
                    // 将请求头添加到 HttpClient
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    // 发送 GET 请求
                    var response = client.GetAsync(webUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        var content = response.Content.ReadAsStringAsync().Result;
                        res = ApiResult.Success();
                        return content;
                    }
                    else
                    {
                        res = ApiResult.Error(response.StatusCode.ToString(), 13010001);
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"(13010001)访问[{webUrl}]异常,[{ex.Message}]");
                    res = ApiResult.Error(ex.Message);
                    return "";
                }
            }
            catch (Exception ex)
            {
                res = ApiResult.Error(ex.Message);
                return "";
            }

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
            //string returnStr = "";

            string url = "";
            if (ipPort.StartsWith("http"))
                url = ipPort + "/" + path.TrimStart('/');
            else
                url = "http://" + ipPort + "/" + path.TrimStart('/');
            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);
                using var client = new HttpClient();

                // 添加 Token 到请求头
                if (HelperHttpContext.Current!=null)
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(HelperHttpContext.Current.Request.Headers["Accept-Language"].ToString());
                client.DefaultRequestHeaders.Add("splitDb", splitDbCode);
                client.DefaultRequestHeaders.Add("token", token);
                try
                {
                    // 将请求头添加到 HttpClient
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    // 发送 GET 请求

                    var response = client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")).Result;
                    //string resp = await httpResponseMessage.Content.ReadAsStringAsync();

                    //var response = client.PostAsync(url,).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        var content = response.Content.ReadAsStringAsync().Result;
                        res = ApiResult.Success();
                        return content;
                    }
                    else
                    {
                        res = ApiResult.Error(response.StatusCode.ToString(), 13010001);
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
                    res = ApiResult.Error(ex.Message);
                    return "";
                }
            }
            catch (Exception ex)
            {
                res = ApiResult.Error(ex.Message);
                return "";
            }


            //    //对所有代码产生的垃圾资源进行强制回收-慎用
            //    //System.GC.Collect();
            //    //请求地址获取用户授权信息
            //    WebRequest webRequest = System.Net.WebRequest.Create(url);
            //    webRequest.Timeout = 1000 * 10;//设置请求超时时间10秒

            //    System.Net.ServicePointManager.ServerCertificateValidationCallback =
            //        ((sender, certificate, chain, sslPolicyErrors) => true);

            //    HttpWebRequest httpRequest = webRequest as System.Net.HttpWebRequest;
            //    httpRequest.Method = "post";
            //    httpRequest.ContentType = "application/json";
            //    httpRequest.KeepAlive = false;//设置不是常连接
            //    httpRequest.Headers.Add("token", token);
            //    httpRequest.Headers.Add("splitDb", splitDbCode);
            //    httpRequest.Headers.Add("Accept-Language", HttpContext.Current.Request.Headers["Accept-Language"].First());

            //    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            //    byte[] bytesToPost = encoding.GetBytes(data);
            //    httpRequest.ContentLength = bytesToPost.Length;
            //    System.IO.Stream requestStream = httpRequest.GetRequestStream();
            //    requestStream.Write(bytesToPost, 0, bytesToPost.Length);
            //    requestStream.Close();

            //    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            //    Stream stream = response.GetResponseStream();
            //    StreamReader Reader = new StreamReader(stream, Encoding.UTF8);
            //    returnStr = Reader.ReadToEnd();
            //    if (!string.IsNullOrEmpty(returnStr))
            //    {
            //        returnStr = returnStr.Replace("\\", "");
            //        returnStr = returnStr.TrimEnd('\"');
            //        returnStr = returnStr.TrimStart('\"');
            //    }

            //    if (webRequest != null)
            //    {
            //        webRequest.Abort();
            //    }
            //    if (httpRequest != null)
            //    {
            //        httpRequest.Abort();
            //    }
            //    if (response != null)
            //    {
            //        response.Dispose();
            //        response.Close();
            //    }
            //    if (stream != null)
            //    {
            //        stream.Dispose();
            //        stream.Close();
            //    }
            //    if (Reader != null)
            //    {
            //        Reader.Dispose();
            //        Reader.Close();
            //    }
            //    res.IsSuccess = true;
            //    return returnStr;
            //}
            //catch (WebException ex)
            //{
            //    res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
            //    return "";
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Info($"{url}无法访问，请检查");
            //    res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
            //    return "";
            //}
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
            try
            {
                string webUrl = "";
                if (url.StartsWith("http"))
                    webUrl = url;
                else
                    webUrl = "http://" + url;

                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                using var client = new HttpClient();
                // 添加 Token 到请求头
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Content-Type", "application/json");
                if (HelperHttpContext.Current != null)
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(HelperHttpContext.Current.Request.Headers["Accept-Language"].ToString());
                client.DefaultRequestHeaders.Add("splitDb", splitDbCode);
                client.DefaultRequestHeaders.Add("token", token);
                try
                {
                    // 将请求头添加到 HttpClient
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    // 发送 GET 请求
                    var response = client.GetAsync(webUrl).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        var content = response.Content.ReadAsStringAsync().Result;
                        res = ApiResult.Success();
                        return content;
                    }
                    else
                    {
                        res = ApiResult.Error(response.StatusCode.ToString(), 13010001);
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"(13010001)访问[{webUrl}]异常,[{ex.Message}]");
                    res = ApiResult.Error(ex.Message);
                    return "";
                }
            }
            catch (Exception ex)
            {
                res = ApiResult.Error(ex.Message);
                return "";
            }

            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ipPort);
            //    request.Method = "GET";
            //    request.ContentType = "application/json";
            //    request.KeepAlive = false;//设置不是长连接
            //    request.Headers.Add("token", token);
            //    request.Headers.Add("splitDb", splitDbCode);
            //    request.Headers.Add("Accept-Language", HttpContext.Current.Request.Headers["Accept-Language"].First());

            //    if (headers != null)
            //    {
            //        foreach (var m in headers)
            //            request.Headers.Add(m.Key, m.Value);
            //    }
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //    Stream myResponseStream = response.GetResponseStream();
            //    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            //    string retString = myStreamReader.ReadToEnd();
            //    //if (!string.IsNullOrEmpty(retString))
            //    //{
            //    //    retString = retString.Replace("\\", "");
            //    //    retString = retString.TrimEnd('\"');
            //    //    retString = retString.TrimStart('\"');
            //    //}
            //    if (request != null)
            //    {
            //        request.Abort();
            //    }
            //    if (response != null)
            //    {
            //        response.Dispose();
            //        response.Close();
            //    }
            //    if (myResponseStream != null)
            //    {
            //        myResponseStream.Dispose();
            //        myResponseStream.Close();
            //    }
            //    if (myStreamReader != null)
            //    {
            //        myStreamReader.Dispose();
            //        myStreamReader.Close();
            //    }

            //    myStreamReader.Close();
            //    myResponseStream.Close();

            //    res.IsSuccess = true;
            //    return retString;
            //}
            //catch (WebException ex)
            //{
            //    res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
            //    return "";
            //}
            //catch (Exception ex)
            //{
            //    res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
            //    return "";
            //}
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
            try
            {
                if (!url.StartsWith("http"))
                    url = "http://" + url;

                using var client = new HttpClient();

                if (HelperHttpContext.Current != null)
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(HelperHttpContext.Current.Request.Headers["Accept-Language"].ToString());
                try
                {
                    // 将请求头添加到 HttpClient
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }
                    var response = client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        var content = response.Content.ReadAsStringAsync().Result;
                        res = ApiResult.Success();
                        return content;
                    }
                    else
                    {
                        res = ApiResult.Error(response.StatusCode.ToString(), 13010001);
                        return "";
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
                    res = ApiResult.Error(ex.Message);
                    return "";
                }
            }
            catch (Exception ex)
            {
                res = ApiResult.Error(ex.Message);
                return "";
            }

            //    string returnStr = "";
            //    //对所有代码产生的垃圾资源进行强制回收-慎用
            //    //System.GC.Collect();
            //    //请求地址获取用户授权信息
            //    WebRequest webRequest = System.Net.WebRequest.Create(url);
            //    webRequest.Timeout = 100000000;//设置请求超时时间10秒

            //    System.Net.ServicePointManager.ServerCertificateValidationCallback =
            //        ((sender, certificate, chain, sslPolicyErrors) => true);
            //    HttpWebRequest httpRequest = webRequest as System.Net.HttpWebRequest;
            //    httpRequest.Method = "post";
            //    httpRequest.ContentType = "application/json";
            //    httpRequest.Headers.Add("Accept-Language", HttpContext.Current.Request.Headers["Accept-Language"].First());
            //    httpRequest.KeepAlive = false;//设置不是常连接

            //    if (headers != null)
            //    {
            //        foreach (var m in headers)
            //            httpRequest.Headers.Add(m.Key, m.Value);
            //    }

            //    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            //    byte[] bytesToPost = encoding.GetBytes(data);
            //    httpRequest.ContentLength = bytesToPost.Length;
            //    System.IO.Stream requestStream = httpRequest.GetRequestStream();
            //    requestStream.Write(bytesToPost, 0, bytesToPost.Length);
            //    requestStream.Close();

            //    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

            //    Stream stream = response.GetResponseStream();
            //    StreamReader Reader = new StreamReader(stream, Encoding.UTF8);
            //    returnStr = Reader.ReadToEnd();
            //    if (!string.IsNullOrEmpty(returnStr))
            //    {
            //        returnStr = returnStr.Replace("\\", "");
            //        returnStr = returnStr.TrimEnd('\"');
            //        returnStr = returnStr.TrimStart('\"');
            //    }

            //    if (webRequest != null)
            //    {
            //        webRequest.Abort();
            //    }
            //    if (httpRequest != null)
            //    {
            //        httpRequest.Abort();
            //    }
            //    if (response != null)
            //    {
            //        response.Dispose();
            //        response.Close();
            //    }
            //    if (stream != null)
            //    {
            //        stream.Dispose();
            //        stream.Close();
            //    }
            //    if (Reader != null)
            //    {
            //        Reader.Dispose();
            //        Reader.Close();
            //    }
            //    res.IsSuccess = true;
            //    return returnStr;
            //}
            //catch (WebException ex)
            //{
            //    res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
            //    return "";
            //}
            //catch (Exception ex)
            //{
            //    res = ApiResult.Error($"(13010001)访问[{url}]异常,[{ex.Message}]");
            //    return "";
            //}
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
                if (!url.StartsWith("http"))
                    url = "http://" + url;
                string returnStr = "";
                //对所有代码产生的垃圾资源进行强制回收-慎用
                //System.GC.Collect();
                //请求地址获取用户授权信息
                WebRequest webRequest = System.Net.WebRequest.Create(url);
                webRequest.Timeout = 100000000;//设置请求超时时间10秒

                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);
                HttpWebRequest httpRequest = webRequest as System.Net.HttpWebRequest;
                httpRequest.Method = "post";
                httpRequest.ContentType = "application/json";
                httpRequest.Headers.Add("Accept-Language", HelperHttpContext.Current.Request.Headers["Accept-Language"].First());
                httpRequest.KeepAlive = false;//设置不是常连接

                if (headers != null)
                {
                    foreach (var m in headers)
                        httpRequest.Headers.Add(m.Key, m.Value);
                }

                //（1）设置请求Credentials
                //CredentialCache credentialCache = new CredentialCache();
                //credentialCache.Add(new Uri(url), "Basic", new NetworkCredential(userCode, userPwd));
                //httpRequest.Credentials = credentialCache;

                ////（2）设置Headers Authorization
                //httpRequest.Headers.Add("Authorization", "Basic" + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userCode}:{userPwd}")));

                httpRequest.Credentials = new NetworkCredential(userCode, userPwd);

                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                byte[] bytesToPost = encoding.GetBytes(data);
                httpRequest.ContentLength = bytesToPost.Length;
                System.IO.Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();

                Stream stream = response.GetResponseStream();
                StreamReader Reader = new StreamReader(stream, Encoding.UTF8);
                returnStr = Reader.ReadToEnd();
                if (!string.IsNullOrEmpty(returnStr))
                {
                    returnStr = returnStr.Replace("\\", "");
                    returnStr = returnStr.TrimEnd('\"');
                    returnStr = returnStr.TrimStart('\"');
                }

                if (webRequest != null)
                {
                    webRequest.Abort();
                }
                if (httpRequest != null)
                {
                    httpRequest.Abort();
                }
                if (response != null)
                {
                    response.Dispose();
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Dispose();
                    stream.Close();
                }
                if (Reader != null)
                {
                    Reader.Dispose();
                    Reader.Close();
                }
                res.IsSuccess = true;
                return returnStr;
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
            string Data = webUrl + (paramStr == "" ? "" : "?") + paramStr;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Data);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.KeepAlive = false;//设置不是常连接

            string retString = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                retString = myStreamReader.ReadToEnd();
                if (!string.IsNullOrEmpty(retString))
                {
                    retString = retString.Replace("\\", "");
                    retString = retString.TrimEnd('\"');
                    retString = retString.TrimStart('\"');
                }

                myStreamReader.Close();

                myResponseStream.Close();
                if (request != null)
                {
                    request.Abort();
                }
                if (response != null)
                {
                    response.Dispose();
                    response.Close();
                }
                if (myResponseStream != null)
                {
                    myResponseStream.Dispose();
                    myResponseStream.Close();
                }
                if (myStreamReader != null)
                {
                    myStreamReader.Dispose();
                    myStreamReader.Close();
                }
                return retString;
            }
            catch (Exception ex)
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
            string returnStr = "";
            try
            {
                //对所有代码产生的垃圾资源进行强制回收-慎用
                //System.GC.Collect();
                //请求地址获取用户授权信息
                WebRequest webRequest = System.Net.WebRequest.Create(weburl);
                webRequest.Timeout = 1000000;//设置请求超时时间10秒

                HttpWebRequest httpRequest = webRequest as System.Net.HttpWebRequest;
                httpRequest.Method = "post";
                httpRequest.ContentType = "application/json";
                httpRequest.KeepAlive = false;//设置不是常连接

                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                byte[] bytesToPost = encoding.GetBytes(data);
                httpRequest.ContentLength = bytesToPost.Length;
                System.IO.Stream requestStream = httpRequest.GetRequestStream();
                requestStream.Write(bytesToPost, 0, bytesToPost.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader Reader = new StreamReader(stream, Encoding.UTF8);
                returnStr = Reader.ReadToEnd();
                if (!string.IsNullOrEmpty(returnStr))
                {
                    returnStr = returnStr.Replace("\\", "");
                    returnStr = returnStr.TrimEnd('\"');
                    returnStr = returnStr.TrimStart('\"');
                }

                if (webRequest != null)
                {
                    webRequest.Abort();
                }
                if (httpRequest != null)
                {
                    httpRequest.Abort();
                }
                if (response != null)
                {
                    response.Dispose();
                    response.Close();
                }
                if (stream != null)
                {
                    stream.Dispose();
                    stream.Close();
                }
                if (Reader != null)
                {
                    Reader.Dispose();
                    Reader.Close();
                }
            }
            catch
            {
                returnStr = "";
            }
            return returnStr;
        }
        #endregion

        public static async Task<string> HttpClientGetAsync(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                string ipPort = "";
                if (url.StartsWith("http"))
                    ipPort = url;
                else
                    ipPort = "http://" + url;

                using var client = new HttpClient();
                // 将对象转换为 JSON 字符串
                //var json = JsonConvert.SerializeObject(data);
                //var content = new StringContent("", Encoding.UTF8, "application/json");
                if(HelperHttpContext.Current!=null)
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(HelperHttpContext.Current.Request.Headers["Accept-Language"].ToString());
                if (headers != null)
                {
                    foreach (var m in headers)
                        client.DefaultRequestHeaders.Add(m.Key, m.Value);
                }

                // 发送 POST 请求
                var response = await client.GetAsync(ipPort);

                if (response.IsSuccessStatusCode)
                {
                    // 读取响应内容
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response content: " + responseContent);
                    return responseContent;
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
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
                string ipPort = "";
                if (url.StartsWith("http"))
                    ipPort = url;
                else
                    ipPort = "http://" + url;

                using var client = new HttpClient();
                // 将对象转换为 JSON 字符串
                //var json = JsonConvert.SerializeObject(data);
                if (HelperHttpContext.Current != null)
                    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(HelperHttpContext.Current.Request.Headers["Accept-Language"].ToString());
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                if (headers != null)
                {
                    foreach (var m in headers)
                        client.DefaultRequestHeaders.Add(m.Key, m.Value);
                }

                // 发送 POST 请求
                var response = await client.PutAsync(ipPort, content);

                if (response.IsSuccessStatusCode)
                {
                    // 读取响应内容
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response content: " + responseContent);
                    return responseContent;
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
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
