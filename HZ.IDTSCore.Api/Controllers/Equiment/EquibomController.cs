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
    public class EquibomController : BaseController
    {
        private IEquibomService _IEquibomService;

        public EquibomController()
        {
            _IEquibomService = ServiceLocator.GetService<IEquibomService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquibomService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_equibom model)
        {
            UserSession user = GetSessionInfo();
            model.cn_s_creator = user.UserCode;
            model.cn_s_creator_by = user.UserName;
            model.cn_t_create = DateTime.Now;
            var res = _IEquibomService.Add(model);
            return toResponse(res);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param> 
        /// <returns></returns>
        [HttpPost]
        public IActionResult Update([FromBody] tn_dts_equibom model)
        {
            tn_dts_equibom itemGuid = _IEquibomService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));

            itemGuid.cn_s_equibom_parentno = model.cn_s_equibom_parentno;
            itemGuid.cn_s_equibom_parentname = model.cn_s_equibom_parentname;
            itemGuid.cn_s_equibom_childno = model.cn_s_equibom_childno;
            itemGuid.cn_s_equibom_childname = model.cn_s_equibom_childname;
            itemGuid.cn_t_equibom_effectuatetime = model.cn_t_equibom_effectuatetime;
            itemGuid.cn_t_equibom_lapsetime = model.cn_t_equibom_lapsetime;
            itemGuid.cn_s_equibom_status = model.cn_s_equibom_status;
            itemGuid.cn_s_equibom_remarks = model.cn_s_equibom_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _IEquibomService.Update(itemGuid);
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
            ApiResult result = _IEquibomService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion
    }
}
