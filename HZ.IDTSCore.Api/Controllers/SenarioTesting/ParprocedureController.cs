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
    public class ParprocedureController : BaseController
    {
        private IParprocedureService _IParprocedureService;
        private IEquipmentService _IEquipmentService;
        private IChiprocedureService _IChiprocedureService;
        private IEquicommandService _IEquicommandService;
        private IGoodsequipmentService _IGoodsequipmentService;
        private IChiequipmentService _IChiequipmentService;

        public ParprocedureController()
        {
            _IParprocedureService = ServiceLocator.GetService<IParprocedureService>(HttpContextSession());
            _IEquipmentService = ServiceLocator.GetService<IEquipmentService>(HttpContextSession());
            _IChiprocedureService = ServiceLocator.GetService<IChiprocedureService>(HttpContextSession());
            _IEquicommandService = ServiceLocator.GetService<IEquicommandService>(HttpContextSession());
            _IGoodsequipmentService = ServiceLocator.GetService<IGoodsequipmentService>(HttpContextSession());
            _IChiequipmentService = ServiceLocator.GetService<IChiequipmentService>(HttpContextSession());
        }

        #region 按流程编码和流程名称分页模糊查询
        /// <summary>
        /// 按流程编码和流程名称分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IParprocedureService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 按设备类型、设备编码和设备名称混合模糊分页查询设备信息
        /// <summary>
        /// 按设备类型、设备编码和设备名称混合模糊分页查询设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquipment(PageParm param)
        {
            var res = _IEquipmentService.GetEquipment(param);
            return toResponse(res);
        }
        #endregion

        #region 按（立库/地堆）编码或（立库/地堆）名称混合模糊分页查询（立库/地堆）信息
        /// <summary>
        /// 按（立库/地堆）编码或（立库/地堆）名称混合模糊分页查询（立库/地堆）信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetStockSite(PageParm param)
        {
            var res = _IParprocedureService.GetStockSite(param);
            return toResponse(res);
        }
        #endregion

        #region 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息和终点信息）
        /// <summary>
        /// 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息和终点信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllChisceneByParsceneguid(PageParm param)
        {
            var res = _IChiprocedureService.GetAllChiprocedureByParprocedureguid(param);
            return toResponse(res);
        }
        #endregion

        #region 保存流程
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="saveProcedureDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveProcedure(SaveProcedureDate saveProcedureDate)
        {
            ReturnMessage res = _IParprocedureService.SaveProcedure(saveProcedureDate);
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

        #region 批量删除流程
        /// <summary>
        /// 批量删除流程
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteProcedure([FromBody] List<string> guidList)
        {
            ReturnMessage res = _IParprocedureService.DeleteProcedure(guidList);
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

        #region 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息、终点信息和修改创建信息）
        /// <summary>
        /// 通过流程主表唯一标识获取所有流程子表信息（含设备信息、起点信息、终点信息和修改创建信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllChiscenePlusByParsceneguid(PageParm param)
        {
            var res = _IChiprocedureService.GetAllChiprocedurePlusByParprocedureguid(param);
            return toResponse(res);
        }
        #endregion

        #region 按有无通配符、指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// <summary>
        /// 按有无通配符、指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquicommandPlusInformationPages(PageParm param)
        {
            var res = _IEquicommandService.GetEquicommandPlusInformationPages(param);
            return toResponse(res);
        }
        #endregion

        #region 按货位设备、指令编码和指令名称混合模糊分页查询
        /// <summary>
        /// 按货位设备、指令编码和指令名称混合模糊分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetGoodscommandInformationPages(PageParm param)
        {
            var res = _IGoodsequipmentService.GetGoodscommandInformationPages(param);
            return toResponse(res);
        }
        #endregion

        #region 获取所有货位设备信息
        /// <summary>
        /// 获取所有货位设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllGoodsequipmentInformation(PageParm param)
        {
            var res = _IGoodsequipmentService.GetAllGoodsequipmentInformation(param);
            return toResponse(res);
        }
        #endregion

        #region 保存设备（配置）
        /// <summary>
        /// 保存设备（配置）
        /// </summary>
        /// <param name="saveEquipmentDate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveEquipment(SaveEquipmentDate saveEquipmentDate)
        {
            ReturnMessage res = _IChiequipmentService.SaveEquipment(saveEquipmentDate);
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

        #region 通过流程子表唯一标识获取所有设备子表信息（含指令信息）
        /// <summary>
        /// 通过流程子表唯一标识获取所有设备子表信息（含指令信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllChiequipmentByChiprocedureguid(PageParm param)
        {
            var res = _IChiequipmentService.GetAllChiequipmentByChiprocedureguid(param);
            return toResponse(res);
        }
        #endregion

        #region 通过流程子表唯一标识获取所有设备子表信息（含指令信息和修改创建信息）
        /// <summary>
        /// 通过流程子表唯一标识获取所有设备子表信息（含指令信息和修改创建信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllChiequipmentPlusByChiprocedureguid(PageParm param)
        {
            var res = _IChiequipmentService.GetAllChiequipmentPlusByChiprocedureguid(param);
            return toResponse(res);
        }
        #endregion
    }
}
 