using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.IDTSCore.WebSocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    public class ControlThread
    {
        // <summary>
        /// 发送指令线程(使用消息队列发送)
        /// </summary>
        private Thread _SendThread { get; set; }
        private ILogsService _ILogsService;

        private static readonly ControlThread instance = new ControlThread();

        private string Message { get; set; }

        private ControlThread() { _ILogsService = ServiceLocator.GetService<ILogsService>(new DbHelper.SessionInfo() { splitDbCode = "" }); }

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static ControlThread Instance
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
            while (true)
            {
                try
                {
                    if (SessionInstance.Instance.WCSSessionList.Count > 0)
                    {
                        //SessionInstance.Instance.PLCSendAll("{\"name\":\"abc\"}");
                    }
                }
                catch (Exception ex)
                {
                    if (Message != ex.Message)
                    {
                        tn_dts_logs log = new tn_dts_logs();
                        log.cn_guid = Guid.NewGuid().ToString();
                        log.cn_s_logs_type = "程序";
                        log.cn_s_logs_errorsinfo = "ControlThread线程异常，异常信息为：" + ex.Message;
                        log.cn_t_create = DateTime.Now;
                        int resLogs = _ILogsService.Add(log);
                        if (resLogs <= 0)
                        {
                            LogHelper.Info(DateTime.Now.ToString() + " ControlThread线程异常存入tn_dts_logs表失败，异常信息为 " + ex.Message);
                        }
                    }
                    Message = ex.Message;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        public void Start()
        {
            _SendThread.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _SendThread.Abort();
        }
    }
}
