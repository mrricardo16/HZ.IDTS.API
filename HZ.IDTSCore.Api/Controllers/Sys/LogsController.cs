using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Info
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class LogsController : BaseController
    {
        private ILogsService _ILogsService;

        public LogsController()
        {
            _ILogsService = ServiceLocator.GetService<ILogsService>(HttpContextSession());
        }

        #region 按cn_s_logs_type和cn_t_create分页模糊查询
        /// <summary>
        /// 按cn_s_logs_type和cn_t_create分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _ILogsService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_logs model)
        {
            UserSession user = GetSessionInfo();
            model.cn_s_creator = user.UserCode;
            model.cn_s_creator_by = user.UserName;
            model.cn_t_create = DateTime.Now;
            var res = _ILogsService.Add(model);
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
        public IActionResult Update([FromBody] tn_dts_logs model)
        {
            tn_dts_logs itemGuid = _ILogsService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));
            itemGuid.cn_s_logs_type = model.cn_s_logs_type;
            itemGuid.cn_s_logs_clientip = model.cn_s_logs_clientip;
            itemGuid.cn_s_logs_receiveurl = model.cn_s_logs_receiveurl;
            itemGuid.cn_s_logs_receivepram = model.cn_s_logs_receivepram;
            itemGuid.cn_s_logs_requesturl = model.cn_s_logs_requesturl;
            itemGuid.cn_s_logs_requestpram = model.cn_s_logs_requestpram;
            itemGuid.cn_s_logs_requestresult = model.cn_s_logs_requestresult;
            itemGuid.cn_s_logs_optionpath = model.cn_s_logs_optionpath;
            itemGuid.cn_s_logs_errorsinfo = model.cn_s_logs_errorsinfo;
            itemGuid.cn_s_logs_remarks = model.cn_s_logs_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _ILogsService.Update(itemGuid);
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

        #region 批量删除日志
        /// <summary>
        /// 批量删除日志
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(string[] guidList)
        {
            ReturnMessage res = _ILogsService.Delete(guidList);
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
