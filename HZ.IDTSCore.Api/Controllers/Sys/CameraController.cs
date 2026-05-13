using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class CameraController : BaseController
    {
        private ICameraService _ICameraService;

        public CameraController()
        {
            _ICameraService = ServiceLocator.GetService<ICameraService>(HttpContextSession());
        }

        #region 新增相机
        /// <summary>
        /// 新增相机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddCamera([FromBody] tn_dts_camera model)
        {
            ReturnMessage res = _ICameraService.AddCamera(model);
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
            //ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
            //string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
            //WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
            return toResponse(resultReturn);
        }
        #endregion

        #region 按相机名称分页模糊查询
        /// <summary>
        /// 按相机名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm parm)
        {
            var res = _ICameraService.GetListPages(parm);
            return toResponse(res);
        }
        #endregion

        #region 修改相机
        /// <summary>
        /// 修改相机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateCamera([FromBody] tn_dts_camera model)
        {
            ReturnMessage res = _ICameraService.UpdateCamera(model);
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
            //ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
            //string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
            //WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
            return toResponse(resultReturn);
        }
        #endregion

        #region 批量删除相机
        /// <summary>
        /// 批量删除相机
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteCamera([FromBody] List<string> model)
        {
            ReturnMessage res = _ICameraService.DeleteCamera(model);
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
            //ReturnVirtualCamera returnVirtualCamera = _ICameraService.GetAllVirtualCamera();
            //string sendJSONString = JsonConvert.SerializeObject(returnVirtualCamera);
            //WebSocketServer.SessionInstance.Instance.PLCSendAll(sendJSONString);
            Task.Run(async () => { CameraDriver.Instance.SendCamera(); });
            return toResponse(resultReturn);
        }
        #endregion

    }
}
