using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    public class LogsThread : IHostedService
    {
        /// <summary>
        /// 设备采集日志接口
        /// </summary>
        private IEquireallogsService _RealLogsService;

        /// <summary>
        /// 设备警报日志接口
        /// </summary>
        private IEquialarmlogsService _AlarmLogsService;

        /// <summary>
        /// 系统日志表
        /// </summary>
        private ILogsService _ILogsService;

        private string Message { get; set; }

        public LogsThread()
        {
            _ClearLogThread = new Thread(ClearLogThreadHandle);
            _ClearLogThread.IsBackground = true;
            RefreshTime();

            _RealLogsService = ServiceLocator.GetService<IEquireallogsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
            _AlarmLogsService = ServiceLocator.GetService<IEquialarmlogsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
            _ILogsService= ServiceLocator.GetService<ILogsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
        }

        /// <summary>
        /// 线程1:日志清理
        /// </summary>
        private Thread _ClearLogThread { get; set; }

        /// <summary>
        /// 设置采集日志的清理周期(天)
        /// </summary>
        private int CollectLogsClearDays { get; set; } = 2;
        private DateTime LastCollectLogsClearTime { get; set; }
        private string CollectThreadClassName = "LogsThread";
        private string CollectThreadName = "_ClearLogThread";
        private string CollectEventId = "CollectLogsClearDays";
        private string CollectEventName = "设置采集日志的清理周期(天)";



        /// <summary>
        /// 请求接口日志清理周期(天)
        /// </summary>
        private int RequestLogsClearDays { get; set; } = 7;
        private DateTime LastRequestLogsClearTime { get; set; }
        private string RequestThreadClassName = "LogsThread";
        private string RequestThreadName = "_ClearLogThread";
        private string RequestEventId = "RequestLogsClearDays";
        private string RequestEventName = "请求接口日志清理周期(天)";

        /// <summary>
        /// 设备警报日志的清理周期(天)
        /// </summary>
        private int AlarmLogsClearDays { get; set; } = 3;
        private DateTime LastAlarmLogsClearTime { get; set; }
        private string AlarmThreadClassName = "LogsThread";
        private string AlarmThreadName = "_ClearLogThread";
        private string AlarmEventId = "AlarmLogsClearDays";
        private string AlarmEventName = "设备警报日志的清理周期(天)";


        private void RefreshTime()
        {
            if (MongoDBSingleton.Instance.Connectioned)
            {
                DateTime clcTime = DateTime.Now;
                var clcdModel = AddRefreshThreadEvent("Init", CollectThreadClassName, CollectThreadName, CollectEventId, CollectEventName, clcTime);
                if (clcdModel == null)
                {
                    LastCollectLogsClearTime = clcTime;
                }
                else
                {
                    LastCollectLogsClearTime = clcdModel.lastUpdateTime;
                }

                DateTime rlcTime = DateTime.Now;
                var rlcModel = AddRefreshThreadEvent("Init", RequestThreadClassName, RequestThreadName, RequestEventId, RequestEventName, rlcTime);
                if (rlcModel == null)
                {
                    LastRequestLogsClearTime = rlcTime;
                }
                else
                {
                    LastRequestLogsClearTime = rlcModel.lastUpdateTime;
                }
                DateTime alcdTime = DateTime.Now;
                var alcdModel = AddRefreshThreadEvent("Init", AlarmThreadClassName, AlarmThreadName, AlarmEventId, AlarmEventName, alcdTime);
                if (alcdModel == null)
                {
                    LastAlarmLogsClearTime = alcdTime;
                }
                else
                {
                    LastAlarmLogsClearTime = alcdModel.lastUpdateTime;
                }
            }
        }

        private void ClearLogThreadHandle()
        {
            while (true)
            {
                if (MongoDBSingleton.Instance.Connectioned)
                {
                    try
                    {
                        RefreshTime();

                        #region  CollectLogsClearDays
                        TimeSpan collDiff = DateTime.Now - LastCollectLogsClearTime;
                        if (collDiff.Days >= CollectLogsClearDays)
                        {
                            int rows = _RealLogsService.Delete(p => p.cn_t_equireallogs_timestamp < LastCollectLogsClearTime.AddDays(CollectLogsClearDays));
                            DateTime clcTime = LastCollectLogsClearTime.AddDays(CollectLogsClearDays);
                            var clcdModel = AddRefreshThreadEvent("Refresh", CollectThreadClassName, CollectThreadName, CollectEventId, CollectEventName, clcTime);
                            LastCollectLogsClearTime = clcTime;
                        }
                        #endregion
                        #region  RequestLogsClearDays
                        TimeSpan reqDiff = DateTime.Now - LastRequestLogsClearTime;
                        if (reqDiff.Days >= RequestLogsClearDays)
                        {
                            int rows = _ILogsService.Delete(p => p.cn_t_create < LastRequestLogsClearTime.AddDays(RequestLogsClearDays));
                            DateTime rlcTime = LastRequestLogsClearTime.AddDays(RequestLogsClearDays);
                            var clcdModel = AddRefreshThreadEvent("Refresh", RequestThreadClassName, RequestThreadName, RequestEventId, RequestEventName, rlcTime);
                            LastRequestLogsClearTime = rlcTime;
                        }
                        #endregion
                        #region  AlarmLogsClearDays
                        TimeSpan alarmDiff = DateTime.Now - LastAlarmLogsClearTime;
                        if (alarmDiff.Days >= AlarmLogsClearDays)
                        {
                            int rows = _AlarmLogsService.Delete(p => p.cn_t_equialarmlogs_timestamp < LastAlarmLogsClearTime.AddDays(AlarmLogsClearDays));
                            DateTime alcdTime = LastAlarmLogsClearTime.AddDays(AlarmLogsClearDays);
                            var clcdModel = AddRefreshThreadEvent("Refresh", AlarmThreadClassName, AlarmThreadName, AlarmEventId, AlarmEventName, alcdTime);
                            LastAlarmLogsClearTime = alcdTime;
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        if (Message != ex.Message)
                        {
                            tn_dts_logs log = new tn_dts_logs();
                            log.cn_guid = Guid.NewGuid().ToString();
                            log.cn_s_logs_type = "程序";
                            log.cn_s_logs_errorsinfo = "LogsThread线程异常，异常信息为：" + ex.Message;
                            log.cn_t_create = DateTime.Now;
                            int resLogs = _ILogsService.Add(log);
                            if (resLogs <= 0)
                            {
                                LogHelper.Info(DateTime.Now.ToString() + " LogsThread线程异常存入tn_dts_logs表失败，异常信息为 " + ex.Message);
                            }
                        }
                        Message = ex.Message;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 开启线程
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ClearLogThread.Start();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _ClearLogThread.Abort();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 增加一条事件
        /// </summary>
        /// <param name="option">Init|Update</param>
        /// <param name="threadClassName"></param>
        /// <param name="threadName"></param>
        /// <param name="eventId"></param>
        /// <param name="eventName"></param>
        /// <param name="lastTime"></param>
        private MongoThreadEventSet AddRefreshThreadEvent(string option, string threadClassName, string threadName, string eventId, string eventName,DateTime lastTime)
        {
            MongoThreadEventSet result = null;
            List<FilterDefinition<MongoThreadEventSet>> queryList = new List<FilterDefinition<MongoThreadEventSet>>();
            queryList.Add(Builders<MongoThreadEventSet>.Filter.Eq(p => p.threadClassName, threadClassName));
            queryList.Add(Builders<MongoThreadEventSet>.Filter.Eq(p => p.threadName, threadName));
            queryList.Add(Builders<MongoThreadEventSet>.Filter.Eq(p => p.eventId, eventId));
            if (option == "Init")
            {
                var newModel = MongoDBSingleton.Instance.FindOneFilter(Builders<MongoThreadEventSet>.Filter.And(queryList));
                if (newModel == null)
                {
                    newModel = new MongoThreadEventSet();
                    newModel.threadClassName = threadClassName;
                    newModel.threadName = threadName;
                    newModel.eventId = eventId;
                    newModel.eventName = eventName;
                    newModel.lastUpdateTime = DateTime.Now;
                    MongoDBSingleton.Instance.Add(newModel);
                }
                result = newModel;
            }
            if (option == "Refresh")
            {
                var currModel = MongoDBSingleton.Instance.FindOneFilter(Builders<MongoThreadEventSet>.Filter.And(queryList));
                if (currModel != null)
                {
                    currModel.lastUpdateTime = lastTime;
                    MongoDBSingleton.Instance.Update(currModel, currModel._id.ToString());

                }
            }
            return result;
        }
    }
}
