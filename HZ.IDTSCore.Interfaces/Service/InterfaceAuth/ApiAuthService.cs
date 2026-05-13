using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Common.Const;
using HZ.IDTSCore.Interfaces.IService.InterfaceAuth;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Entity.Basic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Interfaces.Service.InterfaceAuth
{
    public class ApiAuthService : BaseService<object>, IAuthService
    {
        public ApiAuthService(SessionInfo session) : base(session)
        {
        }

        #region Post请求
        private ApiResult PostData(tn_wms_interface_def model)
        {
            return CommonHttpWebRequest(model);
        }
        private async Task<ApiResult> PostDataAsync(tn_wms_interface_def model)
        {
            return await CommonHttpWebRequestAsync(model);
        }
        #endregion

        #region Put请求
        private ApiResult PutData(tn_wms_interface_def model)
        {
            return CommonHttpWebRequest(model);
        }
        private async Task<ApiResult> PutDataAsync(tn_wms_interface_def model)
        {
            return await CommonHttpWebRequestAsync(model);
        }
        #endregion

        #region Delete请求
        private ApiResult DeleteData(tn_wms_interface_def model)
        {
            return CommonHttpWebRequest(model);
        }
        private async Task<ApiResult> DeleteDataAsync(tn_wms_interface_def model)
        {
            return await CommonHttpWebRequestAsync(model);
        }
        #endregion

        #region Get请求
        private ApiResult GetData(tn_wms_interface_def model)
        {
            return CommonHttpWebGetRequest(model);
        }
        private async Task<ApiResult> GetDataAsync(tn_wms_interface_def model)
        {
            return await CommonHttpWebGetRequestAsync(model);
        }
        #endregion



        private string guid = string.Empty;

        //HttpWebRequest通用方法
        private ApiResult CommonHttpWebRequest(tn_wms_interface_def model)
        {
            ApiResult apiResult = new ApiResult();

            try
            {
                InsertInterfaceLog(model, "待执行", string.Empty, string.Empty);

                //错误消息
                string errMsg = string.Empty;
                string result = DoSendRequest(model, out errMsg);
                JObject jObject = null;
                object resultFlag = null;

                if (!string.IsNullOrEmpty(result))
                {
                    //响应类型 JSON、XML
                    if (model.cn_s_result_type.ToUpper() == "JSON")
                    {
                        jObject = JsonConvert.DeserializeObject<JObject>(result);
                        //响应标识   类似 success  code
                        resultFlag = jObject[model.cn_s_result_flag].Value<object>();
                        if (resultFlag.ToString() != model.cn_s_succ_value)
                        {
                            errMsg = jObject[model.cn_s_err_msg_flag].Value<string>();
                            LogHelper.Error($"接口编码【{model.cn_s_infcode}】【{model.cn_s_infname}】返回失败消息：" + errMsg);
                            apiResult = ReTryInterface(model);
                        }
                        else
                        {
                            apiResult.IsSuccess = true;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        tn_wms_interface_log interfaceLog = null;

                        UpdateInterfaceLog(model, "已失败", result, errMsg, ref interfaceLog);
                        Db.Updateable<tn_wms_interface_log>(interfaceLog).ExecuteCommand();
                        //apiResult = UseTransaction(dbTran =>
                        //{
                        //    dbTran.Updateable<tn_wms_interface_log>(interfaceLog).ExecuteCommand();
                        //    long sequencsL = RedisServer.Sequence.LPush(SysKeyword.QueueItem_InterfaceReq, interfaceLog);
                        //});
                        apiResult = ReTryInterface(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, ex);
            }

            return apiResult;
        }


        /// <summary>
        /// 重试接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ApiResult ReTryInterface(tn_wms_interface_def model)
        {
            int i = 0;
            string result = string.Empty;
            string errMsg = string.Empty;
            tn_wms_interface_log log = null;
            JObject jObject = null;
            object resultFlag = null;
            ApiResult apiResult = new ApiResult();

            while (i < model.cn_n_max_retry)
            {
                result = DoSendRequest(model, out errMsg);

                log = new tn_wms_interface_log()
                {
                    cn_s_guid = guid,
                    cn_s_infcode = model.cn_s_infcode,
                    cn_s_url = string.Format("http://{0}:{1}{2}", model.cn_s_ip, model.cn_n_port.ToString(), model.cn_s_url),
                    cn_s_mode = model.cn_s_mode,
                    cn_s_data_type = model.cn_s_data_type,
                    cn_s_data = model.cn_s_data,
                    cn_s_response = result,
                    cn_t_create = DateTime.Now,
                    cn_s_header = model.cn_s_header,
                    cn_b_suspend = false,
                    cn_s_infname = model.cn_s_infname,
                    cn_s_server = "系统"
                };

                if (!string.IsNullOrEmpty(result))
                {
                    jObject = JsonConvert.DeserializeObject<JObject>(result);
                    //响应标识
                    resultFlag = jObject[model.cn_s_result_flag].Value<object>();
                    //失败
                    if (resultFlag.ToString() != model.cn_s_succ_value)
                    {
                        log.cn_s_state = "已失败";
                        log.cn_s_err_msg = errMsg;
                        log.cn_n_retry_coun = i + 1;
                        i++;

                        Db.Updateable<tn_wms_interface_log>(log).ExecuteCommand();
                        Thread.Sleep(3000);
                    }
                    else
                    {
                        log.cn_s_state = "已成功";
                        log.cn_s_err_msg = string.Empty;
                        log.cn_n_retry_coun = 0;
                        Db.Updateable<tn_wms_interface_log>(log).ExecuteCommand();

                        apiResult.IsSuccess = true;
                        break;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(errMsg))
                    {
                        tn_wms_interface_log interfaceLog = null;
                        UpdateInterfaceLog(model, "已失败", result, errMsg, ref interfaceLog);
                        interfaceLog.cn_n_retry_coun = i + 1;
                        i++;

                        Db.Updateable<tn_wms_interface_log>(interfaceLog).ExecuteCommand();
                        //apiResult = UseTransaction(dbTran =>
                        //{
                        //    dbTran.Updateable<tn_wms_interface_log>(interfaceLog).ExecuteCommand();
                        //    long sequencsL = RedisServer.Sequence.LPush(SysKeyword.QueueItem_InterfaceReq, interfaceLog);
                        //});
                        Thread.Sleep(3000);
                    }
                }
            }
            //超出最大重试次数
            if (i >= model.cn_n_max_retry && ((resultFlag != null && resultFlag.ToString() != model.cn_s_succ_value) || resultFlag == null))
            {
                apiResult.IsSuccess = false;
                apiResult.Message = $"接口编码【{model.cn_s_infcode}】【{model.cn_s_infname}】返回失败消息：" + errMsg;

                LogHelper.Error($"接口编码【{model.cn_s_infcode}】【{model.cn_s_infname}】返回失败消息：" + errMsg);
            }

            return apiResult;
        }

        /// <summary>
        /// 插入接口日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="state"></param>
        /// <param name="response"></param>
        /// <param name="errMsg"></param>
        private void InsertInterfaceLog(tn_wms_interface_def model, string state, string response, string errMsg)
        {
            guid = Guid.NewGuid().ToString();
            tn_wms_interface_log log = new tn_wms_interface_log()
            {
                cn_s_guid = guid,
                cn_s_infcode = model.cn_s_infcode,
                cn_s_url = string.Format("http://{0}:{1}{2}", model.cn_s_ip, model.cn_n_port.ToString(), model.cn_s_url),
                cn_s_mode = model.cn_s_mode,
                cn_s_data_type = model.cn_s_data_type,
                cn_s_data = model.cn_s_data,
                cn_s_state = state,
                cn_s_response = response,
                cn_t_create = DateTime.Now,
                cn_s_header = model.cn_s_header,
                cn_b_suspend = false,
                cn_s_infname = model.cn_s_infname,
                cn_s_server = "系统",
                cn_s_err_msg = errMsg,
                cn_n_retry_coun = 0
            };
            Db.Insertable<tn_wms_interface_log>(log).ExecuteCommand();
        }

        /// <summary>
        /// 修改接口日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="state"></param>
        /// <param name="response"></param>
        /// <param name="errMsg"></param>
        private void UpdateInterfaceLog(tn_wms_interface_def model, string state, string response, string errMsg,ref tn_wms_interface_log log)
        {
            log = new tn_wms_interface_log()
            {
                cn_s_guid = guid,
                cn_s_infcode = model.cn_s_infcode,
                cn_s_url = string.Format("http://{0}:{1}{2}", model.cn_s_ip, model.cn_n_port.ToString(), model.cn_s_url),
                cn_s_mode = model.cn_s_mode,
                cn_s_data_type = model.cn_s_data_type,
                cn_s_data = model.cn_s_data,
                cn_s_state = state,
                cn_s_response = response,
                cn_t_create = DateTime.Now,
                cn_s_header = model.cn_s_header,
                cn_b_suspend = false,
                cn_s_infname = model.cn_s_infname,
                cn_s_server = "系统",
                cn_s_err_msg = errMsg,
                cn_n_retry_coun = 0
            };
        }

        public string DoSendRequest(tn_wms_interface_def model, out string errMsg)
        {
            errMsg = string.Empty;
            string url = string.Empty;
            string port = string.Empty;
            if (model.cn_n_port.HasValue && model.cn_n_port.Value != 0)
            {
                port = ":" + model.cn_n_port.Value.ToString();
            }
            url = string.Format("http://{0}{1}{2}", model.cn_s_ip, port, model.cn_s_url);

            //构造http请求的对象
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //转成流
            byte[] buffer = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(model.cn_s_data);
            //请求设置
            request.Method = model.cn_s_mode;
            request.ContentLength = buffer.Length;
            //request.ContentType = "application/...";//设置参数格式，入参一定要与参数格式相对应
            request.ContentType = "application/json";
            request.MaximumAutomaticRedirections = 1;
            request.AllowAutoRedirect = true;

            if (!string.IsNullOrEmpty(model.cn_s_auth_type))
            {
                //Jwt认证
                if (model.cn_s_auth_type == SysKeyword.AuthType_Jwt)
                {
                    if (!string.IsNullOrEmpty(model.cn_s_jwttoken))
                    {
                        SetHeaderValue(request.Headers, "Authorization", model.cn_s_jwttoken);
                    }
                }
                //Basic认证
                if (model.cn_s_auth_type == SysKeyword.AuthType_Basic)
                {
                    //添加基础Basic认证
                    CredentialCache credentialCache = new CredentialCache();
                    credentialCache.Add(new Uri(model.cn_s_url), "Basic", new NetworkCredential(model.cn_s_usercode, model.cn_s_pwd));
                    request.Credentials = credentialCache;
                    string code = "{" + model.cn_s_usercode + "}:{" + model.cn_s_pwd + "}";

                    SetHeaderValue(request.Headers, "Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(code)));
                }
            }

            // 发送请求
            Stream newStream = request.GetRequestStream();
            newStream.Write(buffer, 0, buffer.Length);
            newStream.Close();
            // 获得接口返回值
            HttpWebResponse response;

            string result = string.Empty;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = streamReader.ReadToEnd();
                streamReader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                errMsg = $"【{model.cn_s_infcode}】【{model.cn_s_infname}】接口调用失败！，原因：{ex.Message}";
                LogHelper.Error($"【{model.cn_s_infcode}】【{model.cn_s_infname}】接口调用失败！，原因：{ex.Message}", ex);
            }

            return result;
        }

        //HttpWebRequest通用方法 异步
        private async Task<ApiResult> CommonHttpWebRequestAsync(tn_wms_interface_def model)
        {
            HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            HttpClient httpClient = new HttpClient(handler);
            //HttpContent是HTTP实体正文和内容标头的基类。
            HttpContent httpContent = new StringContent(model.cn_s_data, Encoding.UTF8, "text/json");

            if (!string.IsNullOrEmpty(model.cn_s_auth_type))
            {
                //Jwt认证
                if (model.cn_s_auth_type == SysKeyword.AuthType_Jwt)
                {
                    if (!string.IsNullOrEmpty(model.cn_s_jwttoken))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", model.cn_s_jwttoken);
                    }
                }
                //Basic认证
                if (model.cn_s_auth_type == SysKeyword.AuthType_Basic)
                {
                    //验证请求头赋值
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{model.cn_s_usercode}:{model.cn_s_pwd}")));
                }
            }

            //发送异步Post请求
            HttpResponseMessage response = await httpClient.PostAsync(model.cn_s_url, httpContent);
            response.EnsureSuccessStatusCode();
            string resultStr = await response.Content.ReadAsStringAsync();

            return ApiResult.Success("", resultStr);
        }

        /// <summary>
        /// 往头部加信息
        /// </summary>
        /// <param name="header"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        /// <summary>
        /// Get请求 异步
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<ApiResult> CommonHttpWebGetRequestAsync(tn_wms_interface_def model)
        {
            return await Task.Run(() =>
                CommonHttpWebGetRequest(model)
            );
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ApiResult CommonHttpWebGetRequest(tn_wms_interface_def model)
        {
            string result = string.Empty;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(model.cn_s_url + (!string.IsNullOrEmpty(model.cn_s_data) ? ("?" + model.cn_s_data) : ""));

            //添加参数
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            try
            {
                //获取内容
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }
            finally
            {
                stream.Close();
            }
            return ApiResult.Success("", result);
        }

        /// <summary>
        /// 接口请求发起
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResult SendRequest(tn_wms_interface_def model)
        {
            ApiResult res = new ApiResult();
            string mode = model.cn_s_mode.ToUpper();

            if (mode == "POST")
            {
                res = PostData(model);
            }
            else if (mode == "PUT")
            {
                res = PutData(model);
            }
            else if (mode == "GET")
            {
                res = GetData(model);
            }
            return res;
        }

        /// <summary>
        /// 接口请求发起 异步
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ApiResult> SendRequestAsync(tn_wms_interface_def model)
        {
            ApiResult res = new ApiResult();
            string mode = model.cn_s_mode.ToUpper();

            if (mode == "POST")
            {
                res = await PostDataAsync(model);
            }
            else if (mode == "PUT")
            {
                res = await PutDataAsync(model);
            }
            else if (mode == "GET")
            {
                res = await GetDataAsync(model);
            }
            return res;
        }

        public ApiResult SendRequest(string intfCode, string jsonData, string user, string pwd)
        {
            throw new NotImplementedException();
        }
    }
}
