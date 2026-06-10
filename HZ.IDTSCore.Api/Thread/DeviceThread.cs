using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    /// <summary>
    /// 设备线程类
    /// </summary>
    public class DeviceThread: IHostedService
    {
        public IEquipmentService IEQService;
        private ILogsService _ILogsService;

        public DeviceThread()
        {
            _SyncThread = new Thread(SyncThreadHandle);
            _SyncThread.IsBackground = true;
            _PushThread = new Thread(PushThreadHandle);
            _PushThread.IsBackground = true;
            _ErrorsPushThread= new Thread(ErrorsPushThreadHandle);
            _ErrorsPushThread.IsBackground = true;
            deviceObjcetList = new List<MongoEquipment>();
            IEQService = ServiceLocator.GetService<IEquipmentService>(new DbHelper.SessionInfo() { splitDbCode = "" });
            _ILogsService = ServiceLocator.GetService<ILogsService>(new DbHelper.SessionInfo() { splitDbCode = "" });
        }
        /// <summary>
        /// 线程1:设备档案同步
        /// </summary>
        private Thread _SyncThread { get; set; }

        /// <summary>
        /// 线程2:数据推送线程
        /// </summary>
        private Thread _PushThread { get; set; }

        /// <summary>
        /// 线程3:设备异常推送线程
        /// </summary>
        private Thread _ErrorsPushThread { get; set; }

        /// <summary>
        /// 设备信息内存变量
        /// </summary>
        public List<MongoEquipment> deviceObjcetList { get; set; }

        private string Message { get; set; }

        /// <summary>
        /// 开启线程
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _SyncThread.Start();
            _PushThread.Start();
            _ErrorsPushThread.Start();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _SyncThread.Abort();
            _PushThread.Abort();
            _ErrorsPushThread.Abort();
            return Task.CompletedTask;
        }


        /// <summary>
        /// 设备档案处理
        /// </summary>
        private void SyncThreadHandle()
        {
            while (true)
            {
                try
                {
                    //1:从后台读取当前设备(整机)、装载到deviceObjcetList
                    var deviceList = IEQService.GetWhere(p => p.cn_s_equi_parttype == "整机" && p.cn_s_equi_status == "正常");
                    //List<tn_dts_equipment> deviceList = list.DataSource;
                    foreach (var equi in deviceList)
                    {
                        var cacheDevice = deviceObjcetList.FirstOrDefault(p => p.cn_s_equi_no == equi.cn_s_equi_no);
                        if (cacheDevice == null)
                        {
                            deviceObjcetList.Add(new MongoEquipment()
                            {
                                cn_guid = equi.cn_guid,
                                cn_s_creator = equi.cn_s_creator,
                                cn_s_creator_by = equi.cn_s_creator_by,
                                cn_s_equi_beltline = equi.cn_s_equi_beltline,
                                cn_s_equi_buydate = equi.cn_s_equi_buydate,
                                cn_s_equi_contractno = equi.cn_s_equi_contractno,
                                cn_s_equi_defentperiod = equi.cn_s_equi_defentperiod,
                                cn_s_equi_dept = equi.cn_s_equi_dept,
                                cn_s_equi_dutyman = equi.cn_s_equi_dutyman,
                                cn_s_equi_dutyphone = equi.cn_s_equi_dutyphone,
                                cn_s_equi_firstdate = equi.cn_s_equi_firstdate,
                                cn_s_equi_model = equi.cn_s_equi_model,
                                cn_s_equi_name = equi.cn_s_equi_name,
                                cn_s_equi_no = equi.cn_s_equi_no,
                                cn_s_equi_parttype = equi.cn_s_equi_parttype,
                                cn_s_equi_qadate = equi.cn_s_equi_qadate,
                                cn_s_equi_remarks = equi.cn_s_equi_remarks,
                                cn_s_equi_status = equi.cn_s_equi_status,
                                cn_s_equi_type = equi.cn_s_equi_type,
                                cn_s_equi_xpos = equi.cn_s_equi_xpos,
                                cn_s_equi_ypos = equi.cn_s_equi_ypos,
                                cn_s_equi_zpos = equi.cn_s_equi_zpos,
                                cn_s_modify = equi.cn_s_modify,
                                cn_s_modify_by = equi.cn_s_modify_by,
                                cn_t_create = equi.cn_t_create,
                                cn_t_modify = equi.cn_t_modify
                            });
                        }
                        else
                        {
                            cacheDevice.cn_guid = equi.cn_guid;
                            cacheDevice.cn_s_creator = equi.cn_s_creator;
                            cacheDevice.cn_s_creator_by = equi.cn_s_creator_by;
                            cacheDevice.cn_s_equi_beltline = equi.cn_s_equi_beltline;
                            cacheDevice.cn_s_equi_buydate = equi.cn_s_equi_buydate;
                            cacheDevice.cn_s_equi_contractno = equi.cn_s_equi_contractno;
                            cacheDevice.cn_s_equi_defentperiod = equi.cn_s_equi_defentperiod;
                            cacheDevice.cn_s_equi_dept = equi.cn_s_equi_dept;
                            cacheDevice.cn_s_equi_dutyman = equi.cn_s_equi_dutyman;
                            cacheDevice.cn_s_equi_dutyphone = equi.cn_s_equi_dutyphone;
                            cacheDevice.cn_s_equi_firstdate = equi.cn_s_equi_firstdate;
                            cacheDevice.cn_s_equi_model = equi.cn_s_equi_model;
                            cacheDevice.cn_s_equi_name = equi.cn_s_equi_name;
                            cacheDevice.cn_s_equi_no = equi.cn_s_equi_no;
                            cacheDevice.cn_s_equi_parttype = equi.cn_s_equi_parttype;
                            cacheDevice.cn_s_equi_qadate = equi.cn_s_equi_qadate;
                            cacheDevice.cn_s_equi_remarks = equi.cn_s_equi_remarks;
                            cacheDevice.cn_s_equi_status = equi.cn_s_equi_status;
                            cacheDevice.cn_s_equi_type = equi.cn_s_equi_type;
                            cacheDevice.cn_s_equi_xpos = equi.cn_s_equi_xpos;
                            cacheDevice.cn_s_equi_ypos = equi.cn_s_equi_ypos;
                            cacheDevice.cn_s_equi_zpos = equi.cn_s_equi_zpos;
                            cacheDevice.cn_s_modify = equi.cn_s_modify;
                            cacheDevice.cn_s_modify_by = equi.cn_s_modify_by;
                            cacheDevice.cn_t_create = equi.cn_t_create;
                            cacheDevice.cn_t_modify = equi.cn_t_modify;
                        }
                    }


                    //2:获取设备全局对象，刷新内存状态，并同步到Mongodb
                    foreach (var device in deviceObjcetList)
                    {
                        var cacheDevice = DeviceDriver.Instance.GetDevice(device.cn_s_equi_no);
                        if (cacheDevice != null) device.cn_s_equi_status = cacheDevice.deviceState;
                        var where = Builders<MongoEquipment>.Filter.Eq(o => o.cn_s_equi_no, device.cn_s_equi_no);
                        var model = MongoDBSingleton.Instance.FindOneFilter(where);
                        if (model == null)
                        {
                            MongoDBSingleton.Instance.Add(device);
                        }
                        else
                        {
                            device._id = model._id;
                            MongoDBSingleton.Instance.Update(device, device._id.ToString());
                        }
                    }


                    //Mongodb中不存在的移除
                    var filter = Builders<MongoEquipment>.Filter.Nin(b => b.cn_s_equi_no, deviceObjcetList.Select(p => p.cn_s_equi_no));
                    MongoDBSingleton.Instance.DeleteMany(filter);
                }
                catch (Exception ex)
                {
                    if (Message != ex.Message)
                    {
                        tn_dts_logs log = new tn_dts_logs();
                        log.cn_guid = Guid.NewGuid().ToString();
                        log.cn_s_logs_type = "程序";
                        log.cn_s_logs_errorsinfo = "DeviceThread线程异常，异常信息为："+ ex.Message;
                        log.cn_t_create = DateTime.Now;
                        int resLogs = _ILogsService.Add(log);
                        if (resLogs <= 0)
                        {
                            LogHelper.Info(DateTime.Now.ToString() + " DeviceThread线程异常存入tn_dts_logs表失败，异常信息为 " + ex.Message);
                        }
                    }
                    Message = ex.Message;
                }
                Thread.Sleep(10000);
            }
        }


        public GlobalRealCollectViewModel _deviceRealCollect = new GlobalRealCollectViewModel();
        /// <summary>
        /// 设备推送线程
        /// </summary>
        private void PushThreadHandle()
        {
            while (true)
            {
                if (deviceObjcetList.Count > 0)
                {
                    _deviceRealCollect = new GlobalRealCollectViewModel();
                    foreach (var device in deviceObjcetList)
                    {
                        var cacheDevice = DeviceDriver.Instance.GetDevice(device.cn_s_equi_no);
                        if (cacheDevice != null) device.cn_s_equi_status = cacheDevice.deviceState;

                        if (DeviceDriver.Instance.RealCollectList.eqRealCollect.FirstOrDefault(p => p.deviceNo == device.cn_s_equi_no) == null)
                        {
                            _deviceRealCollect.eqRealCollect.Add(new EQRealCollectModel()
                            {
                                deviceNo = device.cn_s_equi_no,
                                deviceName = device.cn_s_equi_name,
                                deviceType = device.cn_s_equi_type,
                                onlineStatus = device.cn_s_equi_status
                            });
                        }
                    }

                    if (_deviceRealCollect.eqRealCollect.Count > 0)
                    {
                        DeviceDriver.Instance.PublishRealCollect(_deviceRealCollect);
                    }

                    string sendJSONString = "";
                    if (!string.IsNullOrEmpty(DeviceDriver.Instance.DExplosion.deviceCode))
                    {
                        _deviceRealCollect = new GlobalRealCollectViewModel();
                        _deviceRealCollect.eqRealCollect = DeviceDriver.Instance.RealCollectList.eqRealCollect.Where(p => p.deviceNo == DeviceDriver.Instance.DExplosion.deviceCode).ToList();
                        //推送设备JSON数据(设备采集事项)
                        sendJSONString = JsonConvert.SerializeObject(_deviceRealCollect);
                    }
                    else
                    {
                        sendJSONString = JsonConvert.SerializeObject(DeviceDriver.Instance.RealCollectList);
                    }

                    var res = WebSocketServer.SessionInstance.Instance.PLCSendAllV2(sendJSONString);
                }
                Thread.Sleep(1000);
            }
        }

        private EqAlarmCollectViewModel AlarmCollectList = new EqAlarmCollectViewModel();
        private EqAlarmCollectModel AlarmCollect = new EqAlarmCollectModel();

        /// <summary>
        /// 异常推送线程
        /// </summary>
        public void ErrorsPushThreadHandle()
        {
            var filter = Builders<MongoEquialarmlogs>.Filter.Empty;
            //var sort = Builders<MongoEquialarmlogs>.Sort.Descending("cn_t_equialarmlogs_timestamp");
            var sort = Builders<MongoEquialarmlogs>.Sort.Descending("timestamp");
            while (true)
            {
                if (DeviceDriver.Instance.StateList.Count > 0)
                {
                    AlarmCollectList.eqAlarmCollect.Clear();
                    //遍历当前内存的设备信息
                    foreach (var driver in DeviceDriver.Instance.StateList)
                    {
                        AlarmCollect = new EqAlarmCollectModel();
                        AlarmCollect.deviceNo = driver.deviceCode;
                        AlarmCollect.deviceName = driver.deviceName;

                        //根据设备编号，查找该设备的警报明细信息
                        filter = Builders<MongoEquialarmlogs>.Filter.Eq(p => p.deviceCode, driver.deviceCode);
                        var list = MongoDBSingleton.Instance.FindListLimit(filter, 5, null, sort);
                        foreach (var alarm in list)
                        {
                            AlarmCollect.alarmItem.Add(new AlarmItems()
                            {
                                alarmCode = alarm.errCode,
                                alarmName = alarm.errMsg,
                                alarmTime = alarm.timestamp.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                        }
                        AlarmCollect.alarmItemCount = AlarmCollect.alarmItem.Count;
                        AlarmCollectList.eqAlarmCollect.Add(AlarmCollect);
                    }

                    if (AlarmCollectList.eqAlarmCollect.Count > 0)
                    {
                        string sendJSONString = "";
                        if (!string.IsNullOrEmpty(DeviceDriver.Instance.DExplosion.deviceCode))
                        {
                            EqAlarmCollectViewModel alarm = new EqAlarmCollectViewModel();
                            alarm.eqAlarmCollect = AlarmCollectList.eqAlarmCollect.Where(p => p.deviceNo == DeviceDriver.Instance.DExplosion.deviceCode).ToList();
                            //推送设备JSON数据(设备异常信息)
                            sendJSONString = JsonConvert.SerializeObject(alarm);
                        }
                        else
                        {
                            sendJSONString = JsonConvert.SerializeObject(AlarmCollectList);
                        }


                        var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
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
            _SyncThread.Start();
            _PushThread.Start();
            _ErrorsPushThread.Start();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            _SyncThread.Abort();
            _PushThread.Abort();
            _ErrorsPushThread.Abort();
        }
    }
}
