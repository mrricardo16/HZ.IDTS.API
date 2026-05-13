using HZ.CommonUtil.Model;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Common;
using HZ.IDTSCore.Model.Entity.Redis;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Common
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KeyController : BaseController
    {
        IBarcode _IBarcode;
        public KeyController()
        {
            _IBarcode = ServiceLocator.GetService<IBarcode>(HttpContextSession());
        }

        #region 扫描条码 返回条码实体
        /// <summary>
        /// 扫描条码 返回条码实体
        /// </summary>
        /// <param name="barCode">扫描条码</param>
        /// <param name="type">码值类型{物料码、批次号}</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetKeyValue(string barCode, string type)
        {
            try
            {
                BarcodeItem item = _IBarcode.GetItem(barCode, type);
                return toResponse(item);
            }
            catch (Exception ex)
            {
                return toResponse(StatusCodeType.Error, ex.Message);
            }
            return toResponse(StatusCodeType.AppMessage, $"非法的码值！{barCode}");
        }
        #endregion

    }
}
