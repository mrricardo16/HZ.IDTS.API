using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Interfaces.IService.InterfaceAuth;
using HZ.IDTSCore.Model;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.InterfaceAuth
{
    public class WebServiceAuthService : IAuthService
    {


        public WebServiceAuthService(string token)
        {

        }


        public string DoSendRequest(tn_wms_interface_def model, out string errMsg)
        {
            errMsg = "";
            return "";
        }


        /// <summary>
        /// 接口请求发起
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ApiResult SendRequest(tn_wms_interface_def model)
        {
            // StringBuilder sendXml = new StringBuilder();

            ////XML固定头部
            //sendXml.Append("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:jour=\"http://tbea/oracle/apps/ws/Journal.wsdl\" xmlns:typ=\"http://tbea/oracle/apps/ws/Journal.wsdl/types/\">");
            //sendXml.AppendFormat("<soapenv:Header/>");
            //sendXml.AppendFormat("<soapenv:Body>");
            //sendXml.AppendFormat("<jour:glJournalMain>");

            //sendXml.AppendFormat("<typ:User>");
            //sendXml.AppendFormat("<typ:head>{0}</typ:head>", model.cn_s_header);
            //sendXml.AppendFormat("<typ:sourceTrx>{0}</typ:sourceTrx>", model.cn_s_data);
            //sendXml.AppendFormat("</typ:User>");

            //sendXml.AppendFormat("</jour:glJournalMain>");
            //sendXml.AppendFormat("</soapenv:Body>");
            //sendXml.AppendFormat("</soapenv:Envelope>");
            //输出日志
            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "发送XML：" + model.cn_s_send_content);

            string reponseXml = string.Empty;

            bool success = Send_XML(model.cn_s_url, model.cn_s_send_content, out reponseXml);
            LogHelper.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "返回XML：" + reponseXml);

            if (!success)
            {
                int i = 0;
                while (i < model.cn_n_max_retry)
                {
                    success = Send_XML(model.cn_s_url, model.cn_s_send_content, out reponseXml);
                    if (success)
                        break;
                    else
                        i++;
                }
            }

            ApiResult res = success ? ApiResult.Success() : ApiResult.Error(reponseXml);

            return res;
        }

        public ApiResult SendRequest(string intfCode, string jsonData, string user, string pwd)
        {
            throw new NotImplementedException();
        }

        //发送url
        public bool Send_XML(string url, string sendXml, out string result)
        {
            bool success = true;
            byte[] bytes = Encoding.UTF8.GetBytes(sendXml);

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = (long)bytes.Length;
            httpWebRequest.ContentType = "text/xml;charset=UTF-8";
            httpWebRequest.Headers.Add("content", "text/xml;charset=utf8");

            try
            {
                Stream writer = httpWebRequest.GetRequestStream();
                writer.Write(bytes, 0, bytes.Length);
                //关闭请求流  
                writer.Close();
            }
            catch (Exception ex)
            {
                result = ex.Message;
                success = false;
            }
            finally
            {
                //streamWriter.Close();
            }
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
                streamReader.Close();

                success = true;
            }
            return success;
        }
    }
}
