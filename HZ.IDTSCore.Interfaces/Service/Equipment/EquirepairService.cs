using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.IDTSCore.Model.Entity.Sys;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using HZ.CommonUtil.Helpers;

namespace HZ.IDTSCore.Interfaces.Service.Equipment
{
    public class EquirepairService : BaseService<tn_dts_equirepair>, IEquirepairService
    {
        public EquirepairService(SessionInfo session) : base(session)
        {

        }

        #region 按cn_s_equirepair_category、cn_s_equirepair_date、cn_s_equi_parttype、cn_s_equirepair_name(模糊)分页查询
        /// <summary>
        /// 按cn_s_equirepair_category、cn_s_equirepair_date、cn_s_equi_parttype、cn_s_equirepair_name(模糊)分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equirepair> GetListPages(PageParm parm)
        {
            string cn_s_equirepair_no = parm.Parms["cn_s_equirepair_no"].ObjToString();
            string cn_s_equirepair_category = parm.Parms["cn_s_equirepair_category"].ObjToString();
            string cn_s_equirepair_starttime = parm.Parms["cn_s_equirepair_starttime"].ObjToString();
            string cn_s_equirepair_endtime = parm.Parms["cn_s_equirepair_endtime"].ObjToString();
            string cn_s_equi_parttype = parm.Parms["cn_s_equi_parttype"].ObjToString();
            string cn_s_equirepair_name = parm.Parms["cn_s_equirepair_name"].ObjToString();
            return Db.Queryable<tn_dts_equirepair>()
            .LeftJoin<tn_dts_equipment>((er, e) => er.cn_s_equirepair_no == e.cn_s_equi_no)
            .WhereIF(!String.IsNullOrEmpty(cn_s_equirepair_no), er => er.cn_s_equirepair_no == cn_s_equirepair_no)
            .WhereIF(!String.IsNullOrEmpty(cn_s_equirepair_category), er => er.cn_s_equirepair_category == cn_s_equirepair_category)
            .WhereIF((!String.IsNullOrEmpty(cn_s_equirepair_starttime)) && (!String.IsNullOrEmpty(cn_s_equirepair_endtime)), er => (er.cn_s_equirepair_date >= DateTime.Parse(cn_s_equirepair_starttime)) && (er.cn_s_equirepair_date <= DateTime.Parse(cn_s_equirepair_endtime)))
            .WhereIF(!String.IsNullOrEmpty(cn_s_equi_parttype), (er, e) => e.cn_s_equi_parttype == cn_s_equi_parttype)
            .WhereIF(!String.IsNullOrEmpty(cn_s_equirepair_name), er => er.cn_s_equirepair_name.Contains(cn_s_equirepair_name))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 批量删除
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_equirepair>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 保存设备维修
        /// <summary>
        /// 保存设备维修
        /// 注：只有新增时可以进行category选择更换，修改时category不可选择更换
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        public ReturnMessage SaveDataRepair(SaveData_Repair saveData)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            tn_dts_equipment equipment = saveData.newequipment;
            tn_dts_equirepair equirepair = saveData.equirepair;
            UserSession user = GetSessionInfo();

            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            tn_dts_equirepair newRepair = new tn_dts_equirepair();
            tn_dts_matter newMatter = new tn_dts_matter();
            tn_dts_reason newReason = new tn_dts_reason();
            bool isnewMatter = false;
            bool isnewReason = false;
            if (saveData.add_or_modify == "add")
            {
                if (!(Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == equirepair.cn_s_equirepair_no && it.cn_s_equirepair_item == equirepair.cn_s_equirepair_item && it.cn_s_equirepair_date == equirepair.cn_s_equirepair_date).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "同一个设备在同一时间同一维修项目只能有一个！";
                    return returnMessage;
                }
                tn_dts_equipment oldEquipment = new tn_dts_equipment();
                tn_dts_equipment newEquipment = new tn_dts_equipment();
                tn_dts_equibom oldEquibom = new tn_dts_equibom();
                tn_dts_equibom newEquibom = new tn_dts_equibom();
                if (equirepair.cn_s_equirepair_category == "更换")
                {
                    if (equipment is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "未传入更换后新的零部件信息！";
                        return returnMessage;
                    }
                    if (equipment.cn_s_equi_status != "正常")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "新更换的零部件状态不为正常！";
                        return returnMessage;
                    }
                    if (!(Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == equirepair.cn_s_equirepair_no && it.cn_s_equibom_status == "正常").First() is null))
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "保存失败，有下级零部件状态不是报废!";
                        //更换的零部件有子项正常零部件，请到设备档案管理将需要更换零部件的子零部件的设备状态更改为报废！
                        return returnMessage;
                    }
                    if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == equipment.cn_s_equi_no).ToList().Count != 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备编号不能重复！";
                        return returnMessage;
                    }
                    oldEquibom = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_childno == equirepair.cn_s_equirepair_no).First();
                    if (oldEquibom is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "找不到要更换零部件在关系表中的记录！";
                        return returnMessage;
                    }
                    oldEquibom.cn_t_equibom_lapsetime = DateTime.Now;
                    oldEquibom.cn_s_equibom_status = "报废";
                    oldEquibom.cn_s_modify = user.UserCode;
                    oldEquibom.cn_s_modify_by = user.UserName;
                    oldEquibom.cn_t_modify = DateTime.Now;
                    tn_dts_logs oldEquibomLog = new tn_dts_logs();
                    oldEquibomLog.cn_guid = Guid.NewGuid().ToString();
                    oldEquibomLog.cn_s_logs_type = "操作";
                    oldEquibomLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能修改了tn_dts_equibom表中子项设备编号为" + equirepair.cn_s_equirepair_no + "的关系记录中失效时间和关系状态，详细信息为" + JsonConvert.SerializeObject(oldEquibom);
                    oldEquibomLog.cn_t_create = DateTime.Now;
                    logList.Add(oldEquibomLog);
                    oldEquipment = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == equirepair.cn_s_equirepair_no).First();
                    if (oldEquipment is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "找不到要更换零部件在设备表中的记录！";
                        return returnMessage;
                    }
                    oldEquipment.cn_s_equi_status = "报废";
                    oldEquipment.cn_s_modify = user.UserCode;
                    oldEquipment.cn_s_modify_by = user.UserName;
                    oldEquipment.cn_t_modify = DateTime.Now;
                    tn_dts_logs oldEquipmentLog = new tn_dts_logs();
                    oldEquipmentLog.cn_guid = Guid.NewGuid().ToString();
                    oldEquipmentLog.cn_s_logs_type = "操作";
                    oldEquipmentLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能修改了tn_dts_equipmnet表中设备编号为" + equirepair.cn_s_equirepair_no + "的设备记录中设备状态，详细信息为" + JsonConvert.SerializeObject(oldEquipment);
                    oldEquipmentLog.cn_t_create = DateTime.Now;
                    logList.Add(oldEquipmentLog);

                    newEquipment.cn_guid = Guid.NewGuid().ToString();
                    newEquipment.cn_s_equi_parttype = equipment.cn_s_equi_parttype;
                    newEquipment.cn_s_equi_no = equipment.cn_s_equi_no;
                    newEquipment.cn_s_equi_name = equipment.cn_s_equi_name;
                    newEquipment.cn_s_equi_type = equipment.cn_s_equi_type;
                    newEquipment.cn_s_equi_model = equipment.cn_s_equi_model;
                    newEquipment.cn_s_equi_status = equipment.cn_s_equi_status;
                    newEquipment.cn_s_equi_buydate = equipment.cn_s_equi_buydate;
                    newEquipment.cn_s_equi_qadate = equipment.cn_s_equi_qadate;
                    newEquipment.cn_s_equi_firstdate = equipment.cn_s_equi_firstdate;
                    newEquipment.cn_s_equi_defentperiod = equipment.cn_s_equi_defentperiod;
                    newEquipment.cn_s_equi_dept = equipment.cn_s_equi_dept;
                    newEquipment.cn_s_equi_dutyman = equipment.cn_s_equi_dutyman;
                    newEquipment.cn_s_equi_dutyphone = equipment.cn_s_equi_dutyphone;
                    newEquipment.cn_s_equi_contractno = equipment.cn_s_equi_contractno;
                    newEquipment.cn_s_equi_beltline = equipment.cn_s_equi_beltline;
                    newEquipment.cn_s_equi_xpos = equipment.cn_s_equi_xpos;
                    newEquipment.cn_s_equi_ypos = equipment.cn_s_equi_ypos;
                    newEquipment.cn_s_equi_zpos = equipment.cn_s_equi_zpos;
                    newEquipment.cn_s_equi_remarks = equipment.cn_s_equi_remarks;
                    newEquipment.cn_s_creator = user.UserCode;
                    newEquipment.cn_s_creator_by = user.UserName;
                    newEquipment.cn_t_create = DateTime.Now;
                    tn_dts_logs newEquipmentLog = new tn_dts_logs();
                    newEquipmentLog.cn_guid = Guid.NewGuid().ToString();
                    newEquipmentLog.cn_s_logs_type = "操作";
                    newEquipmentLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能向tn_dts_equipmnet表中新增一条零部件信息，详细信息为" + JsonConvert.SerializeObject(newEquipment);
                    newEquipmentLog.cn_t_create = DateTime.Now;
                    logList.Add(newEquipmentLog);
                    newEquibom.cn_guid = Guid.NewGuid().ToString();
                    newEquibom.cn_s_equibom_parentno = oldEquibom.cn_s_equibom_parentno;
                    newEquibom.cn_s_equibom_parentname = oldEquibom.cn_s_equibom_parentname;
                    newEquibom.cn_s_equibom_childno = equipment.cn_s_equi_no;
                    newEquibom.cn_s_equibom_childname = equipment.cn_s_equi_name;
                    newEquibom.cn_t_equibom_effectuatetime = DateTime.Now;
                    newEquibom.cn_t_equibom_lapsetime = DateTime.MaxValue;
                    newEquibom.cn_s_equibom_status = "正常";
                    newEquibom.cn_s_creator = user.UserCode;
                    newEquibom.cn_s_creator_by = user.UserName;
                    newEquibom.cn_t_create = DateTime.Now;
                    tn_dts_logs newEquibomLog = new tn_dts_logs();
                    newEquibomLog.cn_guid = Guid.NewGuid().ToString();
                    newEquibomLog.cn_s_logs_type = "操作";
                    newEquibomLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能向tn_dts_equibom表中新增一条设备关系，详细信息为" + JsonConvert.SerializeObject(newEquibom);
                    newEquibomLog.cn_t_create = DateTime.Now;
                    logList.Add(newEquibomLog);
                }
                newRepair.cn_guid = Guid.NewGuid().ToString();
                newRepair.cn_s_equirepair_no = equirepair.cn_s_equirepair_no;
                newRepair.cn_s_equirepair_name = equirepair.cn_s_equirepair_name;
                newRepair.cn_s_equirepair_category = equirepair.cn_s_equirepair_category;
                newRepair.cn_s_equirepair_item = equirepair.cn_s_equirepair_item;
                newRepair.cn_s_equirepair_cause = equirepair.cn_s_equirepair_cause;
                newRepair.cn_s_equirepair_solution = equirepair.cn_s_equirepair_solution;
                newRepair.cn_s_equirepair_man = equirepair.cn_s_equirepair_man;
                newRepair.cn_s_equirepair_phone = equirepair.cn_s_equirepair_phone;
                newRepair.cn_s_equirepair_result = equirepair.cn_s_equirepair_result;
                newRepair.cn_s_equirepair_date = equirepair.cn_s_equirepair_date;
                newRepair.cn_d_equirepair_cost = equirepair.cn_d_equirepair_cost;
                newRepair.cn_s_equirepair_material = equirepair.cn_s_equirepair_material;
                if (equirepair.cn_s_equirepair_category == "更换")
                {
                    newRepair.cn_s_equirepair_change_no = equipment.cn_s_equi_no;
                    newRepair.cn_s_equirepair_change_name = equipment.cn_s_equi_name;
                }
                else
                {
                    newRepair.cn_s_equirepair_change_no = "";
                    newRepair.cn_s_equirepair_change_name = "";
                }
                newRepair.cn_s_equirepair_remarks = equirepair.cn_s_equirepair_remarks;
                newRepair.cn_s_creator = user.UserCode;
                newRepair.cn_s_creator_by = user.UserName;
                newRepair.cn_t_create = DateTime.Now;
                tn_dts_logs newEquirepairLog = new tn_dts_logs();
                newEquirepairLog.cn_guid = Guid.NewGuid().ToString();
                newEquirepairLog.cn_s_logs_type = "操作";
                newEquirepairLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能向tn_dts_equirepair表中新增一条维修信息，详细信息为" + JsonConvert.SerializeObject(newRepair);
                newEquirepairLog.cn_t_create = DateTime.Now;
                logList.Add(newEquirepairLog);
                if (Db.Queryable<tn_dts_matter>().Where(it => it.cn_s_matter_name == equirepair.cn_s_equirepair_item && it.cn_s_matter_type == "维修").First() is null)
                {
                    isnewMatter = true;
                    newMatter.cn_guid = Guid.NewGuid().ToString();
                    newMatter.cn_s_matter_type = "维修";
                    newMatter.cn_s_matter_sourceid = equirepair.cn_s_equirepair_no;
                    newMatter.cn_s_matter_name = equirepair.cn_s_equirepair_item;
                    newMatter.cn_s_matter_remarks = "";
                    newMatter.cn_s_creator = user.UserCode;
                    newMatter.cn_s_creator_by = user.UserName;
                    newMatter.cn_t_create = DateTime.Now;
                    tn_dts_logs newMatterLog = new tn_dts_logs();
                    newMatterLog.cn_guid = Guid.NewGuid().ToString();
                    newMatterLog.cn_s_logs_type = "操作";
                    newMatterLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能向tn_dts_matter表中新增一条事项信息，详细信息为" + JsonConvert.SerializeObject(newMatter);
                    newMatterLog.cn_t_create = DateTime.Now;
                    logList.Add(newMatterLog);
                }
                if (Db.Queryable<tn_dts_reason>().Where(it => it.cn_s_reason_fault == equirepair.cn_s_equirepair_cause && it.cn_s_reason_solution == equirepair.cn_s_equirepair_solution).First() is null)
                {
                    isnewReason = true;
                    newReason.cn_guid = Guid.NewGuid().ToString();
                    newReason.cn_s_reason_sourceid = equirepair.cn_s_equirepair_no;
                    newReason.cn_s_reason_fault = equirepair.cn_s_equirepair_cause;
                    newReason.cn_s_reason_solution = equirepair.cn_s_equirepair_solution;
                    newReason.cn_s_reason_remarks = "";
                    newReason.cn_s_creator = user.UserCode;
                    newReason.cn_s_creator_by = user.UserName;
                    newReason.cn_t_create = DateTime.Now;
                    tn_dts_logs newReasonLog = new tn_dts_logs();
                    newReasonLog.cn_guid = Guid.NewGuid().ToString();
                    newReasonLog.cn_s_logs_type = "操作";
                    newReasonLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能向tn_dts_reason表中新增一条故障原因与解决方案，详细信息为" + JsonConvert.SerializeObject(newReason);
                    newReasonLog.cn_t_create = DateTime.Now;
                    logList.Add(newReasonLog);
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    if (equirepair.cn_s_equirepair_category == "更换")
                    {
                        dbTran.Updateable<tn_dts_equipment>(oldEquipment).ExecuteCommand();
                        dbTran.Updateable<tn_dts_equibom>(oldEquibom).ExecuteCommand();
                        dbTran.Insertable<tn_dts_equipment>(newEquipment).ExecuteCommand();
                        dbTran.Insertable<tn_dts_equibom>(newEquibom).ExecuteCommand();
                    }
                    dbTran.Insertable<tn_dts_equirepair>(newRepair).ExecuteCommand();
                    if (isnewMatter)
                    {
                        dbTran.Insertable<tn_dts_matter>(newMatter).ExecuteCommand();
                    }
                    if (isnewReason)
                    {
                        dbTran.Insertable<tn_dts_reason>(newReason).ExecuteCommand();
                    }
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "6.3设备（Equirepair）维修管理保存接口新增维修记录失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败！";
                    return returnMessage;
                }
                returnMessage.IsSuccess = true;
                returnMessage.Message = "保存成功！";
                return returnMessage;
            }
            else if (saveData.add_or_modify == "modify")
            {
                if (!(Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == equirepair.cn_s_equirepair_no && it.cn_s_equirepair_item == equirepair.cn_s_equirepair_item && it.cn_s_equirepair_date == equirepair.cn_s_equirepair_date && it.cn_guid != equirepair.cn_guid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "同一个设备在同一时间同一维修项目只能有一个！";
                    return returnMessage;
                }
                tn_dts_equirepair itemRepair = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_guid == equirepair.cn_guid).First();
                if (itemRepair is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "找不到维修记录！";
                    return returnMessage;
                }
                if (itemRepair.cn_s_equirepair_category != equirepair.cn_s_equirepair_category)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "不允许修改维修类别！";
                    return returnMessage;
                }
                if (itemRepair.cn_s_equirepair_no != equirepair.cn_s_equirepair_no)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "不允许修改设备编号！";
                    return returnMessage;
                }
                if (itemRepair.cn_s_equirepair_name != equirepair.cn_s_equirepair_name)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "不允许修改设备名称！";
                    return returnMessage;
                }
                string oldItem = equirepair.cn_s_equirepair_item;
                itemRepair.cn_s_equirepair_item = equirepair.cn_s_equirepair_item;
                itemRepair.cn_s_equirepair_cause = equirepair.cn_s_equirepair_cause;
                itemRepair.cn_s_equirepair_solution = equirepair.cn_s_equirepair_solution;
                itemRepair.cn_s_equirepair_man = equirepair.cn_s_equirepair_man;
                itemRepair.cn_s_equirepair_phone = equirepair.cn_s_equirepair_phone;
                itemRepair.cn_s_equirepair_result = equirepair.cn_s_equirepair_result;
                itemRepair.cn_s_equirepair_date = equirepair.cn_s_equirepair_date;
                itemRepair.cn_d_equirepair_cost = equirepair.cn_d_equirepair_cost;
                itemRepair.cn_s_equirepair_material = equirepair.cn_s_equirepair_material;
                itemRepair.cn_s_equirepair_remarks = equirepair.cn_s_equirepair_remarks;
                itemRepair.cn_s_modify = user.UserCode;
                itemRepair.cn_s_modify_by = user.UserName;
                itemRepair.cn_t_modify = DateTime.Now;
                tn_dts_logs newRepairLog = new tn_dts_logs();
                newRepairLog.cn_guid = Guid.NewGuid().ToString();
                newRepairLog.cn_s_logs_type = "操作";
                newRepairLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能修改了tn_dts_repair表中设备编号为" + equirepair.cn_s_equirepair_no + "的设备的维修项目为" + oldItem + "的维修记录，详细信息为" + JsonConvert.SerializeObject(itemRepair);
                newRepairLog.cn_t_create = DateTime.Now;
                logList.Add(newRepairLog);
                if (Db.Queryable<tn_dts_matter>().Where(it => it.cn_s_matter_name == equirepair.cn_s_equirepair_item && it.cn_s_matter_type == "维修").First() is null)
                {
                    isnewMatter = true;
                    newMatter.cn_guid = Guid.NewGuid().ToString();
                    newMatter.cn_s_matter_type = "维修";
                    newMatter.cn_s_matter_sourceid = equirepair.cn_s_equirepair_no;
                    newMatter.cn_s_matter_name = equirepair.cn_s_equirepair_item;
                    newMatter.cn_s_matter_remarks = "";
                    newMatter.cn_s_creator = user.UserCode;
                    newMatter.cn_s_creator_by = user.UserName;
                    newMatter.cn_t_create = DateTime.Now;
                    tn_dts_logs newMatterLog = new tn_dts_logs();
                    newMatterLog.cn_guid = Guid.NewGuid().ToString();
                    newMatterLog.cn_s_logs_type = "操作";
                    newMatterLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能向tn_dts_matter表中新增一条事项信息，详细信息为" + JsonConvert.SerializeObject(newMatter);
                    newMatterLog.cn_t_create = DateTime.Now;
                    logList.Add(newMatterLog);
                }
                if (Db.Queryable<tn_dts_reason>().Where(it => it.cn_s_reason_fault == equirepair.cn_s_equirepair_cause && it.cn_s_reason_solution == equirepair.cn_s_equirepair_solution).First() is null)
                {
                    isnewReason = true;
                    newReason.cn_guid = Guid.NewGuid().ToString();
                    newReason.cn_s_reason_sourceid = equirepair.cn_s_equirepair_no;
                    newReason.cn_s_reason_fault = equirepair.cn_s_equirepair_cause;
                    newReason.cn_s_reason_solution = equirepair.cn_s_equirepair_solution;
                    newReason.cn_s_reason_remarks = "";
                    newReason.cn_s_creator = user.UserCode;
                    newReason.cn_s_creator_by = user.UserName;
                    newReason.cn_t_create = DateTime.Now;
                    tn_dts_logs newReasonLog = new tn_dts_logs();
                    newReasonLog.cn_guid = Guid.NewGuid().ToString();
                    newReasonLog.cn_s_logs_type = "操作";
                    newReasonLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备维修保存功能向tn_dts_reason表中新增一条故障原因与解决方案，详细信息为" + JsonConvert.SerializeObject(newReason);
                    newReasonLog.cn_t_create = DateTime.Now;
                    logList.Add(newReasonLog);
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Updateable<tn_dts_equirepair>(itemRepair).ExecuteCommand();
                    if (isnewMatter)
                    {
                        dbTran.Insertable<tn_dts_matter>(newMatter).ExecuteCommand();
                    }
                    if (isnewReason)
                    {
                        dbTran.Insertable<tn_dts_reason>(newReason).ExecuteCommand();
                    }
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "6.3设备（Equirepair）维修管理保存接口修改维修记录失败，详细信息为：" + res.Message);
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保存失败！";
                    return returnMessage;
                }
                returnMessage.IsSuccess = true;
                returnMessage.Message = "保存成功！";
                return returnMessage;
            }
            else
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的add_or_modify参数只能为add或modify！";
                return returnMessage;
            }
        }
        #endregion

        #region 删除设备维修
        /// <summary>
        /// 删除设备维修
        /// </summary>
        /// <param name="equirepair"></param>
        /// <returns></returns>
        public ReturnMessage DeleteDataRepair(List<tn_dts_equirepair> equirepairList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_equirepair> latestRepairList = new List<tn_dts_equirepair>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var equirepair in equirepairList)
            {
                tn_dts_equirepair equirepairGuid = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_guid == equirepair.cn_guid).First();
                if (equirepairGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "tn_dts_equirepair表中找不到该唯一标识为：" + equirepair.cn_guid + "的记录！";
                    return returnMessage;
                }
                if (!equirepairGuid.cn_s_equirepair_date.HasValue)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "tn_dts_equirepair表中唯一标识为" + equirepair.cn_guid + "的维修记录的维修时间为空，删除失败！";
                    return returnMessage;
                }
                DateTime? newTimeN = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == equirepairGuid.cn_s_equirepair_no && it.cn_s_equirepair_item == equirepairGuid.cn_s_equirepair_item).OrderBy(it => it.cn_s_equirepair_date, OrderByType.Desc).Select(it => it.cn_s_equirepair_date).First();
                if (newTimeN.HasValue == false)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "维修记录中的设备编号为：" + equirepairGuid.cn_s_equirepair_no + "的设备在维修项目为：" + equirepairGuid.cn_s_equirepair_item + "的最新维修记录的维修时间为空，删除失败!";
                    return returnMessage;
                }
                if (newTimeN.Value != equirepairGuid.cn_s_equirepair_date.Value)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "当前删除的维修记录不是设备编号为：" + equirepairGuid.cn_s_equirepair_no + "的设备在维修项目为：" + equirepairGuid.cn_s_equirepair_item + "的最新维修记录，最新维修时间为：" + newTimeN.Value + "，请先删除该记录，删除失败！";
                    return returnMessage;
                }
                latestRepairList.Add(equirepairGuid);
                tn_dts_logs equirepairLog = new tn_dts_logs();
                equirepairLog.cn_guid = Guid.NewGuid().ToString();
                equirepairLog.cn_s_logs_type = "操作";
                equirepairLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能将tn_dts_equirepair表中设备编号为" + equirepairGuid.cn_s_equirepair_no + "的设备在维修项目为" + equirepairGuid.cn_s_equirepair_item + "的最新维修记录删除，详细信息为" + JsonConvert.SerializeObject(equirepairGuid);
                equirepairLog.cn_t_create = DateTime.Now;
                logList.Add(equirepairLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equirepair>(latestRepairList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "6.4设备（Equirepair）维修管理删除接口删除维修记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        //#region 获取所有维修事项名称
        ///// <summary>
        ///// 获取所有维修事项名称
        ///// </summary>
        ///// <returns></returns>
        //public List<MatterInfo> GetAllRepairMatters()
        //{
        //    List<tn_dts_matter> matterList = Db.Queryable<tn_dts_matter>().Where(it => it.cn_s_matter_type == "维修").ToList();
        //    List<MatterInfo> matterInfoList = new List<MatterInfo>();
        //    foreach (var matter in matterList)
        //    {
        //        MatterInfo matterInfo = new MatterInfo();
        //        matterInfo.mattername = matter.cn_s_matter_name;
        //        matterInfo.creator = matter.cn_s_creator_by;
        //        matterInfo.createtime = matter.cn_t_create;
        //        matterInfoList.Add(matterInfo);
        //    }
        //    return matterInfoList;
        //}
        //#endregion

        //#region  获取所有故障原因解决方案对
        ///// <summary>
        /////  获取所有故障原因解决方案对
        ///// </summary>
        ///// <returns></returns>
        //public List<FaultSolutionPairInfo> GetAllFaultSolutionPairs()
        //{
        //    List<tn_dts_reason> reasonList = Db.Queryable<tn_dts_reason>().Where(it => it.cn_guid == it.cn_guid).ToList();
        //    List<FaultSolutionPairInfo> faultSolutionPairInfoList = new List<FaultSolutionPairInfo>();
        //    foreach (var reason in reasonList)
        //    {
        //        FaultSolutionPairInfo faultSolutionPairInfo = new FaultSolutionPairInfo();
        //        faultSolutionPairInfo.fault = reason.cn_s_reason_fault;
        //        faultSolutionPairInfo.solution = reason.cn_s_reason_solution;
        //        faultSolutionPairInfo.creator = reason.cn_s_creator_by;
        //        faultSolutionPairInfo.createtime = reason.cn_t_create;
        //        faultSolutionPairInfoList.Add(faultSolutionPairInfo);
        //    }
        //    return faultSolutionPairInfoList;
        //}
        //#endregion

        #region 查看详情
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public tn_dts_equirepair Detail(string guid)
        {
            return Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_guid == guid).First();
        }
        #endregion

        #region 删除故障原因
        /// <summary>
        /// 删除故障原因
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ReturnMessage DeleteReason(string guid)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_reason reasonGuid = Db.Queryable<tn_dts_reason>().Where(it => it.cn_guid == guid).First();
            if (reasonGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "tn_dts_reason表中找不到唯一标识为" + guid + "的故障原因记录！";
                return returnMessage;
            }
            tn_dts_logs reasonLog = new tn_dts_logs();
            reasonLog.cn_guid = Guid.NewGuid().ToString();
            reasonLog.cn_s_logs_type = "操作";
            reasonLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能将tn_dts_reason表中故障原因为" + reasonGuid.cn_s_reason_fault + "，解决方案为" + reasonGuid.cn_s_reason_solution + "的故障原因记录删除，详细信息为" + JsonConvert.SerializeObject(reasonGuid);
            reasonLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_reason>(reasonGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(reasonLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "6.9设备（Equirepair&&reason）维修管理删除故障原因接口删除故障原因记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        #region 按事项名称分页模糊查询
        /// <summary>
        /// 按事项名称分页模糊查询
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PagedInfo<MatterInfo> GetMatterInfoPageList(PageParm parm)
        {
            string cn_s_matter_name = parm.Parms["cn_s_matter_name"].ObjToString();
            PagedInfo<tn_dts_matter> matterPagedList = Db.Queryable<tn_dts_matter>()
            .Where(it => it.cn_s_matter_type == "维修")
            .WhereIF(!String.IsNullOrEmpty(cn_s_matter_name), er => er.cn_s_matter_name.Contains(cn_s_matter_name))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
            List<tn_dts_matter> matterList = matterPagedList.DataSource;
            List<MatterInfo> matterInfoList = new List<MatterInfo>();
            foreach (var matter in matterList)
            {
                MatterInfo matterInfo = new MatterInfo();
                matterInfo.cn_guid = matter.cn_guid;
                matterInfo.mattername = matter.cn_s_matter_name;
                matterInfo.creator = matter.cn_s_creator;
                matterInfo.createtime = matter.cn_t_create;
                matterInfoList.Add(matterInfo);
            }
            PagedInfo<MatterInfo> matterInfoPagedList = new PagedInfo<MatterInfo>();
            matterInfoPagedList.PageIndex = matterPagedList.PageIndex;
            matterInfoPagedList.PageSize = matterPagedList.PageSize;
            matterInfoPagedList.TotalCount = matterPagedList.TotalCount;
            matterInfoPagedList.TotalPages = matterPagedList.TotalPages;
            matterInfoPagedList.DataSource = matterInfoList;
            return matterInfoPagedList;
        }
        #endregion

        #region 按故障原因分页模糊查询
        /// <summary>
        /// 按故障原因分页模糊查询
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public PagedInfo<ReasonInfo> GetReasonInfoPageList(PageParm parm)
        {
            string cn_s_reason_fault = parm.Parms["cn_s_reason_fault"].ObjToString();
            PagedInfo<tn_dts_reason> reasonPagedList = Db.Queryable<tn_dts_reason>()
            .WhereIF(!String.IsNullOrEmpty(cn_s_reason_fault), er => er.cn_s_reason_fault.Contains(cn_s_reason_fault))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
            List<tn_dts_reason> reasonList = reasonPagedList.DataSource;
            List<ReasonInfo> reasonInfoList = new List<ReasonInfo>();
            foreach (var reason in reasonList)
            {
                ReasonInfo reasonInfo = new ReasonInfo();
                reasonInfo.cn_guid = reason.cn_guid;
                reasonInfo.fault = reason.cn_s_reason_fault;
                reasonInfo.solution = reason.cn_s_reason_solution;
                reasonInfo.creator = reason.cn_s_creator;
                reasonInfo.createtime = reason.cn_t_create;
                reasonInfoList.Add(reasonInfo);
            }
            PagedInfo<ReasonInfo> reasonInfoPagedList = new PagedInfo<ReasonInfo>();
            reasonInfoPagedList.PageIndex = reasonPagedList.PageIndex;
            reasonInfoPagedList.PageSize = reasonPagedList.PageSize;
            reasonInfoPagedList.TotalCount = reasonPagedList.TotalCount;
            reasonInfoPagedList.TotalPages = reasonPagedList.TotalPages;
            reasonInfoPagedList.DataSource = reasonInfoList;
            return reasonInfoPagedList;
        }
        #endregion

        #region 删除维修项目
        /// <summary>
        /// 删除维修项目
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ReturnMessage DeleteMatter(string guid)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_matter matterGuid = Db.Queryable<tn_dts_matter>().Where(it => it.cn_guid == guid).First();
            if (matterGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "tn_dts_matter表中找不到唯一标识为" + guid + "的维修事项记录！";
                return returnMessage;
            }
            if (matterGuid.cn_s_matter_type != "维修")
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除事项记录的事项类型不是维修，不能删除！";
                return returnMessage;
            }
            tn_dts_logs matterLog = new tn_dts_logs();
            matterLog.cn_guid = Guid.NewGuid().ToString();
            matterLog.cn_s_logs_type = "操作";
            matterLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能将tn_dts_matter表中事项名称为" + matterGuid.cn_s_matter_name + "的维修事项记录删除，详细信息为" + JsonConvert.SerializeObject(matterGuid);
            matterLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_matter>(matterGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(matterLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "6.7设备（Equirepair&&matter）维修管理删除维修项目接口删除维修项目记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        //#region 返回前指定个数个设备维修提醒项
        ///// <summary>
        ///// 返回前指定个数个设备维修提醒项
        ///// </summary>
        ///// <param name="returnNum"></param>
        ///// <returns></returns>
        //public List<EQRepairCollect> GetEQRepairCollectList(int returnNum)
        //{
        //    List<tn_dts_equirepair> equirepairList = Db.Queryable<tn_dts_equirepair>().OrderBy(it => it.cn_s_equirepair_date, OrderByType.Desc).ToList();
        //    List<string> equirepairRepeatNoList = Db.Queryable<tn_dts_equirepair>().OrderBy(it => it.cn_s_equirepair_date, OrderByType.Desc).Select(it => it.cn_s_equirepair_no).ToList();
        //    List<string> equirepairNoList = equirepairRepeatNoList.Distinct().ToList();
        //    List<EQRepairCollect> eQRepairCollectList = new List<EQRepairCollect>();
        //    for (int i = 0; i < returnNum; i++)
        //    {
        //        EQRepairCollect eQRepairCollect = new EQRepairCollect();
        //        if (equirepairNoList.Count <= i)
        //        {
        //            continue;
        //        }
        //        string equino = equirepairNoList[i];
        //        eQRepairCollect.deviceNo = equino;
        //        eQRepairCollect.repairItemCount = equirepairList.Where(it => it.cn_s_equirepair_no == equino).Count().ToString();
        //        List<tn_dts_equirepair> equirepairByEquinoList = equirepairList.Where(it => it.cn_s_equirepair_no == equino).ToList();
        //        List<RepairItemModel> repairItemList = new List<RepairItemModel>();
        //        foreach (var equirepairByEquino in equirepairByEquinoList)
        //        {
        //            RepairItemModel repairItem = new RepairItemModel();
        //            repairItem.repairTime = equirepairByEquino.cn_s_equirepair_date.ToString();
        //            repairItem.repairItemName = equirepairByEquino.cn_s_equirepair_item;
        //            repairItem.repairMain = equirepairByEquino.cn_s_equirepair_man;
        //            repairItem.ext1 = "";
        //            repairItem.ext2 = "";
        //            repairItemList.Add(repairItem);
        //        }
        //        eQRepairCollect.repairItem = repairItemList;
        //        eQRepairCollectList.Add(eQRepairCollect);
        //    }
        //    return eQRepairCollectList;
        //}
        //#endregion
        #region 返回指定设备的前指定个数个维修提醒项
        /// <summary>
        /// 返回指定设备的前指定个数个维修提醒项
        /// </summary>
        /// <param name="returnNum"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public List<EQRepairCollect> GetEQRepairCollectList(int returnNum, string deviceCode)
        {
            List<EQRepairCollect> eQRepairCollectList = new List<EQRepairCollect>();
            //PagedInfo<tn_dts_equirepair> repairPagedList = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == deviceCode).OrderBy(it => it.cn_s_equirepair_date, OrderByType.Desc).ToPage(1, returnNum);
            //List<tn_dts_equirepair> equirepairList = repairPagedList.DataSource;
            List<tn_dts_equirepair> equirepairList = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == deviceCode).OrderBy(it => it.cn_s_equirepair_date, OrderByType.Desc).Take(returnNum).ToList();
            EQRepairCollect eQRepairCollect = new EQRepairCollect();
            eQRepairCollect.deviceNo = deviceCode;
            eQRepairCollect.repairItemCount = equirepairList.Count().ToString();
            List<RepairItemModel> repairItemModelList = new List<RepairItemModel>();
            int count = equirepairList.Count;
            foreach (var equirepair in equirepairList)
            {
                RepairItemModel repairItemModel = new RepairItemModel();
                if (equirepair.cn_s_equirepair_date.HasValue == false)
                {
                    continue;
                }
                repairItemModel.repairTime = equirepair.cn_s_equirepair_date.ToString();
                repairItemModel.repairItemName = equirepair.cn_s_equirepair_item;
                repairItemModel.repairMain = equirepair.cn_s_equirepair_man;
                repairItemModel.ext1 = "";
                repairItemModel.ext2 = "";
                repairItemModelList.Add(repairItemModel);
            }
            eQRepairCollect.repairItem = repairItemModelList;
            eQRepairCollectList.Add(eQRepairCollect);
            return eQRepairCollectList;
        }
        #endregion
    }
}
