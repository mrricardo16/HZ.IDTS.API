using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Equipment
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    //[Authorization]
    public class EquirepairController : BaseController
    {
        private IEquirepairService _IEquirepairService;
        private IEquipmentService _IEquipmentService;

        public EquirepairController()
        {
            _IEquirepairService = ServiceLocator.GetService<IEquirepairService>(HttpContextSession());
            _IEquipmentService = ServiceLocator.GetService<IEquipmentService>(HttpContextSession());
        }

        #region 按cn_s_equirepair_category、cn_s_equirepair_date、cn_s_equi_parttype、cn_s_equirepair_name(模糊)分页查询
        /// <summary>
        /// 按cn_s_equirepair_category、cn_s_equirepair_date、cn_s_equi_parttype、cn_s_equirepair_name(模糊)分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquirepairService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 新增
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([FromBody] tn_dts_equirepair model)
        {
            var first = _IEquirepairService.GetFirst(x => x.cn_s_equirepair_no == model.cn_s_equirepair_no);
            if (first != null)
            {
                return toResponse(StatusCodeType.AppMessage, "设备编号不能重复！");
            }
            else
            {
                UserSession user = GetSessionInfo();
                model.cn_s_creator = user.UserCode;
                model.cn_s_creator_by = user.UserName;
                model.cn_t_create = DateTime.Now;
                var res = _IEquirepairService.Add(model);
                return toResponse(res);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Update([FromBody] tn_dts_equirepair model)
        {
            tn_dts_equirepair itemGuid = _IEquirepairService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));


            tn_dts_equirepair itemNo = _IEquirepairService.GetFirst(o => o.cn_s_equirepair_no == model.cn_s_equirepair_no && o.cn_guid != model.cn_guid);
            if (itemNo != null)
            {
                return toResponse(StatusCodeType.ParameterError, "设备编号不能重复！");
            }

            tn_dts_equirepair itemName = _IEquirepairService.GetFirst(o => o.cn_s_equirepair_name == model.cn_s_equirepair_name && o.cn_guid != model.cn_guid);
            if (itemName != null)
            {
                return toResponse(StatusCodeType.ParameterError, "设备名换不能重复！");
            }
            itemGuid.cn_s_equirepair_no = model.cn_s_equirepair_no;
            itemGuid.cn_s_equirepair_name = model.cn_s_equirepair_name;
            itemGuid.cn_s_equirepair_category = model.cn_s_equirepair_category;
            itemGuid.cn_s_equirepair_item = model.cn_s_equirepair_item;
            itemGuid.cn_s_equirepair_cause = model.cn_s_equirepair_cause;
            itemGuid.cn_s_equirepair_solution = model.cn_s_equirepair_solution;
            itemGuid.cn_s_equirepair_man = model.cn_s_equirepair_man;
            itemGuid.cn_s_equirepair_phone = model.cn_s_equirepair_phone;
            itemGuid.cn_s_equirepair_result = model.cn_s_equirepair_result;
            itemGuid.cn_s_equirepair_date = model.cn_s_equirepair_date;
            itemGuid.cn_d_equirepair_cost = model.cn_d_equirepair_cost;
            itemGuid.cn_s_equirepair_material = model.cn_s_equirepair_material;
            itemGuid.cn_s_equirepair_change_no = model.cn_s_equirepair_change_no;
            itemGuid.cn_s_equirepair_change_name = model.cn_s_equirepair_change_name;
            itemGuid.cn_s_equirepair_remarks = model.cn_s_equirepair_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _IEquirepairService.Update(itemGuid);
            ApiResult result = new ApiResult();
            if (res > 0)
            {
                result.IsSuccess = true;
                result.StatusCode = 200;
            }
            else
            {
                result.IsSuccess = false;
                result.StatusCode = 500;
                result.Message = "无影响行数";
            }

            return new JsonResult(result);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="cn_s_guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(string[] cn_s_guid)
        {
            ApiResult result = _IEquirepairService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        #region 获取设备树
        /// <summary>
        /// 获取设备树
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquipmentTree(PageParm param)
        {
            var res = _IEquipmentService.GetCompletemachineTreeList(param);
            return toResponse(res);
        }
        #endregion

        #region 保存设备维修
        /// <summary>
        /// 保存设备维修
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveDataRepair(SaveData_Repair saveData)
        {
            ReturnMessage res = _IEquirepairService.SaveDataRepair(saveData);
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

        #region 删除设备维修
        /// <summary>
        /// 删除设备维修
        /// </summary>
        /// <param name="equirepair"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteDataRepair(List<tn_dts_equirepair> equirepairList)
        {
            ReturnMessage res = _IEquirepairService.DeleteDataRepair(equirepairList);
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

        //#region 获取所有维修事项名称
        ///// <summary>
        ///// 获取所有维修事项名称
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult GetAllRepairMatters()
        //{
        //    List<MatterInfo> result = _IEquirepairService.GetAllRepairMatters();
        //    return toResponse(result);
        //}
        //#endregion

        //#region 获取所有故障原因解决方案对
        ///// <summary>
        ///// 获取所有故障原因解决方案对
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public IActionResult GetAllFaultSolutionPairs()
        //{
        //    List<FaultSolutionPairInfo> result = _IEquirepairService.GetAllFaultSolutionPairs();
        //    return toResponse(result);
        //}
        //#endregion

        #region 查看详情
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Detail([FromBody] string guid)
        {
            tn_dts_equirepair result = _IEquirepairService.Detail(guid);
            return toResponse(result);
        }
        #endregion

        #region 删除故障原因
        /// <summary>
        /// 删除故障原因
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteReason([FromBody] string guid)
        {
            ReturnMessage res = _IEquirepairService.DeleteReason(guid);
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

        #region 按事项名称分页模糊查询
        /// <summary>
        /// 按事项名称分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetMatterPageList(PageParm param)
        {
            var res = _IEquirepairService.GetMatterInfoPageList(param);
            return toResponse(res);
        }
        #endregion

        #region 删除维修项目
        /// <summary>
        /// 删除维修项目
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteMatter([FromBody] string guid)
        {
            ReturnMessage res = _IEquirepairService.DeleteMatter(guid);
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

        #region 按故障原因分页模糊查询
        /// <summary>
        /// 按故障原因分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetReasonPageList(PageParm param)
        {
            var res = _IEquirepairService.GetReasonInfoPageList(param);
            return toResponse(res);
        }
        #endregion
    }
}
