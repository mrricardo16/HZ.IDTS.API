using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Interfaces.IService.OpenApi;
using HZ.IDTSCore.Interfaces.IService.Simulate;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static HZ.IDTSCore.Model.Entity.OpenApi.ItemRowRack;

namespace HZ.IDTSCore.Api.Controllers.OpenApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class DtsController : BaseController
    {
        private IDtsService _IDtsService;
        private IEquireallogsService _RealLogsService;
        private IEquialarmlogsService _AlarmLogsService;
        private IEquipmentService _IEQService;
        private ICameraService _ICameraService;
        private ILogsService _ILogsService;
        private IAgvTaskTrackingService _ITrackingService;
        private IAgvTimeTrackingService _ITimeTrackingService;
        private ISettingService _ISettingService;

        public DtsController()
        {
            _IDtsService = ServiceLocator.GetService<IDtsService>(HttpContextSession());
            _RealLogsService = ServiceLocator.GetService<IEquireallogsService>(HttpContextSession());
            _AlarmLogsService = ServiceLocator.GetService<IEquialarmlogsService>(HttpContextSession());
            _IEQService = ServiceLocator.GetService<IEquipmentService>(HttpContextSession());
            _ICameraService = ServiceLocator.GetService<ICameraService>(HttpContextSession());
            _ILogsService = ServiceLocator.GetService<ILogsService>(HttpContextSession());
            _ITrackingService = ServiceLocator.GetService<IAgvTaskTrackingService>(HttpContextSession());
            _ITimeTrackingService = ServiceLocator.GetService<IAgvTimeTrackingService>(HttpContextSession());
            _ISettingService = ServiceLocator.GetService<ISettingService>(HttpContextSession());
        }


        #region 开放监控大屏初始化接口V2
        [HttpGet]
        public async Task<IActionResult> GetSysconfigV2()
        {
            UserSession user = GetSessionInfo();
            if (user != null)
            {
                ViewSystemconfigModel res = new ViewSystemconfigModel();
                string receiveresult = "";
                try
                {
                    res = _IDtsService.GetSystemconfigV2();
                    //ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
                    //string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
                    //await WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                    Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
                    receiveresult = "true";
                }
                catch (Exception ex)
                {
                    receiveresult = ex.Message;
                }
                OpenLog log = new OpenLog()
                {
                    logtype = "接口",
                    clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    receiveurl = "/api/dts/GetSysconfig",
                    receiveresult = receiveresult
                };
                UniversallyAddLog(log);

                return new JsonResult(res);
            }
            else
            {
                return new JsonResult("token is null");
            }
        }
        #endregion

        #region 开放监控大屏初始化接口
        [HttpGet]
        public async Task<IActionResult> GetSysconfig()
        {
            UserSession user = GetSessionInfo();
            if (user != null)
            {
                tn_dts_setting locationApiSetting = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSLocationApi");
                Newtonsoft.Json.Linq.JObject data = new Newtonsoft.Json.Linq.JObject();
                LocationRealMonitorViewModel locationRealMonitorViewModel = new LocationRealMonitorViewModel();
                locationRealMonitorViewModel.stock = new List<StockViewModel>();
                if (!(locationApiSetting is null))
                {
                    string locationApi = locationApiSetting.cn_s_setting_keyvalue;
                    try
                    {
                        Uri uri = new Uri(locationApi);
                        // 提取基础URL部分（包括协议、主机名和端口）  
                        string baseUrl = uri.Scheme + "://" + uri.Host + ":" + uri.Port;
                        // 提取路径和查询字符串部分  
                        string pathAndQuery = uri.PathAndQuery;
                        string stockCode = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSLocationApiStockCode").cn_s_setting_keyvalue;
                        string areaCode = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSLocationApiAreaCode").cn_s_setting_keyvalue;
                        ApiResult apiResLocation = new ApiResult();
                        string strLocationList = WebApiManager.HttpGet(baseUrl, pathAndQuery + $"?stockCode={stockCode}&areaCode={areaCode}", ref apiResLocation);
                        List<LocationStateWMS> locationStateInfoList = JsonConvert.DeserializeObject<List<LocationStateWMS>>(strLocationList);
                        LogHelper.Info("【locationStateInfoList】：" + JsonConvert.SerializeObject(locationStateInfoList));
                        LogHelper.Info("【strLocationList】：" + JsonConvert.SerializeObject(strLocationList));
                        if (locationStateInfoList is null)
                        {
                            OpenLog requestLog = new OpenLog()
                            {
                                logtype = "接口",
                                clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                                receiveurl = "/api/dts/GetSysconfig",
                                receiveresult = "请求WMS货位状态接口返回为空"
                            };
                            UniversallyAddLog(requestLog);
                        }
                        else
                        {
                            locationStateInfoList = locationStateInfoList.Where(it => it.moveLocations != null).ToList();
                            foreach (var locationState in locationStateInfoList)
                            {
                                StockViewModel stockViewModel = new StockViewModel();
                                stockViewModel.stockCode = locationState.stockCode;
                                stockViewModel.areaCode = locationState.areaCode;
                                stockViewModel.locationCode = locationState.fixLocationCode;
                                stockViewModel.locationType = locationState.structMode;
                                stockViewModel.state = locationState.locationState;
                                stockViewModel.storageState = locationState.useState;
                                stockViewModel.itemRow = new List<ItemRowViewModel>();
                                RackInfoViewModel rackInfoViewModel = new RackInfoViewModel();
                                rackInfoViewModel.remarks = "";
                                rackInfoViewModel.rackCode = locationState.moveLocations.rackCode;
                                rackInfoViewModel.rackAngle = "90";
                                rackInfoViewModel.ext1 = "";
                                rackInfoViewModel.ext2 = "";
                                List<BoxInfoViewModel> boxInfoViewModelList = new List<BoxInfoViewModel>();
                                foreach (var container in locationState.moveLocations.containers)
                                {
                                    BoxInfoViewModel boxInfoViewModel = new BoxInfoViewModel();
                                    boxInfoViewModel.boxCode = locationState.moveLocations.rackCode;
                                    boxInfoViewModel.boxLocation = HandleBoxLocation(container.moveLocationCode);
                                    boxInfoViewModel.storageState = container.useState == "满" ? "满箱" : "空箱";
                                    boxInfoViewModel.itemRow = new List<ItemRowRack>();
                                    boxInfoViewModelList.Add(boxInfoViewModel);
                                }
                                rackInfoViewModel.boxInfo = boxInfoViewModelList;
                                stockViewModel.rackInfo = rackInfoViewModel;
                                locationRealMonitorViewModel.stock.Add(stockViewModel);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OpenLog requestLog = new OpenLog()
                        {
                            logtype = "接口",
                            clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                            receiveurl = "/api/dts/GetSysconfig",
                            receiveresult = "请求WMS_货位状态数据接口失败，详细信息：" + ex.Message
                        };
                        UniversallyAddLog(requestLog);
                    }
                }

                //Newtonsoft.Json.Linq.JObject data = new Newtonsoft.Json.Linq.JObject();
                string receiveresult = "";
                try
                {
                    LogHelper.Info("【locationRealMonitorViewModel】：" + JsonConvert.SerializeObject(locationRealMonitorViewModel));
                    data = _IDtsService.GetSystemconfig(locationRealMonitorViewModel);
                    //ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
                    //string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
                    //WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                    Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
                    receiveresult = "true";
                }
                catch (Exception ex)
                {
                    receiveresult = ex.Message;
                }
                OpenLog log = new OpenLog()
                {
                    logtype = "接口",
                    clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    receiveurl = "/api/dts/GetSysconfig",
                    receiveresult = receiveresult
                };
                UniversallyAddLog(log);

                return new ContentResult
                {
                    Content = data.ToString(),
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }
            else
            {
                return new JsonResult("token is null");
            }
        }

        [HttpPost]
        public string HandleBoxLocation(string boxLocation)
        {
            string[] parts = boxLocation.Split('_'); // 使用下划线分割字符串  
            if (parts.Length > 1)
            {
                if (parts[1] == "0")
                {
                    parts[1] = "1";
                }
                else
                {
                    parts[1] = "2";
                }
                // 跳过第一部分（即"YD00003"），并用连字符连接剩余部分  
                //大屏：排-列-层
                //WMS：列 - 层 - 排
                string result = parts[3] + "-" + parts[1] + "-" + parts[2];
                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        //[HttpPost]
        //public string GetStorageState(string useState, bool trayHasItem)
        //{
        //    if(useState == "空")
        //    {
        //        return "空";
        //    }
        //    else
        //    {
        //        if(trayHasItem)
        //        {
        //            return "满";
        //        }
        //        else
        //        {
        //            return "空托盘";
        //        }
        //    }
        //}
        #endregion

        #region 开放货位同步接口
        [HttpPost]
        public IActionResult LocationRealMonitor(LocationRealMonitorViewModel model)
        {
            ApiResult result = new ApiResult();
            if (model != null)
            {
                string sendJSONString = JsonConvert.SerializeObject(model);
                var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
                if (model.stock.Count > 0)
                    StorageDriver.Instance.PublishStocksInfo(model.stock);

                result.IsSuccess = true;
                result.ErrCode = 0;
                result.Message = "";
            }
            else
            {
                result.IsSuccess = false;
                result.ErrCode = -1;
                result.Message = "请求参数不能为空或NULL";
            }
            return toResponse(result);
        }
        #endregion

        #region AGV仿真接口
        [HttpPost]
        public async Task AgvSimulate(AgvRealMonitorViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            await Task.Run(() =>
            {
                LogHelper.Info(sendJSONString);
                WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            });
        }
        #endregion

        #region 开放的AGV同步数据接口（异步）
        [HttpPost]
        public async Task<IActionResult> AgvRealMonitor(AgvRealMonitorViewModel model)
        {
            var result = await DtsDriver.Instance.AgvRealMonitorSynchronous(model);
            return result;
        }
        #endregion

        #region 开放的AGV同步数据接口（同步）
        [HttpPost]
        public IActionResult AgvRealMonitorSynchronous(AgvRealMonitorViewModel model)
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
                }
                result.ErrCode = 200;
                result.IsSuccess = true;
                result.Message = "成功";
                result.Timestamp = DateTime.Now.ToString();
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
        #endregion

        #region 开放的工位呼叫提醒接口
        [HttpPost]
        public IActionResult StationCallMonitor(StationCallMonitorViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            return toResponse(res);
        }
        #endregion

        #region 开放的充电机状态接口
        [HttpPost]
        public IActionResult BatteryChargerMonitor(BatteryChargerMonitorViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            ApiResult result = new ApiResult();
            try
            {
                if (model != null)
                {
                    List<tn_dts_equialarmlogs> alarmLogs = new List<tn_dts_equialarmlogs>();//设备异常表
                    List<MongoEquialarmlogs> mogoAlarmLogs = new List<MongoEquialarmlogs>();//设备异常表
                    foreach (var device in model.Charger)
                    {

                        var cacheDevice = DeviceDriver.Instance.StateList.FirstOrDefault(p => p.deviceCode == device.deviceName);
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
                                cn_s_equialarmlogs_no = device.deviceName,
                                cn_s_equialarmlogs_name = device.deviceName,
                                cn_t_equialarmlogs_timestamp = DateTime.Now
                            });

                            //异常插入到Mongo
                            mogoAlarmLogs.Add(new MongoEquialarmlogs()
                            {
                                errCode = errCode,
                                errMsg = errMsg,
                                deviceCode = device.deviceName,
                                deviceName = device.deviceName,
                                timestamp = DateTime.Now
                            });
                        }

                        DeviceDriver.Instance.PublishDevices(device.deviceName, device.deviceName, "Charger", device.onlineState, errCode, errMsg, DateTime.Now);
                    }

                    if (alarmLogs.Count > 0)
                        _AlarmLogsService.BatchAdd(alarmLogs);

                    if (mogoAlarmLogs.Count > 0)
                        MongoDBSingleton.Instance.InsertMany<MongoEquialarmlogs>(mogoAlarmLogs);
                }

                result.ErrCode = 200;
                result.IsSuccess = true;
                result.Message = "成功";
                result.Timestamp = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                result.ErrCode = 500;
                result.IsSuccess = false;
                result.Message = "BatteryChargerMonitor接口异常-原因=" + ex.Message;
                result.Timestamp = DateTime.Now.ToString();
            }
            return toResponse(result);
        }
        #endregion

        #region 通用设备实时采集接口
        [HttpPost]
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
                            string errCode = cacheDevice.errCode;//异常代码
                            string errMsg = cacheDevice.errMsg;//异常详情
                            if (cacheDevice != null && cacheDevice.errCode != errCode)
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
        #endregion

        #region 设备查询位置定位
        /// <summary>
        /// 设备查询位置定位
        /// </summary>
        /// <param name="equiCode">设备编号</param>
        /// <param name="equiName">设备名称</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EquiQuerylist(string equiCode, string equiName)
        {
            EquiQueryResult result = new EquiQueryResult();
            string receiveresult = "";
            try
            {
                var builder = Builders<MongoEquipment>.Filter;
                List<FilterDefinition<MongoEquipment>> queryList = new List<FilterDefinition<MongoEquipment>>();
                var filter = builder.Empty;
                if (!string.IsNullOrEmpty(equiCode))
                {
                    queryList.Add(builder.Or(builder.Regex("cn_s_equi_no", new BsonRegularExpression("/" + equiCode + "/"))));
                }
                if (!string.IsNullOrEmpty(equiName))
                {
                    queryList.Add(builder.Or(builder.Regex("cn_s_equi_name", new BsonRegularExpression("/" + equiName + "/"))));
                }
                if (queryList.Count > 0) filter = Builders<MongoEquipment>.Filter.Or(queryList);
                var list = MongoDBSingleton.Instance.FindList(filter);
                foreach (var device in list)
                {
                    result.equiQueryList.Add(new EquiQueryViewModel()
                    {
                        equiCode = device.cn_s_equi_no,
                        equiName = device.cn_s_equi_name
                    });
                }
                receiveresult = "true";
            }
            catch (Exception ex)
            {
                receiveresult = ex.Message;
            }
            EquiQueryViewModel queryModel = new EquiQueryViewModel()
            {
                equiCode = equiCode,
                equiName = equiName
            };
            OpenLog log = new OpenLog()
            {
                logtype = "接口",
                clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),

                receivepram = JsonConvert.SerializeObject(queryModel),
                receiveurl = "/api/dts/EquiQuerylist",
                receiveresult = receiveresult
            };
            UniversallyAddLog(log);
            return new JsonResult(result);
        }
        #endregion

        #region 设备查询位置定位
        //[HttpGet]
        //public IActionResult EquiQuerylist(EquiQueryViewModel queryModel)
        //{
        //    EquiQueryResult result = new EquiQueryResult();
        //    string receiveresult = "";
        //    try
        //    {
        //        var builder = Builders<MongoEquipment>.Filter;
        //        List<FilterDefinition<MongoEquipment>> queryList = new List<FilterDefinition<MongoEquipment>>();
        //        var filter = builder.Empty;
        //        if (queryModel != null && !string.IsNullOrEmpty(queryModel.equiCode))
        //        {
        //            queryList.Add(builder.Or(builder.Regex("cn_s_equi_no", new BsonRegularExpression("/" + queryModel.equiCode + "/"))));
        //        }
        //        if (queryModel != null && !string.IsNullOrEmpty(queryModel.equiName))
        //        {
        //            queryList.Add(builder.Or(builder.Regex("cn_s_equi_name", new BsonRegularExpression("/" + queryModel.equiName + "/"))));
        //        }
        //        if (queryList.Count > 0) filter = Builders<MongoEquipment>.Filter.Or(queryList);
        //        var list = MongoDBSingleton.Instance.FindList(filter);
        //        foreach (var device in list)
        //        {
        //            result.equiQueryList.Add(new EquiQueryViewModel()
        //            {
        //                equiCode = device.cn_s_equi_no,
        //                equiName = device.cn_s_equi_name
        //            });
        //        }
        //        receiveresult = "true";
        //    }
        //    catch (Exception ex)
        //    {
        //        receiveresult = ex.Message;
        //    }
        //    OpenLog log = new OpenLog()
        //    {
        //        logtype = "接口",
        //        clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
        //        receivepram = JsonConvert.SerializeObject(queryModel),
        //        receiveurl = "/api/dts/EquiQuerylist",
        //        receiveresult = receiveresult
        //    };
        //    UniversallyAddLog(log);
        //    return new JsonResult(result);
        //}
        #endregion

        #region 物资查询位置定位接口
        /// <summary>
        /// 物资查询位置定位接口（兼容料架）
        /// </summary>
        /// <param name="goodsCode">物料编码</param>
        /// <param name="goodsName">物料名称</param>
        /// <param name="pileofland">项目编号</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GoodsQuerylist(string goodsCode, string goodsName, string pileofland = "")
        {
            GoodsQueryResult result = new GoodsQueryResult();
            GoodsQueryRackResult goodsQueryRackResult = new GoodsQueryRackResult();
            string receiveresult = "";
            bool isSuccess = false;
            if (pileofland == "DD691")
            {
                try
                {
                    tn_dts_setting goodsApiSetting = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSGoodsApi");
                    string goodsApi = goodsApiSetting.cn_s_setting_keyvalue;
                    Uri uri = new Uri(goodsApi);
                    // 提取基础URL部分（包括协议、主机名和端口）  
                    string baseUrl = uri.Scheme + "://" + uri.Host + ":" + uri.Port;
                    // 提取路径和查询字符串部分  
                    string pathAndQuery = uri.PathAndQuery;
                    string stockCode = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSLocationApiStockCode").cn_s_setting_keyvalue;
                    string areaCode = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSLocationApiAreaCode").cn_s_setting_keyvalue;
                    ApiResult apiResLocation = new ApiResult();
                    string strGoodsList = WebApiManager.HttpGet(baseUrl, pathAndQuery + $"?stockCode={stockCode}&areaCode={areaCode}&itemCodeName={goodsCode}", ref apiResLocation);
                    if(string.IsNullOrEmpty(strGoodsList))
                    {
                        throw new Exception("WMS物料查询接口返回为空");
                    }
                    List<GoodsQuery> locationStateInfoList = JsonConvert.DeserializeObject<List<GoodsQuery>>(strGoodsList);
                    foreach (var locationStateInfo in locationStateInfoList)
                    {
                        if (locationStateInfo.rackInfo != null && locationStateInfo.rackInfo.boxInfo.Count != 0)
                        {
                            locationStateInfo.rackInfo.boxInfo[0].boxLocation = HandleBoxLocation(locationStateInfo.rackInfo.boxInfo[0].boxLocation);
                        }
                        goodsQueryRackResult.goodsQueryList.Add(locationStateInfo);
                    }
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    receiveresult = ex.Message;
                }
                GoodsQueryRackViewModel queryRackModel = new GoodsQueryRackViewModel()
                {
                    goodsCode = goodsCode,
                    goodsName = goodsName,
                    pileofland = pileofland
                };
                OpenLog log = new OpenLog()
                {
                    logtype = "接口",
                    clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    receivepram = JsonConvert.SerializeObject(queryRackModel),
                    receiveurl = "/api/dts/GoodsQuerylist",
                    receiveresult = isSuccess ? "成功匹配到的物资个数有：" + goodsQueryRackResult.goodsQueryList.Count : receiveresult
                };
                UniversallyAddLog(log);
                return new JsonResult(goodsQueryRackResult);
            }
            else
            {
                try
                {
                    foreach (var goods in StorageDriver.Instance.GoodsList)
                    {
                        //goods.itemRow.FindAll(p => p.itemCode.Contains(queryModel.goodsCode) || p.itemName.Contains(queryModel.goodsName));
                        foreach (var itemRow in goods.itemRow)
                        {
                            //Regex regCode = new Regex("/" + queryModel.goodsCode + "/");
                            Regex regCode = new Regex(goodsCode);
                            //Regex regName = new Regex("/" + queryModel.goodsName + "/");
                            Regex regName = new Regex(goodsName);
                            if (regCode.IsMatch(itemRow.itemCode) || regName.IsMatch(itemRow.itemName))
                            {
                                GoodsQueryResultModel goodsQueryResultModel = new GoodsQueryResultModel();
                                goodsQueryResultModel.goodsCode = itemRow.itemCode;
                                goodsQueryResultModel.goodsName = itemRow.itemName;
                                goodsQueryResultModel.locationCode = goods.locationCode;
                                result.goodsQueryList.Add(goodsQueryResultModel);
                            }
                        }
                    }
                    //receiveresult = JsonConvert.SerializeObject(result);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    receiveresult = ex.Message;
                }
                GoodsQueryViewModel queryModel = new GoodsQueryViewModel()
                {
                    goodsCode = goodsCode,
                    goodsName = goodsName
                };
                OpenLog log = new OpenLog()
                {
                    logtype = "接口",
                    clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    receivepram = JsonConvert.SerializeObject(queryModel),
                    receiveurl = "/api/dts/GoodsQuerylist",
                    receiveresult = isSuccess ? "成功匹配到的物资个数有：" + result.goodsQueryList.Count : receiveresult
                };
                UniversallyAddLog(log);
                return new JsonResult(result);
            }
        }
        #endregion

        #region 物资查询位置定位接口（兼容料架）
        ///// <summary>
        ///// 物资查询位置定位接口（兼容料架）
        ///// </summary>
        ///// <param name="goodsCode">物料编码</param>
        ///// <param name="goodsName">物料名称</param>
        ///// <param name="pileofland">项目编号</param>
        ///// <returns></returns>
        //[HttpGet]
        //public IActionResult GoodsQuerylist(string goodsCode, string goodsName, string pileofland)
        //{
        //    GoodsQueryRackResult goodsQueryRackResult = new GoodsQueryRackResult();
        //    if (pileofland == "DD691")
        //    {
        //        string receiveresult = "";
        //        bool isSuccess = false;
        //        try
        //        {
        //            tn_dts_setting goodsApiSetting = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSGoodsApi");
        //            string goodsApi = goodsApiSetting.cn_s_setting_keyvalue;
        //            Uri uri = new Uri(goodsApi);
        //            // 提取基础URL部分（包括协议、主机名和端口）  
        //            string baseUrl = uri.Scheme + "://" + uri.Host + ":" + uri.Port;
        //            // 提取路径和查询字符串部分  
        //            string pathAndQuery = uri.PathAndQuery;
        //            string stockCode = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSLocationApiStockCode").cn_s_setting_keyvalue;
        //            string areaCode = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSLocationApiAreaCode").cn_s_setting_keyvalue;
        //            ApiResult apiResLocation = new ApiResult();
        //            string strGoodsList = WebApiManager.HttpGet(baseUrl, pathAndQuery + $"?stockCode={stockCode}&areaCode={areaCode}&itemCodeName={goodsCode}", ref apiResLocation);
        //            List<GoodsQuery> locationStateInfoList = JsonConvert.DeserializeObject<List<GoodsQuery>>(strGoodsList);
        //            foreach (var locationStateInfo in locationStateInfoList)
        //            {
        //                if (locationStateInfo.rackInfo != null && locationStateInfo.rackInfo.boxInfo.Count != 0)
        //                {
        //                    locationStateInfo.rackInfo.boxInfo[0].boxLocation = HandleBoxLocation(locationStateInfo.rackInfo.boxInfo[0].boxLocation);
        //                }
        //                goodsQueryRackResult.goodsQueryList.Add(locationStateInfo);
        //            }
        //            isSuccess = true;
        //        }
        //        catch(Exception ex)
        //        {
        //            receiveresult = ex.Message;
        //        }
        //        GoodsQueryRackViewModel queryRackModel = new GoodsQueryRackViewModel()
        //        {
        //            goodsCode = goodsCode,
        //            goodsName = goodsName,
        //            pileofland = pileofland
        //        };
        //        OpenLog log = new OpenLog()
        //        {
        //            logtype = "接口",
        //            clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
        //            receivepram = JsonConvert.SerializeObject(queryRackModel),
        //            receiveurl = "/api/dts/GoodsQuerylist",
        //            receiveresult = isSuccess ? "成功匹配到的物资个数有：" + goodsQueryRackResult.goodsQueryList.Count : receiveresult
        //        };
        //        UniversallyAddLog(log);
        //    }
        //    return new JsonResult(goodsQueryRackResult);
        //}
        #endregion


        #region 物资查询位置定位接口
        ///// <summary>
        ///// 物资查询位置定位接口
        ///// </summary>
        ///// <param name="queryModel"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public IActionResult GoodsQuerylist(GoodsQueryViewModel queryModel)
        //{
        //    GoodsQueryResult result = new GoodsQueryResult();
        //    string receiveresult = "";
        //    try
        //    {
        //        foreach (var goods in StorageDriver.Instance.GoodsList)
        //        {
        //            //goods.itemRow.FindAll(p => p.itemCode.Contains(queryModel.goodsCode) || p.itemName.Contains(queryModel.goodsName));
        //            foreach (var itemRow in goods.itemRow)
        //            {
        //                //Regex regCode = new Regex("/" + queryModel.goodsCode + "/");
        //                Regex regCode = new Regex(queryModel.goodsCode);
        //                //Regex regName = new Regex("/" + queryModel.goodsName + "/");
        //                Regex regName = new Regex(queryModel.goodsName);
        //                if (regCode.IsMatch(itemRow.itemCode) || regName.IsMatch(itemRow.itemName))
        //                {
        //                    GoodsQueryResultModel goodsQueryResultModel = new GoodsQueryResultModel();
        //                    goodsQueryResultModel.goodsCode = itemRow.itemCode;
        //                    goodsQueryResultModel.goodsName = itemRow.itemName;
        //                    goodsQueryResultModel.locationCode = goods.locationCode;
        //                    result.goodsQueryList.Add(goodsQueryResultModel);
        //                }
        //            }
        //        }
        //        receiveresult = JsonConvert.SerializeObject(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        receiveresult = ex.Message;
        //    }
        //    OpenLog log = new OpenLog()
        //    {
        //        logtype = "接口",
        //        clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
        //        receivepram = JsonConvert.SerializeObject(queryModel),
        //        receiveurl = "/api/dts/GoodsQuerylist",
        //        receiveresult = receiveresult
        //    };
        //    UniversallyAddLog(log);
        //    return new JsonResult(result);
        //}
        #endregion

        #region AGV爆炸图核心部件展示交互项
        /// <summary>
        /// AGV爆炸图核心部件展示交互项
        /// </summary>
        /// <param name="deviceExplosionModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Dexplosion(DeviceExplosionModel deviceExplosionModel)
        {
            DeviceDriver.Instance.DExplosion = deviceExplosionModel;
            DexplosionResultModel dexplosionResultModel = new DexplosionResultModel();
            dexplosionResultModel.IsSuccess = true;
            dexplosionResultModel.ErrCode = "";
            dexplosionResultModel.Message = "";
            string receiveresult = JsonConvert.SerializeObject(dexplosionResultModel);
            OpenLog log = new OpenLog()
            {
                logtype = "接口",
                clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                receivepram = JsonConvert.SerializeObject(deviceExplosionModel),
                receiveurl = "/api/dts/Dexplosion",
                receiveresult = receiveresult
            };
            UniversallyAddLog(log);
            return new JsonResult(dexplosionResultModel);
        }
        #endregion

        #region 数据汇总统计接口
        [HttpPost]
        public IActionResult DataSummaryMonitor(DataSummaryViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            return new JsonResult(res);
        }
        #endregion

        #region 出入库排名接口
        [HttpPost]
        public IActionResult InOutStockRankingMonitor(InOutStockRankingViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            return new JsonResult(res);
        }
        #endregion

        #region 任务分时统计接口
        [HttpPost]
        public IActionResult HourTaskGroupMonitor(HourTaskGroupViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            return new JsonResult(res);
        }
        #endregion

        #region 出入库业务分类统计接口
        [HttpPost]
        public IActionResult InOutStockCategoryMonitor(InOutStockCategoryViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            return new JsonResult(res);
        }
        #endregion

        #region 标准计划完成比接口
        [HttpPost]
        public IActionResult PlanMonitor(PlanViewModel model)
        {
            string sendJSONString = JsonConvert.SerializeObject(model);
            var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            return new JsonResult(res);
        }
        #endregion

        #region 获取零部件接口
        /// <summary>
        /// 获取零部件接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPartDetail(PartDetail model)
        {
            var res = _IDtsService.GetPartDetail(model);
            if (res is null)
            {
                ApiResult<ReturnPartDetail> response = new ApiResult<ReturnPartDetail>();
                response.StatusCode = (int)StatusCodeType.Success;
                response.IsSuccess = false;
                response.Message = "传入的设备编号不在数据库中，请重试!";
                response.Data = new ReturnPartDetail() { deviceCode = model.deviceCode };
                return new JsonResult(response);
            }
            else
            {
                return toResponse(res);
            }

        }
        #endregion

        #region 通用新增日志接口
        /// <summary>
        /// 通用新增日志接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UniversallyAddLog(OpenLog model)
        {
            ReturnMessage res = _IDtsService.UniversallyAddLog(model);
            ApiResult resultReturn = new ApiResult();
            if (res.IsSuccess)
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Success;
                resultReturn.Message = res.Message;
            }
            else
            {
                resultReturn.IsSuccess = res.IsSuccess;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = res.Message;
            }
            return toResponse(resultReturn);
        }
        #endregion

        #region 开放堆垛车数据同步接口
        /// <summary>
        /// 开放堆垛车数据同步接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> StackerRealCollect(StackerRealCollectViewModel model)
        {
            await Task.Run(() =>
            {
                string sendJSONString = JsonConvert.SerializeObject(model);
                string log = "外部在" + DateTime.Now.ToString() + "调用开放堆垛车数据同步接口，传入数据为：" + sendJSONString;
                LogHelper.Info(log);
                WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            });

            ApiResult result = new ApiResult();
            try
            {
                if (model != null && model.Stacker.Count > 0)
                {
                    List<tn_dts_equialarmlogs> alarmLogs = new List<tn_dts_equialarmlogs>();//设备异常表
                    List<MongoEquialarmlogs> mogoAlarmLogs = new List<MongoEquialarmlogs>();//设备异常表
                    DeviceRealCollectViewModel _deviceRealCollect = new DeviceRealCollectViewModel();
                    foreach (var stacker in model.Stacker)
                    {

                        var cacheDevice = DeviceDriver.Instance.StateList.FirstOrDefault(it => it.deviceCode == stacker.Code);
                        string errCode = stacker.ErrCode;
                        string errMsg = stacker.ErrMsg;
                        if (cacheDevice != null && cacheDevice.errCode != errCode)
                        {
                            //增加到设备异常表
                            alarmLogs.Add(new tn_dts_equialarmlogs()
                            {
                                cn_guid = Guid.NewGuid().ToString(),
                                cn_s_equialarmlogs_errcode = errCode,
                                cn_s_equialarmlogs_errmsg = errMsg,
                                cn_s_equialarmlogs_no = stacker.Code,
                                cn_s_equialarmlogs_name = stacker.Name,
                                cn_t_equialarmlogs_timestamp = DateTime.Now
                            });

                            //异常插入到Mongo
                            mogoAlarmLogs.Add(new MongoEquialarmlogs()
                            {
                                errCode = errCode,
                                errMsg = errMsg,
                                deviceCode = stacker.Code,
                                deviceName = stacker.Name,
                                timestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                            });
                        }

                        DeviceDriver.Instance.PublishDevices(stacker.Code, stacker.Name, "堆垛机", stacker.State, errCode, errMsg, DateTime.Now);

                        EQRealCollectModel _EQRealCollectModel = new EQRealCollectModel();
                        _EQRealCollectModel.deviceNo = stacker.Code;
                        _EQRealCollectModel.deviceName = stacker.Name;
                        _EQRealCollectModel.deviceType = "堆垛机";
                        _EQRealCollectModel.onlineStatus = stacker.State;
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "堆垛机", collectObjectName = "任务类型", collectObjectVal = stacker.TaskType, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "堆垛机", collectObjectName = "任务号", collectObjectVal = stacker.TaskNo, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "堆垛机", collectObjectName = "目的位置", collectObjectVal = stacker.EndStation, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "堆垛机", collectObjectName = "托盘类型", collectObjectVal = stacker.TrayType, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "堆垛机", collectObjectName = "运行状态", collectObjectVal = stacker.State, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "堆垛机", collectObjectName = "货物信息", collectObjectVal = stacker.GoodsInfo, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItemCount = _EQRealCollectModel.collectItem.Count.ToString();
                        _deviceRealCollect.eqRealCollect.Add(_EQRealCollectModel);
                    }

                    if (alarmLogs.Count > 0)
                        _AlarmLogsService.BatchAdd(alarmLogs);

                    if (mogoAlarmLogs.Count > 0)
                        MongoDBSingleton.Instance.InsertMany<MongoEquialarmlogs>(mogoAlarmLogs);

                    if (_deviceRealCollect.eqRealCollect.Count > 0)
                    {
                        DeviceRealCollect(_deviceRealCollect);
                    }
                }
                result.ErrCode = 200;
                result.IsSuccess = true;
                result.Message = "成功";
                result.Timestamp = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                result.ErrCode = 500;
                result.IsSuccess = false;
                result.Message = "StackingtruckRealCollect接口异常-原因=" + ex.Message;
                result.Timestamp = DateTime.Now.ToString();
            }
            return toResponse(result);
        }
        #endregion

        #region 开放RGV数据同步接口
        /// <summary>
        /// 开放RGV数据同步接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RGVRealCollect(RGVRealCollectViewModel model)
        {
            await Task.Run(() =>
            {
                string sendJSONString = JsonConvert.SerializeObject(model);
                string log = "外部在" + DateTime.Now.ToString() + "调用开放RGV数据同步接口，传入数据为：" + sendJSONString;
                LogHelper.Info(log);
                WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            });

            ApiResult result = new ApiResult();
            try
            {
                if (model != null && model.RGV.Count > 0)
                {
                    List<tn_dts_equialarmlogs> alarmLogs = new List<tn_dts_equialarmlogs>();//设备异常表
                    List<MongoEquialarmlogs> mogoAlarmLogs = new List<MongoEquialarmlogs>();//设备异常表
                    DeviceRealCollectViewModel _deviceRealCollect = new DeviceRealCollectViewModel();
                    foreach (var rgv in model.RGV)
                    {

                        var cacheDevice = DeviceDriver.Instance.StateList.FirstOrDefault(it => it.deviceCode == rgv.Code);
                        string errCode = rgv.ErrCode;
                        string errMsg = rgv.ErrMsg;
                        if (cacheDevice != null && cacheDevice.errCode != errCode)
                        {
                            //增加到设备异常表
                            alarmLogs.Add(new tn_dts_equialarmlogs()
                            {
                                cn_guid = Guid.NewGuid().ToString(),
                                cn_s_equialarmlogs_errcode = errCode,
                                cn_s_equialarmlogs_errmsg = errMsg,
                                cn_s_equialarmlogs_no = rgv.Code,
                                cn_s_equialarmlogs_name = rgv.Name,
                                cn_t_equialarmlogs_timestamp = DateTime.Now
                            });

                            //异常插入到Mongo
                            mogoAlarmLogs.Add(new MongoEquialarmlogs()
                            {
                                errCode = errCode,
                                errMsg = errMsg,
                                deviceCode = rgv.Code,
                                deviceName = rgv.Name,
                                timestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                            });
                        }

                        DeviceDriver.Instance.PublishDevices(rgv.Code, rgv.Name, "RGV", rgv.State, errCode, errMsg, DateTime.Now);

                        EQRealCollectModel _EQRealCollectModel = new EQRealCollectModel();
                        _EQRealCollectModel.deviceNo = rgv.Code;
                        _EQRealCollectModel.deviceName = rgv.Name;
                        _EQRealCollectModel.deviceType = "RGV";
                        _EQRealCollectModel.onlineStatus = rgv.State;
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "RGV", collectObjectName = "任务号", collectObjectVal = rgv.TaskNo, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "RGV", collectObjectName = "起点位置", collectObjectVal = rgv.StartStation, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "RGV", collectObjectName = "目的位置", collectObjectVal = rgv.EndStation, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "RGV", collectObjectName = "托盘类型", collectObjectVal = rgv.TrayType, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "RGV", collectObjectName = "运行状态", collectObjectVal = rgv.State, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "RGV", collectObjectName = "货物信息", collectObjectVal = rgv.GoodsInfo, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        _EQRealCollectModel.collectItemCount = _EQRealCollectModel.collectItem.Count.ToString();
                        _deviceRealCollect.eqRealCollect.Add(_EQRealCollectModel);
                    }

                    if (alarmLogs.Count > 0)
                        _AlarmLogsService.BatchAdd(alarmLogs);

                    if (mogoAlarmLogs.Count > 0)
                        MongoDBSingleton.Instance.InsertMany<MongoEquialarmlogs>(mogoAlarmLogs);

                    if (_deviceRealCollect.eqRealCollect.Count > 0)
                    {
                        DeviceRealCollect(_deviceRealCollect);
                    }
                }

                result.ErrCode = 200;
                result.IsSuccess = true;
                result.Message = "成功";
                result.Timestamp = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                result.ErrCode = 500;
                result.IsSuccess = false;
                result.Message = "RGVRealCollect接口异常-原因=" + ex.Message;
                result.Timestamp = DateTime.Now.ToString();
            }
            return toResponse(result);
        }
        #endregion

        #region 开放输送线数据同步接口
        /// <summary>
        /// 开放输送线数据同步接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ConveyorRealCollect(ConveyorRealCollectViewModel model)
        {
            await Task.Run(() =>
            {
                string sendJSONString = JsonConvert.SerializeObject(model);
                string log = "外部在" + DateTime.Now.ToString() + "调用开放输送线数据同步接口，传入数据为：" + sendJSONString;
                LogHelper.Info(log);
                WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            });

            ApiResult result = new ApiResult();
            try
            {
                if (model != null && model.Conveyor.Count > 0)
                {
                    List<tn_dts_equialarmlogs> alarmLogs = new List<tn_dts_equialarmlogs>();//设备异常表
                    List<MongoEquialarmlogs> mogoAlarmLogs = new List<MongoEquialarmlogs>();//设备异常表
                    DeviceRealCollectViewModel _deviceRealCollect = new DeviceRealCollectViewModel();
                    foreach (var conveyor in model.Conveyor)
                    {
                        var cacheDevice = DeviceDriver.Instance.StateList.FirstOrDefault(it => it.deviceCode == conveyor.Code);
                        string errCode = conveyor.ErrCode;
                        string errMsg = conveyor.ErrMsg;
                        if (cacheDevice != null && cacheDevice.errCode != errCode)
                        {
                            //增加到设备异常表
                            alarmLogs.Add(new tn_dts_equialarmlogs()
                            {
                                cn_guid = Guid.NewGuid().ToString(),
                                cn_s_equialarmlogs_errcode = errCode,
                                cn_s_equialarmlogs_errmsg = errMsg,
                                cn_s_equialarmlogs_no = conveyor.Code,
                                cn_s_equialarmlogs_name = conveyor.Name,
                                cn_t_equialarmlogs_timestamp = DateTime.Now
                            });

                            //异常插入到Mongo
                            mogoAlarmLogs.Add(new MongoEquialarmlogs()
                            {
                                errCode = errCode,
                                errMsg = errMsg,
                                deviceCode = conveyor.Code,
                                deviceName = conveyor.Name,
                                timestamp = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                            });
                        }

                        DeviceDriver.Instance.PublishDevices(conveyor.Code, "输送线", "输送线", conveyor.State, errCode, errMsg, DateTime.Now);
                        EQRealCollectModel _EQRealCollectModel = new EQRealCollectModel();
                        _EQRealCollectModel.deviceNo = conveyor.Code;
                        _EQRealCollectModel.deviceName = conveyor.Name;
                        _EQRealCollectModel.deviceType = "输送线";
                        _EQRealCollectModel.onlineStatus = conveyor.State;
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "线体编号", collectObjectVal = conveyor.LineNo, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        if (conveyor.Signal.HasValue && conveyor.Signal.Value)
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "光电信号", collectObjectVal = conveyor.Signal.ToString(), collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        if (conveyor.GoodsList != null && conveyor.GoodsList.Count >= 1)
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "托盘类型", collectObjectVal = conveyor.GoodsList[0].TrayType, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        else
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "托盘类型", collectObjectVal = "", collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        if (conveyor.GoodsList != null && conveyor.GoodsList.Count >= 1)
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "任务号", collectObjectVal = conveyor.GoodsList[0].TaskNo, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        else
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "任务号", collectObjectVal = "", collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        if (conveyor.GoodsList != null && conveyor.GoodsList.Count >= 1)
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "外形检测", collectObjectVal = conveyor.GoodsList[0].ShapeCheck, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        else
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "外形检测", collectObjectVal = "", collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        if (conveyor.GoodsList != null && conveyor.GoodsList.Count >= 1)
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "检测结果", collectObjectVal = conveyor.GoodsList[0].ShapeInfo, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        else
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "检测结果", collectObjectVal = "", collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        if (conveyor.GoodsList != null && conveyor.GoodsList.Count >= 1)
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "托盘扫码", collectObjectVal = conveyor.GoodsList[0].Scancode, collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        else
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "托盘扫码", collectObjectVal = "", collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                        }
                        if (conveyor.GoodsList != null && conveyor.Equitype == 1)
                        {
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "提升层数", collectObjectVal = conveyor.Equilayer.ToString(), collectObjectUnit = "", collectTime = DateTime.Now.ToString() });
                            _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "输送线", collectObjectName = "提升高度", collectObjectVal = conveyor.Equiheight.ToString(), collectObjectUnit = "mm", collectTime = DateTime.Now.ToString() });
                        }
                        _EQRealCollectModel.collectItemCount = _EQRealCollectModel.collectItem.Count.ToString();
                        _deviceRealCollect.eqRealCollect.Add(_EQRealCollectModel);
                    }

                    if (alarmLogs.Count > 0)
                        _AlarmLogsService.BatchAdd(alarmLogs);

                    if (mogoAlarmLogs.Count > 0)
                        MongoDBSingleton.Instance.InsertMany<MongoEquialarmlogs>(mogoAlarmLogs);

                    if (_deviceRealCollect.eqRealCollect.Count > 0)
                    {
                        DeviceRealCollect(_deviceRealCollect);
                    }
                }

                result.ErrCode = 200;
                result.IsSuccess = true;
                result.Message = "成功";
                result.Timestamp = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                result.ErrCode = 500;
                result.IsSuccess = false;
                result.Message = "ConveyorRealCollect接口异常-原因=" + ex.Message;
                result.Timestamp = DateTime.Now.ToString();
            }
            return toResponse(result);
        }
        #endregion

        #region 获取菜单权限
        /// <summary>
        /// 获取菜单权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMenu()
        {
            MenuAuthorityModel menuAuthorityModel = new MenuAuthorityModel();
            try
            {
                menuAuthorityModel = _IDtsService.GetMenu();
            }
            catch (Exception ex)
            {
                tn_dts_logs log = new tn_dts_logs();
                log.cn_guid = Guid.NewGuid().ToString();
                log.cn_s_logs_type = "接口";
                log.cn_s_logs_errorsinfo = "外部调用开放获取菜单权限接口/GetMenu接口异常，异常信息为：" + ex.Message;
                log.cn_t_create = DateTime.Now;
                int resLogs = _ILogsService.Add(log);
                if (resLogs <= 0)
                {
                    LogHelper.Info(DateTime.Now.ToString() + " 外部调用开放获取菜单权限接口/GetMenu接口异常存入tn_dts_logs表失败，异常内容为 " + ex.Message);
                }
            }
            return new JsonResult(menuAuthorityModel);
        }
        #endregion

        #region 获取首页视角区域
        /// <summary>
        /// 获取首页视角区域
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetHomeAngle()
        {
            HomeAngleViewAreaModel homeAngleViewAreaModel = new HomeAngleViewAreaModel();
            try
            {
                homeAngleViewAreaModel = _IDtsService.GetHomeAngle();
            }
            catch (Exception ex)
            {
                tn_dts_logs log = new tn_dts_logs();
                log.cn_guid = Guid.NewGuid().ToString();
                log.cn_s_logs_type = "接口";
                log.cn_s_logs_errorsinfo = "外部调用开放获取首页视角区域接口/GetHomeAngle接口异常，异常信息为：" + ex.Message;
                log.cn_t_create = DateTime.Now;
                int resLogs = _ILogsService.Add(log);
                if (resLogs <= 0)
                {
                    LogHelper.Info(DateTime.Now.ToString() + " 外部调用开放获取首页视角区域接口/GetHomeAngle接口异常存入tn_dts_logs表失败，异常内容为 " + ex.Message);
                }
            }
            return new JsonResult(homeAngleViewAreaModel);
        }
        #endregion

        #region 获取载具上物料详细信息
        /// <summary>
        /// 获取载具上物料详细信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetGoodsInfo(string bitCode, string trayCode, string rockCode, string boxCode)
        {
            UserSession user = GetSessionInfo();
            if (user != null)
            {
                tn_dts_setting trayApiSetting = _ISettingService.GetFirst(it => it.cn_s_setting_keycode == "WMSTrayApi");
                string receiveresult = "";
                ItemRackInfo itemRackInfo = new ItemRackInfo();
                List<ItemRowPlus> itemRowPlusList = new List<ItemRowPlus>();
                RackInfoViewModel rackInfoViewModel = new RackInfoViewModel();
                if (!(trayApiSetting is null))
                {
                    string trayApi = trayApiSetting.cn_s_setting_keyvalue;
                    try
                    {
                        Uri uri = new Uri(trayApi);
                        // 提取基础URL部分（包括协议、主机名和端口）  
                        string baseUrl = uri.Scheme + "://" + uri.Host + ":" + uri.Port;
                        // 提取路径和查询字符串部分  
                        string pathAndQuery = uri.PathAndQuery;
                        ApiResult apiResLocation = new ApiResult();
                        if (!(bitCode is null))
                        {

                        }
                        else if (!(trayCode is null))
                        {

                        }
                        else if (!(rockCode is null))
                        {
                            string strLocationList = WebApiManager.HttpGet(baseUrl, pathAndQuery + $"?locationCode=&trayCode={rockCode}", ref apiResLocation);
                            List<TrayItemInfoWMS> trayItemInfoList = JsonConvert.DeserializeObject<List<TrayItemInfoWMS>>(strLocationList);

                            if (trayItemInfoList is null)
                            {
                                receiveresult = "请求WMS获取容器中的物料信息接口-料架-接口返回为空";
                            }
                            else
                            {
                                foreach (var trayItemInfo in trayItemInfoList)
                                {
                                    rackInfoViewModel.remarks = "";
                                    rackInfoViewModel.rackCode = trayItemInfo.trayCode;
                                    rackInfoViewModel.rackAngle = "";
                                    rackInfoViewModel.ext1 = "";
                                    rackInfoViewModel.ext2 = "";
                                    List<BoxInfoViewModel> boxInfoViewModelList = new List<BoxInfoViewModel>();
                                    if (trayItemInfo.moveLocations is null)
                                    {

                                        receiveresult = "请求WMS获取容器中的物料信息接口-料架-moveLocations返回为空";
                                    }
                                    else
                                    {
                                        foreach (var moveLocation in trayItemInfo.moveLocations)
                                        {
                                            BoxInfoViewModel boxInfoViewModel = new BoxInfoViewModel();
                                            boxInfoViewModel.boxCode = moveLocation.trayCode;
                                            boxInfoViewModel.boxLocation = "";
                                            boxInfoViewModel.storageState = "";
                                            boxInfoViewModel.itemRow = new List<ItemRowRack>();
                                            foreach (var itemState in moveLocation.items)
                                            {
                                                ItemRowRack item = new ItemRowRack();
                                                item.itemCode = itemState.itemCode;
                                                item.itemName = itemState.itemName;
                                                item.Qty = itemState.qty + itemState.unit;
                                                item.bacthNo = itemState.lotCode;
                                                boxInfoViewModel.itemRow.Add(item);
                                            }
                                            boxInfoViewModelList.Add(boxInfoViewModel);
                                        }
                                        receiveresult = "true";
                                    }
                                    rackInfoViewModel.boxInfo = boxInfoViewModelList;
                                }

                            }

                        }
                        else if (!(boxCode is null))
                        {
                            string strLocationList = WebApiManager.HttpGet(baseUrl, pathAndQuery + $"?locationCode=&trayCode={boxCode}", ref apiResLocation);
                            List<TrayItemInfoWMS> trayItemInfoList = JsonConvert.DeserializeObject<List<TrayItemInfoWMS>>(strLocationList);

                            if (trayItemInfoList is null)
                            {
                                receiveresult = "请求WMS获取容器中的物料信息接口-料箱-接口返回为空";
                            }
                            else
                            {
                                //rackInfoViewModel.remarks = "";
                                //rackInfoViewModel.rackCode = "";
                                //rackInfoViewModel.rackAngle = "";
                                //rackInfoViewModel.ext1 = "";
                                //rackInfoViewModel.ext2 = "";
                                //List<BoxInfoViewModel> boxInfoViewModelList = new List<BoxInfoViewModel>();
                                //BoxInfoViewModel boxInfoViewModel = new BoxInfoViewModel();
                                //boxInfoViewModel.boxCode = boxCode;
                                //boxInfoViewModel.boxLocation = "";
                                //boxInfoViewModel.storageState = "";
                                //boxInfoViewModel.itemRow = new List<ItemRowRack>();
                                //foreach (var trayItemInfo in trayItemInfoList)
                                //{
                                //    if (trayItemInfo.items is null)
                                //    {
                                //        receiveresult = "请求WMS获取容器中的物料信息接口-料箱-items返回为空";
                                //    }
                                //    else
                                //    {
                                //        foreach (var itemState in trayItemInfo.items)
                                //        {
                                //            ItemRowRack item = new ItemRowRack();
                                //            item.itemCode = itemState.itemCode;
                                //            item.itemName = itemState.itemName;
                                //            item.Qty = itemState.qty + itemState.unit;
                                //            item.bacthNo = itemState.lotCode;
                                //            boxInfoViewModel.itemRow.Add(item);
                                //        }
                                //        receiveresult = "true";
                                //    }
                                //}
                                //boxInfoViewModelList.Add(boxInfoViewModel);
                                //rackInfoViewModel.boxInfo = boxInfoViewModelList;
                                if (trayItemInfoList.Count > 0)
                                {
                                    foreach (var itemInfo in trayItemInfoList[0].items)
                                    {
                                        ItemRowPlus item = new ItemRowPlus();
                                        item.itemCode = itemInfo.itemCode;
                                        item.itemName = itemInfo.itemName;
                                        item.trayCode = trayItemInfoList[0].trayCode;
                                        item.qty = itemInfo.qty;
                                        item.bacthNo = itemInfo.lotCode;
                                        itemRowPlusList.Add(item);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        receiveresult = "请求WMS_获取容器中的物料信息接口失败，详细信息：" + ex.Message;
                    }
                }

                itemRackInfo.itemRow = itemRowPlusList;
                itemRackInfo.rackInfo = rackInfoViewModel;
                //Newtonsoft.Json.Linq.JObject data = new Newtonsoft.Json.Linq.JObject();
                OpenLog log = new OpenLog()
                {
                    logtype = "接口",
                    clientip = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    receiveurl = "/api/dts/GetGoodsInfo",
                    receiveresult = receiveresult
                };
                UniversallyAddLog(log);

                return new ContentResult
                {
                    Content = JsonConvert.SerializeObject(itemRackInfo),
                    ContentType = "application/json",
                    StatusCode = 200
                };
            }
            else
            {
                return new JsonResult("token is null");
            }
        }
        #endregion
    }
}
