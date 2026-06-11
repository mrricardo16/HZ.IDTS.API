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
        /// 场景测试线程开关。关闭时不创建线程，也不允许 WebSocket 指令启动循环。
        /// </summary>
        public bool IsEnabled { get; private set; } = false;

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
        public void Initialize(bool isEnabled)
        {
            IsEnabled = isEnabled;
            IsExitLoop = true;
            if (!IsEnabled)
            {
                return;
            }

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
                Thread.Sleep(1000);
            }
        }


        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            if (!IsEnabled)
            {
                IsExitLoop = true;
                LogHelper.Info("SenarioTestingThread-Start ignored because EnableSenarioTesting=false");
                return;
            }

            if (_SendThread == null || !_SendThread.IsAlive)
            {
                _SendThread = new Thread(SendThreadHandle);
            }
            else
            {
                IsExitLoop = false;
                LogHelper.Info("SenarioTestingThread-Start-IsExitLoop=" + IsExitLoop);
                return;
            }

            IsExitLoop = false;
            LogHelper.Info("SenarioTestingThread-Start-IsExitLoop=" + IsExitLoop);
            _SendThread.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            IsExitLoop = true;
            LogHelper.Info("SenarioTestingThread-Start-IsExitLoop=" + IsExitLoop);
            if (_SendThread != null && _SendThread.IsAlive)
            {
                _SendThread.Abort();
            }
        }
    }
}
