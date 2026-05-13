using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Controllers;
using HZ.IDTSCore.Api.Controllers.OpenApi;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.iWCS.MData.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class DtsDriver : BaseController
    {
        private static readonly DtsDriver instance = new DtsDriver();
        private IEquialarmlogsService _AlarmLogsService;
        private IEquireallogsService _RealLogsService;
        private DtsDriver()
        {
            _AlarmLogsService = ServiceLocator.GetService<IEquialarmlogsService>(HttpContextSession());
            _RealLogsService = ServiceLocator.GetService<IEquireallogsService>(HttpContextSession());
        }

        public static DtsDriver Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task<IActionResult> AgvRealMonitorSynchronous(AgvRealMonitorViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            //var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            ApiResult result = new ApiResult();
            try
            {
                if (model != null)
                {
                    //启用AGV时间轨迹
                    //_ITimeTrackingService.Add(new Model.Entity.Simulate.tn_dts_agvtimetracking()
                    //{
                    //    cn_t_tracking_timestamp = DateTime.Now,
                    //    cn_t_tracking_statejson = JsonConvert.SerializeObject(model.Agv)
                    //});

                    List<tn_dts_equialarmlogs> alarmLogs = new List<tn_dts_equialarmlogs>();//设备异常表
                    List<MongoEquialarmlogs> mogoAlarmLogs = new List<MongoEquialarmlogs>();//设备异常表
                    //采集事项表
                    DeviceRealCollectViewModel _deviceRealCollect = new DeviceRealCollectViewModel();
                    foreach (var device in model.Agv)
                    {
                        var cacheDevice = DeviceDriver.Instance.StateList.FirstOrDefault(p => p.deviceCode == device.carCode);
                        string errCode = device.errMsg.Split('#')[0];//异常代码
                        string errMsg = device.errMsg.Split('#')[1];//异常详情
                        if (cacheDevice != null && cacheDevice.errCode != errCode)
                        {
                            //增加到设备异常表
                            alarmLogs.Add(new tn_dts_equialarmlogs()
                            {
                                cn_guid = Guid.NewGuid().ToString(),
                                cn_s_equialarmlogs_errcode = errCode,
                                cn_s_equialarmlogs_errmsg = errMsg,
                                cn_s_equialarmlogs_no = device.carCode,
                                cn_s_equialarmlogs_name = device.carName,
                                cn_t_equialarmlogs_timestamp = DateTime.Now
                            });

                            //异常插入到Mongo
                            mogoAlarmLogs.Add(new MongoEquialarmlogs()
                            {
                                errCode = errCode,
                                errMsg = errMsg,
                                deviceCode = device.carCode,
                                deviceName = device.carName,
                                timestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                            });
                        }

                        //刷新内存设备状态
                        DeviceDriver.Instance.PublishDevices(device.carCode, device.carName, "AGV", device.onlineState, errCode, errMsg, DateTime.Now);


                        #region 增加到设备采集信息
                        EQRealCollectModel _EQRealCollectModel = new EQRealCollectModel();
                        _EQRealCollectModel.deviceNo = device.carCode;
                        _EQRealCollectModel.deviceName = device.carName;
                        _EQRealCollectModel.deviceType = "AGV";
                        _EQRealCollectModel.onlineStatus = device.onlineState;
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "X坐标", collectObjectVal = device.x, collectObjectUnit = "mm", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "Y坐标", collectObjectVal = device.y, collectObjectUnit = "mm", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "电量", collectObjectVal = device.power.ToString(), collectObjectUnit = "ah", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "角度", collectObjectVal = device.angle.ToString(), collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "速度", collectObjectVal = device.speed.ToString(), collectObjectUnit = "s", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "状态", collectObjectVal = errMsg, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItemCount = _EQRealCollectModel.collectItem.Count.ToString();
                        _deviceRealCollect.eqRealCollect.Add(_EQRealCollectModel);
                        #endregion

                        //启用任务轨迹记录
                        //if (!string.IsNullOrEmpty(device.goodsInfo.taskNo))
                        //{
                        //    var agvList = new List<AgvModel>();
                        //    agvList.Add(device);
                        //    _ITrackingService.Add(new Model.Entity.Simulate.tn_dts_agvtasktracking()
                        //    {
                        //        cn_t_tracking_timestamp = DateTime.Now,
                        //        cn_t_tracking_taskno = device.goodsInfo.taskNo,
                        //        cn_t_tracking_statejson = JsonConvert.SerializeObject(agvList)
                        //    });
                        //}
                    }

                    if (alarmLogs.Count > 0)
                        _AlarmLogsService.BatchAdd(alarmLogs);

                    if (mogoAlarmLogs.Count > 0)
                        MongoDBSingleton.Instance.InsertMany<MongoEquialarmlogs>(mogoAlarmLogs);

                    if (_deviceRealCollect.eqRealCollect.Count > 0)
                        DeviceRealCollect(_deviceRealCollect);
                    result.ErrCode = 200;
                    result.IsSuccess = true;
                    result.Message = "成功";
                    result.Timestamp = DateTime.Now.ToString();
                }

            }
            catch (Exception ex)
            {
                result.ErrCode = 500;
                result.IsSuccess = false;
                result.Message = "AgvRealMonitor接口异常-原因=" + ex.Message;
                result.Timestamp = DateTime.Now.ToString();
            }
            return toResponse(result);
        }

        public IActionResult DeviceRealCollect(DeviceRealCollectViewModel model)
        {
            ApiResult result = new ApiResult();
            if (model.eqRealCollect != null && model.eqRealCollect.Count > 0)
            {
                GlobalRealCollectViewModel _GlobalRealCollectViewModel = new GlobalRealCollectViewModel();
                _GlobalRealCollectViewModel.eqRealCollect = model.eqRealCollect;
                DeviceDriver.Instance.PublishRealCollect(_GlobalRealCollectViewModel);

                List<tn_dts_equireallogs> logs = new List<tn_dts_equireallogs>();//采集记录表
                List<tn_dts_equialarmlogs> alarmLogs = new List<tn_dts_equialarmlogs>();//设备异常表
                List<MongoEquialarmlogs> mogoAlarmLogs = new List<MongoEquialarmlogs>();//设备异常表
                foreach (var device in model.eqRealCollect)
                {
                    foreach (var item in device.collectItem)
                    {
                        logs.Add(new tn_dts_equireallogs()
                        {
                            cn_guid = Guid.NewGuid().ToString(),
                            cn_s_equireallogs_no = device.deviceNo,
                            cn_s_equireallogs_name = device.deviceName,
                            cn_t_equireallogs_timestamp = Convert.ToDateTime(item.collectTime),
                            cn_s_equireallogs_itemname = item.collectItemName,
                            cn_s_equireallogs_objectname = item.collectObjectName,
                            cn_s_equireallogs_objectval = item.collectObjectVal,
                            cn_s_equireallogs_objectvalunit = item.collectObjectUnit
                        });

                        //通过获取状态来判断异常
                        if (item.collectObjectName == "状态")//状态名称必须固定死
                        {
                            var cacheDevice = DeviceDriver.Instance.StateList.FirstOrDefault(p => p.deviceCode == device.deviceNo);
                            
                            if(cacheDevice != null)
                            {
                                string errCode = cacheDevice.errCode;//异常代码
                                string errMsg = cacheDevice.errMsg;//异常详情
                                if (cacheDevice.errCode != errCode)
                                {
                                    //增加到设备异常表
                                    alarmLogs.Add(new tn_dts_equialarmlogs()
                                    {
                                        cn_guid = Guid.NewGuid().ToString(),
                                        cn_s_equialarmlogs_errcode = errCode,
                                        cn_s_equialarmlogs_errmsg = errMsg,
                                        cn_s_equialarmlogs_no = device.deviceNo,
                                        cn_s_equialarmlogs_name = device.deviceName,
                                        cn_t_equialarmlogs_timestamp = DateTime.Now
                                    });

                                    //异常插入到Mongo
                                    mogoAlarmLogs.Add(new MongoEquialarmlogs()
                                    {
                                        errCode = errCode,
                                        errMsg = errMsg,
                                        deviceCode = device.deviceNo,
                                        deviceName = device.deviceName,
                                        timestamp = DateTime.Now
                                    });
                                }
                            }
                        }
                    }

                    //MongoDB 存储一份最新的值
                    var eqWhere = Builders<MongoRealCollect>.Filter.Eq(o => o.deviceNo, device.deviceNo);
                    MongoRealCollect _mogoRealCollect = MongoDBSingleton.Instance.FindOneFilter(eqWhere);
                    if (_mogoRealCollect == null)
                    {
                        _mogoRealCollect = new MongoRealCollect();
                        _mogoRealCollect.deviceNo = device.deviceNo;
                        _mogoRealCollect.deviceName = device.deviceName;
                        _mogoRealCollect.deviceType = device.deviceType;
                        _mogoRealCollect.onlineStatus = device.onlineStatus;
                        _mogoRealCollect.collectItemCount = device.collectItemCount;
                        _mogoRealCollect.collectItem = device.collectItem;
                        MongoDBSingleton.Instance.Add(_mogoRealCollect);
                    }
                    else
                    {
                        _mogoRealCollect.deviceNo = device.deviceNo;
                        _mogoRealCollect.deviceName = device.deviceName;
                        _mogoRealCollect.deviceType = device.deviceType;
                        _mogoRealCollect.onlineStatus = device.onlineStatus;
                        _mogoRealCollect.collectItemCount = device.collectItemCount;
                        _mogoRealCollect.collectItem = device.collectItem;
                        MongoDBSingleton.Instance.Update(_mogoRealCollect, _mogoRealCollect._id.ToString());
                    }
                }

                //这里插入到数据库 监控的历史记录，数据库每次产生一条记录
                if (logs.Count > 0)
                    _RealLogsService.BatchAdd(logs);

                if (alarmLogs.Count > 0)
                    _AlarmLogsService.BatchAdd(alarmLogs);

                if (mogoAlarmLogs.Count > 0)
                    MongoDBSingleton.Instance.InsertMany<MongoEquialarmlogs>(mogoAlarmLogs);

                result.ErrCode = 200;
                result.IsSuccess = true;
                result.Message = "成功";
                result.Timestamp = DateTime.Now.ToString();
            }
            else
            {
                result.ErrCode = 500;
                result.IsSuccess = false;
                result.Message = "采集设备集合无数据";
                result.Timestamp = DateTime.Now.ToString();
            }
            return toResponse(result);
        }
    }
}
