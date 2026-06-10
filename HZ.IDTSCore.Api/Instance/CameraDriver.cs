using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Api.Controllers;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class CameraDriver : BaseController
    {
        private static readonly CameraDriver instance = new CameraDriver();
        private ICameraService _ICameraService;

        private CameraDriver()
        {
            _ICameraService = ServiceLocator.GetService<ICameraService>(HttpContextSession());
        }

        public static CameraDriver Instance
        {
            get
            {
                return instance;
            }
        }

        public async void SendCamera()
        {
            ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
            string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
            string virtualCameraNumberString = new Interfaces.Service.Sys.SettingService(new DbHelper.SessionInfo()
            {
                token = "",
                splitDbCode = ""
            }).GetFirst(it => it.cn_s_setting_keycode == "VirtualCameraNumber").cn_s_setting_keyvalue;
            if (virtualCameraNumberString is null)
            {
                LogHelper.Info("tn_dts_setting表中没有关键字编码为VirtualCameraNumber的记录或其对应的设置值为空。");
                virtualCameraNumberString = "10";
            }
            int virtualCameraNumber = int.Parse(virtualCameraNumberString);
            for (int i = 0; i < virtualCameraNumber; i++)
            {
                WebSocketServer.SessionInstance.Instance.PLCSendAllV2(sendJSONString);
                Thread.Sleep(1000);
            }            
        }
    } 
}
