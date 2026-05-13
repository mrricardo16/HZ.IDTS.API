using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.OpenApi;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Equipment;
using Newtonsoft.Json;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.CommonUtil.Helpers;

namespace HZ.IDTSCore.Api
{
    public class RepairThread : IHostedService
    {
        public IEquirepairService IEQRepairService;
        private ILogsService _ILogsService;

        public RepairThread()
        {
            _SyncPushThread = new Thread(SyncPushThreadHandle);
            _SyncPushThread.IsBackground = true;
            equipmentRepairReminderView = new EquipmentRepairReminderViewModel();
            IEQRepairService = ServiceLocator.GetService<IEquirepairService>(new DbHelper.SessionInfo() { splitDbCode = "" });
            _ILogsService = ServiceLocator.GetService<ILogsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
        }

        /// <summary>
        /// 线程:设备维修处理与推送
        /// </summary>
        private Thread _SyncPushThread { get; set; }

        /// <summary>
        /// 设备维修提醒项
        /// </summary>
        public EquipmentRepairReminderViewModel equipmentRepairReminderView { get; set; }

        /// <summary>
        /// 开启线程
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _SyncPushThread.Start();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _SyncPushThread.Abort();
            return Task.CompletedTask;
        }

        //private DateTime latestSyncUpdate { get; set; } = DateTime.Now;
        //private int SyncSleep { get; set; } = 1;
        private string Message { get; set; }

        /// <summary>
        /// 设备维修处理与推送线程
        /// </summary>
        private void SyncPushThreadHandle()
        {
            while (true)
            {
                //DateTime dt = DateTime.Now;

                //TimeSpan differenceSend = dt - latestSyncUpdate;

                //if (differenceSend.Minutes < SyncSleep)
                //{
                //    continue;
                //}
                try
                {
                    int returnNum = 5;
                    equipmentRepairReminderView.eqRepairCollect = IEQRepairService.GetEQRepairCollectList(returnNum, DeviceDriver.Instance.DExplosion.deviceCode);

                    string sendJSONString = JsonConvert.SerializeObject(equipmentRepairReminderView);
                    var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                    //latestSyncUpdate = DateTime.Now;                   
                }
                catch (Exception ex)
                {
                    if (Message != ex.Message)
                    {
                        tn_dts_logs log = new tn_dts_logs();
                        log.cn_guid = Guid.NewGuid().ToString();
                        log.cn_s_logs_type = "程序";
                        log.cn_s_logs_errorsinfo = "RepairThread线程异常，异常信息为：" + ex.Message;
                        log.cn_t_create = DateTime.Now;
                        int resLogs = _ILogsService.Add(log);
                        if (resLogs <= 0)
                        {
                            LogHelper.Info(DateTime.Now.ToString() + " RepairThread线程异常存入tn_dts_logs表失败，异常内容为 " + ex.Message);
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
            _SyncPushThread.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _SyncPushThread.Abort();
        }
    }
}
