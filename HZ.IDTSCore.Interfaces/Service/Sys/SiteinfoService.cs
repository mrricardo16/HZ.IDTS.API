using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class SiteinfoService : BaseService<tn_dts_siteinfo>, ISiteinfoService
    {
        public SiteinfoService(SessionInfo session) : base(session)
        {

        }

        #region 分页模糊查询站点编号和站点名称
        /// <summary>
        /// 分页模糊查询站点编号和站点名称
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_siteinfo> GetListPages(PageParm parm)
        {
            string cn_s_siteinfo_code = parm.Parms["cn_s_siteinfo_code"].ObjToString();
            string cn_s_siteinfo_name = parm.Parms["cn_s_siteinfo_name"].ObjToString();
            return Db.Queryable<tn_dts_siteinfo>().WhereIF(!string.IsNullOrEmpty(cn_s_siteinfo_code), (s => s.cn_s_siteinfo_code.Contains(cn_s_siteinfo_code)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_siteinfo_name), (s => s.cn_s_siteinfo_name.Contains(cn_s_siteinfo_name)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除站点
        /// <summary>
        /// 删除站点
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_siteinfo>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 新增地堆
        /// <summary>
        /// 新增地堆
        /// </summary>
        /// <param name="locationSite">同步缓存到MongoDB中的LocationSiteInformation表中的MDG站点信息</param>
        /// <returns></returns>
        public ReturnMessage AddSiteinfo(LocationSiteInformation locationSite)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_siteinfo siteinfo = new tn_dts_siteinfo();
            siteinfo.cn_guid = Guid.NewGuid().ToString();
            siteinfo.cn_s_siteinfo_code = locationSite.locationCode;
            siteinfo.cn_s_siteinfo_name = locationSite.location_name;
            siteinfo.cn_s_creator = user.UserCode;
            siteinfo.cn_s_creator_by = user.UserName;
            siteinfo.cn_t_create = DateTime.Now;
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用一键同步功能向tn_dts_siteinfo表中新增一条地堆记录，详细信息为" + JsonConvert.SerializeObject(siteinfo);
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_siteinfo>(siteinfo).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "4.2地堆（Siteinfo）货位管理一键同步接口新增地堆失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "一键同步失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "新增成功！";
            return returnMessage;
        }
        #endregion

        #region 修改地堆
        /// <summary>
        /// 修改地堆
        /// </summary>
        /// <param name="siteinfo"></param>
        /// <returns></returns>
        public ReturnMessage UpdateSiteinfo(tn_dts_siteinfo siteinfo)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_siteinfo itemGuid = Db.Queryable<tn_dts_siteinfo>().Where(it => it.cn_guid == siteinfo.cn_guid).First();
            if (itemGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "找不到唯一标识为" + siteinfo.cn_guid + "地堆记录！";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_siteinfo>().Where(it => it.cn_s_siteinfo_code == siteinfo.cn_s_siteinfo_code && it.cn_guid != siteinfo.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "站点编号不能重复！";
                return returnMessage;
            }
            if(siteinfo.cn_d_siteinfo_angle < 0 || siteinfo.cn_d_siteinfo_angle > 360)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "规划角度只能从0取到360！";
                return returnMessage;
            }
            itemGuid.cn_s_siteinfo_code = siteinfo.cn_s_siteinfo_code;
            itemGuid.cn_s_siteinfo_name = siteinfo.cn_s_siteinfo_name;
            itemGuid.cn_s_siteinfo_xpos = siteinfo.cn_s_siteinfo_xpos;
            itemGuid.cn_s_siteinfo_ypos = siteinfo.cn_s_siteinfo_ypos;
            itemGuid.cn_s_siteinfo_isshow = siteinfo.cn_s_siteinfo_isshow;
            itemGuid.cn_s_siteinfo_lenght = siteinfo.cn_s_siteinfo_lenght;
            itemGuid.cn_s_siteinfo_width = siteinfo.cn_s_siteinfo_width;
            itemGuid.cn_s_siteinfo_height = siteinfo.cn_s_siteinfo_height;
            itemGuid.cn_s_siteinfo_remarks = siteinfo.cn_s_siteinfo_remarks;
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            itemGuid.cn_d_siteinfo_angle = siteinfo.cn_d_siteinfo_angle;
            itemGuid.cn_s_siteinfo_pileofland = siteinfo.cn_s_siteinfo_pileofland;
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用修改功能修改了一条tn_dts_siteinfo表中站点编号为" + siteinfo.cn_s_siteinfo_code + "的地堆记录，详细信息为" + JsonConvert.SerializeObject(siteinfo);
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_siteinfo>(itemGuid).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "4.3地堆（Siteinfo）货位管理修改删除批量设置接口修改地堆失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改成功！";
            return returnMessage;
        }
        #endregion

        #region 删除地堆
        /// <summary>
        /// 删除地堆
        /// </summary>
        /// <param name="siteinfoList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteSiteinfo(List<tn_dts_siteinfo> siteinfoList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var siteinfo in siteinfoList)
            {
                if (Db.Queryable<tn_dts_siteinfo>().Where(it => it.cn_guid == siteinfo.cn_guid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的地堆中有地堆的唯一标识不存在！";
                    return returnMessage;
                }
                tn_dts_logs log = new tn_dts_logs();
                log.cn_guid = Guid.NewGuid().ToString();
                log.cn_s_logs_type = "操作";
                log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能删除了一条tn_dts_siteinfo表中站点编号为" + siteinfo.cn_s_siteinfo_code + "的地堆记录，详细信息为" + JsonConvert.SerializeObject(siteinfo);
                log.cn_t_create = DateTime.Now;
                logList.Add(log);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_siteinfo>(siteinfoList).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "4.3地堆（Siteinfo）货位管理修改删除批量设置接口删除（批量）地堆失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        #region 批量设置地堆
        /// <summary>
        /// 批量设置地堆
        /// </summary>
        /// <param name="siteinfoList"></param>
        /// <returns></returns>
        public ReturnMessage BatchsettingSiteinfo(List<tn_dts_siteinfo> siteinfoList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            List<tn_dts_siteinfo> updatedSiteinfoList = new List<tn_dts_siteinfo>();
            foreach (var siteinfo in siteinfoList)
            {
                tn_dts_siteinfo itemGuid = Db.Queryable<tn_dts_siteinfo>().Where(it => it.cn_guid == siteinfo.cn_guid).First();
                if (itemGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的地堆中有地堆的唯一标识不存在！";
                    return returnMessage;
                }
                if(siteinfo.cn_d_siteinfo_angle < 0 || siteinfo.cn_d_siteinfo_angle > 360)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "规划角度只能从0取到360！";
                    return returnMessage;
                }
                itemGuid.cn_s_siteinfo_isshow = siteinfo.cn_s_siteinfo_isshow;
                itemGuid.cn_s_siteinfo_xpos = siteinfo.cn_s_siteinfo_xpos;
                itemGuid.cn_s_siteinfo_ypos = siteinfo.cn_s_siteinfo_ypos;
                itemGuid.cn_s_siteinfo_lenght = siteinfo.cn_s_siteinfo_lenght;
                itemGuid.cn_s_siteinfo_width = siteinfo.cn_s_siteinfo_width;
                itemGuid.cn_s_siteinfo_height = siteinfo.cn_s_siteinfo_height;
                itemGuid.cn_s_siteinfo_remarks = siteinfo.cn_s_siteinfo_remarks;
                itemGuid.cn_s_modify = user.UserCode;
                itemGuid.cn_s_modify_by = user.UserName;
                itemGuid.cn_t_modify = DateTime.Now;
                itemGuid.cn_d_siteinfo_angle = siteinfo.cn_d_siteinfo_angle;
                itemGuid.cn_s_siteinfo_pileofland = siteinfo.cn_s_siteinfo_pileofland;
                updatedSiteinfoList.Add(itemGuid);
                tn_dts_logs log = new tn_dts_logs();
                log.cn_guid = Guid.NewGuid().ToString();
                log.cn_s_logs_type = "操作";
                log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用批量设置功能修改了一条tn_dts_siteinfo表中站点编号为" + siteinfo.cn_s_siteinfo_code + "的地堆记录，详细信息为" + JsonConvert.SerializeObject(siteinfo);
                log.cn_t_create = DateTime.Now;
                logList.Add(log);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_siteinfo>(updatedSiteinfoList).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "4.3地堆（Siteinfo）货位管理修改删除批量设置接口批量设置地堆失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量设置失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量设置成功！";
            return returnMessage;
        }
        #endregion

    }
}
