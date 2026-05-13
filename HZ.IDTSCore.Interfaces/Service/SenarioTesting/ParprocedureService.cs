using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HZ.IDTSCore.Model.Entity.location;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using HZ.CommonUtil.Helpers;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class ParprocedureService : BaseService<tn_dts_parprocedure>, IParprocedureService
    {
        public ParprocedureService(SessionInfo session) : base(session)
        {

        }

        #region 按流程编码和流程名称混合模糊分页查询流程信息
        /// <summary>
        /// 按流程编码和流程名称混合模糊分页查询流程信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<ParprocedureInformation> GetProcedure(PageParm param)
        {
            string parprocedureNoOrName = param.Parms["cn_s_parprocedure_no_or_name"].ObjToString();
            List<tn_dts_parprocedure> parprocedureList = Db.Queryable<tn_dts_parprocedure>()
                .WhereIF(!string.IsNullOrEmpty(parprocedureNoOrName), it => it.cn_s_parprocedure_no.Contains(parprocedureNoOrName) || it.cn_s_parprocedure_name.Contains(parprocedureNoOrName))
               .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
               .ToList();
            return parprocedureList.Select(it => new ParprocedureInformation
            {
                ParprocedureGuid = it.cn_guid,
                ParprocedureNo = it.cn_s_parprocedure_no,
                ParprocedureName = it.cn_s_parprocedure_name
            }).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 按流程编码和流程名称分页模糊查询
        /// <summary>
        /// 按流程编码和流程名称分页模糊查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_parprocedure> GetListPages(PageParm param)
        {
            string parprocedureNo = param.Parms["cn_s_parprocedure_no"].ObjToString();
            string parprocedureName = param.Parms["cn_s_parprocedure_name"].ObjToString();
            return Db.Queryable<tn_dts_parprocedure>()
                .WhereIF(!string.IsNullOrEmpty(parprocedureNo), it => it.cn_s_parprocedure_no.Contains(parprocedureNo))
                .WhereIF(!string.IsNullOrEmpty(parprocedureName), it => it.cn_s_parprocedure_name.Contains(parprocedureName))
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                .ToPage(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 按（立库/地堆）编码或（立库/地堆）名称混合模糊分页查询（立库/地堆）信息
        /// <summary>
        /// 按（立库/地堆）编码或（立库/地堆）名称混合模糊分页查询（立库/地堆）信息
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<StandardInformation> GetStockSite(PageParm param)
        {
            string parprocedureType = param.Parms["cn_s_parprocedure_type"].ObjToString();
            string parprocedureNoOrName = param.Parms["cn_s_parprocedure_no_or_name"].ObjToString();
            if (parprocedureType == "立库")
            {
                List<tn_dts_stock3d> stock3dList = Db.Queryable<tn_dts_stock3d>()
                .WhereIF(!string.IsNullOrEmpty(parprocedureNoOrName), it => it.cn_s_location_areacode.Contains(parprocedureNoOrName) || it.cn_s_location_areaname.Contains(parprocedureNoOrName))
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                .ToList();
                return stock3dList.Select(it => new StandardInformation
                {
                    Guid = it.cn_guid,
                    No = it.cn_s_location_areacode,
                    Name = it.cn_s_location_areaname
                }).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
            }
            else if (parprocedureType == "地堆")
            {
                List<tn_dts_siteinfo> siteinfoList = Db.Queryable<tn_dts_siteinfo>()
                    .WhereIF(!string.IsNullOrEmpty(parprocedureNoOrName), it => it.cn_s_siteinfo_code.Contains(parprocedureNoOrName) || it.cn_s_siteinfo_name.Contains(parprocedureNoOrName))
                    .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                    .ToList();
                return siteinfoList.Select(it => new StandardInformation
                {
                    Guid = it.cn_guid,
                    No = it.cn_s_siteinfo_code,
                    Name = it.cn_s_siteinfo_name
                }).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
            }
            else
            {
                List<StandardInformation> standardInformationList = new List<StandardInformation>();
                return standardInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
            }
        }
        #endregion

        #region 保存流程
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="saveProcedureDate"></param>
        /// <returns></returns>
        public ReturnMessage SaveProcedure(SaveProcedureDate saveProcedureDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();

            if (saveProcedureDate.AddOrModify == "add")
            {
                string parprocedureGuid = Guid.NewGuid().ToString();
                if (!(Db.Queryable<tn_dts_parprocedure>().Where(it => it.cn_s_parprocedure_no == saveProcedureDate.ParprocedureNo).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "流程编码不能重复，请重试！";
                    return returnMessage;
                }
                tn_dts_parprocedure parprocedure = new tn_dts_parprocedure();
                parprocedure.cn_guid = parprocedureGuid;
                parprocedure.cn_s_parprocedure_no = saveProcedureDate.ParprocedureNo;
                parprocedure.cn_s_parprocedure_name = saveProcedureDate.ParprocedureName;
                parprocedure.cn_s_creator = user.UserCode;
                parprocedure.cn_s_creator_by = user.UserName;
                parprocedure.cn_t_create = DateTime.Now;
                tn_dts_logs parprocedureLog = new tn_dts_logs();
                parprocedureLog.cn_guid = Guid.NewGuid().ToString();
                parprocedureLog.cn_s_logs_type = "操作";
                parprocedureLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存流程功能新增了一条流程记录，详细信息：" + JsonConvert.SerializeObject(parprocedure);
                parprocedureLog.cn_t_create = DateTime.Now;
                logList.Add(parprocedureLog);
                List<tn_dts_chiprocedure> chiprocedureList = new List<tn_dts_chiprocedure>();
                foreach (var chiprocedureInformation in saveProcedureDate.ChiprocedureInformationList)
                {
                    tn_dts_chiprocedure chiprocedure = new tn_dts_chiprocedure();
                    chiprocedure.cn_guid = Guid.NewGuid().ToString();
                    chiprocedure.cn_s_chiprocedure_parproguid = parprocedureGuid;
                    chiprocedure.cn_s_chiprocedure_equiguid = chiprocedureInformation.EquipmentGuid;
                    chiprocedure.cn_s_chiprocedure_startguid = chiprocedureInformation.StartGuid;
                    chiprocedure.cn_s_chiprocedure_startcategory = chiprocedureInformation.StartType;
                    chiprocedure.cn_s_chiprocedure_endguid = chiprocedureInformation.EndGuid;
                    chiprocedure.cn_s_chiprocedure_endcategory = chiprocedureInformation.EndType;
                    chiprocedure.cn_n_chiprocedure_sequence = chiprocedureInformation.ChiprocedureSequence;
                    chiprocedure.cn_n_chiprocedure_interval = chiprocedureInformation.ChiprocedureInterval;
                    chiprocedure.cn_s_creator = user.UserCode;
                    chiprocedure.cn_s_creator_by = user.UserName;
                    chiprocedure.cn_t_create = DateTime.Now;
                    chiprocedureList.Add(chiprocedure);
                    tn_dts_logs chiprocedureLog = new tn_dts_logs();
                    chiprocedureLog.cn_guid = Guid.NewGuid().ToString();
                    chiprocedureLog.cn_s_logs_type = "操作";
                    chiprocedureLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存流程功能新增了一条流程-设备关系，详细信息：" + JsonConvert.SerializeObject(chiprocedure);
                    chiprocedureLog.cn_t_create = DateTime.Now;
                    logList.Add(chiprocedureLog);
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_parprocedure>(parprocedure).ExecuteCommand();
                    dbTran.Insertable<tn_dts_chiprocedure>(chiprocedureList).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Error("3.5流程设备（Parprocedure）管理保存流程失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败！";
                    return returnMessage;
                }
            }
            else if (saveProcedureDate.AddOrModify == "modify")
            {
                tn_dts_parprocedure parprocedureGuid = Db.Queryable<tn_dts_parprocedure>().Where(it => it.cn_guid == saveProcedureDate.ParprocedureGuid).First();
                if (parprocedureGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "tn_dts_parprocedure表中找不到唯一标识为：" + saveProcedureDate.ParprocedureGuid + "的流程记录，请检查后重试！";
                    return returnMessage;
                }
                if (!(Db.Queryable<tn_dts_parprocedure>().Where(it => it.cn_s_parprocedure_no == saveProcedureDate.ParprocedureNo && it.cn_guid != saveProcedureDate.ParprocedureGuid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "流程编码不能重复，请重试！";
                    return returnMessage;
                }
                parprocedureGuid.cn_s_parprocedure_no = saveProcedureDate.ParprocedureNo;
                parprocedureGuid.cn_s_parprocedure_name = saveProcedureDate.ParprocedureName;
                parprocedureGuid.cn_s_modify = user.UserCode;
                parprocedureGuid.cn_s_modify_by = user.UserName;
                parprocedureGuid.cn_t_modify = DateTime.Now;
                tn_dts_logs parprocedureLog = new tn_dts_logs();
                parprocedureLog.cn_guid = Guid.NewGuid().ToString();
                parprocedureLog.cn_s_logs_type = "操作";
                parprocedureLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存流程功能修改了一条流程记录，详细信息为：" + JsonConvert.SerializeObject(parprocedureGuid);
                parprocedureLog.cn_t_create = DateTime.Now;
                logList.Add(parprocedureLog);
                bool isAdd = false;
                bool isModify = false;
                bool isDelete = false;
                List<tn_dts_chiprocedure> chiprocedureAddList = new List<tn_dts_chiprocedure>();
                List<tn_dts_chiprocedure> chiprocedureModifyList = new List<tn_dts_chiprocedure>();
                List<tn_dts_chiprocedure> chiprocedureDeleteList = new List<tn_dts_chiprocedure>();
                List<tn_dts_chiprocedure> chiprocedureParproguidList = Db.Queryable<tn_dts_chiprocedure>().Where(it => it.cn_s_chiprocedure_parproguid == saveProcedureDate.ParprocedureGuid).ToList();
                List<string> deleteChiprocedureGuidList = saveProcedureDate.DeleteChiprocedureGuidList;
                if (deleteChiprocedureGuidList.Count != 0)
                {
                    //删除
                    foreach (var deleteChiprocedureGuid in deleteChiprocedureGuidList)
                    {
                        tn_dts_chiprocedure chiprocedureGuid = Db.Queryable<tn_dts_chiprocedure>().Where(it => it.cn_guid == deleteChiprocedureGuid).First();
                        if (chiprocedureGuid is null)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "传入的删除列表中唯一标识为：" + deleteChiprocedureGuid + "的流程-设备关系在tn_dts_chiprocedure表中不存在，请检查后重试！";
                            return returnMessage;
                        }
                        chiprocedureDeleteList.Add(chiprocedureGuid);
                        tn_dts_logs deleteChiprocedureLog = new tn_dts_logs();
                        deleteChiprocedureLog.cn_guid = Guid.NewGuid().ToString();
                        deleteChiprocedureLog.cn_s_logs_type = "操作";
                        deleteChiprocedureLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用保存流程功能删除了一条流程-设备关系，详细信息为：" + JsonConvert.SerializeObject(chiprocedureGuid);
                        deleteChiprocedureLog.cn_t_create = DateTime.Now;
                        logList.Add(deleteChiprocedureLog);
                    }
                    isDelete = true;
                }
                foreach (var chiprocedureInformation in saveProcedureDate.ChiprocedureInformationList)
                {
                    if (chiprocedureParproguidList.Where(it => it.cn_guid == chiprocedureInformation.ChiprocedureGuid).ToList().Count != 0)
                    {
                        //修改
                        tn_dts_chiprocedure chiprocedureGuid = Db.Queryable<tn_dts_chiprocedure>().Where(it => it.cn_guid == chiprocedureInformation.ChiprocedureGuid).First();
                        chiprocedureGuid.cn_s_chiprocedure_startguid = chiprocedureInformation.StartGuid;
                        chiprocedureGuid.cn_s_chiprocedure_startcategory = chiprocedureInformation.StartType;
                        chiprocedureGuid.cn_s_chiprocedure_endguid = chiprocedureInformation.EndGuid;
                        chiprocedureGuid.cn_s_chiprocedure_endcategory = chiprocedureInformation.EndType;
                        chiprocedureGuid.cn_n_chiprocedure_sequence = chiprocedureInformation.ChiprocedureSequence;
                        chiprocedureGuid.cn_n_chiprocedure_interval = chiprocedureInformation.ChiprocedureInterval;
                        chiprocedureGuid.cn_s_modify = user.UserCode;
                        chiprocedureGuid.cn_s_modify_by = user.UserName;
                        chiprocedureGuid.cn_t_modify = DateTime.Now;
                        isModify = true;
                        chiprocedureModifyList.Add(chiprocedureGuid);
                        tn_dts_logs modifyChiprocedureLog = new tn_dts_logs();
                        modifyChiprocedureLog.cn_guid = Guid.NewGuid().ToString();
                        modifyChiprocedureLog.cn_s_logs_type = "操作";
                        modifyChiprocedureLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存流程功能修改了一条流程-设备关系，详细信息为：" + JsonConvert.SerializeObject(chiprocedureGuid);
                        modifyChiprocedureLog.cn_t_create = DateTime.Now;
                        logList.Add(modifyChiprocedureLog);
                    }
                    else
                    {
                        //新增
                        tn_dts_chiprocedure chiprocedure = new tn_dts_chiprocedure();
                        chiprocedure.cn_guid = Guid.NewGuid().ToString();
                        chiprocedure.cn_s_chiprocedure_parproguid = saveProcedureDate.ParprocedureGuid;
                        chiprocedure.cn_s_chiprocedure_equiguid = chiprocedureInformation.EquipmentGuid;
                        chiprocedure.cn_s_chiprocedure_startguid = chiprocedureInformation.StartGuid;
                        chiprocedure.cn_s_chiprocedure_startcategory = chiprocedureInformation.StartType;
                        chiprocedure.cn_s_chiprocedure_endguid = chiprocedureInformation.EndGuid;
                        chiprocedure.cn_s_chiprocedure_endcategory = chiprocedureInformation.EndType;
                        chiprocedure.cn_n_chiprocedure_sequence = chiprocedureInformation.ChiprocedureSequence;
                        chiprocedure.cn_n_chiprocedure_interval = chiprocedureInformation.ChiprocedureInterval;
                        chiprocedure.cn_s_creator = user.UserCode;
                        chiprocedure.cn_s_creator_by = user.UserName;
                        chiprocedure.cn_t_create = DateTime.Now;
                        isAdd = true;
                        chiprocedureAddList.Add(chiprocedure);
                        tn_dts_logs addChiprocedureLog = new tn_dts_logs();
                        addChiprocedureLog.cn_guid = Guid.NewGuid().ToString();
                        addChiprocedureLog.cn_s_logs_type = "操作";
                        addChiprocedureLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用保存流程功能新增了一条流程-设备关系，详细信息为：" + JsonConvert.SerializeObject(chiprocedure);
                        addChiprocedureLog.cn_t_create = DateTime.Now;
                        logList.Add(addChiprocedureLog);
                    }
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Updateable<tn_dts_parprocedure>(parprocedureGuid).ExecuteCommand();
                    if (isAdd)
                    {
                        dbTran.Insertable<tn_dts_chiprocedure>(chiprocedureAddList).ExecuteCommand();
                    }
                    if (isModify)
                    {
                        dbTran.Updateable<tn_dts_chiprocedure>(chiprocedureModifyList).ExecuteCommand();
                    }
                    if (isDelete)
                    {
                        dbTran.Deleteable<tn_dts_chiprocedure>(chiprocedureDeleteList).ExecuteCommand();
                    }
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Error("3.5流程设备（Parprocedure）管理保存流程失败，详细信息为：" + res.Message);
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

        #region 批量删除流程
        ///// <summary>
        ///// 批量删除流程
        ///// </summary>
        ///// <param name="guidList"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteProcedure(List<string> guidList)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    List<tn_dts_parprocedure> parprocedureGuidList = new List<tn_dts_parprocedure>();
        //    List<tn_dts_chiprocedure> chiprocedureList = new List<tn_dts_chiprocedure>();
        //    List<tn_dts_logs> logList = new List<tn_dts_logs>();
        //    foreach (var guid in guidList)
        //    {
        //        tn_dts_parprocedure parprocedureGuid = Db.Queryable<tn_dts_parprocedure>().Where(it => it.cn_guid == guid).First();
        //        if (parprocedureGuid is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "选中的流程中有流程的唯一标识在tn_dts_parprocedure表中不存在，请检查后重试！";
        //            return returnMessage;
        //        }
        //        parprocedureGuidList.Add(parprocedureGuid);
        //        tn_dts_logs parprocedureLog = new tn_dts_logs();
        //        parprocedureLog.cn_guid = Guid.NewGuid().ToString();
        //        parprocedureLog.cn_s_logs_type = "操作";
        //        parprocedureLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用批量删除流程功能删除了一条流程记录，详细信息为：" + JsonConvert.SerializeObject(parprocedureGuid);
        //        parprocedureLog.cn_t_create = DateTime.Now;
        //        logList.Add(parprocedureLog);
        //        List<tn_dts_chiprocedure> chiprocedureParproguidList = Db.Queryable<tn_dts_chiprocedure>().Where(it => it.cn_s_chiprocedure_parproguid == guid).ToList();
        //        chiprocedureList.AddRange(chiprocedureParproguidList);
        //        foreach (var chiprocedureParproguid in chiprocedureParproguidList)
        //        {
        //            tn_dts_logs chiprocedureLog = new tn_dts_logs();
        //            chiprocedureLog.cn_guid = Guid.NewGuid().ToString();
        //            chiprocedureLog.cn_s_logs_type = "操作";
        //            chiprocedureLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用批量删除流程功能删除了一条流程-设备关系，详细信息为：" + JsonConvert.SerializeObject(chiprocedureParproguid);
        //            chiprocedureLog.cn_t_create = DateTime.Now;
        //            logList.Add(chiprocedureLog);
        //        }
        //    }
        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        dbTran.Deleteable<tn_dts_parprocedure>(parprocedureGuidList).ExecuteCommand();
        //        dbTran.Deleteable<tn_dts_chiprocedure>(chiprocedureList).ExecuteCommand();
        //        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        LogHelper.Error("3.6流程设备（Parprocedure）管理批量删除流程失败，详细信息为：" + res.Message);
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "批量删除流程失败！";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "批量删除流程成功！";
        //    return returnMessage;
        //}

        /// <summary>
        /// 批量删除流程
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteProcedure(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_logs log  = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用批量删除流程功能删除了" + guidList.Count + "条流程记录及其对应的流程-设备关系";
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_parprocedure>().In(guidList).ExecuteCommand();
                dbTran.Deleteable<tn_dts_chiprocedure>().In(it => it.cn_s_chiprocedure_parproguid, guidList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("3.6流程设备（Parprocedure）管理批量删除流程失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除流程失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量删除流程成功！";
            return returnMessage;
        }
        #endregion
    }
}
