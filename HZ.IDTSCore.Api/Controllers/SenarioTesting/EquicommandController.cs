using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.SenarioTesting
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class EquicommandController : BaseController
    {
        private IEquicommandService _IEquicommandService;
        private IEquipmentService _IEquipmentService;

        public EquicommandController()
        {
            _IEquicommandService = ServiceLocator.GetService<IEquicommandService>(HttpContextSession());
            _IEquipmentService = ServiceLocator.GetService<IEquipmentService>(HttpContextSession());
        }

        #region 获取设备类型-设备树信息（按设备编号和设备名称混合模糊）
        /// <summary>
        ///  获取设备类型-设备树信息（按设备编号和设备名称混合模糊）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquipmentTypeTree(PageParm param)
        {
            var res = _IEquipmentService.GetEquipmentTypeTree(param);
            return toResponse(res);
        }
        #endregion

        #region 按设备唯一标识、指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// <summary>
        /// 按设备唯一标识、指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquicommandService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 获取一个设备类型下所有设备信息
        /// <summary>
        /// 获取一个设备类型下所有设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquipmentInformationByEquitype(PageParm param)
        {
            var res = _IEquipmentService.GetEquipmentInformationByEquitype(param);
            return toResponse(res);
        }
        #endregion

        #region 批量添加设备指令
        /// <summary>
        /// 批量添加设备指令
        /// </summary>
        /// <param name="batchAddDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult BatchAddEquicommand(BatchAddEquicommandDate batchAddDate)
        {
            ReturnMessage res = _IEquicommandService.BatchAddEquicommand(batchAddDate);
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

        #region 修改设备指令
        /// <summary>
        /// 修改设备指令
        /// </summary>
        /// <param name="equicommand"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UpdateEquicommand(tn_dts_equicommand equicommand)
        {
            ReturnMessage res = _IEquicommandService.UpdateEquicommand(equicommand);
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

        #region 编辑模版
        /// <summary>
        /// 编辑模版
        /// </summary>
        /// <param name="editEquicommandDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditEquicommandJson(EditEquicommandDate editEquicommandDate)
        {
            ReturnMessage res = _IEquicommandService.EditEquicommandJson(editEquicommandDate);
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

        #region 按指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// <summary>
        /// 按指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquicommandInformationPages(PageParm param)
        {
            var res = _IEquicommandService.GetEquicommandInformationPages(param);
            return toResponse(res);
        }
        #endregion

        #region 复制设备指令
        /// <summary>
        /// 复制设备指令
        /// </summary>
        /// <param name="copyEquicommandDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CopyEquicommand(CopyEquicommandDate copyEquicommandDate)
        {
            ReturnMessage res = _IEquicommandService.CopyEquicommand(copyEquicommandDate);
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

        #region 批量删除设备指令
        /// <summary>
        /// 批量删除设备指令
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteEquicommand([FromBody] List<string> guidList)
        {
            ReturnMessage res = _IEquicommandService.DeleteEquicommand(guidList);
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
    }
}
