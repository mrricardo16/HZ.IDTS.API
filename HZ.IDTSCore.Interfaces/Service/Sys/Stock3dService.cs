using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Sys;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.MongoDB;
using HZ.IDTSCore.Model.Entity.Sys;
using HZ.iWCS.MData.Core;
using MongoDB.Driver;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public class Stock3dService : BaseService<tn_dts_stock3d>, IStock3dService
    {
        public Stock3dService(SessionInfo session) : base(session)
        {

        }

        #region 分页模糊查询区域编号和区域名称
        /// <summary>
        /// 分页模糊查询区域编号和区域名称
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_stock3d> GetListPages(PageParm parm)
        {
            string cn_s_location_stockname = parm.Parms["cn_s_location_stockname"].ObjToString();
            string cn_s_location_areaname = parm.Parms["cn_s_location_areaname"].ObjToString();
            return Db.Queryable<tn_dts_stock3d>().WhereIF(!string.IsNullOrEmpty(cn_s_location_stockname), (s => s.cn_s_location_stockname.Contains(cn_s_location_stockname)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_location_areaname), (s => s.cn_s_location_areaname.Contains(cn_s_location_areaname)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除货位
        /// <summary>
        /// 删除货位
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_stock3d>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 获取指定仓库，指定区域的报废货位列表字符串
        /// <summary>
        /// 获取指定仓库，指定区域的报废货位列表字符串
        /// </summary>
        /// <param name="stockcode"></param>
        /// <param name="areacode"></param>
        /// <returns></returns>
        public string GetNullify(string stockcode, string areacode)
        {
            var filter = Builders<LocationSiteInformation>.Filter.Where(it => it.stockCode == stockcode && it.area_code == areacode && it.type == "货位" && it.location_state == "报废");
            List<LocationSiteInformation> locationSiteInformationList = MongoDBSingleton.Instance.FindList<LocationSiteInformation>(filter);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var locationSiteInformation in locationSiteInformationList)
            {
                string locationCode = "\"" + locationSiteInformation.locationCode + "\",";
                sb.Append(locationCode);
            }
            if (sb[sb.Length - 1] == ',')
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            return sb.ToString();
        }
        #endregion

        #region 新增立库
        /// <summary>
        /// 新增立库
        /// </summary>
        /// <param name="stock3d">要新增的立库数据</param>
        /// <returns></returns>
        public ReturnMessage AddStock3d(tn_dts_stock3d stock3d)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            if (!(Db.Queryable<tn_dts_stock3d>().Where(it => it.cn_s_location_stockcode == stock3d.cn_s_location_stockcode
                 && it.cn_s_location_stockname == stock3d.cn_s_location_stockname && it.cn_s_location_areacode == stock3d.cn_s_location_areacode
                 && it.cn_s_location_areaname == stock3d.cn_s_location_areaname && it.cn_s_location_row == stock3d.cn_s_location_row
                 && it.cn_s_location_col == stock3d.cn_s_location_col && it.cn_s_location_layer == stock3d.cn_s_location_layer).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "相同仓库相同区域的相同排列层已有立库，新增失败！";
                return returnMessage;
            }
            stock3d.cn_guid = Guid.NewGuid().ToString();
            stock3d.cn_s_location_nullify = GetNullify(stock3d.cn_s_location_stockcode, stock3d.cn_s_location_areacode);
            stock3d.cn_s_creator = user.UserCode;
            stock3d.cn_s_creator_by = user.UserName;
            stock3d.cn_t_create = DateTime.Now;
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_stock3d表中新增一条立库记录，详细信息为" + JsonConvert.SerializeObject(stock3d);
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_stock3d>(stock3d).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口新增立库失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "新增失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "新增成功!";
            return returnMessage;
        }
        #endregion

        #region 修改立库
        /// <summary>
        /// 修改立库
        /// </summary>
        /// <param name="stock3d"></param>
        /// <returns></returns>
        public ReturnMessage UpdateStock3d(tn_dts_stock3d stock3d)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_stock3d itemGuid = Db.Queryable<tn_dts_stock3d>().Where(it => it.cn_guid == stock3d.cn_guid).First();
            if (itemGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "未找到唯一标识为" + stock3d.cn_guid + "的立库信息，请重试！";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_stock3d>().Where(it => it.cn_s_location_stockcode == stock3d.cn_s_location_stockcode
                 && it.cn_s_location_stockname == stock3d.cn_s_location_stockname && it.cn_s_location_areacode == stock3d.cn_s_location_areacode
                 && it.cn_s_location_areaname == stock3d.cn_s_location_areaname && it.cn_s_location_row == stock3d.cn_s_location_row
                 && it.cn_s_location_col == stock3d.cn_s_location_col && it.cn_s_location_layer == stock3d.cn_s_location_layer && it.cn_guid != stock3d.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "相同仓库相同区域的相同排列层不能有多个立库,修改失败！";
                return returnMessage;
            }
            itemGuid.cn_s_location_stockcode = stock3d.cn_s_location_stockcode;
            itemGuid.cn_s_location_stockname = stock3d.cn_s_location_stockname;
            itemGuid.cn_s_location_areacode = stock3d.cn_s_location_areacode;
            itemGuid.cn_s_location_areaname = stock3d.cn_s_location_areaname;
            itemGuid.cn_s_location_isshow = stock3d.cn_s_location_isshow;
            itemGuid.cn_s_location_row = stock3d.cn_s_location_row;
            itemGuid.cn_s_location_col = stock3d.cn_s_location_col;
            itemGuid.cn_s_location_layer = stock3d.cn_s_location_layer;
            itemGuid.cn_s_location_length = stock3d.cn_s_location_length;
            itemGuid.cn_s_location_height = stock3d.cn_s_location_height;
            itemGuid.cn_s_location_width = stock3d.cn_s_location_width;
            itemGuid.cn_s_location_xpos = stock3d.cn_s_location_xpos;
            itemGuid.cn_s_location_ypos = stock3d.cn_s_location_ypos;
            itemGuid.cn_s_location_nullify = stock3d.cn_s_location_nullify;
            itemGuid.cn_s_location_gap = stock3d.cn_s_location_gap;
            itemGuid.cn_s_location_remarks = stock3d.cn_s_location_remarks;
            itemGuid.cn_s_modify = user.UserCode;
            itemGuid.cn_s_modify_by = user.UserName;
            itemGuid.cn_t_modify = DateTime.Now;
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_stock3d表中修改一条立库记录，详细信息为" + JsonConvert.SerializeObject(stock3d);
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_stock3d>(itemGuid).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口修改立库失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改成功!";
            return returnMessage;
        }
        #endregion

        #region 删除立库
        /// <summary>
        /// 删除立库
        /// </summary>
        /// <param name="stock3d"></param>
        /// <returns></returns>
        public ReturnMessage DeleteStock3d(List<tn_dts_stock3d> stock3dList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();

            foreach (var stock3d in stock3dList)
            {
                if (Db.Queryable<tn_dts_stock3d>().Where(it => it.cn_guid == stock3d.cn_guid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的立库中有立库的唯一标识不存在！";
                    return returnMessage;
                }
                tn_dts_logs log = new tn_dts_logs();
                log.cn_guid = Guid.NewGuid().ToString();
                log.cn_s_logs_type = "操作";
                log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户向tn_dts_stock3d表中删除了唯一标识为：" + stock3d.cn_guid + "的立库记录。";
                log.cn_t_create = DateTime.Now;
                logList.Add(log);
            }
            
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_stock3d>(stock3dList).ExecuteCommand();

                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "3.4立库（stock3d）结构管理增加修改删除接口删除立库失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败!";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功!";
            return returnMessage;
        }
        #endregion
    }
}
