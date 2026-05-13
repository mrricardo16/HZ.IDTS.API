using HZ.IDTSCore.Api.Controllers;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class CameraAreaDriver
    {
        private static readonly CameraAreaDriver instance = new CameraAreaDriver();
        private ICameraAreaService _ICameraAreaService;

        private CameraAreaDriver()
        {

        }

        public static CameraAreaDriver Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 最新相机区域坐标系列表
        /// </summary>
        public List<CameraAreaPoint> LatestCameraAreaPointList = new List<CameraAreaPoint>();
    }
}
