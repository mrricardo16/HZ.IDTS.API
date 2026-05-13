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
using System.Linq;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class ParsceneService : BaseService<tn_dts_parscene>, IParsceneService
    {
        public ParsceneService(SessionInfo session) : base(session)
        {

        }

        #region 按场景编码（模糊）、场景名称（模糊）、流程关系分页查询
        /// <summary>
        /// 按场景编码（模糊）、场景名称（模糊）、流程关系分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_parscene> GetListPages(PageParm param)
        {
            string parsceneNo = param.Parms["cn_s_parscene_no"].ObjToString();
            string parsceneName = param.Parms["cn_s_parscene_name"].ObjToString();
            string prorelationship = param.Parms["cn_s_parscene_prorelationship"].ObjToString();
            return Db.Queryable<tn_dts_parscene>()
                .WhereIF(!string.IsNullOrEmpty(parsceneNo), it => it.cn_s_parscene_no.Contains(parsceneNo))
                .WhereIF(!string.IsNullOrEmpty(parsceneName), it => it.cn_s_parscene_name.Contains(parsceneName))
                .WhereIF(!string.IsNullOrEmpty(prorelationship), it => it.cn_s_parscene_prorelationship == prorelationship)
               .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
               .ToPage(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 保存场景
        /// <summary>
        /// 保存场景
        /// </summary>
        /// <param name="saveSceneDate"></param>
        /// <returns></returns>
        public ReturnMessage SaveScene(SaveSceneDate saveSceneDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();

            if (saveSceneDate.AddOrModify == "add")
            {
                string parsceneGuid = Guid.NewGuid().ToString();
                if (!(Db.Queryable<tn_dts_parscene>().Where(it => it.cn_s_parscene_no == saveSceneDate.ParsceneNo).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "场景编码不能重复，请重试！";
                    return returnMessage;
                }
                tn_dts_parscene parscene = new tn_dts_parscene();
                parscene.cn_guid = parsceneGuid;
                parscene.cn_s_parscene_no = saveSceneDate.ParsceneNo;
                parscene.cn_s_parscene_name = saveSceneDate.ParsceneName;
                parscene.cn_s_parscene_prorelationship = saveSceneDate.Prorelationship;
                parscene.cn_s_creator = user.UserCode;
                parscene.cn_s_creator_by = user.UserName;
                parscene.cn_t_create = DateTime.Now;
                tn_dts_logs parsceneLog = new tn_dts_logs();
                parsceneLog.cn_guid = Guid.NewGuid().ToString();
                parsceneLog.cn_s_logs_type = "操作";
                parsceneLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存场景功能新增了一条场景记录，详细信息：" + JsonConvert.SerializeObject(parscene);
                parsceneLog.cn_t_create = DateTime.Now;
                logList.Add(parsceneLog);
                List<tn_dts_chiscene> chisceneList = new List<tn_dts_chiscene>();
                foreach (var chisceneInformation in saveSceneDate.ChisceneInformationList)
                {
                    tn_dts_chiscene chiscene = new tn_dts_chiscene();
                    chiscene.cn_guid = Guid.NewGuid().ToString();
                    chiscene.cn_s_chiscene_parsceguid = parsceneGuid;
                    chiscene.cn_s_chiscene_parproguid = chisceneInformation.ProcedureGuid;
                    chiscene.cn_n_chiscene_sequence = chisceneInformation.ChisceneSequence;
                    chiscene.cn_n_chiscene_interval = chisceneInformation.ChisceneInterval;
                    chiscene.cn_s_creator = user.UserCode;
                    chiscene.cn_s_creator_by = user.UserName;
                    chiscene.cn_t_create = DateTime.Now;
                    chisceneList.Add(chiscene);
                    tn_dts_logs chisceneLog = new tn_dts_logs();
                    chisceneLog.cn_guid = Guid.NewGuid().ToString();
                    chisceneLog.cn_s_logs_type = "操作";
                    chisceneLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存场景功能新增了一条场景-流程关系，详细信息：" + JsonConvert.SerializeObject(chiscene);
                    chisceneLog.cn_t_create = DateTime.Now;
                    logList.Add(chisceneLog);
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_parscene>(parscene).ExecuteCommand();
                    dbTran.Insertable<tn_dts_chiscene>(chisceneList).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Error("4.4业务场景（Parscene）管理保存场景失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败！";
                    return returnMessage;
                }
            }
            else if (saveSceneDate.AddOrModify == "modify")
            {
                tn_dts_parscene parsceneGuid = Db.Queryable<tn_dts_parscene>().Where(it => it.cn_guid == saveSceneDate.ParsceneGuid).First();
                if (parsceneGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "tn_dts_parscene表中找不到唯一标识为：" + saveSceneDate.ParsceneGuid + "的场景记录，请检查后重试！";
                    return returnMessage;
                }
                if (!(Db.Queryable<tn_dts_parscene>().Where(it => it.cn_s_parscene_no == saveSceneDate.ParsceneNo && it.cn_guid != saveSceneDate.ParsceneGuid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "场景编码不能重复，请重试！";
                    return returnMessage;
                }
                parsceneGuid.cn_s_parscene_no = saveSceneDate.ParsceneNo;
                parsceneGuid.cn_s_parscene_name = saveSceneDate.ParsceneName;
                parsceneGuid.cn_s_parscene_prorelationship = saveSceneDate.Prorelationship;
                parsceneGuid.cn_s_modify = user.UserCode;
                parsceneGuid.cn_s_modify_by = user.UserName;
                parsceneGuid.cn_t_modify = DateTime.Now;
                tn_dts_logs parsceneLog = new tn_dts_logs();
                parsceneLog.cn_guid = Guid.NewGuid().ToString();
                parsceneLog.cn_s_logs_type = "操作";
                parsceneLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存场景功能修改了一条场景记录，详细信息为：" + JsonConvert.SerializeObject(parsceneGuid);
                parsceneLog.cn_t_create = DateTime.Now;
                bool isAdd = false;
                bool isModify = false;
                bool isDelete = false;
                List<tn_dts_chiscene> chisceneAddList = new List<tn_dts_chiscene>();
                List<tn_dts_chiscene> chisceneModifyList = new List<tn_dts_chiscene>();
                List<tn_dts_chiscene> chisceneDeleteList = new List<tn_dts_chiscene>();
                List<tn_dts_chiscene> chisceneParsceguidList = Db.Queryable<tn_dts_chiscene>().Where(it => it.cn_s_chiscene_parsceguid == saveSceneDate.ParsceneGuid).ToList();
                List<string> deleteChisceneGuidList = saveSceneDate.DeleteChisceneGuidList;
                if (deleteChisceneGuidList.Count != 0)
                {
                    //删除
                    foreach (var deleteChisceneGuid in deleteChisceneGuidList)
                    {
                        tn_dts_chiscene chisceneGuid = Db.Queryable<tn_dts_chiscene>().Where(it => it.cn_guid == deleteChisceneGuid).First();
                        if (chisceneGuid is null)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "传入的删除列表中唯一标识为：" + deleteChisceneGuid + "的场景-流程关系在tn_dts_chiscene表中不存在，请检查后重试！";
                            return returnMessage;
                        }
                        chisceneDeleteList.Add(chisceneGuid);
                        tn_dts_logs deleteChisceneLog = new tn_dts_logs();
                        deleteChisceneLog.cn_guid = Guid.NewGuid().ToString();
                        deleteChisceneLog.cn_s_logs_type = "操作";
                        deleteChisceneLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用保存场景功能删除了一条场景-流程关系，详细信息为：" + JsonConvert.SerializeObject(chisceneGuid);
                        deleteChisceneLog.cn_t_create = DateTime.Now;
                        logList.Add(deleteChisceneLog);
                    }
                    isDelete = true;
                }
                foreach (var chisceneInformation in saveSceneDate.ChisceneInformationList)
                {
                    if (chisceneParsceguidList.Where(it => it.cn_guid == chisceneInformation.ChisceneGuid).ToList().Count != 0)
                    {
                        //修改
                        tn_dts_chiscene chisceneGuid = Db.Queryable<tn_dts_chiscene>().Where(it => it.cn_guid == chisceneInformation.ChisceneGuid).First();
                        chisceneGuid.cn_n_chiscene_sequence = chisceneInformation.ChisceneSequence;
                        chisceneGuid.cn_n_chiscene_interval = chisceneInformation.ChisceneInterval;
                        chisceneGuid.cn_s_modify = user.UserCode;
                        chisceneGuid.cn_s_modify_by = user.UserName;
                        chisceneGuid.cn_t_modify = DateTime.Now;
                        isModify = true;
                        chisceneModifyList.Add(chisceneGuid);
                        tn_dts_logs modifyChisceneLog = new tn_dts_logs();
                        modifyChisceneLog.cn_guid = Guid.NewGuid().ToString();
                        modifyChisceneLog.cn_s_logs_type = "操作";
                        modifyChisceneLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存场景功能修改了一条场景-流程关系，详细信息为：" + JsonConvert.SerializeObject(chisceneGuid);
                        modifyChisceneLog.cn_t_create = DateTime.Now;
                        logList.Add(modifyChisceneLog);
                    }
                    else
                    {
                        //新增
                        tn_dts_chiscene chiscene = new tn_dts_chiscene();
                        chiscene.cn_guid = Guid.NewGuid().ToString();
                        chiscene.cn_s_chiscene_parsceguid = saveSceneDate.ParsceneGuid;
                        chiscene.cn_s_chiscene_parproguid = chisceneInformation.ProcedureGuid;
                        chiscene.cn_n_chiscene_sequence = chisceneInformation.ChisceneSequence;
                        chiscene.cn_n_chiscene_interval = chisceneInformation.ChisceneInterval;
                        chiscene.cn_s_creator = user.UserCode;
                        chiscene.cn_s_creator_by = user.UserName;
                        chiscene.cn_t_create = DateTime.Now;
                        isAdd = true;
                        chisceneAddList.Add(chiscene);
                        tn_dts_logs addChisceneLog = new tn_dts_logs();
                        addChisceneLog.cn_guid = Guid.NewGuid().ToString();
                        addChisceneLog.cn_s_logs_type = "操作";
                        addChisceneLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用保存场景功能新增了一条场景-流程关系，详细信息为：" + JsonConvert.SerializeObject(chiscene);
                        addChisceneLog.cn_t_create = DateTime.Now;
                        logList.Add(addChisceneLog);
                    }
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Updateable<tn_dts_parscene>(parsceneGuid).ExecuteCommand();
                    if (isAdd)
                    {
                        dbTran.Insertable<tn_dts_chiscene>(chisceneAddList).ExecuteCommand();
                    }
                    if (isModify)
                    {
                        dbTran.Updateable<tn_dts_chiscene>(chisceneModifyList).ExecuteCommand();
                    }
                    if (isDelete)
                    {
                        dbTran.Deleteable<tn_dts_chiscene>(chisceneDeleteList).ExecuteCommand();
                    }
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Error("4.4业务场景（Parscene）管理保存场景失败，详细信息为：" + res.Message);
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

        #region 批量删除场景
        ///// <summary>
        ///// 批量删除场景
        ///// </summary>
        ///// <param name="guidList"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteScene(List<string> guidList)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    List<tn_dts_parscene> parsceneGuidList = new List<tn_dts_parscene>();
        //    List<tn_dts_chiscene> chisceneList = new List<tn_dts_chiscene>();
        //    List<tn_dts_logs> logList = new List<tn_dts_logs>();
        //    foreach (var guid in guidList)
        //    {
        //        tn_dts_parscene parsceneGuid = Db.Queryable<tn_dts_parscene>().Where(it => it.cn_guid == guid).First();
        //        if (parsceneGuid is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "选中的场景中有场景的唯一标识在tn_dts_parscene表中不存在，请检查后重试！";
        //            return returnMessage;
        //        }
        //        parsceneGuidList.Add(parsceneGuid);
        //        tn_dts_logs parsceneLog = new tn_dts_logs();
        //        parsceneLog.cn_guid = Guid.NewGuid().ToString();
        //        parsceneLog.cn_s_logs_type = "操作";
        //        parsceneLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用批量删除场景功能删除了一条场景记录，详细信息为：" + JsonConvert.SerializeObject(parsceneGuid);
        //        parsceneLog.cn_t_create = DateTime.Now;
        //        logList.Add(parsceneLog);
        //        List<tn_dts_chiscene> chisceneParsceguidList = Db.Queryable<tn_dts_chiscene>().Where(it => it.cn_s_chiscene_parsceguid == guid).ToList();
        //        chisceneList.AddRange(chisceneParsceguidList);
        //        foreach (var chisceneParsceguid in chisceneParsceguidList)
        //        {
        //            tn_dts_logs chisceneLog = new tn_dts_logs();
        //            chisceneLog.cn_guid = Guid.NewGuid().ToString();
        //            chisceneLog.cn_s_logs_type = "操作";
        //            chisceneLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用批量删除场景功能删除了一条场景-流程关系，详细信息为：" + JsonConvert.SerializeObject(chisceneParsceguid);
        //            chisceneLog.cn_t_create = DateTime.Now;
        //            logList.Add(chisceneLog);
        //        }
        //    }
        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        dbTran.Deleteable<tn_dts_parscene>(parsceneGuidList).ExecuteCommand();
        //        dbTran.Deleteable<tn_dts_chiscene>(chisceneList).ExecuteCommand();
        //        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        LogHelper.Error("4.5业务场景（Parscene）管理批量删除场景失败，详细信息为：" + res.Message);
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "批量删除场景失败！";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "批量删除场景成功！";
        //    return returnMessage;
        //}

        /// <summary>
        /// 批量删除场景
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteScene(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用批量删除场景功能删除了" + guidList.Count + "条场景记录";
            log.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_parscene>().In(guidList).ExecuteCommand();
                dbTran.Deleteable<tn_dts_chiscene>().In(it => it.cn_s_chiscene_parsceguid, guidList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("4.5业务场景（Parscene）管理批量删除场景失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除场景失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量删除场景成功！";
            return returnMessage;
        }
        #endregion
    }
}
