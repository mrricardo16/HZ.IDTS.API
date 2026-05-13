using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using HZ.CommonUtil.Helpers;
using SqlSugar.Extensions;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class ChiequipmentService : BaseService<tn_dts_chiequipment>, IChiequipmentService
    {
        public ChiequipmentService(SessionInfo session) : base(session)
        {

        }

        #region 读取一个设备下所有货位指令
        /// <summary>
        /// 读取一个设备下所有货位指令
        /// </summary>
        /// <param name="chiproguid"></param>
        /// <param name="startLocationCode"></param>
        /// <param name="endLocationCode"></param>
        /// <returns></returns>
        /// 注：tn_dts_goodscommand表中rowcollayer字段删除，需要新增方法从Json中读取locationCode
        public List<tn_dts_goodscommand> GetGoodscommand(string chiproguid, ref string startRowColLayer, ref string endRowColLayer)
        {
            tn_dts_chiprocedure chiprocedureGuid = Db.Queryable<tn_dts_chiprocedure>().Where(it => it.cn_guid == "chiproguid").First();
            startRowColLayer = chiprocedureGuid.cn_s_chiprocedure_startrcl;

            List<tn_dts_goodscommand> goodscommandList = new List<tn_dts_goodscommand>();
            List<string> comGuidList = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_equiguid == chiproguid && it.cn_s_chiequipment_category == "货位").OrderBy(it => it.cn_n_chiequipment_sequence).Select(it => it.cn_s_chiequipment_comguid).ToList();
            int comGuidCount = comGuidList.Count;
            for (int i = 0; i < comGuidCount; i++)
            {
                tn_dts_goodscommand goodscommandGuid = new tn_dts_goodscommand();
                goodscommandGuid = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == comGuidList[i]).First();
                if (!(goodscommandGuid is null))
                {
                    if (i == 0)
                    {
                        //startRowColLayer = goodscommandGuid.cn_s_goodscommand_rowcollayer;
                        
                    }
                    if (i != 0 && i == comGuidCount - 1)
                    {
                        //endRowColLayer = goodscommandGuid.cn_s_goodscommand_rowcollayer;
                        
                    }
                    goodscommandList.Add(goodscommandGuid);
                }
            }
            return goodscommandList;
        }
        #endregion

        //#region 读取一个设备下货位指令的位置（0：该设备不含货位指令；1：只有一个货位指令在开头；2：只有一个货位指令在结尾；3：只有两个一个货位指令在开头一个货位指令在结尾；4：其他情况）；
        /// <summary>
        /// 读取一个设备下货位指令的位置（0：该设备不含货位指令；1：只有一个货位指令在开头；2：只有一个货位指令在结尾；3：只有两个一个货位指令在开头一个货位指令在结尾；4：其他情况）；
        /// </summary>
        /// <param name = "equiguid" ></ param >
        /// < param name="startLocationCode"></param>
        /// <param name = "endLocationCode" ></ param >
        /// < returns ></ returns >
        //public int GetGoodscommandLocation(string equiguid, ref string startLocationCode, ref string endLocationCode)
        //{
        //    int result = 0;
        //    List<tn_dts_chiequipment> chiequipmentEquiguidList = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_equiguid == equiguid).OrderBy(it => it.cn_n_chiequipment_sequence).ToList();
        //    int goodsCount = chiequipmentEquiguidList.Where(it => it.cn_s_chiequipment_category == "货位").ToList().Count;
        //    if (goodsCount == 0)
        //    {
        //        return result;
        //    }

        //    if (chiequipmentEquiguidList[0].cn_s_chiequipment_category == "货位" && chiequipmentEquiguidList[chiequipmentEquiguidList.Count - 1].cn_s_chiequipment_category != "货位" && goodsCount == 1)
        //    {
        //        result = 1;
        //        startLocationCode = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == chiequipmentEquiguidList[0].cn_s_chiequipment_comguid).Select(it => it.cn_s_goodscommand_rowcollayer).First();
        //        return result;
        //    }
        //    if (chiequipmentEquiguidList[0].cn_s_chiequipment_category != "货位" && chiequipmentEquiguidList[chiequipmentEquiguidList.Count - 1].cn_s_chiequipment_category == "货位" && goodsCount == 1)
        //    {
        //        result = 2;
        //        endLocationCode = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == chiequipmentEquiguidList[chiequipmentEquiguidList.Count - 1].cn_s_chiequipment_comguid).Select(it => it.cn_s_goodscommand_rowcollayer).First();
        //        return result;
        //    }
        //    if (chiequipmentEquiguidList[0].cn_s_chiequipment_category == "货位" && chiequipmentEquiguidList[chiequipmentEquiguidList.Count - 1].cn_s_chiequipment_category == "货位" && goodsCount == 2)
        //    {
        //        result = 3;
        //        startLocationCode = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == chiequipmentEquiguidList[0].cn_s_chiequipment_comguid).Select(it => it.cn_s_goodscommand_rowcollayer).First();
        //        endLocationCode = Db.Queryable<tn_dts_goodscommand>().Where(it => it.cn_guid == chiequipmentEquiguidList[chiequipmentEquiguidList.Count - 1].cn_s_chiequipment_comguid).Select(it => it.cn_s_goodscommand_rowcollayer).First();
        //        return result;
        //    }
        //    result = 4;
        //    return result;
        //}
        //#endregion

        #region 保存设备
        /// <summary>
        /// 保存设备
        /// </summary>
        /// <param name="saveEquipmentDate"></param>
        /// <returns></returns>
        public ReturnMessage SaveEquipment(SaveEquipmentDate saveEquipmentDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            string chiprocedureGuid = saveEquipmentDate.ChiprocedureGuid;
            //if (saveEquipmentDate.AddOrModify == "add")
            //{
            //List<tn_dts_chiequipment> chiequipmentList = new List<tn_dts_chiequipment>();
            //foreach (var chiequipmentInformation in saveEquipmentDate.ChiequipmentInformationList)
            //{
            //    tn_dts_chiequipment chiequipment = new tn_dts_chiequipment();
            //    chiequipment.cn_guid = Guid.NewGuid().ToString();
            //    chiequipment.cn_s_chiequipment_equiguid = chiprocedureGuid;
            //    chiequipment.cn_s_chiequipment_comguid = chiequipmentInformation.CommandGuid;
            //    chiequipment.cn_s_chiequipment_category = chiequipmentInformation.ChiequipmentCategory;
            //    chiequipment.cn_s_chiequipment_type = "流程设备管理";
            //    chiequipment.cn_n_chiequipment_sequence = chiequipmentInformation.ChiequipmentSequence;
            //    chiequipment.cn_n_chiequipment_interval = chiequipmentInformation.ChiequipmentInterval;
            //    chiequipment.cn_s_creator = user.UserCode;
            //    chiequipment.cn_s_creator_by = user.UserName;
            //    chiequipment.cn_t_create = DateTime.Now;
            //    chiequipmentList.Add(chiequipment);
            //    tn_dts_logs chiequipmentLog = new tn_dts_logs();
            //    chiequipmentLog.cn_guid = Guid.NewGuid().ToString();
            //    chiequipmentLog.cn_s_logs_type = "操作";
            //    chiequipmentLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存设备（配置）功能新增了一条设备-指令关系，详细信息：" + JsonConvert.SerializeObject(chiequipment);
            //    chiequipmentLog.cn_t_create = DateTime.Now;
            //    logList.Add(chiequipmentLog);
            //}
            //ApiResult res = UseTransaction(dbTran =>
            //{
            //    dbTran.Insertable<tn_dts_chiequipment>(chiequipmentList).ExecuteCommand();
            //    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            //});
            //if (!res.IsSuccess)
            //{
            //    LogHelper.Error("失败，详细信息为：" + res.Message);
            //    returnMessage.IsSuccess = false;
            //    returnMessage.Message = "保存失败！";
            //    return returnMessage;
            //}
            //}
            //else if (saveEquipmentDate.AddOrModify == "modify")
            //{
            bool isAdd = false;
            bool isModify = false;
            bool isDelete = false;
            List<tn_dts_chiequipment> chiequipmentAddList = new List<tn_dts_chiequipment>();
            List<tn_dts_chiequipment> chiequipmentModifyList = new List<tn_dts_chiequipment>();
            List<tn_dts_chiequipment> chiequipmentDeleteList = new List<tn_dts_chiequipment>();
            List<tn_dts_chiequipment> chiequipmentEquiguidList = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_equiguid == chiprocedureGuid).ToList();
            List<string> deleteChiequipmentGuidList = saveEquipmentDate.DeleteChiequipmentGuidList;
            if (deleteChiequipmentGuidList.Count != 0)
            {
                //删除
                foreach (var deleteChiequipmentGuid in deleteChiequipmentGuidList)
                {
                    tn_dts_chiequipment chiequipmentGuid = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_guid == deleteChiequipmentGuid).First();
                    if (chiequipmentGuid is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "传入的删除列表中唯一标识为：" + deleteChiequipmentGuid + "的设备-指令关系在tn_dts_chiequipment表中不存在，请检查后重试！";
                        return returnMessage;
                    }
                    chiequipmentDeleteList.Add(chiequipmentGuid);
                    tn_dts_logs deleteChiequipmentLog = new tn_dts_logs();
                    deleteChiequipmentLog.cn_guid = Guid.NewGuid().ToString();
                    deleteChiequipmentLog.cn_s_logs_type = "操作";
                    deleteChiequipmentLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用保存设备（配置）功能删除了一条设备-指令关系，详细信息为：" + JsonConvert.SerializeObject(chiequipmentGuid);
                    deleteChiequipmentLog.cn_t_create = DateTime.Now;
                    logList.Add(deleteChiequipmentLog);
                }
                isDelete = true;
            }
            foreach (var chiequipmentInformation in saveEquipmentDate.ChiequipmentInformationList)
            {
                if (chiequipmentEquiguidList.Where(it => it.cn_guid == chiequipmentInformation.ChiequipmentGuid).ToList().Count != 0)
                {
                    //修改
                    tn_dts_chiequipment chiequipmentGuid = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_guid == chiequipmentInformation.ChiequipmentGuid).First();
                    chiequipmentGuid.cn_n_chiequipment_sequence = chiequipmentInformation.ChiequipmentSequence;
                    chiequipmentGuid.cn_n_chiequipment_interval = chiequipmentInformation.ChiequipmentInterval;
                    chiequipmentGuid.cn_s_modify = user.UserCode;
                    chiequipmentGuid.cn_s_modify_by = user.UserName;
                    chiequipmentGuid.cn_t_modify = DateTime.Now;
                    isModify = true;
                    chiequipmentModifyList.Add(chiequipmentGuid);
                    tn_dts_logs modifyChiequipmentLog = new tn_dts_logs();
                    modifyChiequipmentLog.cn_guid = Guid.NewGuid().ToString();
                    modifyChiequipmentLog.cn_s_logs_type = "操作";
                    modifyChiequipmentLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用保存设备（配置）功能修改了一条设备-指令关系，详细信息为：" + JsonConvert.SerializeObject(chiequipmentGuid);
                    modifyChiequipmentLog.cn_t_create = DateTime.Now;
                    logList.Add(modifyChiequipmentLog);
                }
                else
                {
                    //新增
                    tn_dts_chiequipment chiequipment = new tn_dts_chiequipment();
                    chiequipment.cn_guid = Guid.NewGuid().ToString();
                    chiequipment.cn_s_chiequipment_equiguid = chiprocedureGuid;
                    chiequipment.cn_s_chiequipment_comguid = chiequipmentInformation.CommandGuid;
                    chiequipment.cn_s_chiequipment_type = "流程设备管理";
                    chiequipment.cn_s_chiequipment_category = chiequipmentInformation.ChiequipmentCategory;
                    chiequipment.cn_n_chiequipment_sequence = chiequipmentInformation.ChiequipmentSequence;
                    chiequipment.cn_n_chiequipment_interval = chiequipmentInformation.ChiequipmentInterval;
                    chiequipment.cn_s_creator = user.UserCode;
                    chiequipment.cn_s_creator_by = user.UserName;
                    chiequipment.cn_t_create = DateTime.Now;
                    isAdd = true;
                    chiequipmentAddList.Add(chiequipment);
                    tn_dts_logs addChiequipmentLog = new tn_dts_logs();
                    addChiequipmentLog.cn_guid = Guid.NewGuid().ToString();
                    addChiequipmentLog.cn_s_logs_type = "操作";
                    addChiequipmentLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用保存设备（配置）功能新增了一条设备-指令关系，详细信息为：" + JsonConvert.SerializeObject(chiequipment);
                    addChiequipmentLog.cn_t_create = DateTime.Now;
                    logList.Add(addChiequipmentLog);
                }
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                if (isAdd)
                {
                    dbTran.Insertable<tn_dts_chiequipment>(chiequipmentAddList).ExecuteCommand();
                }
                if (isModify)
                {
                    dbTran.Updateable<tn_dts_chiequipment>(chiequipmentModifyList).ExecuteCommand();
                }
                if (isDelete)
                {
                    dbTran.Deleteable<tn_dts_chiequipment>(chiequipmentDeleteList).ExecuteCommand();
                }
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("3.11流程设备（Chiequipment）管理保存设备（配置）失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "保存失败！";
                return returnMessage;
            }
            //}
            //else
            //{
            //    returnMessage.IsSuccess = false;
            //    returnMessage.Message = "前端传入的AddOrModify参数只能为add或modify！";
            //    return returnMessage;
            //}
            returnMessage.IsSuccess = true;
            returnMessage.Message = "保存成功！";
            return returnMessage;
        }
        #endregion

        #region 通过流程子表唯一标识获取所有设备子表信息（含指令信息）
        /// <summary>
        /// 通过流程子表唯一标识获取所有设备子表信息（含指令信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiequipmentInformation> GetAllChiequipmentByChiprocedureguid(PageParm param)
        {
            string chiprocedureguid = param.Parms["cn_s_chiprocedure_guid"].ObjToString();
            List<tn_dts_chiequipment> chiequipmentList = Db.Queryable<tn_dts_chiequipment>()
                .FullJoin<tn_dts_equicommand>((ch, eq) => ch.cn_s_chiequipment_comguid == eq.cn_guid)
                .FullJoin<tn_dts_goodscommand>((ch, eq, go) => ch.cn_s_chiequipment_comguid == go.cn_guid)
                .WhereIF(!string.IsNullOrEmpty(chiprocedureguid), (ch, eq, go) => ch.cn_s_chiequipment_equiguid == chiprocedureguid)
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_n_chiequipment_sequence asc" : param.OrderBy)
                .ToList();
            List<ChiequipmentInformation> chiequipmentInformationList = new List<ChiequipmentInformation>();
            foreach (var chiequipment in chiequipmentList)
            {
                if (chiequipment.cn_s_chiequipment_category == "设备")
                {
                    ChiequipmentInformation chiequipmentInformation = Db.Queryable<tn_dts_chiequipment>()
                    .InnerJoin<tn_dts_equicommand>((ch, eq) => ch.cn_s_chiequipment_comguid == eq.cn_guid)
                    .Where((ch, eq) => ch.cn_s_chiequipment_type == "流程设备管理" && ch.cn_guid == chiequipment.cn_guid)
                    .Select((ch, eq) => new ChiequipmentInformation
                    {
                        ChiequipmentGuid = ch.cn_guid,
                        CommandGuid = eq.cn_guid,
                        CommandNo = eq.cn_s_equicommand_no,
                        CommandName = eq.cn_s_equicommand_name,
                        ChiequipmentCategory = ch.cn_s_chiequipment_category,
                        ChiequipmentSequence = ch.cn_n_chiequipment_sequence,
                        ChiequipmentInterval = ch.cn_n_chiequipment_interval
                    }).First();
                    chiequipmentInformationList.Add(chiequipmentInformation);
                }
                else if (chiequipment.cn_s_chiequipment_category == "货位")
                {
                    ChiequipmentInformation chiequipmentInformation = Db.Queryable<tn_dts_chiequipment>()
                        .InnerJoin<tn_dts_goodscommand>((ch, go) => ch.cn_s_chiequipment_comguid == go.cn_guid)
                        .Where((ch, go) => ch.cn_s_chiequipment_type == "流程设备管理" && ch.cn_guid == chiequipment.cn_guid)
                        .Select((ch, go) => new ChiequipmentInformation
                        {
                            ChiequipmentGuid = ch.cn_guid,
                            CommandGuid = go.cn_guid,
                            CommandNo = go.cn_s_goodscommand_no,
                            CommandName = go.cn_s_goodscommand_name,
                            ChiequipmentCategory = ch.cn_s_chiequipment_category,
                            ChiequipmentSequence = ch.cn_n_chiequipment_sequence,
                            ChiequipmentInterval = ch.cn_n_chiequipment_interval
                        }).First();
                    chiequipmentInformationList.Add(chiequipmentInformation);
                }
            }
            return chiequipmentInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 通过流程子表唯一标识获取所有设备子表信息（含指令信息和修改创建信息）
        /// <summary>
        /// 通过流程子表唯一标识获取所有设备子表信息（含指令信息和修改创建信息）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<ChiequipmentInformationPlus> GetAllChiequipmentPlusByChiprocedureguid(PageParm param)
        {
            string chiprocedureguid = param.Parms["cn_s_chiprocedure_guid"].ObjToString();
            string commandno = param.Parms["cn_s_command_commandno"].ObjToString();
            string commandname = param.Parms["cn_s_command_commandname"].ObjToString();
            string chiequipmentcategory = param.Parms["cn_s_chiequipment_category"].ObjToString();
            string haswildcard = param.Parms["cn_n_command_haswildcard"].ObjToString();
            List<tn_dts_chiequipment> chiequipmentList = Db.Queryable<tn_dts_chiequipment>()
                .FullJoin<tn_dts_equicommand>((ch, eq) => ch.cn_s_chiequipment_comguid == eq.cn_guid)
                .FullJoin<tn_dts_goodscommand>((ch, eq, go) => ch.cn_s_chiequipment_comguid == go.cn_guid)
                //.Where((ch, eq, go) => go.cn_s_goodscommand_type == "执行")
                .WhereIF(!string.IsNullOrEmpty(chiprocedureguid), (ch, eq, go) => ch.cn_s_chiequipment_equiguid == chiprocedureguid)
                .WhereIF(chiequipmentcategory == "设备", (ch, eq, go) => ch.cn_s_chiequipment_category == "设备")
                .WhereIF(chiequipmentcategory == "货位", (ch, eq, go) => ch.cn_s_chiequipment_category == "货位" && go.cn_s_goodscommand_type == "执行")
                .WhereIF(chiequipmentcategory == null || chiequipmentcategory == string.Empty, (ch, eq, go) => eq.cn_n_equicommand_haswildcard == 1 || eq.cn_n_equicommand_haswildcard == 0 || go.cn_s_goodscommand_type == "执行")
                .WhereIF(!string.IsNullOrEmpty(commandno) && chiequipmentcategory == "设备", (ch, eq, go) => eq.cn_s_equicommand_no.Contains(commandno))
                .WhereIF(!string.IsNullOrEmpty(commandno) && chiequipmentcategory == "货位", (ch, eq, go) => go.cn_s_goodscommand_no.Contains(commandno))
                .WhereIF(!string.IsNullOrEmpty(commandno) && (chiequipmentcategory == null || chiequipmentcategory == string.Empty), (ch, eq, go) => eq.cn_s_equicommand_no.Contains(commandno) || go.cn_s_goodscommand_no.Contains(commandno))
                .WhereIF(!string.IsNullOrEmpty(commandname) && chiequipmentcategory == "设备", (ch, eq, go) => eq.cn_s_equicommand_name.Contains(commandname))
                .WhereIF(!string.IsNullOrEmpty(commandname) && chiequipmentcategory == "货位", (ch, eq, go) => go.cn_s_goodscommand_name.Contains(commandname))
                .WhereIF(!string.IsNullOrEmpty(commandname) && (chiequipmentcategory == null || chiequipmentcategory == string.Empty), (ch, eq, go) => eq.cn_s_equicommand_name.Contains(commandname) || go.cn_s_goodscommand_name.Contains(commandname))
                .WhereIF(!string.IsNullOrEmpty(haswildcard) && chiequipmentcategory == "设备", (ch, eq, go) => eq.cn_n_equicommand_haswildcard == int.Parse(haswildcard))
                .WhereIF(!string.IsNullOrEmpty(haswildcard) && chiequipmentcategory == "货位", (ch, eq, go) => go.cn_n_goodscommand_haswildcard == int.Parse(haswildcard))
                .WhereIF(!string.IsNullOrEmpty(haswildcard) && (chiequipmentcategory == null || chiequipmentcategory == string.Empty), (ch, eq, go) => eq.cn_n_equicommand_haswildcard == int.Parse(haswildcard) || go.cn_n_goodscommand_haswildcard == int.Parse(haswildcard))
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_n_chiequipment_sequence asc" : param.OrderBy)
                .ToList();
            List<ChiequipmentInformationPlus> chiequipmentPlusInformationPlusList = new List<ChiequipmentInformationPlus>();
            foreach (var chiequipment in chiequipmentList)
            {
                if (chiequipment.cn_s_chiequipment_category == "设备")
                {
                    ChiequipmentInformationPlus chiequipmentInformationPlus = Db.Queryable<tn_dts_chiequipment>()
                    .InnerJoin<tn_dts_equicommand>((ch, eq) => ch.cn_s_chiequipment_comguid == eq.cn_guid)
                    .Where((ch, eq) => ch.cn_s_chiequipment_type == "流程设备管理" && ch.cn_guid == chiequipment.cn_guid)
                    .Select((ch, eq) => new ChiequipmentInformationPlus
                    {
                        ChiequipmentGuid = ch.cn_guid,
                        CommandGuid = eq.cn_guid,
                        CommandNo = eq.cn_s_equicommand_no,
                        CommandName = eq.cn_s_equicommand_name,
                        ChiequipmentCategory = ch.cn_s_chiequipment_category,
                        CommandJson = eq.cn_s_equicommand_json,
                        HasWildcard = eq.cn_n_equicommand_haswildcard,
                        ChiequipmentSequence = ch.cn_n_chiequipment_sequence,
                        ChiequipmentInterval = ch.cn_n_chiequipment_interval,
                        ModifyNo = ch.cn_s_modify,
                        ModifyName = ch.cn_s_modify_by,
                        ModifyTime = ch.cn_t_modify,
                        CreateNo = ch.cn_s_creator,
                        CreateName = ch.cn_s_creator_by,
                        CreateTime = ch.cn_t_create
                    }).First();
                    chiequipmentPlusInformationPlusList.Add(chiequipmentInformationPlus);
                }
                else if (chiequipment.cn_s_chiequipment_category == "货位")
                {
                    ChiequipmentInformationPlus chiequipmentInformationPlus = Db.Queryable<tn_dts_chiequipment>()
                        .InnerJoin<tn_dts_goodscommand>((ch, go) => ch.cn_s_chiequipment_comguid == go.cn_guid)
                        .Where((ch, go) => ch.cn_s_chiequipment_type == "流程设备管理" && ch.cn_guid == chiequipment.cn_guid)
                        .Select((ch, go) => new ChiequipmentInformationPlus
                        {
                            ChiequipmentGuid = ch.cn_guid,
                            CommandGuid = go.cn_guid,
                            CommandNo = go.cn_s_goodscommand_no,
                            CommandName = go.cn_s_goodscommand_name,
                            ChiequipmentCategory = ch.cn_s_chiequipment_category,
                            CommandJson = go.cn_s_goodscommand_json,
                            HasWildcard = go.cn_n_goodscommand_haswildcard,
                            ChiequipmentSequence = ch.cn_n_chiequipment_sequence,
                            ChiequipmentInterval = ch.cn_n_chiequipment_interval,
                            ModifyNo = ch.cn_s_modify,
                            ModifyName = ch.cn_s_modify_by,
                            ModifyTime = ch.cn_t_modify,
                            CreateNo = ch.cn_s_creator,
                            CreateName = ch.cn_s_creator_by,
                            CreateTime = ch.cn_t_create
                        }).First();
                    chiequipmentPlusInformationPlusList.Add(chiequipmentInformationPlus);
                }
            }
            return chiequipmentPlusInformationPlusList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion
    }
}
