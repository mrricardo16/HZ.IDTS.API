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
    public class EquiobjectattrController : BaseController
    {
        private IEquiObjectAttrService _IEquiObjectAttrService;

        public EquiobjectattrController()
        {
            _IEquiObjectAttrService = ServiceLocator.GetService<IEquiObjectAttrService>(HttpContextSession());
        }

        #region 按对象名、属性名（模糊）分页查询
        /// <summary>
        /// 按对象名、属性名（模糊）分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquiObjectAttrService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 新增对象属性
        /// <summary>
        /// 新增对象属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([FromBody] tn_dts_equiobjectattr model)
        {
            ReturnMessage res = _IEquiObjectAttrService.AddObjectAttr(model);
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

        #region 修改对象属性
        /// <summary>
        /// 修改对象属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Update([FromBody] tn_dts_equiobjectattr model)
        {
            ReturnMessage res = _IEquiObjectAttrService.UpdateObjectAttr(model);
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

        #region 批量删除对象属性
        /// <summary>
        /// 批量删除对象属性
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete([FromBody] List<string> model)
        {
            ReturnMessage res = _IEquiObjectAttrService.DeleteObjectAttr(model);
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
