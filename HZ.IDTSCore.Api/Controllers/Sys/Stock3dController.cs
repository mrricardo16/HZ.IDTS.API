using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Interfaces;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Info
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class Stock3dController : BaseController
    {
        private IStock3dService _IStock3dService;
        private ILogsService _ILogsService;

        public Stock3dController()
        {
            _IStock3dService = ServiceLocator.GetService<IStock3dService>(HttpContextSession());
            _ILogsService = ServiceLocator.GetService<ILogsService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 按cn_s_location_areacode和cn_s_location_areaname分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _IStock3dService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_stock3d model)
        {
            var first = _IStock3dService.GetFirst(x => x.cn_s_location_stockcode == model.cn_s_location_stockcode || x.cn_s_location_stockname == model.cn_s_location_stockname || x.cn_s_location_areacode == model.cn_s_location_areacode || x.cn_s_location_areaname == model.cn_s_location_areaname);
            if (first != null)
            {
                return toResponse(StatusCodeType.AppMessage, "仓库编号/仓库名称/区域编号/区域名称不能重复！");
            }
            else
            {
                UserSession user = GetSessionInfo();
                model.cn_s_creator = user.UserCode;
                model.cn_s_creator_by = user.UserName;
                model.cn_t_create = DateTime.Now;
                var res = _IStock3dService.Add(model);
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
        public IActionResult Update([FromBody] tn_dts_stock3d model)
        {
            tn_dts_stock3d itemGuid = _IStock3dService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
                return toResponse(ApiResult.Error("未找到该记录"));

            tn_dts_stock3d itemStockcode = _IStock3dService.GetFirst(o => o.cn_s_location_stockcode == model.cn_s_location_stockcode && o.cn_guid != model.cn_guid);
            if (itemStockcode != null)
            {
                return toResponse(StatusCodeType.ParameterError, "仓库编号不能重复！");
            }
            tn_dts_stock3d itemStockname = _IStock3dService.GetFirst(o => o.cn_s_location_stockname == model.cn_s_location_stockname && o.cn_guid != model.cn_guid);
            if (itemStockname != null)
            {
                return toResponse(StatusCodeType.ParameterError, "仓库名称不能重复！");
            }
            tn_dts_stock3d itemAreacode = _IStock3dService.GetFirst(o => o.cn_s_location_areacode == model.cn_s_location_areacode && o.cn_guid != model.cn_guid);
            if (itemAreacode != null)
            {
                return toResponse(StatusCodeType.ParameterError, "区域编号不能重复！");
            }
            tn_dts_stock3d itemAreaname = _IStock3dService.GetFirst(o => o.cn_s_location_areaname == model.cn_s_location_areaname && o.cn_guid != model.cn_guid);
            if (itemAreaname != null)
            {
                return toResponse(StatusCodeType.ParameterError, "区域名称不能重复！");
            }
            itemGuid.cn_s_location_stockcode = model.cn_s_location_stockcode;
            itemGuid.cn_s_location_stockname = model.cn_s_location_stockname;
            itemGuid.cn_s_location_areacode = model.cn_s_location_areacode;
            itemGuid.cn_s_location_areaname = model.cn_s_location_areaname;
            itemGuid.cn_s_location_isshow = model.cn_s_location_isshow;
            itemGuid.cn_s_location_row = model.cn_s_location_row;
            itemGuid.cn_s_location_col = model.cn_s_location_col;
            itemGuid.cn_s_location_layer = model.cn_s_location_layer;
            itemGuid.cn_s_location_length = model.cn_s_location_length;
            itemGuid.cn_s_location_height = model.cn_s_location_height;
            itemGuid.cn_s_location_width = model.cn_s_location_width;
            itemGuid.cn_s_location_xpos = model.cn_s_location_xpos;
            itemGuid.cn_s_location_ypos = model.cn_s_location_ypos;
            itemGuid.cn_s_location_nullify = model.cn_s_location_nullify;
            itemGuid.cn_s_location_gap = model.cn_s_location_gap;
            itemGuid.cn_s_location_remarks = model.cn_s_location_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            int res = _IStock3dService.Update(itemGuid);
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
            ApiResult result = _IStock3dService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        #region 获取全部仓库名和仓库编号（新增调用）
        /// <summary>
        /// 获取全部仓库名和仓库编号（新增调用）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllStockUsedByAdd()
        {
            var filter = Builders<StockInformation>.Filter.Or(Builders<StockInformation>.Filter.Where(it => it.stockName != ""), Builders<StockInformation>.Filter.Where(it => it.stockName == ""));
            List<StockInformation> stockInformationList = MongoDBSingleton.Instance.FindList<StockInformation>(filter);
            List<GetReturnStock> stockList = new List<GetReturnStock>();
            foreach (var stock in stockInformationList)
            {
                GetReturnStock returnStock = new GetReturnStock();
                returnStock.cn_s_location_stockcode = stock.stockCode;
                returnStock.cn_s_location_stockname = stock.stockName;
                stockList.Add(returnStock);
            }
            return toResponse(stockList);
        }
        #endregion

        #region 根据选择的仓库名获取全部库区名及其最大排列行（新增调用）
        /// <summary>
        /// 根据选择的仓库名获取全部库区名及其最大排列行（新增调用）
        /// 注：这里返回的最大排列层在区域没有货位或区域里的货位排列层都为null时，返回排列层全为0
        /// </summary>
        /// <param name="getReturnStock"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAllAreaWithStocknameUsedByAdd(GetReturnStock getReturnStock)
        {
            var filterArea = Builders<AreaInformation>.Filter.Where(it => it.stock_code == getReturnStock.cn_s_location_stockcode);
            List<AreaInformation> areaInformationList = MongoDBSingleton.Instance.FindList<AreaInformation>(filterArea);
            List<ReturnArea> areaList = new List<ReturnArea>();
            foreach (var areaInformation in areaInformationList)
            {
                ReturnArea returnArea = new ReturnArea();
                returnArea.area_code = areaInformation.area_code;
                returnArea.area_name = areaInformation.area_name;
                var filterLocation = Builders<LocationSiteInformation>.Filter.Where(it => it.stockCode == getReturnStock.cn_s_location_stockcode && it.area_code == areaInformation.area_code && it.type == "货位");
                List<LocationSiteInformation> locationSiteInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filterLocation);
                double maxRow = 0;
                double maxCol = 0;
                double maxFloor = 0;
                foreach (var locationSiteInformation in locationSiteInformationList)
                {
                    string row = locationSiteInformation.row;
                    string col = locationSiteInformation.col;
                    string floor = locationSiteInformation.floor;
                    double rowDouble = 0;
                    double colDouble = 0;
                    double floorDouble = 0;
                    if (!String.IsNullOrEmpty(row))
                    {
                        rowDouble = double.Parse(row);
                    }
                    if (!String.IsNullOrEmpty(col))
                    {
                        colDouble = double.Parse(col);
                    }
                    if (!String.IsNullOrEmpty(floor))
                    {
                        floorDouble = double.Parse(floor);
                    }
                    if (rowDouble > maxRow)
                    {
                        maxRow = rowDouble;
                    }
                    if (colDouble > maxCol)
                    {
                        maxCol = colDouble;
                    }
                    if (floorDouble > maxFloor)
                    {
                        maxFloor = floorDouble;
                    }
                }
                returnArea.max_row = maxRow.ToString();
                returnArea.max_col = maxCol.ToString();
                returnArea.max_floor = maxFloor.ToString();
                areaList.Add(returnArea);
            }
            return toResponse(areaList);
        }
        #endregion

        #region 获取指定库区最大排列层
        /// <summary>
        /// 获取指定库区最大排列层
        /// </summary>
        /// <param name="getAreaMaxRowColLayer"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetAreaMaxRowcollayer(GetAreaMaxRowColLayer getAreaMaxRowColLayer)
        {
            return toResponse(GoodscommandDriver.Instance.GetAreaMaxRowcollayer(getAreaMaxRowColLayer));
        }
        #endregion

        #region 获取修改显示信息（修改调用）
        /// <summary>
        /// 获取修改显示信息（修改调用）
        /// </summary>
        /// <param name="getUpdate"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetUpdateUsedByUpdate(GetUpdate getUpdate)
        {
            tn_dts_stock3d result = _IStock3dService.GetFirst(it => it.cn_guid == getUpdate.cn_guid);

            return toResponse(result);
        }
        #endregion

        #region 执行新增、修改和删除
        /// <summary>
        /// 执行新增、修改和删除
        /// </summary>
        /// <param name="executeAddUpdateDelete"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ExecuteAddUpdateDelete(ExecuteAddUpdateDelete executeAddUpdateDelete)
        {
            ApiResult resultReturn = new ApiResult();
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            string token = HttpContextSession().token;
            ReturnMessage res = new ReturnMessage();
            ApiResult result = new ApiResult();
            if (executeAddUpdateDelete.stock3dList.Count == 0)
            {
                result.IsSuccess = false;
                result.StatusCode = (int)StatusCodeType.Error;
                result.Message = "传入接口的立库列表为空，请重试！";
                return toResponse(result);
            }

            if (executeAddUpdateDelete.button == "新增")
            {
                MDGAddLog mDGAddLog = new MDGAddLog();
                mDGAddLog.logType = "新增";
                mDGAddLog.appCode = "MDG";
                UserSession user = GetSessionInfo();
                string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
                string savetyJoin = "仓库名称为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_stockname + "区域名称为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_areaname
                    + "排列层为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_row + "/" + executeAddUpdateDelete.stock3dList[0].cn_s_location_col + "/" + executeAddUpdateDelete.stock3dList[0].cn_s_location_layer
                    + "的区域立库3D结构";
                mDGAddLog.logDesc = userJoin + "新增了" + savetyJoin;
                mDGAddLog.ip = HZ.DbHelper.HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                string dataLog = JsonConvert.SerializeObject(mDGAddLog);
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
                if (failResult <= 0)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口请求MDG新增日志接口存入tn_dts_logs表失败。");
                }

                MDGGetLocation mDGGetLocation = new MDGGetLocation();
                mDGGetLocation.locationCode = "";
                string paramLocation = JsonConvert.SerializeObject(mDGGetLocation);
                ApiResult apiResLocation = new ApiResult();
                string strLocationList = WebApiManager.HttpGet(mdg, "/api/StockWork/GetLocation", ref apiResLocation, paramLocation, token);
                if (!apiResLocation.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口请求MDG货位接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "新增失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strLocationList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口MDG货位接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "新增失败！";
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
                var filterLocation = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "货位");
                if (!MongoDBSingleton.Instance.DeleteMany<LocationSiteInformation>(filterLocation).IsAcknowledged)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口清除MongoDB中的LocationSite表中货位数据失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "新增失败！";
                    return new JsonResult(resultReturn);
                }
                if (!MongoDBSingleton.Instance.InsertMany<LocationSiteInformation>(locationSiteList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口向MongoDB中插入货位信息失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "新增失败！";
                    return new JsonResult(resultReturn);
                }

                res = _IStock3dService.AddStock3d(executeAddUpdateDelete.stock3dList[0]);
                if (!res.IsSuccess)
                {
                    resultReturn.IsSuccess = res.IsSuccess;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = res.Message;
                    return toResponse(resultReturn);
                }
            }
            else if (executeAddUpdateDelete.button == "修改")
            {
                MDGAddLog mDGAddLog = new MDGAddLog();
                mDGAddLog.logType = "修改";
                mDGAddLog.appCode = "MDG";
                UserSession user = GetSessionInfo();
                string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
                string savetyJoin = "仓库名称为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_stockname + "区域名称为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_areaname
                     + "排列层为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_row + "/" + executeAddUpdateDelete.stock3dList[0].cn_s_location_col + "/" + executeAddUpdateDelete.stock3dList[0].cn_s_location_layer
                     + "的区域立库3D结构";
                mDGAddLog.logDesc = userJoin + "修改了" + savetyJoin;
                mDGAddLog.ip = HZ.DbHelper.HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                string dataLog = JsonConvert.SerializeObject(mDGAddLog);
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
                if (failResult <= 0)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口请求MDG新增日志接口存入tn_dts_logs表失败。");
                }

                MDGGetLocation mDGGetLocation = new MDGGetLocation();
                mDGGetLocation.locationCode = "";
                string paramLocation = JsonConvert.SerializeObject(mDGGetLocation);
                ApiResult apiResLocation = new ApiResult();
                string strLocationList = WebApiManager.HttpGet(mdg, "/api/StockWork/GetLocation", ref apiResLocation, paramLocation, token);
                if (!apiResLocation.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口请求MDG货位接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "修改失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strLocationList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口MDG货位接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "修改失败！";
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
                var filterLocation = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "货位");
                if (!MongoDBSingleton.Instance.DeleteMany<LocationSiteInformation>(filterLocation).IsAcknowledged)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口清除MongoDB中的LocationSite表中货位数据失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "修改失败！";
                    return new JsonResult(resultReturn);
                }
                if (!MongoDBSingleton.Instance.InsertMany<LocationSiteInformation>(locationSiteList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口向MongoDB中插入货位信息失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "修改失败！";
                    return new JsonResult(resultReturn);
                }

                res = _IStock3dService.UpdateStock3d(executeAddUpdateDelete.stock3dList[0]);
                if (!res.IsSuccess)
                {
                    resultReturn.IsSuccess = res.IsSuccess;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = res.Message;
                    return toResponse(resultReturn);
                }
            }
            else if (executeAddUpdateDelete.button == "删除")
            {
                foreach (var stock3d in executeAddUpdateDelete.stock3dList)
                {
                    MDGAddLog mDGAddLog = new MDGAddLog();
                    mDGAddLog.logType = "删除";
                    mDGAddLog.appCode = "MDG";
                    UserSession user = GetSessionInfo();
                    string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
                    string savetyJoin = "仓库名称为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_stockname + "区域名称为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_areaname
                     + "排列层为" + executeAddUpdateDelete.stock3dList[0].cn_s_location_row + "/" + executeAddUpdateDelete.stock3dList[0].cn_s_location_col + "/" + executeAddUpdateDelete.stock3dList[0].cn_s_location_layer
                     + "的区域立库3D结构";
                    mDGAddLog.logDesc = userJoin + "删除了" + savetyJoin;
                    mDGAddLog.ip = HZ.DbHelper.HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
                    string dataLog = JsonConvert.SerializeObject(mDGAddLog);
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
                    if (failResult <= 0)
                    {
                        LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口请求MDG新增日志接口存入tn_dts_logs表失败。");
                    }
                }

                MDGGetLocation mDGGetLocation = new MDGGetLocation();
                mDGGetLocation.locationCode = "";
                string paramLocation = JsonConvert.SerializeObject(mDGGetLocation);
                ApiResult apiResLocation = new ApiResult();
                string strLocationList = WebApiManager.HttpGet(mdg, "/api/StockWork/GetLocation", ref apiResLocation, paramLocation, token);
                if (!apiResLocation.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口请求MDG货位接口失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "删除失败！";
                    return new JsonResult(resultReturn);
                }
                if (String.IsNullOrEmpty(strLocationList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口MDG货位接口返回信息为空。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "删除失败！";
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
                var filterLocation = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "货位");
                if (!MongoDBSingleton.Instance.DeleteMany<LocationSiteInformation>(filterLocation).IsAcknowledged)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口清除MongoDB中的LocationSite表中货位数据失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "删除失败！";
                    return new JsonResult(resultReturn);
                }
                if (!MongoDBSingleton.Instance.InsertMany<LocationSiteInformation>(locationSiteList))
                {
                    LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口向MongoDB中插入货位信息失败。");
                    resultReturn.IsSuccess = false;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = "删除失败！";
                    return new JsonResult(resultReturn);
                }

                res = _IStock3dService.DeleteStock3d(executeAddUpdateDelete.stock3dList);
                if (!res.IsSuccess)
                {
                    resultReturn.IsSuccess = res.IsSuccess;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = res.Message;
                    return toResponse(resultReturn);
                }
            }
            else
            {
                resultReturn.IsSuccess = false;
                resultReturn.StatusCode = (int)StatusCodeType.Error;
                resultReturn.Message = "前端请求的用户点击选项必须在“新增”、“修改”“删除”之中";
                return new JsonResult(resultReturn);
            }

            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "执行成功";
            return toResponse(resultReturn);
        }
        #endregion

        #region 获取报废列表
        /// <summary>
        /// 获取报废列表
        /// </summary>
        /// <param name="getScrapList"></param>
        /// <returns></returns>
        [HttpPost]
        //public IActionResult GetScrapList(GetScrapList getScrapList)
        public IActionResult GetScrapList(PageParm param)
        {
            //var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.stockCode == getScrapList.location_stockcode && it.area_code == getScrapList.location_area_code && it.location_state == "报废");
            //List<LocationSiteInformation> locationInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filter);

            string location_stockcode = param.Parms["location_stockcode"].ObjToString();
            string location_area_code = param.Parms["location_area_code"].ObjToString();
            var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.stockCode == location_stockcode && it.area_code == location_area_code && it.location_state == "报废" && it.type == "货位");
            PagedInfo<LocationSiteInformation> locationInformationPagedList = MongoDBSingleton.Instance.ToPageMongo<LocationSiteInformation>(filter, param.PageIndex, param.PageSize);
            //List<LocationSiteInformation> locationInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filter);
            //List <ReturnScrap> scrapList = new List<ReturnScrap>();
            //foreach (var locationInformation in locationInformationList)
            //{
            //    ReturnScrap returnScrap = new ReturnScrap();
            //    returnScrap.location_code = locationInformation.locationCode;
            //    returnScrap.row = locationInformation.row;
            //    returnScrap.col = locationInformation.col;
            //    returnScrap.floor = locationInformation.floor;
            //    returnScrap.location_state = locationInformation.location_state;
            //    scrapList.Add(returnScrap);
            //}
            //return toResponse(scrapList);
            return toResponse(locationInformationPagedList);
        }
        #endregion
    }
}
