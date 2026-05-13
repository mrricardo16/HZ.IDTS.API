using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    public class BackupsThread : IHostedService
    {
        public IBackupsService IBackupsService;
        private ILogsService _ILogsService;

        public BackupsThread()
        {
            _AutomaticBackUpThread = new Thread(AutomaticBackUpThreadHandle);
            _AutomaticBackUpThread.IsBackground = true;
            IBackupsService = ServiceLocator.GetService<IBackupsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
            _ILogsService = ServiceLocator.GetService<ILogsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
        }

        /// <summary>
        /// 线程:自动备份线程
        /// </summary>
        private Thread _AutomaticBackUpThread { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        private string Message { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _AutomaticBackUpThread.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _AutomaticBackUpThread.Abort();
            return Task.CompletedTask;
        }

        private void AutomaticBackUpThreadHandle()
        {
            while (true)
            {
                try
                {
                    SaveBackups currentSaveBackups = BackupsDriver.Instance.DBackups;
                    SaveBackups lastSaveBackups = BackupsDriver.Instance.LastDBackups;
                    if (currentSaveBackups is null || currentSaveBackups.IsAutomatic is null || currentSaveBackups.Span is null || currentSaveBackups.HourDayWeek is null || currentSaveBackups.BackupsDiretory is null)
                    {
                        continue;
                    }
                    if (!currentSaveBackups.IsAutomatic.Value)
                    {
                        continue;
                    }

                    bool isFirst = BackupsDriver.Instance.IsFirst;
                    if (isFirst)
                    {
                        BackupsDriver.Instance.BackupsFlag = true;
                        BackupsDriver.Instance.IsFirst = false;
                    }
                    if (currentSaveBackups.Span != lastSaveBackups.Span)
                    {
                        BackupsDriver.Instance.LastDBackups = BackupsDriver.Instance.DBackups;
                        BackupsDriver.Instance.BackupsFlag = true;
                    }
                    DateTime nowBackups = DateTime.Now;
                    DateTime lastBackups = BackupsDriver.Instance.LastBackups;
                    //WeekInformation lastWeek = new WeekInformation()
                    //{
                    //    Year = lastBackups.Year,
                    //    Month = lastBackups.Month,
                    //    Week = lastBackups.DayOfWeek
                    //};

                    if (currentSaveBackups.Span.Value == Span.Day)
                    {
                        if (BackupsDriver.Instance.BackupsFlag)
                        {
                            //(nowBackups - BackupsDriver.Instance.LastBackups).Days > 1
                            if (nowBackups.Date == lastBackups.Date)
                            {
                                continue;
                            }
                            if (nowBackups.Hour == int.Parse(currentSaveBackups.HourDayWeek))
                            {
                                IBackupsService.BackUp(currentSaveBackups.BackupsDiretory, "自动");
                                BackupsDriver.Instance.BackupsFlag = false;
                                int backupsHour = int.Parse(currentSaveBackups.HourDayWeek);
                                int nextHour;
                                if (backupsHour == 23)
                                {
                                    nextHour = 0;
                                }
                                else
                                {
                                    nextHour = ++backupsHour;
                                }
                                BackupsDriver.Instance.RestoreFlagTime = nextHour;
                            }
                        }
                        else
                        { 
                            if (nowBackups.Hour == BackupsDriver.Instance.RestoreFlagTime)
                            {
                                BackupsDriver.Instance.BackupsFlag = true;
                            }
                        }
                    }
                    if (currentSaveBackups.Span.Value == Span.Week)
                    {
                        if (BackupsDriver.Instance.BackupsFlag)
                        {
                            WeekDayOfYearRange range = BackupsDriver.Instance.GetWeekDayOfYearRange(lastBackups);
                            if (range.Status == 0 || range.Status == 1)
                            {
                                if (nowBackups.DayOfYear <= range.ThisWeekSunday && nowBackups.Year == range.Year)
                                {
                                    continue;
                                }
                            }
                            else//range.Status == 2
                            {
                                if (nowBackups.DayOfYear <= range.AllYearDays && nowBackups.Year == range.Year)
                                {
                                    continue;
                                }
                                if (nowBackups.Year == range.Year + 1)
                                {
                                    switch (range.LastWeek)
                                    {
                                        case DayOfWeek.Monday:
                                            if (nowBackups.DayOfYear <= range.ThisWeekSunday)
                                            {
                                                continue;
                                            }
                                            break;
                                        case DayOfWeek.Tuesday:
                                            if (nowBackups.DayOfYear <= range.ThisWeekSunday)
                                            {
                                                continue;
                                            }
                                            break;
                                        case DayOfWeek.Wednesday:
                                            if (nowBackups.DayOfYear <= range.ThisWeekSunday)
                                            {
                                                continue;
                                            }
                                            break;
                                        case DayOfWeek.Thursday:
                                            if (nowBackups.DayOfYear <= range.ThisWeekSunday)
                                            {
                                                continue;
                                            }
                                            break;
                                        case DayOfWeek.Friday:
                                            if (nowBackups.DayOfYear <= range.ThisWeekSunday)
                                            {
                                                continue;
                                            }
                                            break;
                                        case DayOfWeek.Saturday:
                                            if (nowBackups.DayOfYear <= range.ThisWeekSunday)
                                            {
                                                continue;
                                            }
                                            break;
                                        case DayOfWeek.Sunday:

                                            break;
                                    }
                                }
                            }
                            //(nowBackups - BackupsDriver.Instance.LastBackups).Days > 7 
                            if ((int)nowBackups.DayOfWeek == int.Parse(currentSaveBackups.HourDayWeek))
                            {
                                IBackupsService.BackUp(currentSaveBackups.BackupsDiretory, "自动");
                                BackupsDriver.Instance.BackupsFlag = false;
                                int backupsWeek = int.Parse(currentSaveBackups.HourDayWeek);
                                int nextWeek;
                                if (backupsWeek == 6)
                                {
                                    nextWeek = 0;
                                }
                                else
                                {
                                    nextWeek = ++backupsWeek;
                                }
                                BackupsDriver.Instance.RestoreFlagTime = nextWeek;
                            }
                        }
                        else
                        {                         
                            if ((int)nowBackups.DayOfWeek == BackupsDriver.Instance.RestoreFlagTime)
                            {
                                BackupsDriver.Instance.BackupsFlag = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Message != ex.Message)
                    {
                        tn_dts_logs log = new tn_dts_logs();
                        log.cn_guid = Guid.NewGuid().ToString();
                        log.cn_s_logs_type = "程序";
                        log.cn_s_logs_errorsinfo = "BackupsThread线程异常，异常信息为：" + ex.Message;
                        log.cn_t_create = DateTime.Now;
                        int resLogs = _ILogsService.Add(log);
                        if (resLogs <= 0)
                        {
                            LogHelper.Info(DateTime.Now.ToString() + " BackupsThread线程异常存入tn_dts_logs表失败，异常内容为 " + ex.Message);
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
            _AutomaticBackUpThread.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _AutomaticBackUpThread.Abort();
        }
    }
}
