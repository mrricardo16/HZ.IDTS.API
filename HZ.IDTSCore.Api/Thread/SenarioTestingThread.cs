using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Api.Global;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    public class SenarioTestingThread
    {
        private static readonly SenarioTestingThread instance = new SenarioTestingThread();

        // <summary>
        /// 发送指令线程(使用消息队列发送)
        /// </summary>
        private Thread _SendThread { get; set; }

        /// <summary>
        /// 停止状态
        /// </summary>
        public bool IsExitLoop { get; set; } = true;

        /// <summary>
        /// 脚本
        /// </summary>
        public StartTestingViewModel startJSON { get; set; } = new StartTestingViewModel();

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static SenarioTestingThread Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 构造函数 单例模式 只执行一次
        /// </summary>
        public void Initialize()
        {
            _SendThread = new Thread(SendThreadHandle);
        }

        /// <summary>
        /// 待处理命令线程
        /// </summary>
        private void SendThreadHandle()
        {
            int i = 1;
            while (true)
            {
                if (!IsExitLoop)
                {
                    if (startJSON != null)
                    {
                        var result = SenarioTestingProcess.StartTesting(startJSON);
                        if (result.ReturnMessage[0].IsSuccess)
                        {
                            result.ReturnMessage[0].Message = "第" + i++ + "遍发送完成！";
                        }
                        string sendJSONString = JsonConvert.SerializeObject(result);
                        WebSocketServer.SessionInstance.Instance.PLCSendAllV2(sendJSONString);

                    }
                }
                Thread.Sleep(10);
            }
        }


        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            IsExitLoop = false;
            LogHelper.Info("SenarioTestingThread-Start-IsExitLoop=" + IsExitLoop);
            //_SendThread = new Thread(SendThreadHandle);
            _SendThread.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            IsExitLoop = true;
            LogHelper.Info("SenarioTestingThread-Start-IsExitLoop=" + IsExitLoop);
            _SendThread.Abort();
        }
    }
}
