using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Sys;
using SqlSugar.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Net;
using HZ.IDTSCore.Model.Entity.Equipment;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class SettingService : BaseService<tn_dts_setting>, ISettingService
    {
        public SettingService(SessionInfo session) : base(session)
        {

        }
        #region 分页模糊查询关键字分类和关键字编码
        /// <summary>
        /// 分页模糊查询关键字分类、关键字分类和关键字编码
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_setting> GetListPages(PageParm parm)
        {
            string cn_s_setting_class = parm.Parms["cn_s_setting_class"].ObjToString();
            string cn_s_setting_keycode = parm.Parms["cn_s_setting_keycode"].ObjToString();
            string cn_s_setting_keyname = parm.Parms["cn_s_setting_keyname"].ObjToString();
            return Db.Queryable<tn_dts_setting>()
            .WhereIF(!string.IsNullOrEmpty(cn_s_setting_class), (s => s.cn_s_setting_class.Contains(cn_s_setting_class)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_setting_keycode), (s => s.cn_s_setting_keycode.Contains(cn_s_setting_keycode)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_setting_keyname), (s => s.cn_s_setting_keyname.Contains(cn_s_setting_keyname)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除系统设置
        /// <summary>
        /// 删除系统设置
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_setting>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 批量添加
        /// <summary>
        /// 批量增加
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ApiResult BatchAdd(List<tn_dts_setting> listModel)
        {
            return UseTransaction(trans =>
            {
                trans.Insertable<tn_dts_setting>(listModel).ExecuteCommand();
            });
        }
        #endregion

        //public List<tn_dts_setting> GroupByTest()
        //{
        //    var res = Db.Queryable<tn_dts_setting>().Where(it => it == it).GroupBy(it => it.cn_s_setting_class).ToList();
        //    return res;
        //}

        //public PagedInfo<tn_dts_setting> GetAllListPages(PageParm parm)
        //{
        //    string cn_s_setting_class = parm.Parms["cn_s_setting_class"].ObjToString();
        //    string cn_s_setting_keycode = parm.Parms["cn_s_setting_keycode"].ObjToString();
        //    string cn_s_setting_keyname = parm.Parms["cn_s_setting_keyname"].ObjToString();
        //    return Db.Queryable<tn_dts_setting>()
        //    .WhereIF(!string.IsNullOrEmpty(cn_s_setting_class), (s => s.cn_s_setting_class.Contains(cn_s_setting_class)))
        //    .WhereIF(!string.IsNullOrEmpty(cn_s_setting_keycode), (s => s.cn_s_setting_keycode.Contains(cn_s_setting_keycode)))
        //    .WhereIF(!string.IsNullOrEmpty(cn_s_setting_keyname), (s => s.cn_s_setting_keyname.Contains(cn_s_setting_keyname)))
        //    .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
        //    .ToPage(parm.PageIndex, parm.PageSize);
        //}

        //public ApiResult AddMDGLog(SettingSavety ss)
        //{
        //    string token = GetCurrSession().token;
        //    MDGSavety mDGSavety = new MDGSavety();
        //    mDGSavety.logType = "修改";
        //    mDGSavety.appCode = "MDG";
        //    UserSession user = GetSessionInfo();
        //    string userJoin = "用户编码为" + user.UserCode + "的用户" + user.UserName;
        //    string savetyJoin = "关键字编码为" + ss.cn_s_setting_keycode + "的" + ss.cn_s_setting_keyname;
        //    mDGSavety.logDesc = userJoin + "修改了" + savetyJoin;
        //    mDGSavety.ip = HttpContext.Current.Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        //    string data = JsonConvert.SerializeObject(mDGSavety);
        //    string mdg = AppSettings.GetValue<string>("SysInterface:Mdg");
        //    ApiResult apiRes = new ApiResult();
        //    string str = WebApiManager.HttpPost(mdg, "/api/LogWork/AddLog", data, ref apiRes, token);
        //    if (!apiRes.IsSuccess)
        //        throw new WebException(apiRes.Message);
        //    apiRes = JsonConvert.DeserializeObject<ApiResult>(str);
        //    return apiRes;
        //}

        #region 保存系统配置
        /// <summary>
        /// 保存系统配置
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public ReturnMessage SaveSetting(SettingSavety ss)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_setting model = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == ss.cn_s_setting_keycode && it.cn_s_setting_keyname == ss.cn_s_setting_keyname).First();
            if (model is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "系统中找不到该关键字编码和名称的数据！";
                return returnMessage;
            }
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户将tn_dts_setting中关键字编码为" + ss.cn_s_setting_keycode + "的字段由" + model.cn_s_setting_keyvalue + "修改为" + ss.keyValue;
            log.cn_t_create = DateTime.Now;
            model.cn_s_setting_keyvalue = ss.keyValue;
            model.cn_s_setting_remarks = ss.describe;
            model.cn_s_modify = user.UserCode;
            model.cn_s_modifyBy = user.UserName;
            model.cn_t_modify = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_setting>(model).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "1.2系统（Setting）参数设置保存接口修改配置失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "保存失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "保存成功！";
            return returnMessage;
        }
        #endregion

        #region 下一步接口
        /// <summary>
        /// 下一步接口
        /// </summary>
        /// <param name="settingItemList"></param>
        /// <returns></returns>
        public ReturnMessage NextStep(List<SettingItem> settingItemList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            List<tn_dts_setting> newSettingList = new List<tn_dts_setting>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var settingItem in settingItemList)
            {
                tn_dts_setting settingKeycode = Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == settingItem.SettingKeyCode).First();
                if (settingKeycode is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "传入的设置项中有关键字编码不符合要求";
                    return returnMessage;
                }
                settingKeycode.cn_s_setting_keyvalue = settingItem.SettingKeyValue;
                newSettingList.Add(settingKeycode);
                tn_dts_logs settingLog = new tn_dts_logs();
                settingLog.cn_guid = Guid.NewGuid().ToString();
                settingLog.cn_s_logs_type = "操作";
                settingLog.cn_s_logs_remarks = "用户使用安装向导修改了关键字编码为：" + settingItem.SettingKeyCode + "的设置记录的设置值，详细信息为：" + JsonConvert.SerializeObject(settingKeycode);
                settingLog.cn_t_create = DateTime.Now;
                logList.Add(settingLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_setting>(newSettingList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "18.6系统（Setting）向导管理下一步接口修改设置失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "设置失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "设置成功!";
            return returnMessage;
        }
        #endregion

        #region 根据关键字编码读取设置值
        /// <summary>
        /// 根据关键字编码读取设置值
        /// </summary>
        /// <param name="keycode"></param>
        /// <returns></returns>
        public string GetKeyvalueByKeycode(string keycode)
        {
            return Db.Queryable<tn_dts_setting>().Where(it => it.cn_s_setting_keycode == keycode).Select(it => it.cn_s_setting_keyvalue).First();
        }
        #endregion

    }
}
