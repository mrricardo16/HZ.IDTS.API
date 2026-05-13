using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.iWCS.MData.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.SenarioTesting
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class GoodsequipmentController : BaseController
    {
        private IGoodsequipmentService _IGoodsequipmentService;

        public GoodsequipmentController()
        {
            _IGoodsequipmentService = ServiceLocator.GetService<IGoodsequipmentService>(HttpContextSession());
        }

        #region 按货位设备编码和名称混合模糊分页查询
        /// <summary>
        /// 按货位设备编码和名称混合模糊分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IGoodsequipmentService.GetListPages(param);
            return toResponse(res);
        }
        #endregion

        #region 获取指定仓库库区下所有地堆
        /// <summary>
        /// 获取指定仓库库区下所有地堆
        /// </summary>
        /// <param name="getReturnSite"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetSiteByStockArea(GetReturnSite getReturnSite)
        {
            var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.stockCode == getReturnSite.StockCode && it.area_code == getReturnSite.AreaCode && it.type == "站点");
            List<LocationSiteInformation> locationSiteInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filter);
            List<ReturnSite> siteList = locationSiteInformationList.Select(it =>
            new ReturnSite
            {
                SiteCode = it.locationCode,
                SiteName = it.location_name
            }).ToList();
            return toResponse(siteList);
        }
        #endregion

        #region 获取指定货位设备信息
        /// <summary>
        /// 获取指定货位设备信息
        /// </summary>
        /// <param name="goodsequipmentguid"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetGoodsequipment([FromBody] string goodsequipmentguid)
        {
            var res = _IGoodsequipmentService.GetGoodsequipment(goodsequipmentguid);
            return toResponse(res);
        }
        #endregion

        #region 保存货位设备
        /// <summary>
        /// 保存货位设备
        /// </summary>
        /// <param name="saveGoodsequipment"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SaveGoodsequipment(SaveGoodsequipment saveGoodsequipment)
        {
            ReturnMessage res = _IGoodsequipmentService.SaveGoodsequipment(saveGoodsequipment);
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

        #region 批量删除货位设备
        /// <summary>
        /// 批量删除货位设备
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DeleteGoodsequipment(List<string> guidList)
        {
            ReturnMessage res = _IGoodsequipmentService.DeleteGoodsequipment(guidList);
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
