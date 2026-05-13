using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class WinservicesController : BaseController
    {
        private IWinservicesService _IWinservicesService;

        public WinservicesController()
        {
            _IWinservicesService = ServiceLocator.GetService<IWinservicesService>(HttpContextSession());
        }

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="saveBackups"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Save(SaveWinservices saveBackups)
        {
            ReturnMessage res = _IWinservicesService.Save(saveBackups);
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

        #region 分页获取所有备份记录（不含查询）
        /// <summary>
        /// 分页获取所有备份记录（不含查询）
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm parm)
        {
            var res = _IWinservicesService.GetListPages(parm);
            return toResponse(res);
        }
        #endregion

        #region 操作服务
        /// <summary>
        /// 操作服务
        /// </summary>
        /// <param name="operateServiceParameter"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult OperateService([FromBody] OperateServiceParameter operateServiceParameter)
        {
            ReturnMessage res = _IWinservicesService.OperateService(operateServiceParameter);
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

        #region 根据指定备注读取服务名称 
        /// <summary>
        /// 根据指定备注读取服务名称
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetWinServiceName([FromBody] string remarks)
        {
            var res = _IWinservicesService.GetWinServiceName(remarks);
            return toResponse(res);
        }
        #endregion

    }
}
