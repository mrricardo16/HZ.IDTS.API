using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class GoodsequipmentService : BaseService<tn_dts_goodsequipment>, IGoodsequipmentService
    {
        public GoodsequipmentService(SessionInfo session) : base(session)
        {

        }

        #region 按货位设备、指令编码和指令名称混合模糊分页查询
        /// <summary>
        /// 按货位设备、指令编码和指令名称混合模糊分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<GoodscommandInformation> GetGoodscommandInformationPages(PageParm param)
        {
            string goodsequipGuid = param.Parms["cn_s_goodscommand_goodsequipguid"].ObjToString();
            string goodscommandNoOrName = param.Parms["cn_s_goodscommand_no_or_name"].ObjToString();
            string haswildcard = param.Parms["cn_n_goodscommand_haswildcard"].ObjToString();
            List<tn_dts_goodscommand> goodscommandList = Db.Queryable<tn_dts_goodscommand>()
                .WhereIF(!string.IsNullOrEmpty(goodsequipGuid), it => it.cn_s_goodscommand_goodsequipguid == goodsequipGuid)
                .WhereIF(!string.IsNullOrEmpty(goodscommandNoOrName), it => it.cn_s_goodscommand_no.Contains(goodscommandNoOrName) || it.cn_s_goodscommand_name.Contains(goodscommandNoOrName))
                .WhereIF(!string.IsNullOrEmpty(haswildcard), it => it.cn_n_goodscommand_haswildcard == int.Parse(haswildcard))
                .Where(it => it.cn_s_goodscommand_type == "执行")
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy).ToList();
            List<GoodscommandInformation> goodscommandInformationList = new List<GoodscommandInformation>();
            foreach (var goodscommand in goodscommandList)
            {
                GoodscommandInformation goodscommandInformation = new GoodscommandInformation
                {
                    GoodscommandGuid = goodscommand.cn_guid,
                    GoodscommandNo = goodscommand.cn_s_goodscommand_no,
                    GoodscommandName = goodscommand.cn_s_goodscommand_name
                };
                goodscommandInformationList.Add(goodscommandInformation);
            }
            return goodscommandInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 获取所有货位设备信息
        /// <summary>
        /// 获取所有货位设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<GoodsequipmentInformation> GetAllGoodsequipmentInformation(PageParm param)
        {
            List<tn_dts_goodsequipment> goodsequipmentList = Db.Queryable<tn_dts_goodsequipment>()
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy).ToList();
            List<GoodsequipmentInformation> goodsequipmentInformationList = new List<GoodsequipmentInformation>();
            foreach (var goodsequipment in goodsequipmentList)
            {
                GoodsequipmentInformation goodsequipmentInformation = new GoodsequipmentInformation
                {
                    GoodsequipmentGuid = goodsequipment.cn_guid,
                    GoodsequipmentNo = goodsequipment.cn_s_goodsequipment_no,
                    GoodsequipmentName = goodsequipment.cn_s_goodsequipment_name
                };
                goodsequipmentInformationList.Add(goodsequipmentInformation);
            }
            return goodsequipmentInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 按货位设备编码和名称混合模糊分页查询
        /// <summary>
        /// 按货位设备类型、货位设备编码和名称混合模糊分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_goodsequipment> GetListPages(PageParm parm)
        {
            string goodsequipmentNoOrName = parm.Parms["cn_s_goodsequipment_no_or_name"].ObjToString();
            string goodsequipmentType = parm.Parms["cn_s_goodsequipment_type"].ObjToString();
            return Db.Queryable<tn_dts_goodsequipment>()
                .WhereIF(!string.IsNullOrEmpty(goodsequipmentNoOrName), it => it.cn_s_goodsequipment_no.Contains(goodsequipmentNoOrName) || it.cn_s_goodsequipment_name.Contains(goodsequipmentNoOrName))
                .WhereIF(!string.IsNullOrEmpty(goodsequipmentType), it => it.cn_s_goodsequipment_type == goodsequipmentType)
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 获取指定货位设备信息
        /// <summary>
        /// 获取指定货位设备信息
        /// </summary>
        /// <param name="goodsequipmentguid"></param>
        /// <returns></returns>
        public tn_dts_goodsequipment GetGoodsequipment(string goodsequipmentguid)
        {
            return Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_guid == goodsequipmentguid).First();
        }
        #endregion

        #region 保存货位设备
        /// <summary>
        /// 保存货位设备
        /// </summary>
        /// <param name="saveGoodsequipment"></param>
        /// <returns></returns>
        public ReturnMessage SaveGoodsequipment(SaveGoodsequipment saveGoodsequipment)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_goodsequipment inGoodsequipment = saveGoodsequipment.goodsequipment;
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            if (saveGoodsequipment.AddOrModify == "add")
            {
                if (!(Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_s_goodsequipment_no == inGoodsequipment.cn_s_goodsequipment_no).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "货位设备编码不能重复，请重试！";
                    return returnMessage;
                }
                string goodsequipmentGuid = Guid.NewGuid().ToString();
                tn_dts_goodsequipment goodsequipment = new tn_dts_goodsequipment();
                goodsequipment.cn_guid = goodsequipmentGuid;
                goodsequipment.cn_s_goodsequipment_type = inGoodsequipment.cn_s_goodsequipment_type;
                goodsequipment.cn_s_goodsequipment_stockcode = inGoodsequipment.cn_s_goodsequipment_stockcode;
                goodsequipment.cn_s_goodsequipment_stockname = inGoodsequipment.cn_s_goodsequipment_stockname;
                goodsequipment.cn_s_goodsequipment_areacode = inGoodsequipment.cn_s_goodsequipment_areacode;
                goodsequipment.cn_s_goodsequipment_areaname = inGoodsequipment.cn_s_goodsequipment_areaname;
                goodsequipment.cn_s_goodsequipment_locationcode = inGoodsequipment.cn_s_goodsequipment_locationcode;
                goodsequipment.cn_s_goodsequipment_locationname = inGoodsequipment.cn_s_goodsequipment_locationname;
                goodsequipment.cn_s_goodsequipment_no = inGoodsequipment.cn_s_goodsequipment_no;
                goodsequipment.cn_s_goodsequipment_name = inGoodsequipment.cn_s_goodsequipment_name;
                goodsequipment.cn_s_creator = user.UserCode;
                goodsequipment.cn_s_creator_by = user.UserName;
                goodsequipment.cn_t_create = DateTime.Now;
                tn_dts_logs goodsequipmentLog = new tn_dts_logs();
                goodsequipmentLog.cn_guid = Guid.NewGuid().ToString();
                goodsequipmentLog.cn_s_logs_type = "操作";
                goodsequipmentLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存货位设备功能新增了一条货位设备记录，详细信息：" + JsonConvert.SerializeObject(goodsequipment);
                goodsequipmentLog.cn_t_create = DateTime.Now;
                logList.Add(goodsequipmentLog);
                tn_dts_goodscommand goodscommand = new tn_dts_goodscommand();
                goodscommand.cn_guid = Guid.NewGuid().ToString();
                goodscommand.cn_s_goodscommand_goodsequipguid = goodsequipmentGuid;
                goodscommand.cn_s_goodscommand_no = (inGoodsequipment.cn_s_goodsequipment_no + "_initialization").Length > 32 ? (inGoodsequipment.cn_s_goodsequipment_no + "_initialization").Substring(0, 32) : inGoodsequipment.cn_s_goodsequipment_no + "_initialization";
                goodscommand.cn_s_goodscommand_name = (inGoodsequipment.cn_s_goodsequipment_name + "_初始化").Length > 64 ? (inGoodsequipment.cn_s_goodsequipment_name + "_初始化").Substring(0, 64) : inGoodsequipment.cn_s_goodsequipment_name + "_初始化";
                goodscommand.cn_s_goodscommand_type = "初始化";
                goodscommand.cn_n_goodscommand_haswildcard = 0;
                goodscommand.cn_s_goodscommand_json = "{\"stock\": []}";
                goodscommand.cn_s_creator = user.UserCode;
                goodscommand.cn_s_creator_by = user.UserName;
                goodscommand.cn_t_create = DateTime.Now;
                tn_dts_logs goodscommandLog = new tn_dts_logs();
                goodscommandLog.cn_guid = Guid.NewGuid().ToString();
                goodscommandLog.cn_s_logs_type = "操作";
                goodscommandLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存货位设备功能新增了一条初始化货位指令，详细信息：" + JsonConvert.SerializeObject(goodscommand);
                goodscommandLog.cn_t_create = DateTime.Now;
                logList.Add(goodscommandLog);
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_goodsequipment>(goodsequipment).ExecuteCommand();
                    dbTran.Insertable<tn_dts_goodscommand>(goodscommand).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Error("2.4货位指令（Goodsequipment）管理保存货位设备失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败！";
                    return returnMessage;
                }
            }
            else if (saveGoodsequipment.AddOrModify == "modify")
            {
                tn_dts_goodsequipment goodsequipmentGuid = Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_guid == inGoodsequipment.cn_guid).First();
                if (goodsequipmentGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "tn_dts_goodsequipment表中找不到唯一标识为：" + inGoodsequipment.cn_guid + "的货位设备记录，请检查后重试！";
                    return returnMessage;
                }
                if (!(Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_s_goodsequipment_no == inGoodsequipment.cn_s_goodsequipment_no && it.cn_guid != inGoodsequipment.cn_guid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "货位设备编码不能重复，请重试！";
                    return returnMessage;
                }
                goodsequipmentGuid.cn_s_goodsequipment_type = inGoodsequipment.cn_s_goodsequipment_type;
                goodsequipmentGuid.cn_s_goodsequipment_stockcode = inGoodsequipment.cn_s_goodsequipment_stockcode;
                goodsequipmentGuid.cn_s_goodsequipment_stockname = inGoodsequipment.cn_s_goodsequipment_stockname;
                goodsequipmentGuid.cn_s_goodsequipment_areacode = inGoodsequipment.cn_s_goodsequipment_areacode;
                goodsequipmentGuid.cn_s_goodsequipment_areaname = inGoodsequipment.cn_s_goodsequipment_areaname;
                goodsequipmentGuid.cn_s_goodsequipment_locationcode = inGoodsequipment.cn_s_goodsequipment_locationcode;
                goodsequipmentGuid.cn_s_goodsequipment_locationname = inGoodsequipment.cn_s_goodsequipment_locationname;
                goodsequipmentGuid.cn_s_goodsequipment_no = inGoodsequipment.cn_s_goodsequipment_no;
                goodsequipmentGuid.cn_s_goodsequipment_name = inGoodsequipment.cn_s_goodsequipment_name;
                goodsequipmentGuid.cn_s_modify = user.UserCode;
                goodsequipmentGuid.cn_s_modify_by = user.UserName;
                goodsequipmentGuid.cn_t_modify = DateTime.Now;
                tn_dts_logs goodsequipmentLog = new tn_dts_logs();
                goodsequipmentLog.cn_guid = Guid.NewGuid().ToString();
                goodsequipmentLog.cn_s_logs_type = "操作";
                goodsequipmentLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存货位设备功能修改了一条货位设备记录，详细信息：" + JsonConvert.SerializeObject(goodsequipmentGuid);
                goodsequipmentLog.cn_t_create = DateTime.Now;
                logList.Add(goodsequipmentLog);
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Updateable<tn_dts_goodsequipment>(goodsequipmentGuid).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Error("2.4货位指令（Goodsequipment）管理保存货位设备失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败！";
                    return returnMessage;
                }
            }
            else
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的AddOrModify参数只能为add或modify！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "保存成功！";
            return returnMessage;
        }
        #endregion

        #region 批量删除货位设备
        ///// <summary>
        ///// 批量删除货位设备
        ///// </summary>
        ///// <param name="guidList"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteGoodsequipment(List<string> guidList)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    List<tn_dts_goodsequipment> goodsequipmentList = new List<tn_dts_goodsequipment>();
        //    List<tn_dts_goodscommand> goodscommandList = new List<tn_dts_goodscommand>();
        //    List<tn_dts_logs> logList = new List<tn_dts_logs>();
        //    foreach (var guid in guidList)
        //    {
        //        tn_dts_goodsequipment goodsequipmentGuid = Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_guid == guid).First();
        //        if (goodsequipmentGuid is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "选中的货位设备中，有设备的唯一标识在tn_dts_goodsequipment表中不存在，请检查后重试！";
        //            return returnMessage;
        //        }
        //        goodsequipmentList.Add(goodsequipmentGuid);
        //        tn_dts_logs goodsequipmentLog = new tn_dts_logs();
        //        goodsequipmentLog.cn_guid = Guid.NewGuid().ToString();
        //        goodsequipmentLog.cn_s_logs_type = "操作";
        //        goodsequipmentLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用批量删除货位设备功能删除了一条货位设备记录，详细信息为：" + JsonConvert.SerializeObject(goodsequipmentGuid);
        //        goodsequipmentLog.cn_t_create = DateTime.Now;
        //        logList.Add(goodsequipmentLog);
        //        List<tn_dts_goodscommand> equicommandGoodsequipguidList = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_s_goodscommand_goodsequipguid == guid).ToList();
        //        goodscommandList.AddRange(equicommandGoodsequipguidList);
        //        foreach (var equicommandGoodsequipguid in equicommandGoodsequipguidList)
        //        {
        //            tn_dts_logs goodscommandLog = new tn_dts_logs();
        //            goodscommandLog.cn_guid = Guid.NewGuid().ToString();
        //            goodscommandLog.cn_s_logs_type = "操作";
        //            goodscommandLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用批量删除货位设备功能删除了一条货位指令，详细信息为：" + JsonConvert.SerializeObject(equicommandGoodsequipguid);
        //            goodscommandLog.cn_t_create = DateTime.Now;
        //            logList.Add(goodscommandLog);
        //        }
        //    }
        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        dbTran.Deleteable<tn_dts_goodsequipment>(goodsequipmentList).ExecuteCommand();
        //        dbTran.Deleteable<tn_dts_goodscommand>(goodscommandList).ExecuteCommand();
        //        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        LogHelper.Error("2.5货位指令（Goodsequipment）管理批量删除货位设备失败，详细信息为：" + res.Message);
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "批量删除失败！";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "批量删除成功！";
        //    return returnMessage;
        //}

        /// <summary>
        /// 批量删除货位设备
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteGoodsequipment(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用批量删除货位设备功能删除了" + guidList.Count + "条货位设备记录及其对应的所有货位指令";
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_goodsequipment>().In(guidList).ExecuteCommand();
                dbTran.Deleteable<tn_dts_goodscommand>().In(it => it.cn_s_goodscommand_goodsequipguid, guidList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("2.5货位指令（Goodsequipment）管理批量删除货位设备失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量删除成功！";
            return returnMessage;
        }
        #endregion

        #region 获取一个货位设备类型下所有货位设备信息
        /// <summary>
        /// 获取一个货位设备类型下所有货位设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<GoodsequipmentInformation> GetGoodsequipmentInformationByGoodsequitype(PageParm param)
        {
            string goodscommandType = param.Parms["cn_s_goodsequipment_type"].ObjToString();
            return Db.Queryable<tn_dts_goodsequipment>().Where(it => it.cn_s_goodsequipment_type == goodscommandType)
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                .Select(it => new GoodsequipmentInformation
                {
                    GoodsequipmentGuid = it.cn_guid,
                    GoodsequipmentNo = it.cn_s_goodsequipment_no,
                    GoodsequipmentName = it.cn_s_goodsequipment_name
                }).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

    }
}
