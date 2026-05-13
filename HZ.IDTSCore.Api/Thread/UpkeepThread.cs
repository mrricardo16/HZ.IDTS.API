using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    public class UpkeepThread : IHostedService
    {
        public IEquiupkeepService IEQUpkeepService;
        public IEquipmentService IEQEquipmentService;
        private ILogsService _ILogsService;

        public UpkeepThread()
        {
            _SyncPushThread = new Thread(SyncPushThreadHandle);
            _SyncPushThread.IsBackground = true;
            IEQUpkeepService = ServiceLocator.GetService<IEquiupkeepService>(new DbHelper.SessionInfo() { splitDbCode = "" });
            IEQEquipmentService = ServiceLocator.GetService<IEquipmentService>(new DbHelper.SessionInfo() { splitDbCode = "" });
            _ILogsService = ServiceLocator.GetService<ILogsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
        }

        /// <summary>
        /// 线程:设备维修处理推送线程
        /// </summary>
        private Thread _SyncPushThread { get; set; }

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
        /// <summary>
        /// 异常信息
        /// </summary>
        private string Message { get; set; }

        private void SyncPushThreadHandle()
        {
            while (true)
            {
                //DateTime dt = DateTime.Now;

                //TimeSpan differenceSend = dt - latestSyncUpdate;

                //if (differenceSend.Minutes < SyncSleep)
                //{
                //    latestSyncUpdate = DateTime.Now;
                //    continue;
                //}
                //EquipmentUpkeepReminderViewModel equipmentUpkeepReminderView = new EquipmentUpkeepReminderViewModel();
                //string deviceCode = DeviceDriver.Instance.DExplosion.deviceCode;
                //PagedInfo<tn_dts_equiupkeep> upkeepPagedList = IEQUpkeepService.GetPages(it => it.cn_s_equiupkeep_no == deviceCode, new CommonUtil.Model.PageParm());
                //List<tn_dts_equiupkeep> tn_Dts_Equipment = upkeepPagedList.DataSource.OrderByDescending(it => it.cn_t_equiupkeep_date).ToList();
                //EQUpkeepCollect eQUpkeepCollect = new EQUpkeepCollect();
                //eQUpkeepCollect.deviceNo = deviceCode;
                //eQUpkeepCollect.upkeepItemCount = upkeepPagedList.DataSource.Count().ToString();
                //List<UpkeepItemModel> upkeepItemModelList = new List<UpkeepItemModel>();
                ////if (upkeepPagedList.DataSource.Count == 1)
                ////{
                ////    UpkeepItemModel upkeepItemModel = new UpkeepItemModel();
                ////    upkeepItemModel.
                ////}
                ////else
                ////{
                //int count = upkeepPagedList.DataSource.Count;
                //for (int i = 0; i < count; i++)
                //{
                //    int? defentperiod = IEQEquipmentService.GetPages(it => it.cn_s_equi_no == deviceCode, new CommonUtil.Model.PageParm()).DataSource.Select(it => it.cn_s_equi_defentperiod).First();
                //    if (defentperiod.HasValue == false)
                //    {
                //        break;
                //    }
                //    UpkeepItemModel upkeepItemModel = new UpkeepItemModel();
                //    if (i == 0)
                //    {
                //        DateTime? date = tn_Dts_Equipment[0].cn_t_equiupkeep_date;
                //        if (date.HasValue == false)
                //        {
                //            continue;
                //        }
                //        if (count == 1)
                //        {
                //            DateTime upkeepTime = DateTime.Parse(date.Value.AddDays(defentperiod.Value).ToString());
                //            upkeepItemModel.upkeepTime = upkeepTime.ToString();
                //            upkeepItemModel.upkeepItemName = tn_Dts_Equipment[0].cn_s_equiupkeep_item;
                //            upkeepItemModel.lastUpkeepTime = date.Value.ToString();
                //            upkeepItemModel.defentperiod = defentperiod.Value.ToString();
                //            TimeSpan difference = upkeepTime - DateTime.Now;
                //            if (difference.Days > 7)
                //            {
                //                upkeepItemModel.ext1 = "否";
                //            }
                //            else if (difference.Days >= 0)
                //            {
                //                upkeepItemModel.ext1 = "是（" + difference.Days.ToString() + "）";
                //            }
                //            else
                //            {
                //                upkeepItemModel.ext1 = "是（超期" + Math.Abs(difference.Days).ToString() + "）";
                //            }
                //            upkeepItemModel.ext2 = "";
                //        }
                //        else
                //        {
                //            DateTime upkeepTime = DateTime.Parse(date.Value.AddDays(defentperiod.Value).ToString());
                //            upkeepItemModel.upkeepTime = upkeepTime.ToString();
                //            upkeepItemModel.upkeepItemName = tn_Dts_Equipment[0].cn_s_equiupkeep_item;
                //            upkeepItemModel.lastUpkeepTime = date.Value.ToString();
                //            upkeepItemModel.defentperiod = defentperiod.Value.ToString();
                //            TimeSpan difference = upkeepTime - DateTime.Now;
                //            if (difference.Days > 7)
                //            {
                //                upkeepItemModel.ext1 = "否";
                //            }
                //            else if (difference.Days >= 0)
                //            {
                //                upkeepItemModel.ext1 = "是（" + difference.Days.ToString() + "）";
                //            }
                //            else
                //            {
                //                upkeepItemModel.ext1 = "是（超期" + Math.Abs(difference.Days).ToString() + "）";
                //            }
                //            upkeepItemModel.ext2 = "";
                //        }
                //    }
                //    else
                //    {
                //        DateTime? daterear = tn_Dts_Equipment[i - 1].cn_t_equiupkeep_date;
                //        DateTime? datefront = tn_Dts_Equipment[i].cn_t_equiupkeep_date;
                //        if (daterear.HasValue == false || datefront.HasValue == false)
                //        {
                //            continue;
                //        }
                //        upkeepItemModel.upkeepTime = daterear.Value.ToString();
                //        upkeepItemModel.upkeepItemName = tn_Dts_Equipment[i].cn_s_equiupkeep_item;
                //        upkeepItemModel.lastUpkeepTime = datefront.Value.ToString();
                //        upkeepItemModel.defentperiod = defentperiod.Value.ToString();
                //        upkeepItemModel.ext1 = "";
                //        upkeepItemModel.ext2 = "";
                //    }
                //    upkeepItemModelList.Add(upkeepItemModel);
                //}
                //eQUpkeepCollect.upkeepItem = upkeepItemModelList;
                //equipmentUpkeepReminderView.eqUpkeepCollect.Add(eQUpkeepCollect);
                try
                {
                    int returnNum = 5;
                    EquipmentUpkeepReminderViewModel equipmentUpkeepReminderView = new EquipmentUpkeepReminderViewModel();
                    equipmentUpkeepReminderView.eqUpkeepCollect = IEQUpkeepService.GetEQUpkeepCollectList(returnNum, DeviceDriver.Instance.DExplosion.deviceCode);
                    string sendJSONString = JsonConvert.SerializeObject(equipmentUpkeepReminderView);
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
                        log.cn_s_logs_errorsinfo = "UpkeepThread线程异常，异常信息为：" + ex.Message;
                        log.cn_t_create = DateTime.Now;
                        int resLogs = _ILogsService.Add(log);
                        if (resLogs <= 0)
                        {
                            LogHelper.Info(DateTime.Now.ToString() + " UpkeepThread线程异常存入tn_dts_logs表失败，异常内容为 " + ex.Message);
                        }
                    }
                    Message = ex.Message;
                }
                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 设备维修处理与推送线程
        /// </summary>
        //private void SyncPushThreadHandle()
        //{
        //    while (true)
        //    {
        //        DateTime dt = DateTime.Now;

        //        TimeSpan differenceSend = dt - latestSyncUpdate;

        //        if (differenceSend.Minutes < SyncSleep)
        //        {
        //            latestSyncUpdate = DateTime.Now;
        //            continue;
        //        }


        //        List<UpkeepRecrusionModel> allUpkeepRecrusion = GetAllUpkeepRecrusion();
        //        foreach (var upkeepRecrusion in allUpkeepRecrusion)
        //        {
        //            PagedInfo<tn_dts_equiupkeep> upkeepPagedList = IEQUpkeepService.GetPages(it => it.cn_s_equiupkeep_no == upkeepRecrusion.cn_s_equi_no, new CommonUtil.Model.PageParm());
        //            List<UpkeepItemModel> upkeepItemModelList = new List<UpkeepItemModel>();
        //            UpkeepRecrusionModel upkeepRecrusionModel = allUpkeepRecrusion.FirstOrDefault(it => it.cn_s_equi_no == upkeepRecrusion.cn_s_equi_no);
        //            DateTime? latestupkeeptime = null;
        //            if (upkeepPagedList.DataSource.Count != 0)
        //            {
        //                latestupkeeptime = upkeepPagedList.DataSource.OrderByDescending(it => it.cn_t_equiupkeep_date).Where(it => it.cn_t_equiupkeep_date != null).Select(it => it.cn_t_equiupkeep_date).FirstOrDefault();
        //            }
        //            foreach (var upkeep in upkeepPagedList.DataSource)
        //            {
        //                UpkeepItemModel upkeepItemModel = new UpkeepItemModel();
        //                DateTime compareTime;
        //                if (upkeepRecrusionModel.cn_s_equi_firstdate.HasValue == false && upkeepRecrusionModel.cn_s_equi_defentperiod.HasValue == false)
        //                {
        //                    continue;
        //                }

        //                if (upkeepPagedList.DataSource.Count == 0)
        //                {
        //                    compareTime = upkeepRecrusionModel.cn_s_equi_firstdate.Value;
        //                    upkeepItemModel.upkeepTime = compareTime.ToString();
        //                    upkeepItemModel.lastUpkeepTime = "";
        //                }
        //                else
        //                {
        //                    if (latestupkeeptime is null)
        //                    {
        //                        continue;
        //                    }
        //                    compareTime = latestupkeeptime.Value.AddDays(upkeepRecrusionModel.cn_s_equi_defentperiod.Value);
        //                    upkeepItemModel.upkeepTime = compareTime.ToString();
        //                    upkeepItemModel.lastUpkeepTime = latestupkeeptime.Value.ToString();
        //                }
        //                upkeepItemModel.upkeepItemName = upkeep.cn_s_equiupkeep_item;
        //                upkeepItemModel.defentperiod = upkeepRecrusion.cn_s_equi_defentperiod.Value.ToString();
        //                TimeSpan difference = compareTime - DateTime.Now;
        //                if (difference.Days > 7)
        //                {
        //                    upkeepItemModel.ext1 = "否";
        //                }
        //                else if (difference.Days >= 0)
        //                {
        //                    upkeepItemModel.ext1 = "是（" + difference.ToString() + "）";
        //                }
        //                else
        //                {
        //                    upkeepItemModel.ext1 = "是（超期" + Math.Abs(difference.Days).ToString() + "）";
        //                }
        //                upkeepItemModel.ext2 = "";
        //                upkeepItemModelList.Add(upkeepItemModel);
        //            }

        //            if (upkeepRecrusionModel.cn_s_equi_firstdate.HasValue == false || upkeepRecrusionModel.cn_s_equi_defentperiod.HasValue == false)
        //            {
        //                continue;
        //            }
        //            if (upkeepPagedList.DataSource.Count != 0 && latestupkeeptime is null)
        //            {
        //                continue;
        //            }
        //            equipmentUpkeepReminderView.eqUpkeepCollect.Add(new EQUpkeepCollect
        //            {
        //                deviceNo = upkeepRecrusion.cn_s_equi_no,
        //                upkeepItemCount = upkeepPagedList.DataSource.Count.ToString(),
        //                upkeepItem = upkeepItemModelList
        //            });
        //        }

        //        string sendJSONString = JsonConvert.SerializeObject(equipmentUpkeepReminderView);
        //        var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
        //        latestSyncUpdate = DateTime.Now;
        //        Thread.Sleep(1000);
        //    }
        //}

        ///// <summary>
        ///// 调用递归算法获取所有正常整机及其零部件设备编号
        ///// </summary>
        ///// <returns></returns>
        //private List<UpkeepRecrusionModel> GetAllUpkeepRecrusion()
        //{
        //    PagedInfo<tn_dts_equipment> completeMachinePagedList = IEQEquipmentService.GetPages(it => it.cn_s_equi_parttype == "整机" && it.cn_s_equi_status == "正常", new CommonUtil.Model.PageParm());
        //    List<UpkeepRecrusionModel> completeMachineUpkeepRecrusionList = completeMachinePagedList.DataSource
        //        .Select(it => new UpkeepRecrusionModel()
        //        {
        //            cn_s_equi_no = it.cn_s_equi_no,
        //            cn_s_equi_firstdate = it.cn_s_equi_firstdate,
        //            cn_s_equi_defentperiod = it.cn_s_equi_defentperiod
        //        }
        //        ).ToList();
        //    List<UpkeepRecrusionModel> allUpkeepRecrusionList = new List<UpkeepRecrusionModel>();
        //    foreach (var completeMachineUpkeepRecrusion in completeMachineUpkeepRecrusionList)
        //    {
        //        allUpkeepRecrusionList.Add(completeMachineUpkeepRecrusion);
        //        GetAllChildrenEquino_Recrusion(ref allUpkeepRecrusionList, completeMachineUpkeepRecrusion.cn_s_equi_no);
        //    }
        //    return allUpkeepRecrusionList;
        //}

        ///// <summary>
        ///// 根据父项设备编号递归获取所有子项设备编号
        ///// </summary>
        ///// <param name="equinoList"></param>
        ///// <param name="parentEquino"></param>
        //private void GetAllChildrenEquino_Recrusion(ref List<UpkeepRecrusionModel> equinoList, string parentEquino)
        //{
        //    if (IEQEquibomService.GetPages(it => it.cn_s_equibom_parentno == parentEquino, new CommonUtil.Model.PageParm()) is null)
        //    {

        //    }
        //    else
        //    {
        //        PagedInfo<tn_dts_equibom> childrenEquinoPagedList = IEQEquibomService.GetPages(it => it.cn_s_equibom_parentno == parentEquino && it.cn_s_equibom_status == "正常", new CommonUtil.Model.PageParm());
        //        foreach (var childrenEquino in childrenEquinoPagedList.DataSource)
        //        {
        //            UpkeepRecrusionModel upkeepRecrusionModel = IEQEquipmentService.GetPages(it => it.cn_s_equi_no == childrenEquino.cn_s_equibom_childno, new CommonUtil.Model.PageParm()).DataSource
        //                 .Select(it => new UpkeepRecrusionModel()
        //                 {
        //                     cn_s_equi_no = it.cn_s_equi_no,
        //                     cn_s_equi_firstdate = it.cn_s_equi_firstdate,
        //                     cn_s_equi_defentperiod = it.cn_s_equi_defentperiod

        //                 }).FirstOrDefault();

        //            equinoList.Add(upkeepRecrusionModel);
        //            GetAllChildrenEquino_Recrusion(ref equinoList, upkeepRecrusionModel.cn_s_equi_no);
        //        }
        //    }
        //}

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
