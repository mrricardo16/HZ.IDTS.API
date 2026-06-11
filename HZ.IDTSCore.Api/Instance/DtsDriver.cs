using HZ.CommonUtil.Helpers;
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
        private const int AgvRealMonitorSlowLogMilliseconds = 500;
        private static readonly System.Threading.SemaphoreSlim _agvWebSocketSendLock = new System.Threading.SemaphoreSlim(1, 1);
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> AgvRealCollectItemValueCacheV2 = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> AgvRealCollectDeviceMetaCacheV2 = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
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

        #region AGV实时采集Mongo最新值批量Upsert构建 - 2026-06-10
        /// <summary>
        /// 构建AGV实时采集Mongo最新值批量Upsert操作。
        /// 优化内容：AgvRealMonitor当前走DtsDriver独立调用链，不能复用DtsController里的私有优化方法；
        /// 这里单独为AGV链路构建BulkWrite数据，把原来每台AGV一次FindOneFilter加一次Add/Update，改为一次批量提交。
        /// </summary>
        private static List<WriteModel<MongoRealCollect>> BuildAgvMongoRealCollectBulkWritesV2(List<EQRealCollectModel> devices)
        {
            List<WriteModel<MongoRealCollect>> writeModels = new List<WriteModel<MongoRealCollect>>();
            if (devices == null || devices.Count == 0)
            {
                return writeModels;
            }

            // 同一批AGV如果重复上报同一个deviceNo，以最后一条为准，避免BulkWrite里重复更新同一台车。
            Dictionary<string, EQRealCollectModel> latestDeviceMap = new Dictionary<string, EQRealCollectModel>();
            foreach (var device in devices)
            {
                if (device == null || string.IsNullOrWhiteSpace(device.deviceNo))
                {
                    continue;
                }
                latestDeviceMap[device.deviceNo] = device;
            }

            foreach (var device in latestDeviceMap.Values)
            {
                var filter = Builders<MongoRealCollect>.Filter.Eq(o => o.deviceNo, device.deviceNo);
                var update = Builders<MongoRealCollect>.Update
                    .Set(o => o.deviceNo, device.deviceNo)
                    .Set(o => o.deviceName, device.deviceName)
                    .Set(o => o.deviceType, device.deviceType)
                    .Set(o => o.onlineStatus, device.onlineStatus)
                    .Set(o => o.collectItemCount, device.collectItemCount)
                    .Set(o => o.collectItem, device.collectItem);

                writeModels.Add(new UpdateOneModel<MongoRealCollect>(filter, update)
                {
                    // Mongo中不存在该AGV最新值时自动插入，存在则更新，保持原Add/Update语义。
                    IsUpsert = true
                });
            }

            return writeModels;
        }
        #endregion

        #region AGV采集项变更过滤V2 - 2026-06-11
        /// <summary>
        /// 判断AGV采集项值是否变化。
        /// 时间：2026-06-11
        /// 优化内容：500ms高频上报时，不变采集项不再重复写SQL和Mongo，降低AGV链路数据库压力。
        /// </summary>
        private static bool IsAgvRealCollectItemChangedV2(EQRealCollectModel device, CollectItemModel item)
        {
            if (device == null || item == null || string.IsNullOrWhiteSpace(device.deviceNo))
            {
                return true;
            }

            string keyV2 = BuildAgvRealCollectItemCacheKeyV2(device, item);
            string newSignatureV2 = BuildAgvRealCollectItemValueSignatureV2(item);
            bool changedV2 = false;
            AgvRealCollectItemValueCacheV2.AddOrUpdate(keyV2,
                key =>
                {
                    changedV2 = true;
                    return newSignatureV2;
                },
                (key, oldSignature) =>
                {
                    if (oldSignature == newSignatureV2)
                    {
                        return oldSignature;
                    }

                    changedV2 = true;
                    return newSignatureV2;
                });
            return changedV2;
        }

        /// <summary>
        /// 判断AGV设备基础信息是否变化。基础信息变化时需要刷新Mongo整台设备最新值。
        /// </summary>
        private static bool IsAgvRealCollectDeviceMetaChangedV2(EQRealCollectModel device)
        {
            if (device == null || string.IsNullOrWhiteSpace(device.deviceNo))
            {
                return true;
            }

            string newSignatureV2 = (device.deviceName ?? string.Empty) + "\u001f"
                + (device.deviceType ?? string.Empty) + "\u001f"
                + (device.onlineStatus ?? string.Empty) + "\u001f"
                + (device.collectItemCount ?? string.Empty);
            bool changedV2 = false;
            AgvRealCollectDeviceMetaCacheV2.AddOrUpdate(device.deviceNo,
                key =>
                {
                    changedV2 = true;
                    return newSignatureV2;
                },
                (key, oldSignature) =>
                {
                    if (oldSignature == newSignatureV2)
                    {
                        return oldSignature;
                    }

                    changedV2 = true;
                    return newSignatureV2;
                });
            return changedV2;
        }

        private static string BuildAgvRealCollectItemCacheKeyV2(EQRealCollectModel device, CollectItemModel item)
        {
            return (device.deviceNo ?? string.Empty) + "\u001f"
                + (item.collectItemName ?? string.Empty) + "\u001f"
                + (item.collectObjectName ?? string.Empty);
        }

        private static string BuildAgvRealCollectItemValueSignatureV2(CollectItemModel item)
        {
            return (item.collectObjectVal ?? string.Empty) + "\u001f" + (item.collectObjectUnit ?? string.Empty);
        }
        #endregion

        #region AGV异常信息解析 - 2026-06-10
        /// <summary>
        /// 解析AGV异常信息。
        /// 优化内容：原代码每台AGV直接Split两次并强制取[1]，当第三方偶发传空或不含#时会抛异常；
        /// 这里保持“编码#描述”的兼容格式，同时增加兜底，避免单台AGV异常数据导致整个接口失败。
        /// </summary>
        private static void ParseAgvErrorMessageV2(string rawErrMsg, out string errCode, out string errMsg)
        {
            errCode = "";
            errMsg = "";
            if (string.IsNullOrEmpty(rawErrMsg))
            {
                return;
            }

            string[] parts = rawErrMsg.Split(new char[] { '#' }, 2);
            errCode = parts.Length > 0 ? parts[0] : "";
            errMsg = parts.Length > 1 ? parts[1] : rawErrMsg;
        }
        #endregion

        #region AGV WebSocket后台推送 - 2026-06-10
        /// <summary>
        /// AGV WebSocket后台推送。
        /// 优化内容：AgvRealMonitor以高频上报时，WebSocket推送不能阻塞HTTP接口；
        /// 如果上一批还未发送完成，说明客户端或网络已经偏慢，本轮跳过推送，避免后台任务堆积。
        /// </summary>
        private static async Task SendAgvWebSocketAsync(string sendJSONString, int agvCount)
        {
            bool hasLock = false;
            try
            {
                hasLock = await _agvWebSocketSendLock.WaitAsync(0);
                if (!hasLock)
                {
                    LogHelper.Info("AgvRealMonitor WebSocket上一批仍未完成，本次跳过推送，AGV数量：" + agvCount);
                    return;
                }

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                await WebSocketServer.SessionInstance.Instance.PLCSendAllV2(sendJSONString);
                if (stopwatch.ElapsedMilliseconds >= AgvRealMonitorSlowLogMilliseconds)
                {
                    LogHelper.Info("AgvRealMonitor WebSocket推送耗时：" + stopwatch.ElapsedMilliseconds + "ms，AGV数量：" + agvCount);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("AgvRealMonitor WebSocket推送异常：" + ex.Message, ex);
            }
            finally
            {
                if (hasLock)
                {
                    _agvWebSocketSendLock.Release();
                }
            }
        }
        #endregion

        public async Task<IActionResult> AgvRealMonitorSynchronous(AgvRealMonitorViewModel model)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            string sendJSONString = JsonConvert.SerializeObject(model);
            //var res = WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            _ = SendAgvWebSocketAsync(sendJSONString, model == null || model.Agv == null ? 0 : model.Agv.Count);
            ApiResult result = new ApiResult();
            try
            {
                if (model != null && model.Agv != null)
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
                    DateTime collectNow = DateTime.Now;
                    string collectTime = collectNow.ToString();
                    foreach (var device in model.Agv)
                    {
                        if (device == null)
                        {
                            continue;
                        }

                        var cacheDevice = DeviceDriver.Instance.GetDevice(device.carCode);
                        ParseAgvErrorMessageV2(device.errMsg, out string errCode, out string errMsg);
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
                                cn_t_equialarmlogs_timestamp = collectNow
                            });

                            //异常插入到Mongo
                            mogoAlarmLogs.Add(new MongoEquialarmlogs()
                            {
                                errCode = errCode,
                                errMsg = errMsg,
                                deviceCode = device.carCode,
                                deviceName = device.carName,
                                timestamp = DateTime.SpecifyKind(collectNow, DateTimeKind.Utc)
                            });
                        }

                        //刷新内存设备状态
                        DeviceDriver.Instance.PublishDevicesV2(device.carCode, device.carName, "AGV", device.onlineState, errCode, errMsg, collectNow);


                        #region 增加到设备采集信息
                        EQRealCollectModel _EQRealCollectModel = new EQRealCollectModel();
                        _EQRealCollectModel.deviceNo = device.carCode;
                        _EQRealCollectModel.deviceName = device.carName;
                        _EQRealCollectModel.deviceType = "AGV";
                        _EQRealCollectModel.onlineStatus = device.onlineState;
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "X坐标", collectObjectVal = device.x, collectObjectUnit = "mm", collectTime = collectTime });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "Y坐标", collectObjectVal = device.y, collectObjectUnit = "mm", collectTime = collectTime });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "电量", collectObjectVal = device.power.ToString(), collectObjectUnit = "ah", collectTime = collectTime });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "角度", collectObjectVal = device.angle.ToString(), collectObjectUnit = "", collectTime = collectTime });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "速度", collectObjectVal = device.speed.ToString(), collectObjectUnit = "s", collectTime = collectTime });
                        _EQRealCollectModel.collectItem.Add(new CollectItemModel() { collectItemName = "AGV", collectObjectName = "状态", collectObjectVal = errMsg, collectObjectUnit = "", collectTime = collectTime });
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

                    if (stopwatch.ElapsedMilliseconds >= AgvRealMonitorSlowLogMilliseconds)
                    {
                        LogHelper.Info("AgvRealMonitor接口总耗时：" + stopwatch.ElapsedMilliseconds
                            + "ms，AGV数量：" + model.Agv.Count
                            + "，采集设备数量：" + _deviceRealCollect.eqRealCollect.Count
                            + "，报警日志数量：" + alarmLogs.Count);
                    }
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

                int logCapacityV2 = 0;
                foreach (var device in model.eqRealCollect)
                {
                    logCapacityV2 += device?.collectItem?.Count ?? 0;
                }

                List<tn_dts_equireallogs> logs = new List<tn_dts_equireallogs>(logCapacityV2);//采集记录表，只保留本次发生变化的采集项
                List<EQRealCollectModel> changedMongoDevicesV2 = new List<EQRealCollectModel>();
                List<tn_dts_equialarmlogs> alarmLogs = new List<tn_dts_equialarmlogs>();//设备异常表
                List<MongoEquialarmlogs> mogoAlarmLogs = new List<MongoEquialarmlogs>();//设备异常表
                DateTime fallbackCollectTimeV2 = DateTime.Now;
                foreach (var device in model.eqRealCollect)
                {
                    if (device == null)
                    {
                        continue;
                    }

                    bool deviceMetaChangedV2 = IsAgvRealCollectDeviceMetaChangedV2(device);
                    bool deviceHasChangedItemV2 = false;
                    if (device.collectItem == null || device.collectItem.Count == 0)
                    {
                        if (deviceMetaChangedV2)
                        {
                            changedMongoDevicesV2.Add(device);
                        }
                        continue;
                    }

                    foreach (var item in device.collectItem)
                    {
                        if (item == null)
                        {
                            continue;
                        }

                        bool itemChangedV2 = IsAgvRealCollectItemChangedV2(device, item);
                        if (itemChangedV2)
                        {
                            deviceHasChangedItemV2 = true;
                            DateTime collectTimeV2;
                            if (!DateTime.TryParse(item.collectTime, out collectTimeV2))
                            {
                                collectTimeV2 = fallbackCollectTimeV2;
                            }

                            logs.Add(new tn_dts_equireallogs()
                            {
                                cn_guid = Guid.NewGuid().ToString(),
                                cn_s_equireallogs_no = device.deviceNo,
                                cn_s_equireallogs_name = device.deviceName,
                                cn_t_equireallogs_timestamp = collectTimeV2,
                                cn_s_equireallogs_itemname = item.collectItemName,
                                cn_s_equireallogs_objectname = item.collectObjectName,
                                cn_s_equireallogs_objectval = item.collectObjectVal,
                                cn_s_equireallogs_objectvalunit = item.collectObjectUnit
                            });
                        }
                        else
                        {
                            // 2026-06-11优化：未变化采集项只刷新内存，不再重复写SQL/Mongo。
                        }

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

                    if (deviceMetaChangedV2 || deviceHasChangedItemV2)
                    {
                        // Mongo存整台AGV最新值：发生变化时仍用完整device刷新，避免Mongo最新值丢字段。
                        changedMongoDevicesV2.Add(device);
                    }
                    // 2026-06-10 AGV链路优化：Mongo最新值不再逐台FindOneFilter + Add/Update。
                    // 2026-06-11优化：只有变更设备才会进入BulkWrite，未变化设备不再每500ms重复写Mongo。
                }

                #region AGV Mongo最新值批量Upsert - 2026-06-10
                List<WriteModel<MongoRealCollect>> mongoRealCollectWrites = BuildAgvMongoRealCollectBulkWritesV2(changedMongoDevicesV2);
                if (mongoRealCollectWrites.Count > 0)
                {
                    // 原逻辑每台AGV先查Mongo再新增/更新，150台AGV最多约300次Mongo往返。
                    // BulkWriteV2把每台AGV不同的最新值合并为一次批量提交，保持原有Upsert语义。
                    MongoDBSingleton.Instance.BulkWriteV2<MongoRealCollect>(mongoRealCollectWrites, isOrdered: false);
                }
                #endregion
                // 2026-06-11优化：AGV链路也改为同步批量Upsert采集最新值。
                // 原来这里仍然调用BatchAdd，会导致AgvRealMonitor高频上报时tn_dts_equireallogs持续新增。
                // 现在按“设备编号+采集事项+采集对象”更新最新值，不再为每次采集产生历史明细。
                if (logs.Count > 0)
                {
                    ApiResult realLogsResultV2 = _RealLogsService.BatchUpsertLatestV2(logs);
                    if (realLogsResultV2 == null || !realLogsResultV2.IsSuccess)
                    {
                        throw new Exception("AGV SQL采集最新值Upsert失败：" + (realLogsResultV2 == null ? "" : realLogsResultV2.Message));
                    }
                }

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
