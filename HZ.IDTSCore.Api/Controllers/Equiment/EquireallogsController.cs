using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace HZ.IDTSCore.Api.Controllers.Equiment
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class EquireallogsController : BaseController
    {
        private IEquireallogsService _IEquireallogsService;

        public EquireallogsController()
        {
            _IEquireallogsService = ServiceLocator.GetService<IEquireallogsService>(HttpContextSession());
        }

        #region 按cn_s_equireallogs_name和cn_s_equireallogs_timestamp分页模糊查询
        /// <summary>
        /// 按cn_s_equireallogs_name和cn_s_equireallogs_timestamp分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquireallogsService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(string[] guidList)
        {
            ReturnMessage res = _IEquireallogsService.Delete(guidList);
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
