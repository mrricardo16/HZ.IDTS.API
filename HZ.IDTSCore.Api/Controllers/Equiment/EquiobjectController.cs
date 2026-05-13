using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
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
    public class EquiobjectController : BaseController
    {
        private IEquiObjectService _IEquiObjectService;

        public EquiobjectController()
        {
            _IEquiObjectService = ServiceLocator.GetService<IEquiObjectService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 按cn_s_object_equiguid和cn_s_object_name分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IEquiObjectService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 新增对象
        /// <summary>
        /// 新增对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Add([FromBody] tn_dts_equiobject model)
        {
            ReturnMessage res = _IEquiObjectService.AddObject(model);
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

        #region 修改对象
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Update([FromBody] tn_dts_equiobject model)
        {
            ReturnMessage res = _IEquiObjectService.UpdateObject(model);
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

        #region 删除对象
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete([FromBody] tn_dts_equiobject model)
        {
            ReturnMessage res = _IEquiObjectService.DeleteObject(model);
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

        #region 获取设备树
        /// <summary>
        /// 获取设备树
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetObjectTree()
        {
            var res = _IEquiObjectService.GetObjectTree();
            return toResponse(res);
        }
        #endregion

        #region 复制对象
        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CopyObject([FromBody] string guid)
        {
            ApiResult resultReturn = new ApiResult();
            if (_IEquiObjectService.GetFirst(it => it.cn_guid == guid) is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "数据库中找不到唯一标识为" + guid + "的对象，无法复制！";
                return toResponse(resultReturn);
            }
            ObjectDriver.Instance.latestCopyObjectGuid = guid;
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "复制成功！";
            return toResponse(resultReturn);
        }
        #endregion

        #region 粘贴对象
        /// <summary>
        /// 粘贴对象
        /// </summary>
        /// <param name="pasteObject"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PasteObject([FromBody] PasteObject pasteObject)
        {
            ReturnMessage res = _IEquiObjectService.PasteObject(pasteObject);
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
