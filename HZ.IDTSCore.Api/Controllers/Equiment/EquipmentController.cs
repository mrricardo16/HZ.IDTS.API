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
    public class EquipmentController : BaseController
    {
        private IEquipmentService _IEquipmentService;

        public EquipmentController()
        {
            _IEquipmentService = ServiceLocator.GetService<IEquipmentService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 按cn_s_equi_no和cn_s_equi_name分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquipmentService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_equipment model)
        {
            var first = _IEquipmentService.GetFirst(x => x.cn_s_equi_no == model.cn_s_equi_no);
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
                var res = _IEquipmentService.Add(model);
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
        public IActionResult Update([FromBody] tn_dts_equipment model)
        {
            tn_dts_equipment itemGuid = _IEquipmentService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));


            tn_dts_equipment itemNo = _IEquipmentService.GetFirst(o => o.cn_s_equi_no == model.cn_s_equi_no && o.cn_guid != model.cn_guid);
            if (itemNo != null)
            {
                return toResponse(StatusCodeType.ParameterError, "设备编号不能重复！");
            }

            tn_dts_equipment itemName = _IEquipmentService.GetFirst(o => o.cn_s_equi_name == model.cn_s_equi_name && o.cn_guid != model.cn_guid);
            if (itemName != null)
            {
                return toResponse(StatusCodeType.ParameterError, "设备名称不能重复！");
            }
            itemGuid.cn_s_equi_parttype = model.cn_s_equi_parttype;
            itemGuid.cn_s_equi_no = model.cn_s_equi_no;
            itemGuid.cn_s_equi_name = model.cn_s_equi_name;
            itemGuid.cn_s_equi_type = model.cn_s_equi_type;
            itemGuid.cn_s_equi_model = model.cn_s_equi_model;
            itemGuid.cn_s_equi_status = model.cn_s_equi_status;
            itemGuid.cn_s_equi_buydate = model.cn_s_equi_buydate;
            itemGuid.cn_s_equi_qadate = model.cn_s_equi_qadate;
            itemGuid.cn_s_equi_firstdate = model.cn_s_equi_firstdate;
            itemGuid.cn_s_equi_defentperiod = model.cn_s_equi_defentperiod;
            itemGuid.cn_s_equi_dept = model.cn_s_equi_dept;
            itemGuid.cn_s_equi_dutyman = model.cn_s_equi_dutyman;
            itemGuid.cn_s_equi_dutyphone = model.cn_s_equi_dutyphone;
            itemGuid.cn_s_equi_contractno = model.cn_s_equi_contractno;
            itemGuid.cn_s_equi_beltline = model.cn_s_equi_beltline;
            itemGuid.cn_s_equi_xpos = model.cn_s_equi_xpos;
            itemGuid.cn_s_equi_ypos = model.cn_s_equi_ypos;
            itemGuid.cn_s_equi_zpos = model.cn_s_equi_zpos;
            itemGuid.cn_s_equi_remarks = model.cn_s_equi_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _IEquipmentService.Update(itemGuid);
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
            ApiResult result = _IEquipmentService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        #region 按设备编号和设备名称混合模糊查询获取列表数据
        /// <summary>
        /// 按设备编号和设备名称混合模糊查询获取列表数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetListData(PageParm param)
        {
            var res = _IEquipmentService.GetCompletemachineTreeList(param);
            return toResponse(res);
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveData(SaveData saveData)
        {
            ReturnMessage res = _IEquipmentService.SaveData(saveData);
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

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteData(tn_dts_equipment equipment)
        {
            ReturnMessage res = _IEquipmentService.DeleteData(equipment);
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

        #region 复制整机
        /// <summary>
        /// 复制整机
        /// </summary>
        /// <param name="completemachineTree"></param>
        /// <param name="needCopyComponents"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CopyEquipment(CompletemachineTree completemachineTree, bool needCopyComponents)
        {
            var res = _IEquipmentService.CopyEquipment(completemachineTree, needCopyComponents);
            return toResponse(res);
        }
        #endregion
    }
}
