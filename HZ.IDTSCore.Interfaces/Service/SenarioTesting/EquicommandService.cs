using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.SenarioTesting;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HZ.IDTSCore.Interfaces.Service.SenarioTesting
{
    public class EquicommandService : BaseService<tn_dts_equicommand>, IEquicommandService
    {
        public EquicommandService(SessionInfo session) : base(session)
        {

        }

        #region 按设备唯一标识、指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        ///// <summary>
        ///// 按设备唯一标识、指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        ///// </summary>
        ///// <param name="parm"></param>
        ///// <returns></returns>
        //public PagedInfo<tn_dts_equicommand> GetListPages(PageParm param)
        //{
        //    string equicommandNo = param.Parms["cn_s_equicommand_no"].ObjToString();
        //    string equicommandName = param.Parms["cn_s_equicommand_name"].ObjToString();
        //    string equicommandType = param.Parms["cn_s_equicommand_type"].ObjToString();
        //    string equipmentGuidListString = param.Parms["cn_s_chiequipment_equiguid"].ObjToString();
        //    List<string> equipmentGuidList = JsonConvert.DeserializeObject<List<string>>(equipmentGuidListString);
        //    List<tn_dts_equicommand> equicommandList = new List<tn_dts_equicommand>();
        //    foreach (var equipmentGuid in equipmentGuidList)
        //    {
        //        List<tn_dts_chiequipment> chiequipmentList = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_equiguid == equipmentGuid && it.cn_s_chiequipment_category == "设备" && it.cn_s_chiequipment_type == "设备指令管理")
        //            .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_create asc" : param.OrderBy).ToList();
        //        List<tn_dts_equicommand> equicommandEquiguidList = new List<tn_dts_equicommand>();
        //        foreach (var chiequipment in chiequipmentList)
        //        {
        //            tn_dts_equicommand equicommandEquiguid = Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_guid == chiequipment.cn_s_chiequipment_comguid)
        //                .WhereIF(!string.IsNullOrEmpty(equicommandNo), it => it.cn_s_equicommand_no.Contains(equicommandNo))
        //                .WhereIF(!string.IsNullOrEmpty(equicommandName), it => it.cn_s_equicommand_name.Contains(equicommandName))
        //                .WhereIF(!string.IsNullOrEmpty(equicommandType), it => it.cn_s_equicommand_type == equicommandType).First();
        //            if (!(equicommandEquiguid is null))
        //            {
        //                equicommandEquiguidList.Add(equicommandEquiguid);
        //            }
        //        }
        //        equicommandList.AddRange(equicommandEquiguidList);
        //    }
        //    PagedInfo<tn_dts_equicommand> equicommandPagedList = equicommandList.ToPageEnumerable(param.PageIndex, param.PageSize);
        //    return equicommandPagedList;
        //}

        /// <summary>
        /// 按设备唯一标识、指令编码（模糊）、指令名称（模糊）和指令类型分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<EquicommandPlus> GetListPages(PageParm param)
        {
            string equicommandNo = param.Parms["cn_s_equicommand_no"].ObjToString();
            string equicommandName = param.Parms["cn_s_equicommand_name"].ObjToString();
            string equicommandType = param.Parms["cn_s_equicommand_type"].ObjToString();
            string equipmentGuidListString = param.Parms["cn_s_chiequipment_equiguid"].ObjToString();
            List<string> equipmentGuidList = JsonConvert.DeserializeObject<List<string>>(equipmentGuidListString);
            List<EquicommandPlus> equicommandList = new List<EquicommandPlus>();
            foreach (var equipmentGuid in equipmentGuidList)
            {
                List<EquicommandPlus> equicommandEquiguidList = Db.Queryable<tn_dts_chiequipment>().Where(ch => ch.cn_s_chiequipment_category == "设备" && ch.cn_s_chiequipment_equiguid == equipmentGuid && ch.cn_s_chiequipment_type == "设备指令管理")
                    .InnerJoin<tn_dts_equipment>((ch, eq) => ch.cn_s_chiequipment_equiguid == eq.cn_guid)
                    .InnerJoin<tn_dts_equicommand>((ch, eq, ec) => ch.cn_s_chiequipment_comguid == ec.cn_guid)
                    .Select((ch, eq, ec) => new EquicommandPlus
                    {
                        cn_guid = ec.cn_guid,
                        cn_s_equicommand_equipmentno = eq.cn_s_equi_no,
                        cn_s_equicommand_no = ec.cn_s_equicommand_no,
                        cn_s_equicommand_name = ec.cn_s_equicommand_name,
                        cn_s_equicommand_type = ec.cn_s_equicommand_type,
                        cn_n_equicommand_haswildcard = ec.cn_n_equicommand_haswildcard,
                        cn_s_equicommand_json = ec.cn_s_equicommand_json,
                        cn_s_equicommand_remarks = ec.cn_s_equicommand_remarks,
                        cn_s_modify = ec.cn_s_modify,
                        cn_s_modify_by = ec.cn_s_modify_by,
                        cn_t_modify = ec.cn_t_modify,
                        cn_s_creator = ec.cn_s_creator,
                        cn_s_creator_by = ec.cn_s_creator_by,
                        cn_t_create = ec.cn_t_create
                    }).OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                    .ToList();
                equicommandList.AddRange(equicommandEquiguidList);
            }
            PagedInfo<EquicommandPlus> equicommandPagedList = equicommandList.OrderByDescending(it => it.cn_t_modify).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
            return equicommandPagedList;
        }
        #endregion

        #region 批量添加设备指令
        /// <summary>
        /// 批量添加设备指令
        /// </summary>
        /// <param name="batchAddDate"></param>
        /// <returns></returns>
        public ReturnMessage BatchAddEquicommand(BatchAddEquicommandDate batchAddDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<string> equipmentGuidList = batchAddDate.SelectiveEquipmentGuidList;
            foreach (var equipmentGuid in equipmentGuidList)
            {
                if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == equipmentGuid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "需要添加指令的设备中有唯一标识不存在的设备，请检查后重试！";
                    return returnMessage;
                }
            }
            if (batchAddDate.Haswildcard == 0 && equipmentGuidList.Count > 1)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "不含通配符的设备指令无法批量（超过一个设备）添加，请重试！";
                return returnMessage;
            }
            List<tn_dts_chiequipment> chiequipmentList = new List<tn_dts_chiequipment>();
            List<tn_dts_equicommand> equicommandList = new List<tn_dts_equicommand>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var equipmentGuid in equipmentGuidList)
            {
                string commandGuid = Guid.NewGuid().ToString();
                tn_dts_equicommand equicommand = new tn_dts_equicommand();
                equicommand.cn_guid = commandGuid;
                equicommand.cn_s_equicommand_no = commandGuid.Substring(0, 32);
                equicommand.cn_s_equicommand_name = batchAddDate.EquicommandName;
                equicommand.cn_s_equicommand_type = batchAddDate.EquicommandType;
                equicommand.cn_n_equicommand_haswildcard = batchAddDate.Haswildcard;
                equicommand.cn_s_equicommand_json = batchAddDate.Json;
                equicommand.cn_s_creator = user.UserCode;
                equicommand.cn_s_creator_by = user.UserName;
                equicommand.cn_t_create = DateTime.Now;
                equicommandList.Add(equicommand);
                tn_dts_logs equicommandLog = new tn_dts_logs();
                equicommandLog.cn_guid = Guid.NewGuid().ToString();
                equicommandLog.cn_s_logs_type = "操作";
                equicommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用设备指令管理批量增加功能向tn_dts_equicommand表中新增一条设备指令，详细信息为：" + JsonConvert.SerializeObject(equicommand);
                equicommandLog.cn_t_create = DateTime.Now;
                logList.Add(equicommandLog);
                tn_dts_chiequipment chiequipment = new tn_dts_chiequipment();
                chiequipment.cn_guid = Guid.NewGuid().ToString();
                chiequipment.cn_s_chiequipment_equiguid = equipmentGuid;
                chiequipment.cn_s_chiequipment_comguid = commandGuid;
                chiequipment.cn_s_chiequipment_category = "设备";
                chiequipment.cn_s_chiequipment_type = "设备指令管理";
                chiequipment.cn_s_creator = user.UserCode;
                chiequipment.cn_s_creator_by = user.UserName;
                chiequipment.cn_t_create = DateTime.Now;
                chiequipmentList.Add(chiequipment);
                tn_dts_logs chiequipmentLog = new tn_dts_logs();
                chiequipmentLog.cn_guid = Guid.NewGuid().ToString();
                chiequipmentLog.cn_s_logs_type = "操作";
                chiequipmentLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用设备指令管理批量增加功能向tn_dts_chiequipment表中新增一条设备-设备指令关系（设备指令管理），详细信息为：" + JsonConvert.SerializeObject(chiequipment);
                chiequipment.cn_t_create = DateTime.Now;
                logList.Add(chiequipmentLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_chiequipment>(chiequipmentList).ExecuteCommand();
                dbTran.Insertable<tn_dts_equicommand>(equicommandList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("1.4设备指令（Equicommand）管理批量添加设备指令接口失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量增加失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量增加成功！";
            return returnMessage;
        }
        #endregion

        #region 修改设备指令
        /// <summary>
        /// 修改设备指令
        /// </summary>
        /// <param name="equicommand"></param>
        /// <returns></returns>
        public ReturnMessage UpdateEquicommand(tn_dts_equicommand equicommand)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equicommand equicommandGuid = Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_guid == equicommand.cn_guid).First();
            if (equicommandGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "tn_dts_equicommand表中找不到唯一标识为：" + equicommand.cn_guid + "的设备指令记录，请检查后重试！";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_s_equicommand_no == equicommand.cn_s_equicommand_no && it.cn_guid != equicommand.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "指令编码不能重复，请重试！";
                return returnMessage;
            }
            equicommandGuid.cn_s_equicommand_no = equicommand.cn_s_equicommand_no;
            equicommandGuid.cn_s_equicommand_name = equicommand.cn_s_equicommand_name;
            equicommandGuid.cn_s_equicommand_type = equicommand.cn_s_equicommand_type;
            equicommandGuid.cn_n_equicommand_haswildcard = equicommand.cn_n_equicommand_haswildcard;
            equicommandGuid.cn_s_equicommand_json = equicommand.cn_s_equicommand_json;
            equicommandGuid.cn_s_equicommand_remarks = equicommand.cn_s_equicommand_remarks;
            equicommandGuid.cn_s_modify = user.UserCode;
            equicommandGuid.cn_s_modify_by = user.UserName;
            equicommandGuid.cn_t_modify = DateTime.Now;
            tn_dts_logs equicommandLog = new tn_dts_logs();
            equicommandLog.cn_guid = Guid.NewGuid().ToString();
            equicommandLog.cn_s_logs_type = "操作";
            equicommandLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用修改设备指令功能修改了一条设备指令，详细信息：" + JsonConvert.SerializeObject(equicommandLog);
            equicommandLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_equicommand>(equicommandGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(equicommandLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("1.5设备指令（Equicommand）管理修改设备指令接口失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改设备指令失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改设备指令成功！";
            return returnMessage;
        }
        #endregion

        #region 编辑模版
        /// <summary>
        /// 编辑模版
        /// </summary>
        /// <param name="editEquicommandDate"></param>
        /// <returns></returns>
        public ReturnMessage EditEquicommandJson(EditEquicommandDate editEquicommandDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equicommand equicommandGuid = Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_guid == editEquicommandDate.EquicommandGuid).First();
            if (equicommandGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "tn_dts_equicommand表中找不到唯一标识为：" + editEquicommandDate.EquicommandGuid + "的设备指令记录，请检查后重试！";
                return returnMessage;
            }
            equicommandGuid.cn_n_equicommand_haswildcard = editEquicommandDate.Haswildcard;
            equicommandGuid.cn_s_equicommand_json = editEquicommandDate.Json;
            equicommandGuid.cn_s_modify = user.UserCode;
            equicommandGuid.cn_s_modify_by = user.UserName;
            equicommandGuid.cn_t_modify = DateTime.Now;
            tn_dts_logs equicommandLog = new tn_dts_logs();
            equicommandLog.cn_guid = Guid.NewGuid().ToString();
            equicommandLog.cn_s_logs_type = "操作";
            equicommandLog.cn_s_logs_remarks = "用户编码为：" + user.UserCode + "的用户使用编辑模版功能修改了一条设备指令Json，详细信息：" + JsonConvert.SerializeObject(equicommandGuid);
            equicommandLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_equicommand>(equicommandGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(equicommandLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("1.6设备指令（Equicommand）管理编辑模版接口失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "编辑模版失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "编辑模版成功！";
            return returnMessage;
        }
        #endregion

        #region 按指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// <summary>
        /// 按指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<EquicommandInformation> GetEquicommandInformationPages(PageParm param)
        {
            string equipmentGuid = param.Parms["cn_s_chiequipment_equiguid"].ObjToString();
            string equicommandNoOrName = param.Parms["cn_s_equicommand_no_or_name"].ObjToString();
            List<tn_dts_chiequipment> chiequipmentEquiguidList = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_equiguid == equipmentGuid && it.cn_s_chiequipment_type == "设备指令管理")
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_create asc" : param.OrderBy).ToList();
            List<EquicommandInformation> equicommandInformationList = new List<EquicommandInformation>();
            foreach (var chiequipmentEquiguid in chiequipmentEquiguidList)
            {
                EquicommandInformation equicommandInformation = new EquicommandInformation();
                if (!string.IsNullOrEmpty(equicommandNoOrName))
                {
                    equicommandInformation = Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_guid == chiequipmentEquiguid.cn_s_chiequipment_comguid && (it.cn_s_equicommand_no.Contains(equicommandNoOrName) || it.cn_s_equicommand_name.Contains(equicommandNoOrName)))
                    .Select(it => new EquicommandInformation
                    {
                        EquicommandGuid = it.cn_guid,
                        EquicommandNo = it.cn_s_equicommand_no,
                        EquicommandName = it.cn_s_equicommand_name
                    }).First();
                }
                else
                {
                    equicommandInformation = Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_guid == chiequipmentEquiguid.cn_s_chiequipment_comguid)
                    .Select(it => new EquicommandInformation
                    {
                        EquicommandGuid = it.cn_guid,
                        EquicommandNo = it.cn_s_equicommand_no,
                        EquicommandName = it.cn_s_equicommand_name
                    }).First();
                }
                 
                if (!(equicommandInformation is null))
                {
                    equicommandInformationList.Add(equicommandInformation);
                }
            }
            return equicommandInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 复制设备指令
        /// <summary>
        /// 复制设备指令
        /// </summary>
        /// <param name="copyEquicommandDate"></param>
        /// <returns></returns>
        public ReturnMessage CopyEquicommand(CopyEquicommandDate copyEquicommandDate)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<string> equicommandguidList = copyEquicommandDate.EquicommandGuidList;
            List<string> equipmentguidList = copyEquicommandDate.EquipmentGuidList;
            List<tn_dts_equicommand> equicommandGuidList = new List<tn_dts_equicommand>();
            foreach (var equicommandguid in equicommandguidList)
            {
                tn_dts_equicommand equicommandGuid = Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_guid == equicommandguid).First();
                if (equicommandGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选择需要复制的设备指令中有指令的唯一标识不存在，请检查后重试！";
                    return returnMessage;
                }
                if (equicommandGuid.cn_n_equicommand_haswildcard == 0)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选择需要复制的设备指令中有指令不含通配符，请检查后重试！";
                    return returnMessage;
                }
                equicommandGuidList.Add(equicommandGuid);
            }
            List<tn_dts_chiequipment> chiequipmentList = new List<tn_dts_chiequipment>();
            List<tn_dts_equicommand> equicommandList = new List<tn_dts_equicommand>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var equipmentguid in equipmentguidList)
            {
                if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == equipmentguid).First() is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选择需要粘贴的设备中有设备的唯一标识不存在，请检查后重试！";
                    return returnMessage;
                }
                foreach (var equicommandGuid in equicommandGuidList)
                {
                    string commandGuid = Guid.NewGuid().ToString();
                    tn_dts_equicommand equicommand = new tn_dts_equicommand();
                    equicommand.cn_guid = commandGuid;
                    equicommand.cn_s_equicommand_no = commandGuid.Substring(0, 32);
                    equicommand.cn_s_equicommand_name = equicommandGuid.cn_s_equicommand_name;
                    equicommand.cn_s_equicommand_type = equicommandGuid.cn_s_equicommand_type;
                    equicommand.cn_n_equicommand_haswildcard = equicommandGuid.cn_n_equicommand_haswildcard;
                    equicommand.cn_s_equicommand_json = equicommandGuid.cn_s_equicommand_json;
                    equicommand.cn_s_creator = user.UserCode;
                    equicommand.cn_s_creator_by = user.UserName;
                    equicommand.cn_t_create = DateTime.Now;
                    equicommandList.Add(equicommand);
                    tn_dts_logs equicommandLog = new tn_dts_logs();
                    equicommandLog.cn_guid = Guid.NewGuid().ToString();
                    equicommandLog.cn_s_logs_type = "操作";
                    equicommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用设备指令管理复制设备指令功能向tn_dts_equicommand表中新增一条设备指令，详细信息为：" + JsonConvert.SerializeObject(equicommand);
                    equicommandLog.cn_t_create = DateTime.Now;
                    logList.Add(equicommandLog);
                    tn_dts_chiequipment chiequipment = new tn_dts_chiequipment();
                    chiequipment.cn_guid = Guid.NewGuid().ToString();
                    chiequipment.cn_s_chiequipment_equiguid = equipmentguid;
                    chiequipment.cn_s_chiequipment_comguid = commandGuid;
                    chiequipment.cn_s_chiequipment_category = "设备";
                    chiequipment.cn_s_chiequipment_type = "设备指令管理";
                    chiequipment.cn_s_creator = user.UserCode;
                    chiequipment.cn_s_creator_by = user.UserName;
                    chiequipment.cn_t_create = DateTime.Now;
                    chiequipmentList.Add(chiequipment);
                    tn_dts_logs chiequipmentLog = new tn_dts_logs();
                    chiequipmentLog.cn_guid = Guid.NewGuid().ToString();
                    chiequipmentLog.cn_s_logs_type = "操作";
                    chiequipmentLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用设备指令管理复制设备指令功能向tn_dts_chiequipment表中新增一条设备-设备指令关系（设备指令管理），详细信息为：" + JsonConvert.SerializeObject(chiequipment);
                    chiequipmentLog.cn_t_create = DateTime.Now;
                    logList.Add(chiequipmentLog);
                }
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_equicommand>(equicommandList).ExecuteCommand();
                dbTran.Insertable<tn_dts_chiequipment>(chiequipmentList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("1.8设备指令（Equicommand）管理复制设备指令接口失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "复制设备指令失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "复制设备指令成功！";
            return returnMessage;
        }
        #endregion

        #region 批量删除设备指令
        ///// <summary>
        ///// 批量删除设备指令
        ///// </summary>
        ///// <param name="guidList"></param>
        ///// <returns></returns>
        //public ReturnMessage DeleteEquicommand(List<string> guidList)
        //{
        //    ReturnMessage returnMessage = new ReturnMessage();
        //    UserSession user = GetSessionInfo();
        //    List<tn_dts_equicommand> equicommandGuidList = new List<tn_dts_equicommand>();
        //    List<tn_dts_chiequipment> chiequipmentComguidList = new List<tn_dts_chiequipment>();
        //    List<tn_dts_logs> logList = new List<tn_dts_logs>();
        //    foreach (var guid in guidList)
        //    {
        //        tn_dts_equicommand equicommandGuid = Db.Queryable<tn_dts_equicommand>().Where(it => it.cn_guid == guid).First();
        //        if (equicommandGuid is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "已选设备指令中有设备指令的唯一标识不存在，请检查后重试！";
        //            return returnMessage;
        //        }
        //        equicommandGuidList.Add(equicommandGuid);
        //        tn_dts_logs equicommandLog = new tn_dts_logs();
        //        equicommandLog.cn_guid = Guid.NewGuid().ToString();
        //        equicommandLog.cn_s_logs_type = "操作";
        //        equicommandLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用设备指令管理批量删除设备指令功能删除一条tn_dts_euqicommand设备指令，详细信息为：" + JsonConvert.SerializeObject(equicommandGuid);
        //        equicommandLog.cn_t_create = DateTime.Now;
        //        logList.Add(equicommandLog);
        //        tn_dts_chiequipment chiequipmentComguid = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_comguid == guid && it.cn_s_chiequipment_type == "设备指令管理").First();
        //        if (chiequipmentComguid is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "已选设备指令中有设备指令的设备-设备指令关系（设备指令管理）不存在，请检查后重试！";
        //            return returnMessage;
        //        }
        //        chiequipmentComguidList.Add(chiequipmentComguid);
        //        tn_dts_logs chiequipmentLog = new tn_dts_logs();
        //        chiequipmentLog.cn_guid = Guid.NewGuid().ToString();
        //        chiequipmentLog.cn_s_logs_type = "操作";
        //        chiequipmentLog.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用设备指令管理批量删除设备指令功能删除一条设备-设备指令关系（设备指令管理），详细信息为：" + JsonConvert.SerializeObject(chiequipmentComguid);
        //        chiequipmentLog.cn_t_create = DateTime.Now;
        //        logList.Add(chiequipmentLog);
        //    }
        //    ApiResult res = UseTransaction(dbTran =>
        //    {
        //        dbTran.Deleteable<tn_dts_equicommand>(equicommandGuidList).ExecuteCommand();
        //        dbTran.Deleteable<tn_dts_chiequipment>(chiequipmentComguidList).ExecuteCommand();
        //        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
        //    });
        //    if (!res.IsSuccess)
        //    {
        //        LogHelper.Error("1.9设备指令（Equicommand）管理批量删除设备指令失败，详细信息为：" + res.Message);
        //        returnMessage.IsSuccess = false;
        //        returnMessage.Message = "批量删除设备指令失败！";
        //        return returnMessage;
        //    }
        //    returnMessage.IsSuccess = true;
        //    returnMessage.Message = "批量删除设备指令成功！";
        //    return returnMessage;
        //}

        /// <summary>
        /// 批量删除设备指令
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteEquicommand(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_logs log = new tn_dts_logs();
            log.cn_guid = Guid.NewGuid().ToString();
            log.cn_s_logs_type = "操作";
            log.cn_s_logs_remarks = "用户编号为：" + user.UserCode + "的用户使用设备指令管理批量删除设备指令功能删除了" + guidList.Count + "条tn_dts_euqicommand设备指令及其设备-设备指令关系（设备指令管理）记录";
            log.cn_t_create = DateTime.Now;
            
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equicommand>().In(guidList).ExecuteCommand();
                dbTran.Deleteable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_type == "设备指令管理").In(it => it.cn_s_chiequipment_comguid, guidList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Error("1.9设备指令（Equicommand）管理批量删除设备指令失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "批量删除设备指令失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "批量删除设备指令成功！";
            return returnMessage;
        }
        #endregion

        #region 按有无通配符、指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// <summary>
        /// 按有无通配符、指令编码和指令名称混合模糊分页查询（设备指令管理）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<EquicommandInformation> GetEquicommandPlusInformationPages(PageParm param)
        {
            string equipmentGuid = param.Parms["cn_s_chiequipment_equiguid"].ObjToString();
            string hasWildcard = param.Parms["cn_n_equicommand_haswildcard"].ObjToString();
            string equicommandNoOrName = param.Parms["cn_s_equicommand_no_or_name"].ObjToString();
            List<tn_dts_chiequipment> chiequipmentEquiguidList = Db.Queryable<tn_dts_chiequipment>().Where(it => it.cn_s_chiequipment_equiguid == equipmentGuid && it.cn_s_chiequipment_type == "设备指令管理" && it.cn_s_chiequipment_category == "设备")
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_create asc" : param.OrderBy).ToList();
            List<EquicommandInformation> equicommandInformationList = new List<EquicommandInformation>();
            foreach (var chiequipmentEquiguid in chiequipmentEquiguidList)
            {
                EquicommandInformation equicommandInformation = Db.Queryable<tn_dts_equicommand>()
                    .Where(it => it.cn_guid == chiequipmentEquiguid.cn_s_chiequipment_comguid)
                    .WhereIF(!string.IsNullOrEmpty(equicommandNoOrName), it => it.cn_s_equicommand_no.Contains(equicommandNoOrName) || it.cn_s_equicommand_name.Contains(equicommandNoOrName))
                    .WhereIF(!string.IsNullOrEmpty(hasWildcard), it => it.cn_n_equicommand_haswildcard == int.Parse(hasWildcard))
                    .Select(it => new EquicommandInformation
                    {
                        EquicommandGuid = it.cn_guid,
                        EquicommandNo = it.cn_s_equicommand_no,
                        EquicommandName = it.cn_s_equicommand_name
                    }).First();
                if (!(equicommandInformation is null))
                {
                    equicommandInformationList.Add(equicommandInformation);
                }
            }
            return equicommandInformationList.ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion
    }
}
