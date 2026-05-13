using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Global
{
    /// <summary>
    /// WebSocket 客户端消息接收类
    /// </summary>
    public class WebSocketReceiveFilter
    {
        /// <summary>
        /// 通用接收类
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        public static void Receive(SuperSocket.WebSocket.Server.WebSocketSession session, string message)
        {
            Task.Run(async () =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    if (message.Contains("StartTesting"))
                    {
                        StartTestingViewModel start = JsonConvert.DeserializeObject<StartTestingViewModel>(message);
                        if (start != null)
                        {
                            //SenarioTestingProcess.Sceneno = start.StartTesting[0].Sceneno;
                            //SenarioTestingProcess.IsSynchronizeStock = start.StartTesting[0].IsSynchronizeStock;
                            //SenarioTestingProcess.IsCirculate = start.StartTesting[0].IsCirculate;
                            //SenarioTestingProcess.Goodsequipmentno = start.StartTesting[0].Goodsequipmentno;
                            SenarioTestingProcess.IsExitLoop = false;
                            SenarioTestingProcess.HasSynchronizeStock = false;
                            if (!start.StartTesting[0].IsCirculate)
                            {
                                
                                var result = SenarioTestingProcess.StartTesting(start);
                                SenarioTestingProcess.IsExitLoop = true;
                                //异步反馈
                                //过程执行完后，反馈给客户端
                                string sendJSONString = JsonConvert.SerializeObject(result);//定义好 反馈的JSON格式
                                await session.SendAsync(sendJSONString);
                            }
                            else
                            {
                                #region 旧方法
                                /*int i = 1;
                                while (true)
                                {
                                    
                                    var result = SenarioTestingProcess.StartTesting(start);
                                    if (SenarioTestingProcess.IsExitLoop)
                                    {
                                        break;
                                    }
                                    //if (session.State != SuperSocket.SessionState.Connected)
                                    //{
                                    //    break;
                                    //}
                                    if (result.ReturnMessage[0].IsSuccess)
                                    {
                                        result.ReturnMessage[0].Message = "第" + i++ + "遍发送完成！";
                                    }
                                    string sendJSONString = JsonConvert.SerializeObject(result);//定义好 反馈的JSON格式
                                    await session.SendAsync(sendJSONString);
                                }*/
                                #endregion

                                #region 新方法
                                SenarioTestingThread.Instance.startJSON = start;
                                if (SenarioTestingProcess.IsExitLoop)
                                {
                                    SenarioTestingThread.Instance.Stop();
                                }
                                else
                                {
                                    if (!SenarioTestingThread.Instance.IsExitLoop)
                                    {
                                        SenarioTestingThread.Instance.Stop();
                                    }
                                    SenarioTestingThread.Instance.Start();
                                }
                                #endregion

                            }
                        }
                    }

                    if (message.Contains("StopTesting"))
                    {
                        if (!SenarioTestingThread.Instance.IsExitLoop)
                        {
                            SenarioTestingThread.Instance.Stop();
                        }
                        StopTestingViewModel stop = JsonConvert.DeserializeObject<StopTestingViewModel>(message);
                        if (stop != null)
                        {
                            var result = SenarioTestingProcess.StopTesting(stop);
                            SenarioTestingProcess.IsExitLoop = true;

                            if (result.ReturnMessage[0].IsSuccess)
                            {
                                //过程执行完后，反馈给客户端
                                string sendJSONString = JsonConvert.SerializeObject(result);//定义好 反馈的JSON格式
                                //await WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                                await session.SendAsync(sendJSONString);
                            }
                        }
                    }
                    //立即回复就用这句，如果不是立即回复，执行完代码后再回复就用 广播回复指令
                    //await session.SendAsync(message);

                    if (message.Contains("PauseTesting"))
                    {
                        if (!SenarioTestingThread.Instance.IsExitLoop)
                        {
                            SenarioTestingThread.Instance.Stop();
                        }
                        PauseTestingViewModel pause = JsonConvert.DeserializeObject<PauseTestingViewModel>(message);
                        if (pause != null)
                        {
                            var result = SenarioTestingProcess.PauseTesting(pause);
                            if (result.ReturnMessage[0].IsSuccess)
                            {
                                //过程执行完后，反馈给客户端
                                string sendJSONString = JsonConvert.SerializeObject(result);//定义好 反馈的JSON格式
                                //await WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                                await session.SendAsync(sendJSONString);
                            }
                        }
                    }

                    if (message.Contains("ContinueTesting"))
                    {
                        if (SenarioTestingThread.Instance.IsExitLoop)
                        {
                            SenarioTestingThread.Instance.Start();
                        }
                        ContinueTestingViewModel _continue = JsonConvert.DeserializeObject<ContinueTestingViewModel>(message);
                        if (_continue != null)
                        {
                            var result = SenarioTestingProcess.ContinueTesting(_continue);
                            if (result.ReturnMessage[0].IsSuccess)
                            {
                                //过程执行完后，反馈给客户端
                                string sendJSONString = JsonConvert.SerializeObject(result);//定义好 反馈的JSON格式
                                //await WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                                await session.SendAsync(sendJSONString);
                            }
                        }
                    }
                }
            });
        }
    }
}
