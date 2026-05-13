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
    public class ThreeDimensionAngleAreaController : BaseController
    {
        private IThreeDimensionAngleAreaService _IThreeDimensionAngleAreaService;

        public ThreeDimensionAngleAreaController()
        {
            _IThreeDimensionAngleAreaService = ServiceLocator.GetService<IThreeDimensionAngleAreaService>(HttpContextSession());
        }

        #region 保存3D建模视角区域
        /// <summary>
        /// 保存3D建模视角区域
        /// </summary>
        /// <param name="saveThreeDimensionAngleArea"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveThreeDimensionAngleArea(SaveThreeDimensionAngleArea saveThreeDimensionAngleArea)
        {
            ReturnMessage res = _IThreeDimensionAngleAreaService.SaveThreeDimensionAngleArea(saveThreeDimensionAngleArea);
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

        #region 按视角区域名称分页模糊查询
        /// <summary>
        /// 按视角区域名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm parm)
        {
            var res = _IThreeDimensionAngleAreaService.GetListPages(parm);
            return toResponse(res);
        }
        #endregion

        #region 删除3D建模视角区域
        /// <summary>
        /// 删除3D建模视角区域
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteThreeDimensionAngleArea(List<string> guidList)
        {
            ReturnMessage res = _IThreeDimensionAngleAreaService.DeleteThreeDimensionAngleArea(guidList);
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
