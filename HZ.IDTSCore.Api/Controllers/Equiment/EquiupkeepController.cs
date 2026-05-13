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
    [Authorization]
    public class EquiupkeepController : BaseController
    {
        private IEquiupkeepService _IEquiupkeepService;
        private IEquipmentService _IEquipmentService;

        public EquiupkeepController()
        {
            _IEquiupkeepService = ServiceLocator.GetService<IEquiupkeepService>(HttpContextSession());
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
            var res = _IEquiupkeepService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_equiupkeep model)
        {
            var first = _IEquiupkeepService.GetFirst(x => x.cn_s_equiupkeep_no == model.cn_s_equiupkeep_no);
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
                var res = _IEquiupkeepService.Add(model);
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
        public IActionResult Update([FromBody] tn_dts_equiupkeep model)
        {
            tn_dts_equiupkeep itemGuid = _IEquiupkeepService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));


            tn_dts_equiupkeep itemNo = _IEquiupkeepService.GetFirst(o => o.cn_s_equiupkeep_no == model.cn_s_equiupkeep_no && o.cn_guid != model.cn_guid);
            if (itemNo != null)
            {
                return toResponse(StatusCodeType.ParameterError, "所属设备不能重复！");
            }

            tn_dts_equiupkeep itemName = _IEquiupkeepService.GetFirst(o => o.cn_s_equiupkeep_name == model.cn_s_equiupkeep_name && o.cn_guid != model.cn_guid);
            if (itemName != null)
            {
                return toResponse(StatusCodeType.ParameterError, "设备名称不能重复！");
            }
            itemGuid.cn_s_equiupkeep_no = model.cn_s_equiupkeep_no;
            itemGuid.cn_s_equiupkeep_name = model.cn_s_equiupkeep_name;
            itemGuid.cn_s_equiupkeep_item = model.cn_s_equiupkeep_item;
            itemGuid.cn_s_equiupkeep_cause = model.cn_s_equiupkeep_cause;
            itemGuid.cn_n_equiupkeep_isfirst = model.cn_n_equiupkeep_isfirst;
            itemGuid.cn_t_equiupkeep_nextdate = model.cn_t_equiupkeep_nextdate;
            itemGuid.cn_d_equiupkeep_cost = model.cn_d_equiupkeep_cost;
            itemGuid.cn_s_equiupkeep_material = model.cn_s_equiupkeep_material;
            itemGuid.cn_s_equiupkeep_man = model.cn_s_equiupkeep_man;
            itemGuid.cn_s_equiupkeep_phone = model.cn_s_equiupkeep_phone;
            itemGuid.cn_t_equiupkeep_date = model.cn_t_equiupkeep_date;
            itemGuid.cn_s_equiupkeep_remarks = model.cn_s_equiupkeep_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _IEquiupkeepService.Update(itemGuid);
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
            ApiResult result = _IEquiupkeepService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        #region 获取设备树
        /// <summary>
        /// 获取设备树
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetEquipmentTree(PageParm param)
        {
            var res = _IEquipmentService.GetCompletemachineTreeList(param);
            return toResponse(res);
        }
        #endregion

        #region 保存设备保养
        /// <summary>
        /// 保存设备保养
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveDataUpkeep(SaveData_Upkeep saveData)
        {
            ReturnMessage res = _IEquiupkeepService.SaveDataUpkeep(saveData);
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

        #region 删除设备保养
        /// <summary>
        /// 删除设备保养
        /// </summary>
        /// <param name="equiupkeepList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteDataUpkeep(List<tn_dts_equiupkeep> equiupkeepList)
        {
            ReturnMessage res = _IEquiupkeepService.DeleteDataUpkeep(equiupkeepList);
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

        #region 查看详情
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Detail([FromBody] string guid)
        {
            tn_dts_equiupkeep result = _IEquiupkeepService.Detail(guid);
            return toResponse(result);
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
            var res = _IEquiupkeepService.GetMatterInfoPageList(param);
            return toResponse(res);
        }
        #endregion

        #region 删除保养项目
        /// <summary>
        /// 删除保养项目
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteMatter([FromBody] string guid)
        {
            ReturnMessage res = _IEquiupkeepService.DeleteMatter(guid);
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
