using HZ.IDTSCore.Model.Entity.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    /// <summary>
    /// 通用的设备全局变量
    /// </summary>
    public class DeviceDriver
    {
        private static readonly DeviceDriver instance = new DeviceDriver();

        private DeviceDriver() { }

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static DeviceDriver Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 设备状态集合
        /// </summary>
        //public List<GlobalDevicesViewsModel> StateList = new List<GlobalDevicesViewsModel>();
        private static readonly object _stateListLock = new object();
        private List<GlobalDevicesViewsModel> _stateList = new List<GlobalDevicesViewsModel>();
        public List<GlobalDevicesViewsModel> StateList
        {
            get
            {
                lock (_stateListLock)
                {
                    return _stateList;
                }
            }
            set
            {
                lock (_stateListLock)
                {
                    _stateList = value;
                }
            }
        }

        /// <summary>
        /// 设备采集最新集合
        /// </summary>
        public GlobalRealCollectViewModel RealCollectList = new GlobalRealCollectViewModel();

        /// <summary>
        /// 设备爆炸图当前展示的设备信息
        /// </summary>
        public DeviceExplosionModel DExplosion = new DeviceExplosionModel();

        /// <summary>
        /// 设备刷新内存
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="deviceName">设备名称</param>
        /// <param name="deviceType">设备类型</param>
        /// <param name="deviceState">设备状态</param>
        /// <param name="errCode">异常代码</param>
        /// <param name="errMsg">异常说明</param>
        /// <param name="publishTimestamp">更新时间戳</param>
        /// <returns></returns>
        public bool PublishDevices(string deviceCode, string deviceName, string deviceType, string deviceState, string errCode, string errMsg, DateTime publishTimestamp)
        {
            bool result = false;
            try
            {
                var device = StateList.FirstOrDefault(p => p.deviceCode == deviceCode);
                if (device == null)
                {
                    StateList.Add(new GlobalDevicesViewsModel()
                    {
                        deviceCode = deviceCode,
                        deviceName = deviceName,
                        deviceType = deviceType,
                        deviceState = deviceState,
                        errCode = errCode,
                        errMsg = errMsg,
                        publishTimestamp = publishTimestamp
                    });
                }
                else
                {
                    device.deviceName = deviceName;
                    device.deviceState = deviceState;
                    device.errCode = errCode;
                    device.errMsg = errMsg;
                    device.publishTimestamp = publishTimestamp;
                }


                result = true;
            }
            catch { }

            return result;
        }


        private readonly Dictionary<string, GlobalDevicesViewsModel> _stateMap = new Dictionary<string, GlobalDevicesViewsModel>();

        /// <summary>
        /// 优化后的设备刷新内存方法，减少对内存的查询次数
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="deviceName"></param>
        /// <param name="deviceType"></param>
        /// <param name="deviceState"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <param name="publishTimestamp"></param>
        /// <returns></returns>
        public bool PublishDevicesV2(
        string deviceCode,
        string deviceName,
        string deviceType,
        string deviceState,
        string errCode,
        string errMsg,
        DateTime publishTimestamp)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceCode))
                {
                    return false;
                }

                lock (_stateListLock)
                {
                    if (!_stateMap.TryGetValue(deviceCode, out var device))
                    {
                        device = new GlobalDevicesViewsModel()
                        {
                            deviceCode = deviceCode,
                            deviceName = deviceName,
                            deviceType = deviceType,
                            deviceState = deviceState,
                            errCode = errCode,
                            errMsg = errMsg,
                            publishTimestamp = publishTimestamp
                        };

                        _stateList.Add(device);
                        _stateMap.Add(deviceCode, device);
                    }
                    else
                    {
                        device.deviceName = deviceName;
                        device.deviceType = deviceType;
                        device.deviceState = deviceState;
                        device.errCode = errCode;
                        device.errMsg = errMsg;
                        device.publishTimestamp = publishTimestamp;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        public GlobalDevicesViewsModel GetDevice(string deviceCode)
        {
            return this.StateList.FirstOrDefault(p => p.deviceCode == deviceCode);
        }


        /// <summary>
        /// 刷新内存设备采集数据
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <returns></returns>
        public bool PublishRealCollect(GlobalRealCollectViewModel deviceModel)
        {
            bool result = false;
            try
            {
                foreach (var device in deviceModel.eqRealCollect)
                {

                    var memoryDevice = RealCollectList.eqRealCollect.FirstOrDefault(p => p.deviceNo == device.deviceNo);
                    if (memoryDevice == null)
                    {
                        int emptyStringNumber = device.collectItem.Where(it => it.collectObjectVal == "").Count();
                        device.collectItem.RemoveAll(it => it.collectObjectVal == "");
                        if(!(device.collectItemCount is null))
                        {
                            device.collectItemCount = (int.Parse(device.collectItemCount) - emptyStringNumber).ToString();
                        }
                        //内存不存在这个设备
                        RealCollectList.eqRealCollect.Add(device);
                    }
                    else
                    {
                        //刷新内存设备状态
                        memoryDevice.onlineStatus = device.onlineStatus;
                        foreach (var item in device.collectItem)
                        {
                            var memoryItem = memoryDevice.collectItem.FirstOrDefault(p => p.collectItemName == item.collectItemName && p.collectObjectName == item.collectObjectName);

                            //if (memoryItem == null)
                            //{
                            //    //采集项不在内存中
                            //    memoryDevice.collectItem.Add(item);
                            //}
                            //else
                            //{
                            //    //刷新内存的采集项
                            //    memoryItem.collectTime = item.collectTime;
                            //    memoryItem.collectObjectVal = item.collectObjectVal;
                            //    memoryItem.collectObjectUnit = item.collectObjectUnit;
                            //}

                            if (memoryItem == null)
                            {
                                //采集项不在内存中
                                if (item.collectObjectVal != "")
                                {
                                    memoryDevice.collectItem.Add(item);
                                }
                            }
                            else
                            {
                                if (item.collectObjectVal == "")
                                {
                                    memoryDevice.collectItem.RemoveAll(it => it.collectItemName == item.collectItemName && it.collectObjectName == item.collectObjectName);
                                    continue;
                                }
                                //刷新内存的采集项
                                memoryItem.collectTime = item.collectTime;
                                memoryItem.collectObjectVal = item.collectObjectVal;
                                memoryItem.collectObjectUnit = item.collectObjectUnit;
                            }
                        }
                        memoryDevice.collectItemCount = memoryDevice.collectItem.Count.ToString();
                    }
                }
                result = true;
            }
            catch { }
            return result;
        }
    }
}
