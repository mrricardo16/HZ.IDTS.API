using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Api.Authorization;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Controllers.Info
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorization]
    public class SiteinfoController : BaseController
    {
        private ISiteinfoService _ISiteinfoService;
        private ILogsService _ILogsService;

        public SiteinfoController()
        {
            _ISiteinfoService = ServiceLocator.GetService<ISiteinfoService>(HttpContextSession());
            _ILogsService = ServiceLocator.GetService<ILogsService>(HttpContextSession());
        }

        #region 分页查询
        /// <summary>
        /// 按cn_s_siteinfo_code和cn_s_siteinfo_name分页模糊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetPageList(PageParm param)
        {
            var res = _ISiteinfoService.GetListPages(param);
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
        public IActionResult Add([FromBody] tn_dts_siteinfo model)
        {
            var first = _ISiteinfoService.GetFirst(x => x.cn_s_siteinfo_code == model.cn_s_siteinfo_code);
            if (first != null)
            {
                return toResponse(StatusCodeType.AppMessage, "关键字不能重复！");
            }
            if(model.cn_d_siteinfo_angle <0 || model.cn_d_siteinfo_angle > 360)
            {
                return toResponse(StatusCodeType.AppMessage, "规划角度只能从0取到360！");
            }
            else
            {
                UserSession user = GetSessionInfo();
                model.cn_s_creator = user.UserCode;
                model.cn_s_creator_by = user.UserName;
                model.cn_t_create = DateTime.Now;
                var res = _ISiteinfoService.Add(model);
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
        public IActionResult Update([FromBody] tn_dts_siteinfo model)
        {
            tn_dts_siteinfo itemGuid = _ISiteinfoService.GetFirst(x => x.cn_guid == model.cn_guid);
            if (itemGuid == null)
            {
                return toResponse(ApiResult.Error("未找到该记录"));
            }
            if(model.cn_d_siteinfo_angle < 0 || model.cn_d_siteinfo_angle > 360)
            {
                return toResponse(ApiResult.Error("规划角度只能从0取到360！"));
            }
            tn_dts_siteinfo itemCode = _ISiteinfoService.GetFirst(o => o.cn_s_siteinfo_code == model.cn_s_siteinfo_code && o.cn_guid != model.cn_guid);
            if (itemCode != null)
            {
                return toResponse(StatusCodeType.ParameterError, "站点编号不能重复！");
            }

            tn_dts_siteinfo itemName = _ISiteinfoService.GetFirst(o => o.cn_s_siteinfo_name == model.cn_s_siteinfo_name && o.cn_guid != model.cn_guid);
            if (itemName != null)
            {
                return toResponse(StatusCodeType.ParameterError, "站点名称不能重复！");
            }
            itemGuid.cn_s_siteinfo_code = model.cn_s_siteinfo_code;
            itemGuid.cn_s_siteinfo_name = model.cn_s_siteinfo_name;
            itemGuid.cn_s_siteinfo_xpos = model.cn_s_siteinfo_xpos;
            itemGuid.cn_s_siteinfo_ypos = model.cn_s_siteinfo_ypos;
            itemGuid.cn_s_siteinfo_isshow = model.cn_s_siteinfo_isshow;
            itemGuid.cn_s_siteinfo_lenght = model.cn_s_siteinfo_lenght;
            itemGuid.cn_s_siteinfo_width = model.cn_s_siteinfo_width;
            itemGuid.cn_s_siteinfo_height = model.cn_s_siteinfo_height;
            itemGuid.cn_s_siteinfo_remarks = model.cn_s_siteinfo_remarks;
            UserSession user = GetSessionInfo();
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            itemGuid.cn_d_siteinfo_angle = model.cn_d_siteinfo_angle;
            itemGuid.cn_s_siteinfo_pileofland = model.cn_s_siteinfo_pileofland;
            int res = _ISiteinfoService.Update(itemGuid);
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
            ApiResult result = _ISiteinfoService.Delete(cn_s_guid);

            return toResponse(result);
        }
        #endregion

        #region 一键同步
        /// <summary>
        /// 一键同步
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SynchronizationByOneClick()
        {
            var filterArea = Builders<LocationSiteInformation>.Filter.Where(it => it.type == "站点");
            List<LocationSiteInformation> siteInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filterArea);
            ApiResult resultReturn = new ApiResult();
            ReturnMessage res = new ReturnMessage();
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi; //AppSettings.GetValue<string>("SysInterface:Mdg");
            string token = HttpContextSession().token;
            MDGAddLog mDGAddLog = new MDGAddLog();
            mDGAddLog.logType = "新增";
            mDGAddLog.appCode = "MDG";
            UserSession user = GetSessionInfo();
            string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
            foreach (var siteInformation in siteInformationList)
            {
                if (siteInformationList.Where(it => it.locationCode == siteInformation.locationCode).ToList().Count > 1)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "4.2地堆（Siteinfo）货位管理一键同步接口请求MDG货位接口获取的货位中货位编码有重复，重复货位编码为" + siteInformation.locationCode);
                }
                if (_ISiteinfoService.GetFirst(it => it.cn_s_siteinfo_code == siteInformation.locationCode) is null)
                {

                    string savetyJoin = "仓库名称为" + siteInformation.stockCode + "区域名称为" + siteInformation.area_code + "的平库地堆布局位置";
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
                        LogHelper.Info(DateTime.Now.ToString() + "4.2地堆（Siteinfo）货位管理一键同步接口请求MDG新增日志接口存入tn_dts_logs表失败。");
                    }

                    res = _ISiteinfoService.AddSiteinfo(siteInformation);
                    if (!res.IsSuccess)
                    {
                        resultReturn.IsSuccess = res.IsSuccess;
                        resultReturn.StatusCode = (int)StatusCodeType.Error;
                        resultReturn.Message = res.Message;
                        return toResponse(resultReturn);
                    }
                }
            }
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "一键同步执行成功";
            return new JsonResult(resultReturn);
        }
        #endregion

        #region 执行修改、删除、批量设置
        /// <summary>
        /// 执行修改、删除、批量设置
        /// </summary>
        /// <param name="executeUpdateDeleteBatchsetting"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ExecuteUpdateDeleteBatchsetting(ExecuteUpdateDeleteBatchsetting executeUpdateDeleteBatchsetting)
        {
            ApiResult resultReturn = new ApiResult();
            string mdg = IDTSCore.Common.Const.SysConst.MDGApi;// AppSettings.GetValue<string>("SysInterface:Mdg");
            string token = HttpContextSession().token;
            ReturnMessage res = new ReturnMessage();
            if (executeUpdateDeleteBatchsetting.button == "修改")
            {
                MDGAddLog mDGAddLog = new MDGAddLog();
                mDGAddLog.logType = "修改";
                mDGAddLog.appCode = "MDG";
                UserSession user = GetSessionInfo();
                string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
                string savetyJoin = "站点编号为" + executeUpdateDeleteBatchsetting.siteinfoList[0].cn_s_siteinfo_code + "站点名称为" + executeUpdateDeleteBatchsetting.siteinfoList[0].cn_s_siteinfo_name + "的平库地堆布局位置";
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
                    LogHelper.Info(DateTime.Now.ToString() + "4.3地堆（Siteinfo）货位管理修改删除批量设置接口请求MDG新增日志接口存入tn_dts_logs表失败。");
                }

                res = _ISiteinfoService.UpdateSiteinfo(executeUpdateDeleteBatchsetting.siteinfoList[0]);
                if (!res.IsSuccess)
                {
                    resultReturn.IsSuccess = res.IsSuccess;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = res.Message;
                    return toResponse(resultReturn);
                }
            }
            else if (executeUpdateDeleteBatchsetting.button == "删除")
            {
                foreach (var siteinfo in executeUpdateDeleteBatchsetting.siteinfoList)
                {
                    MDGAddLog mDGAddLog = new MDGAddLog();
                    mDGAddLog.logType = "删除";
                    mDGAddLog.appCode = "MDG";
                    UserSession user = GetSessionInfo();
                    string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
                    string savetyJoin = "站点编码为" + siteinfo.cn_s_siteinfo_code + "站点名称为" + siteinfo.cn_s_siteinfo_name + "平库地堆布局位置";
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
                        LogHelper.Info(DateTime.Now.ToString() + "4.3地堆（Siteinfo）货位管理修改删除批量设置接口请求MDG新增日志接口存入tn_dts_logs表失败。");
                    }
                }

                res = _ISiteinfoService.DeleteSiteinfo(executeUpdateDeleteBatchsetting.siteinfoList);
                if (!res.IsSuccess)
                {
                    resultReturn.IsSuccess = res.IsSuccess;
                    resultReturn.StatusCode = (int)StatusCodeType.Error;
                    resultReturn.Message = res.Message;
                    return toResponse(resultReturn);
                }
            }
            else if (executeUpdateDeleteBatchsetting.button == "批量设置")
            {
                foreach (var siteinfo in executeUpdateDeleteBatchsetting.siteinfoList)
                {
                    MDGAddLog mDGAddLog = new MDGAddLog();
                    mDGAddLog.logType = "修改";
                    mDGAddLog.appCode = "MDG";
                    UserSession user = GetSessionInfo();
                    string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
                    string savetyJoin = "站点编码为" + siteinfo.cn_s_siteinfo_code + "站点名称为" + siteinfo.cn_s_siteinfo_name + "平库地堆布局位置";
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
                        LogHelper.Info(DateTime.Now.ToString() + "4.3地堆（Siteinfo）货位管理修改删除批量设置接口请求MDG新增日志接口存入tn_dts_logs表失败。");
                    }
                }

                res = _ISiteinfoService.BatchsettingSiteinfo(executeUpdateDeleteBatchsetting.siteinfoList);
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
                resultReturn.Message = "前端请求的用户点击选项必须在“修改”“删除”和“批量设置”之中";
                return new JsonResult(resultReturn);
            }
            resultReturn.IsSuccess = true;
            resultReturn.StatusCode = (int)StatusCodeType.Success;
            resultReturn.Message = "执行成功";
            return toResponse(resultReturn);
        }
        #endregion
    }
}
