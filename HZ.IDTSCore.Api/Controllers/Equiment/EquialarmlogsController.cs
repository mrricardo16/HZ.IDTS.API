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
    public class EquialarmlogsController : BaseController
    {
        private IEquialarmlogsService _IEquialarmlogsService;

        public EquialarmlogsController()
        {
            _IEquialarmlogsService = ServiceLocator.GetService<IEquialarmlogsService>(HttpContextSession());
        }

        #region 按cn_s_equialarmlogs_name、cn_s_equialarmlogs_errcode、cn_s_equialarmlogs_errmsg、cn_t_equialarmlogs_timestamp分页模糊查询
        /// <summary>
        /// 按cn_s_equialarmlogs_name、cn_s_equialarmlogs_errcode、cn_s_equialarmlogs_errmsg、cn_t_equialarmlogs_timestamp分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquialarmlogsService.GetListPages(param);
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
            ReturnMessage res = _IEquialarmlogsService.Delete(guidList);
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
