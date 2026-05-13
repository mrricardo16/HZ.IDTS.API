using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Sys
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class CacheController : BaseController
    {
        private ICacheService _ICacheService;
        private ILogsService _ILogsService;

        public CacheController()
        {
            _ICacheService = ServiceLocator.GetService<ICacheService>(HttpContextSession());
            _ILogsService = ServiceLocator.GetService<ILogsService>(HttpContextSession());
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
            var res = _ICacheService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_cache model)
        {
            var first = _ICacheService.GetFirst(x => x.cn_s_cache_code == model.cn_s_cache_code || x.cn_s_cache_name == model.cn_s_cache_name);
            if (first != null)
            {
                return toResponse(StatusCodeType.AppMessage, "缓存编码或名称不能重复！");
            }
            else
            {
                UserSession user = GetSessionInfo();
                model.cn_s_creator = user.UserCode;
                model.cn_s_creator_by = user.UserName;
                model.cn_t_create = DateTime.Now;
                var res = _ICacheService.Add(model);
                return toResponse(res);
            }
        }
        #endregion

        #region 修改
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Update([FromBody] tn_dts_cache model)
        {
            tn_dts_cache itemGuid = _ICacheService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));


            tn_dts_cache itemCode = _ICacheService.GetFirst(o => o.cn_s_cache_code == model.cn_s_cache_code && o.cn_guid != model.cn_guid);
            if (itemCode != null)
            {
                return toResponse(StatusCodeType.ParameterError, "缓存编码不能重复！");
            }

            tn_dts_cache itemName = _ICacheService.GetFirst(o => o.cn_s_cache_name == model.cn_s_cache_name && o.cn_guid != model.cn_guid);
            if (itemName != null)
            {
                return toResponse(StatusCodeType.ParameterError, "缓存名称不能重复！");
            }
            itemGuid.cn_s_cache_code = model.cn_s_cache_code;
            itemGuid.cn_s_cache_name = model.cn_s_cache_name;
            itemGuid.cn_s_cache_type = model.cn_s_cache_type;
            itemGuid.cn_s_cache_remarks = model.cn_s_cache_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _ICacheService.Update(itemGuid);
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
            ApiResult result = _ICacheService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        #region 同步缓存
        /// <summary>
        /// 同步缓存
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SynchronousCache(CacheSynchronousness cs)
        {
            string token = HttpContextSession().token;
            MDGAddLog mDGAddLog = new MDGAddLog();
            mDGAddLog.logType = "修改";
            mDGAddLog.appCode = "MDG";
            UserSession user = GetSessionInfo();
            string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
            string savetyJoin = "缓存类型为" + cs.cn_s_cache_type + "的" + cs.cn_s_cache_code;
            mDGAddLog.logDesc = userJoin + "修改了" + savetyJoin;
            mDGAddLog.ip = HZ.DbHelper.HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            string dataLog = JsonConvert.SerializeObject(mDGAddLog);
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi; //AppSettings.GetValue<string>("SysInterface:Mdg");
            ApiResult apiResLog = new ApiResult();
            string strLog = WebApiManager.HttpPost(mdg, "/api/LogWork/AddLog", dataLog, ref apiResLog, token);

            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_clientip = HZ.DbHelper.HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            log.cn_s_logs_requesturl = mdg + "/api/LogWork/AddLog";
            log.cn_s_logs_requestpram = dataLog;
            log.cn_s_logs_requestresult = JsonConvert.SerializeObject(apiResLog);
            log.cn_s_creator = user.UserCode;
            log.cn_s_creator_by = user.UserName;
            log.cn_t_create = DateTime.Now;
            int failResult = _ILogsService.Add(log);
            ApiResult resultReturn = new ApiResult();
            if (failResult <= 0)
            {
                LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG新增日志接口存入tn_dts_logs表失败。");
            }

            if (cs.cn_s_cache_code == "StockInformation")
            {
                MDGGetStock mDGGetStock = new MDGGetStock();
                mDGGetStock.stockCode = "";
                mDGGetStock.stockName = "";
                string paramStock = JsonConvert.SerializeObject(mDGGetStock);
                ApiResult apiResStock = new ApiResult();
                string strStock = WebApiManager.HttpGet(mdg, "/api/StockWork/GetStock", ref apiResStock, paramStock, token);
                if (!apiResStock.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG仓库信息接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strStock))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG仓库信息接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败!";
                    return new JsonResult(resultReturn);
                }
                dynamic stockDynamic = JsonConvert.DeserializeObject<dynamic>(strStock);
                List<StockInformation> stockList = JsonConvert.DeserializeObject<List<StockInformation>>(stockDynamic.Data.ToString());
                if (!MongoDBSingleton.Instance.DeleteAll<StockInformation>().IsAcknowledged)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口清除MongoDB中的StockInformation表失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (!MongoDBSingleton.Instance.InsertMany<StockInformation>(stockList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口向MongoDB中插入StockInformation类失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
            }
            else if (cs.cn_s_cache_code == "AreaListInformation")
            {
                MDGGetArea mDGGetArea = new MDGGetArea();
                mDGGetArea.stockCode = "";
                mDGGetArea.areaCode = "";
                mDGGetArea.areaName = "";
                string paramArea = JsonConvert.SerializeObject(mDGGetArea);
                ApiResult apiResArea = new ApiResult();
                string strAreaList = WebApiManager.HttpGet(mdg, "/api/StockWork/GetAreaList", ref apiResArea, paramArea, token);
                if (!apiResArea.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG库区列表接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strAreaList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG库区列表接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                dynamic areaListDynamic = JsonConvert.DeserializeObject<dynamic>(strAreaList);
                List<AreaInformation_Middle> areaListMiddles = JsonConvert.DeserializeObject<List<AreaInformation_Middle>>(areaListDynamic.Data.ToString());
                List<AreaInformation> areaList = new List<AreaInformation>();
                foreach (var areaMiddle in areaListMiddles)
                {
                    AreaInformation area = new AreaInformation();
                    area.stock_code = areaMiddle.stock_code;
                    area.area_code = areaMiddle.area_code;
                    area.area_name = areaMiddle.area_name;
                    if (areaMiddle.area_type == "库区")
                    {
                        area.area_type = 1;
                    }
                    else if (areaMiddle.area_type == "作业区")
                    {
                        area.area_type = 2;
                    }
                    else
                    {
                        area.area_type = null;
                    }
                    area.order = areaMiddle.order;
                    area.area_struct = areaMiddle.area_struct;
                    area.area_class = areaMiddle.area_class;
                    area.is_inventory = areaMiddle.is_inventory;
                    area.is_auto = areaMiddle.is_auto;
                    area.control_leve = areaMiddle.control_leve;
                    area.control_qty = areaMiddle.control_qty;
                    area.codedisk_model = areaMiddle.codedisk_model;
                    area.is_tray = areaMiddle.is_tray;
                    area.is_checktray = areaMiddle.is_checktray;
                    area.is_merge = areaMiddle.is_merge;
                    area.is_outarea = areaMiddle.is_outarea;
                    area.is_inout_model = areaMiddle.is_inout_model;
                    area.inout_model = areaMiddle.inout_model;
                    areaList.Add(area);
                }
                if (!MongoDBSingleton.Instance.DeleteAll<AreaInformation>().IsAcknowledged)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口清除MongoDB中的AreaInformation表失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (!MongoDBSingleton.Instance.InsertMany<AreaInformation>(areaList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口向MongoDB中插入AreaInformation类失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
            }
            else if (cs.cn_s_cache_code == "LocationSiteStation")
            {
                MDGGetLocation mDGGetLocation = new MDGGetLocation();
                mDGGetLocation.locationCode = "";
                string paramLocation = JsonConvert.SerializeObject(mDGGetLocation);
                ApiResult apiResLocation = new ApiResult();
                string strLocationList = WebApiManager.HttpGet(mdg, "/api/StockWork/GetLocation", ref apiResLocation, paramLocation, token);
                //MDGGetLocationSite mDGGetLocationSite = new MDGGetLocationSite();
                //mDGGetLocationSite.code = "";
                //string paramLocationSite = JsonConvert.SerializeObject(mDGGetLocationSite);
                //ApiResult apiResLocationSite = new ApiResult();
                //string strLocationSiteList = WebApiManager.HttpGet(mdg, "/api/StockWork/GetLocationSite", ref apiResLocationSite, paramLocationSite, token);
                if (!apiResLocation.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG货位接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strLocationList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG货位接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                dynamic locationDynamic = JsonConvert.DeserializeObject<dynamic>(strLocationList);
                List<LocationInformation_Middle> locationListMiddles = JsonConvert.DeserializeObject<List<LocationInformation_Middle>>(locationDynamic.Data.ToString());
                List<LocationSiteInformation> locationSiteList = new List<LocationSiteInformation>();
                foreach (var locationMiddle in locationListMiddles)
                {
                    LocationSiteInformation locationSite = new LocationSiteInformation();
                    locationSite.stockCode = locationMiddle.stockCode;
                    locationSite.locationCode = locationMiddle.locationCode;
                    if (!String.IsNullOrEmpty(locationMiddle.max_store_num))
                    {
                        locationSite.max_store_num = int.Parse(locationMiddle.max_store_num);
                    }
                    else
                    {
                        locationSite.max_store_num = null;
                    }
                    locationSite.roadway = locationMiddle.roadway;
                    locationSite.row = locationMiddle.row;
                    locationSite.col = locationMiddle.col;
                    locationSite.floor = locationMiddle.floor;
                    locationSite.type = "货位";
                    locationSite.real_location = locationMiddle.real_location;
                    locationSite.position = locationMiddle.position;
                    locationSite.location_state = locationMiddle.location_state;
                    locationSite.location_name = locationMiddle.location_name;
                    locationSite.area_code = locationMiddle.area_code;
                    if (!String.IsNullOrEmpty(locationMiddle.beat))
                    {
                        locationSite.beat = int.Parse(locationMiddle.beat);
                    }
                    else
                    {
                        locationSite.beat = null;
                    }
                    locationSiteList.Add(locationSite);
                }
                MDGGetSite mDGGetSite = new MDGGetSite();
                mDGGetSite.siteCode = "";
                string paramSite = JsonConvert.SerializeObject(mDGGetSite);
                ApiResult apiResSite = new ApiResult();
                string strSiteList = WebApiManager.HttpGet(mdg, "/api/StockWork/GetSite", ref apiResSite, paramSite, token);
                if (!apiResSite.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG站点接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strSiteList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG站点接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                dynamic siteDynamic = JsonConvert.DeserializeObject<dynamic>(strSiteList);
                List<SiteInformation_Middle> siteListMiddles = JsonConvert.DeserializeObject<List<SiteInformation_Middle>>(siteDynamic.Data.ToString());
                foreach (var siteMiddle in siteListMiddles)
                {
                    LocationSiteInformation locationSite = new LocationSiteInformation();
                    locationSite.stockCode = siteMiddle.stockCode;
                    locationSite.locationCode = siteMiddle.siteCode;
                    if (!String.IsNullOrEmpty(siteMiddle.max_store_num))
                    {
                        locationSite.max_store_num = int.Parse(siteMiddle.max_store_num);
                    }
                    else
                    {
                        locationSite.max_store_num = null;
                    }
                    locationSite.roadway = siteMiddle.roadway;
                    locationSite.row = siteMiddle.row;
                    locationSite.col = siteMiddle.col;
                    locationSite.floor = siteMiddle.floor;
                    locationSite.type = "站点";
                    locationSite.real_location = siteMiddle.real_location;
                    locationSite.position = siteMiddle.position;
                    locationSite.location_state = siteMiddle.location_state;
                    locationSite.location_name = siteMiddle.location_name;
                    locationSite.area_code = siteMiddle.area_code;
                    if (!String.IsNullOrEmpty(siteMiddle.beat))
                    {
                        locationSite.beat = int.Parse(siteMiddle.beat);
                    }
                    else
                    {
                        locationSite.beat = null;
                    }
                    locationSiteList.Add(locationSite);
                }
                if (!MongoDBSingleton.Instance.DeleteAll<LocationSiteInformation>().IsAcknowledged)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口清除MongoDB中的LocationSiteInformation表失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (!MongoDBSingleton.Instance.InsertMany<LocationSiteInformation>(locationSiteList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口向MongoDB中插入LocationSiteInformation类失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
            }
            else if (cs.cn_s_cache_code == "BrowserCache")
            {

            }
            else if(cs.cn_s_cache_code=="DataDictionary")
            {
  
                ApiResult apiResDictList = new ApiResult();
                string strDictList = WebApiManager.HttpGet(mdg, "/api/BasicWork/GetDictList?dictName=字典管理", ref apiResDictList,"",token);
                if (!apiResDictList.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG字典接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strDictList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口请求MDG字典接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                dynamic dictListDynamic = JsonConvert.DeserializeObject<dynamic>(strDictList);
                List<DataDictionary> dictList_Middle = JsonConvert.DeserializeObject<List<DataDictionary>>(dictListDynamic.Data.ToString());
                List<DataDictionary> dictList = _ICacheService.GetAllDict(dictList_Middle);
                if (!MongoDBSingleton.Instance.DeleteAll<DataDictionary>().IsAcknowledged)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口清除MongoDB中的DataDictionary表失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
                if (!MongoDBSingleton.Instance.InsertMany<DataDictionary>(dictList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口向MongoDB中插入DataDictionary类失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "缓存失败！";
                    return new JsonResult(resultReturn);
                }
            }
            else
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "前端请求的缓存编码必须在“BrowserCache”、“LocationSiteStation”、“AreaInformation”、“StockInformation”、“DataDictionary”之中";
                return new JsonResult(resultReturn);
            }

            tn_dts_cache model = _ICacheService.GetWhere(it => it.cn_s_cache_code == cs.cn_s_cache_code && it.cn_s_cache_type == cs.cn_s_cache_type)[0];
            if (model is null)
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "系统中找不到该缓存名称和缓存类型的数据";
                return new JsonResult(resultReturn);
            }
            model.cn_s_modify = user.UserCode;
            model.cn_s_modify_by = user.UserName;
            model.cn_t_modify = DateTime.Now;
            int res = _ICacheService.Update(model);
            if (res <= 0)
            {
                LogHelper.Info(DateTime.Now.ToString() + "2.2缓存（Cache）管理同步缓存接口更新tn_dts_cache表失败。");
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "缓存失败！";
                return new JsonResult(resultReturn);
            }
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "数据成功同步到MongoDB数据库";
            return new JsonResult(resultReturn);
        }
        #endregion

        #region 通用获取字典接口
        /// <summary>
        /// 通用获取字典接口
        /// </summary>
        /// <param name="parentname">字典父项名称</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllDictByParentname([FromBody] string parentname)
        {
            var filterDataDictionary = Builders<DataDictionary>.Filter.Where(it => it.parentName == parentname);
            List<DataDictionary> dataDictionaryList = MongoDBSingleton.Instance.FindList<DataDictionary>(filterDataDictionary);
            return toResponse(dataDictionaryList);
        }
        #endregion
    }
}
