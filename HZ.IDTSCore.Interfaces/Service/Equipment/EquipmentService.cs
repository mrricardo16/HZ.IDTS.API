using HZ.CommonUtil.Helpers;
using HZ.CommonUtil.Model;
using HZ.DbHelper;
using HZ.IDTSCore.Interfaces.IService.Equipment;
using HZ.IDTSCore.Model.Entity.Equipment;
using HZ.IDTSCore.Model.Entity.Sys;
using Newtonsoft.Json;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace HZ.IDTSCore.Interfaces.Service.Equipment
{
    public class EquipmentService : BaseService<tn_dts_equipment>, IEquipmentService
    {
        public EquipmentService(SessionInfo session) : base(session)
        {

        }

        #region 分页模糊查询设备编号和设备名称
        /// <summary>
        /// 分页模糊查询设备编号和设备名称
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equipment> GetListPages(PageParm parm)
        {
            string cn_s_equi_no = parm.Parms["cn_s_equi_no"].ObjToString();
            string cn_s_equi_name = parm.Parms["cn_s_equi_name"].ObjToString();
            string cn_s_equi_parttype = parm.Parms["cn_s_equi_parttype"].ObjToString();
            return Db.Queryable<tn_dts_equipment>().WhereIF(!string.IsNullOrEmpty(cn_s_equi_no), (s => s.cn_s_equi_no.Contains(cn_s_equi_no)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_equi_name), (s => s.cn_s_equi_name.Contains(cn_s_equi_name)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_equi_parttype), (s => s.cn_s_equi_name.Contains(cn_s_equi_parttype)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除设备
        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_equipment>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 按设备编号和设备名称混合模糊查询获取列表数据
        /// <summary>
        /// 按设备编号和设备名称混合模糊查询获取列表数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<CompletemachineTree> GetCompletemachineTreeList(PageParm param)
        {
            string cn_s_equi_no_or_name = param.Parms["cn_s_equi_no_or_name"].ObjToString();
            List<tn_dts_equipment> equipmentList = new List<tn_dts_equipment>();
            PagedInfo<tn_dts_equipment> completemachinePagedList = Db.Queryable<tn_dts_equipment>().Where(s => s.cn_s_equi_parttype == "整机")
            .WhereIF(!string.IsNullOrEmpty(cn_s_equi_no_or_name), (s => (s.cn_s_equi_no.Contains(cn_s_equi_no_or_name)) || s.cn_s_equi_name.Contains(cn_s_equi_no_or_name)))
            .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? " cn_t_modify desc" : param.OrderBy)
            .ToPage(param.PageIndex, param.PageSize);
            equipmentList = completemachinePagedList.DataSource;
            List<CompletemachineTree> completemachineTreeList = GetCompletemachineList(equipmentList);
            PagedInfo<CompletemachineTree> completemachineTreePagedList = new PagedInfo<CompletemachineTree>();
            completemachineTreePagedList.PageIndex = completemachinePagedList.PageIndex;
            completemachineTreePagedList.PageSize = completemachinePagedList.PageSize;
            completemachineTreePagedList.TotalCount = completemachinePagedList.TotalCount;
            completemachineTreePagedList.TotalPages = completemachinePagedList.TotalPages;
            completemachineTreePagedList.DataSource = completemachineTreeList;
            return completemachineTreePagedList;
        }

        /// <summary>
        /// 调用递归算法遍历模糊查询得到的所有整机
        /// </summary>
        /// <param name="equipmentList">分页查询的得到的分页整机列表</param>
        /// <returns>设备树列表</returns>
        public List<CompletemachineTree> GetCompletemachineList(List<tn_dts_equipment> equipmentList)
        {
            List<CompletemachineTree> completemachineTreeList = new List<CompletemachineTree>();
            foreach (var equipment in equipmentList)
            {
                CompletemachineTree completemachineTree = new CompletemachineTree();
                completemachineTree.cn_guid = equipment.cn_guid;
                completemachineTree.cn_s_equi_parttype = equipment.cn_s_equi_parttype;
                completemachineTree.cn_s_equi_no = equipment.cn_s_equi_no;
                completemachineTree.cn_s_equi_name = equipment.cn_s_equi_name;
                completemachineTree.cn_s_equi_type = equipment.cn_s_equi_type;
                completemachineTree.cn_s_equi_model = equipment.cn_s_equi_model;
                completemachineTree.cn_s_equi_status = equipment.cn_s_equi_status;
                completemachineTree.cn_s_equi_buydate = equipment.cn_s_equi_buydate;
                completemachineTree.cn_s_equi_qadate = equipment.cn_s_equi_qadate;
                completemachineTree.cn_s_equi_firstdate = equipment.cn_s_equi_firstdate;
                completemachineTree.cn_s_equi_defentperiod = equipment.cn_s_equi_defentperiod;
                completemachineTree.cn_s_equi_dept = equipment.cn_s_equi_dept;
                completemachineTree.cn_s_equi_dutyman = equipment.cn_s_equi_dutyman;
                completemachineTree.cn_s_equi_dutyphone = equipment.cn_s_equi_dutyphone;
                completemachineTree.cn_s_equi_contractno = equipment.cn_s_equi_contractno;
                completemachineTree.cn_s_equi_beltline = equipment.cn_s_equi_beltline;
                completemachineTree.cn_s_equi_xpos = equipment.cn_s_equi_xpos;
                completemachineTree.cn_s_equi_ypos = equipment.cn_s_equi_ypos;
                completemachineTree.cn_s_equi_zpos = equipment.cn_s_equi_zpos;
                completemachineTree.cn_s_equi_remarks = equipment.cn_s_equi_remarks;
                completemachineTree.cn_s_modify = equipment.cn_s_modify;
                completemachineTree.cn_s_modify_by = equipment.cn_s_modify_by;
                completemachineTree.cn_t_modify = equipment.cn_t_modify;
                completemachineTree.cn_s_creator = equipment.cn_s_creator;
                completemachineTree.cn_s_creator_by = equipment.cn_s_creator_by;
                completemachineTree.cn_t_create = equipment.cn_t_create;
                completemachineTree.children = GetChildrenList_Recrusion(completemachineTree);
                completemachineTreeList.Add(completemachineTree);
            }
            return completemachineTreeList;
        }

        /// <summary>
        /// 递归获取父类所有子类信息列表
        /// </summary>
        /// <param name="completemachine">设备树</param>
        /// <returns>设备树列表</returns>
        public List<CompletemachineTree> GetChildrenList_Recrusion(CompletemachineTree completemachine)
        {
            List<string> childnoList = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == completemachine.cn_s_equi_no && it.cn_s_equibom_parentname == completemachine.cn_s_equi_name).OrderBy(it => it.cn_s_equibom_childno).Select(it => it.cn_s_equibom_childno).ToList();
            if (childnoList.Count == 0)
            {
                return new List<CompletemachineTree>();
            }
            List<CompletemachineTree> completemachineTreeList = new List<CompletemachineTree>();
            foreach (var childno in childnoList)
            {
                tn_dts_equipment tn_Dts_Equipment = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == childno).ToList()[0];
                CompletemachineTree completemachineTree = new CompletemachineTree();
                completemachineTree.cn_guid = tn_Dts_Equipment.cn_guid;
                completemachineTree.cn_s_equi_parttype = tn_Dts_Equipment.cn_s_equi_parttype;
                completemachineTree.cn_s_equi_no = tn_Dts_Equipment.cn_s_equi_no;
                completemachineTree.cn_s_equi_name = tn_Dts_Equipment.cn_s_equi_name;
                completemachineTree.cn_s_equi_type = tn_Dts_Equipment.cn_s_equi_type;
                completemachineTree.cn_s_equi_model = tn_Dts_Equipment.cn_s_equi_model;
                completemachineTree.cn_s_equi_status = tn_Dts_Equipment.cn_s_equi_status;
                completemachineTree.cn_s_equi_buydate = tn_Dts_Equipment.cn_s_equi_buydate;
                completemachineTree.cn_s_equi_qadate = tn_Dts_Equipment.cn_s_equi_qadate;
                completemachineTree.cn_s_equi_firstdate = tn_Dts_Equipment.cn_s_equi_firstdate;
                completemachineTree.cn_s_equi_defentperiod = tn_Dts_Equipment.cn_s_equi_defentperiod;
                completemachineTree.cn_s_equi_dept = tn_Dts_Equipment.cn_s_equi_dept;
                completemachineTree.cn_s_equi_dutyman = tn_Dts_Equipment.cn_s_equi_dutyman;
                completemachineTree.cn_s_equi_dutyphone = tn_Dts_Equipment.cn_s_equi_dutyphone;
                completemachineTree.cn_s_equi_contractno = tn_Dts_Equipment.cn_s_equi_contractno;
                completemachineTree.cn_s_equi_beltline = tn_Dts_Equipment.cn_s_equi_beltline;
                completemachineTree.cn_s_equi_xpos = tn_Dts_Equipment.cn_s_equi_xpos;
                completemachineTree.cn_s_equi_ypos = tn_Dts_Equipment.cn_s_equi_ypos;
                completemachineTree.cn_s_equi_zpos = tn_Dts_Equipment.cn_s_equi_zpos;
                completemachineTree.cn_s_equi_remarks = tn_Dts_Equipment.cn_s_equi_remarks;
                completemachineTree.cn_s_modify = tn_Dts_Equipment.cn_s_modify;
                completemachineTree.cn_s_modify_by = tn_Dts_Equipment.cn_s_modify_by;
                completemachineTree.cn_t_modify = tn_Dts_Equipment.cn_t_modify;
                completemachineTree.cn_s_creator = tn_Dts_Equipment.cn_s_creator;
                completemachineTree.cn_s_creator_by = tn_Dts_Equipment.cn_s_creator_by;
                completemachineTree.cn_t_create = tn_Dts_Equipment.cn_t_create;
                completemachineTree.children = GetChildrenList_Recrusion(completemachineTree);
                completemachineTreeList.Add(completemachineTree);
            }
            return completemachineTreeList;
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="saveData">设备档案保存的数据</param>
        /// <returns></returns>
        public ReturnMessage SaveData(SaveData saveData)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            tn_dts_equipment parentEquipment = saveData.parent_equipment;
            tn_dts_equipment childEquipment = saveData.child_equipment;
            UserSession user = GetSessionInfo();
            if (saveData.completemachine_and_part == "completemachine")//整机
            {
                if (saveData.add_or_modify == "modify")//修改整机
                {
                    if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == childEquipment.cn_guid).ToList().Count == 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "未找到唯一标识为" + childEquipment.cn_guid + "的记录";
                        return returnMessage;
                    }
                    if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == childEquipment.cn_s_equi_no && it.cn_guid != childEquipment.cn_guid).ToList().Count != 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备编号不能重复！";
                        return returnMessage;
                    }
                    tn_dts_equipment itemGuid = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == childEquipment.cn_guid).First();
                    if (itemGuid is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "未找到编号为" + childEquipment.cn_s_equi_no + "记录，不能修改！";
                        return returnMessage;
                    }
                    if (itemGuid.cn_s_equi_parttype != "整机")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "修改的设备编号记录在数据库中不为整机，请重试！";
                        return returnMessage;
                    }
                    if (childEquipment.cn_s_equi_parttype != "整机")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备部件类型不允许修改！";
                        return returnMessage;
                    }
                    if (itemGuid.cn_s_equi_no != childEquipment.cn_s_equi_no)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备编号不允许修改，请删除该整机后重新创建！";
                        return returnMessage;
                    }
                    if (itemGuid.cn_s_equi_status != childEquipment.cn_s_equi_status && childEquipment.cn_s_equi_status == "报废")
                    {
                        if (!(Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == childEquipment.cn_s_equi_no && it.cn_s_equibom_status == "正常").First() is null))
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该整机有正常子项，请从最底层开始报废！";
                            return returnMessage;
                        }
                    }
                    if (itemGuid.cn_s_equi_status != childEquipment.cn_s_equi_status && childEquipment.cn_s_equi_status == "正常")
                    {
                        if (!(Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == childEquipment.cn_s_equi_no && it.cn_s_equibom_status == "报废").First() is null))
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该整机有报废子项，请从最底层开始恢复正常！";
                            return returnMessage;
                        }
                    }

                    bool isModifyName = false;
                    List<tn_dts_logs> logList = new List<tn_dts_logs>();
                    List<tn_dts_equibom> newEquibom_parentList = new List<tn_dts_equibom>();
                    List<tn_dts_equirepair> newRepairList = new List<tn_dts_equirepair>();
                    List<tn_dts_equiupkeep> newUpkeepList = new List<tn_dts_equiupkeep>();
                    List<tn_dts_equiobject> newObjectList = new List<tn_dts_equiobject>();
                    if (itemGuid.cn_s_equi_name != childEquipment.cn_s_equi_name)//修改整机设备名称
                    {
                        isModifyName = true;
                        List<tn_dts_equibom> oldEquibom_parentList = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldEquibom_parent in oldEquibom_parentList)
                        {
                            oldEquibom_parent.cn_s_equibom_parentname = childEquipment.cn_s_equi_name;
                            oldEquibom_parent.cn_s_modify = user.UserCode;
                            oldEquibom_parent.cn_s_modify_by = user.UserName;
                            oldEquibom_parent.cn_t_modify = DateTime.Now;
                            newEquibom_parentList.Add(oldEquibom_parent);
                            tn_dts_logs equibomLog = new tn_dts_logs();
                            equibomLog.cn_guid = Guid.NewGuid().ToString();
                            equibomLog.cn_s_logs_type = "操作";
                            equibomLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equibom表中父项设备编号为" + childEquipment.cn_s_equi_no + "的关系记录中父项设备名称，详细信息为" + JsonConvert.SerializeObject(oldEquibom_parent);
                            equibomLog.cn_t_create = DateTime.Now;
                            logList.Add(equibomLog);
                        }
                        List<tn_dts_equirepair> oldRepairList = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldRepair in oldRepairList)
                        {
                            oldRepair.cn_s_equirepair_name = childEquipment.cn_s_equi_name;
                            oldRepair.cn_s_modify = user.UserCode;
                            oldRepair.cn_s_modify_by = user.UserName;
                            oldRepair.cn_t_modify = DateTime.Now;
                            newRepairList.Add(oldRepair);
                            tn_dts_logs equirepairLog = new tn_dts_logs();
                            equirepairLog.cn_guid = Guid.NewGuid().ToString();
                            equirepairLog.cn_s_logs_type = "操作";
                            equirepairLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equirepair表中设备编号为" + childEquipment.cn_s_equi_no + "的维修记录中设备名称，详细信息为" + JsonConvert.SerializeObject(oldRepair);
                            equirepairLog.cn_t_create = DateTime.Now;
                            logList.Add(equirepairLog);
                        }
                        List<tn_dts_equiupkeep> oldUpkeepList = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldUpkeep in oldUpkeepList)
                        {
                            oldUpkeep.cn_s_equiupkeep_name = childEquipment.cn_s_equi_name;
                            oldUpkeep.cn_s_modify = user.UserCode;
                            oldUpkeep.cn_s_modify_by = user.UserName;
                            oldUpkeep.cn_t_modify = DateTime.Now;
                            newUpkeepList.Add(oldUpkeep);
                            tn_dts_logs equiupkeepLog = new tn_dts_logs();
                            equiupkeepLog.cn_guid = Guid.NewGuid().ToString();
                            equiupkeepLog.cn_s_logs_type = "操作";
                            equiupkeepLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equiupkeep表中设备编号为" + childEquipment.cn_s_equi_no + "的保养记录中设备名称，详细信息为" + JsonConvert.SerializeObject(oldUpkeep);
                            equiupkeepLog.cn_t_create = DateTime.Now;
                            logList.Add(equiupkeepLog);
                        }
                        List<tn_dts_equiobject> oldObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_equi_no == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldObject in oldObjectList)
                        {
                            oldObject.cn_s_object_equi_name = childEquipment.cn_s_equi_name;
                            oldObject.cn_s_modify = user.UserCode;
                            oldObject.cn_s_modify_by = user.UserName;
                            oldObject.cn_t_modify = DateTime.Now;
                            newObjectList.Add(oldObject);
                            tn_dts_logs equiobjectLog = new tn_dts_logs();
                            equiobjectLog.cn_guid = Guid.NewGuid().ToString();
                            equiobjectLog.cn_s_logs_type = "操作";
                            equiobjectLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equiobject表中设备编号为" + childEquipment.cn_s_equi_no + "的对象记录中设备名称，详细信息为" + JsonConvert.SerializeObject(oldObject);
                            equiobjectLog.cn_t_create = DateTime.Now;
                            logList.Add(equiobjectLog);
                        }
                    }

                    itemGuid.cn_s_equi_name = childEquipment.cn_s_equi_name;
                    itemGuid.cn_s_equi_type = childEquipment.cn_s_equi_type;
                    itemGuid.cn_s_equi_model = childEquipment.cn_s_equi_model;
                    itemGuid.cn_s_equi_status = childEquipment.cn_s_equi_status;
                    itemGuid.cn_s_equi_buydate = childEquipment.cn_s_equi_buydate;
                    itemGuid.cn_s_equi_qadate = childEquipment.cn_s_equi_qadate;
                    itemGuid.cn_s_equi_firstdate = childEquipment.cn_s_equi_firstdate;
                    itemGuid.cn_s_equi_defentperiod = childEquipment.cn_s_equi_defentperiod;
                    itemGuid.cn_s_equi_dept = childEquipment.cn_s_equi_dept;
                    itemGuid.cn_s_equi_dutyman = childEquipment.cn_s_equi_dutyman;
                    itemGuid.cn_s_equi_dutyphone = childEquipment.cn_s_equi_dutyphone;
                    itemGuid.cn_s_equi_contractno = childEquipment.cn_s_equi_contractno;
                    itemGuid.cn_s_equi_beltline = childEquipment.cn_s_equi_beltline;
                    itemGuid.cn_s_equi_xpos = childEquipment.cn_s_equi_xpos;
                    itemGuid.cn_s_equi_ypos = childEquipment.cn_s_equi_ypos;
                    itemGuid.cn_s_equi_zpos = childEquipment.cn_s_equi_zpos;
                    itemGuid.cn_s_equi_remarks = childEquipment.cn_s_equi_remarks;
                    itemGuid.cn_s_modify = user.UserCode;
                    itemGuid.cn_s_modify_by = user.UserName;
                    itemGuid.cn_t_modify = DateTime.Now;
                    tn_dts_logs log = new tn_dts_logs();
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equipment表中设备编号为" + childEquipment.cn_s_equi_no + "的整机，详细信息为" + JsonConvert.SerializeObject(itemGuid);
                    log.cn_t_create = DateTime.Now;
                    logList.Add(log);
                    ApiResult res = UseTransaction(dbTran =>
                    {
                        dbTran.Updateable<tn_dts_equipment>(itemGuid).ExecuteCommand();
                        if (isModifyName)
                        {
                            dbTran.Updateable<tn_dts_equibom>(newEquibom_parentList).ExecuteCommand();
                            dbTran.Updateable<tn_dts_equirepair>(newRepairList).ExecuteCommand();
                            dbTran.Updateable<tn_dts_equiupkeep>(newUpkeepList).ExecuteCommand();
                            dbTran.Updateable<tn_dts_equiobject>(newObjectList).ExecuteCommand();
                        }
                        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                    });
                    if (!res.IsSuccess)
                    {
                        LogHelper.Info(DateTime.Now.ToString() + "5.2设备（Equipment&&Equibom）档案管理-保存接口修改整机失败，详细信息为：" + res.Message);
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "修改整机失败！";
                        return returnMessage;
                    }

                    returnMessage.IsSuccess = true;
                    returnMessage.Message = "修改整机成功！";
                    return returnMessage;
                }
                else if (saveData.add_or_modify == "add")//新增整机
                {
                    if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == childEquipment.cn_s_equi_no).ToList().Count != 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备编号不能重复！";
                        return returnMessage;
                    }
                    if (childEquipment.cn_s_equi_parttype != "整机")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "新增设备的部件类型不为整机，请重试！";
                        return returnMessage;
                    }
                    tn_dts_equipment equipment = new tn_dts_equipment();
                    equipment.cn_guid = Guid.NewGuid().ToString();
                    equipment.cn_s_equi_parttype = childEquipment.cn_s_equi_parttype;
                    equipment.cn_s_equi_no = childEquipment.cn_s_equi_no;
                    equipment.cn_s_equi_name = childEquipment.cn_s_equi_name;
                    equipment.cn_s_equi_type = childEquipment.cn_s_equi_type;
                    equipment.cn_s_equi_model = childEquipment.cn_s_equi_model;
                    equipment.cn_s_equi_status = childEquipment.cn_s_equi_status;
                    equipment.cn_s_equi_buydate = childEquipment.cn_s_equi_buydate;
                    equipment.cn_s_equi_qadate = childEquipment.cn_s_equi_qadate;
                    equipment.cn_s_equi_firstdate = childEquipment.cn_s_equi_firstdate;
                    equipment.cn_s_equi_defentperiod = childEquipment.cn_s_equi_defentperiod;
                    equipment.cn_s_equi_dept = childEquipment.cn_s_equi_dept;
                    equipment.cn_s_equi_dutyman = childEquipment.cn_s_equi_dutyman;
                    equipment.cn_s_equi_dutyphone = childEquipment.cn_s_equi_dutyphone;
                    equipment.cn_s_equi_contractno = childEquipment.cn_s_equi_contractno;
                    equipment.cn_s_equi_beltline = childEquipment.cn_s_equi_beltline;
                    equipment.cn_s_equi_xpos = childEquipment.cn_s_equi_xpos;
                    equipment.cn_s_equi_ypos = childEquipment.cn_s_equi_ypos;
                    equipment.cn_s_equi_zpos = childEquipment.cn_s_equi_zpos;
                    equipment.cn_s_equi_remarks = childEquipment.cn_s_equi_remarks;
                    equipment.cn_s_creator = user.UserCode;
                    equipment.cn_s_creator_by = user.UserName;
                    equipment.cn_t_create = DateTime.Now;
                    tn_dts_logs log = new tn_dts_logs();
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能向tn_dts_equipment表中新增一条整机记录，详细信息为" + JsonConvert.SerializeObject(equipment);
                    log.cn_t_create = DateTime.Now;
                    ApiResult res = UseTransaction(dbTran =>
                    {
                        dbTran.Insertable<tn_dts_equipment>(equipment).ExecuteCommand();
                        dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
                    });
                    if (!res.IsSuccess)
                    {
                        LogHelper.Info(DateTime.Now.ToString() + "5.2设备（Equipment&&Equibom）档案管理-保存接口新增整机失败，详细信息为：" + res.Message);
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "新增整机失败！";
                        return returnMessage;
                    }
                    returnMessage.IsSuccess = true;
                    returnMessage.Message = "新增整机成功！";
                    return returnMessage;
                }
                else
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的add_or_modify参数只能为add或modify！";
                    return returnMessage;
                }
            }
            else if (saveData.completemachine_and_part == "part")//零部件
            {
                if (saveData.add_or_modify == "modify")//修改零部件
                {
                    if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == childEquipment.cn_guid).ToList().Count == 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "未找到唯一标识为" + childEquipment.cn_guid + "的记录";
                        return returnMessage;
                    }
                    if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == childEquipment.cn_s_equi_no && it.cn_guid != childEquipment.cn_guid).ToList().Count != 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备编号不能重复！";
                        return returnMessage;
                    }
                    tn_dts_equipment itemGuid = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == childEquipment.cn_guid).First();
                    if (itemGuid is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "未找到编号为" + childEquipment.cn_s_equi_no + "记录，不能修改！";
                        return returnMessage;
                    }
                    if (itemGuid.cn_s_equi_parttype != "零部件")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "修改的设备编号记录在数据库中不为零部件，请重试！";
                        return returnMessage;
                    }
                    if (childEquipment.cn_s_equi_parttype != "零部件")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备部件类型不允许修改！";
                        return returnMessage;
                    }
                    if (itemGuid.cn_s_equi_no != childEquipment.cn_s_equi_no)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备编号不允许修改，请删除该零部件后重新创建！";
                        return returnMessage;
                    }
                    tn_dts_equibom oldEquibom = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_childno == childEquipment.cn_s_equi_no).First();
                    if (oldEquibom is null)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "tn_dts_equibom表中找不到子项编号为修改编号的记录！";
                        return returnMessage;
                    }

                    bool ismodifyName = false;
                    bool ismodifyStatus = false;
                    List<tn_dts_logs> logList = new List<tn_dts_logs>();
                    List<tn_dts_equibom> newEquibomList = new List<tn_dts_equibom>();
                    List<tn_dts_equirepair> newRepairList = new List<tn_dts_equirepair>();
                    List<tn_dts_equiupkeep> newUpkeepList = new List<tn_dts_equiupkeep>();
                    List<tn_dts_equiobject> newObjectList = new List<tn_dts_equiobject>();
                    if (itemGuid.cn_s_equi_status != childEquipment.cn_s_equi_status && childEquipment.cn_s_equi_status == "报废")
                    {
                        ismodifyStatus = true;
                        if (!(Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == childEquipment.cn_s_equi_no && it.cn_s_equibom_status == "正常").First() is null))
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该零部件有正常子项，请从最底层开始报废！";
                            return returnMessage;
                        }

                        oldEquibom.cn_s_equibom_status = childEquipment.cn_s_equi_status;
                        oldEquibom.cn_t_equibom_lapsetime = DateTime.Now;
                        oldEquibom.cn_s_modify = user.UserCode;
                        oldEquibom.cn_s_modify_by = user.UserName;
                        oldEquibom.cn_t_modify = DateTime.Now;
                        newEquibomList.Add(oldEquibom);
                        tn_dts_logs equibomStatusLog = new tn_dts_logs();
                        equibomStatusLog.cn_guid = Guid.NewGuid().ToString();
                        equibomStatusLog.cn_s_logs_type = "操作";
                        equibomStatusLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equibom表中子项设备编号为" + childEquipment.cn_s_equi_no + "的关系记录中关系状态和失效时间，详细信息为" + JsonConvert.SerializeObject(oldEquibom);
                        equibomStatusLog.cn_t_create = DateTime.Now;
                        logList.Add(equibomStatusLog);
                    }

                    if (itemGuid.cn_s_equi_status != childEquipment.cn_s_equi_status && childEquipment.cn_s_equi_status == "正常")
                    {
                        ismodifyStatus = true;
                        if (!(Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == childEquipment.cn_s_equi_no && it.cn_s_equibom_status == "报废").First() is null))
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该零部件有报废子项，请从最底层开始恢复正常！";
                            return returnMessage;
                        }

                        oldEquibom.cn_s_equibom_status = childEquipment.cn_s_equi_status;
                        oldEquibom.cn_t_equibom_lapsetime = DateTime.MaxValue;
                        oldEquibom.cn_s_modify = user.UserCode;
                        oldEquibom.cn_s_modify_by = user.UserName;
                        oldEquibom.cn_t_modify = DateTime.Now;
                        newEquibomList.Add(oldEquibom);
                        tn_dts_logs equibomStatusLog = new tn_dts_logs();
                        equibomStatusLog.cn_guid = Guid.NewGuid().ToString();
                        equibomStatusLog.cn_s_logs_type = "操作";
                        equibomStatusLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equibom表中子项设备编号为" + childEquipment.cn_s_equi_no + "的关系记录中关系状态和失效时间，详细信息为" + JsonConvert.SerializeObject(oldEquibom);
                        equibomStatusLog.cn_t_create = DateTime.Now;
                        logList.Add(equibomStatusLog);
                    }


                    if (itemGuid.cn_s_equi_name != childEquipment.cn_s_equi_name)//修改零部件设备名称
                    {
                        ismodifyName = true;
                        oldEquibom.cn_s_equibom_childname = childEquipment.cn_s_equi_name;
                        oldEquibom.cn_s_modify = user.UserCode;
                        oldEquibom.cn_s_modify_by = user.UserName;
                        oldEquibom.cn_t_modify = DateTime.Now;
                        newEquibomList.Add(oldEquibom);
                        tn_dts_logs equibomChildnameLog = new tn_dts_logs();
                        equibomChildnameLog.cn_guid = Guid.NewGuid().ToString();
                        equibomChildnameLog.cn_s_logs_type = "操作";
                        equibomChildnameLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equibom表中子项设备编号为" + childEquipment.cn_s_equi_no + "的关系记录中子项设备名称，详细信息为" + JsonConvert.SerializeObject(oldEquibom);
                        equibomChildnameLog.cn_t_create = DateTime.Now;
                        logList.Add(equibomChildnameLog);

                        List<tn_dts_equibom> oldEquibom_parentList = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldEquibom_parent in oldEquibom_parentList)
                        {
                            oldEquibom_parent.cn_s_equibom_parentname = childEquipment.cn_s_equi_name;
                            oldEquibom_parent.cn_s_modify = user.UserCode;
                            oldEquibom_parent.cn_s_modify_by = user.UserName;
                            oldEquibom_parent.cn_t_modify = DateTime.Now;
                            newEquibomList.Add(oldEquibom_parent);
                            tn_dts_logs equibomParentnameLog = new tn_dts_logs();
                            equibomParentnameLog.cn_guid = Guid.NewGuid().ToString();
                            equibomParentnameLog.cn_s_logs_type = "操作";
                            equibomParentnameLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equibom表中父项设备编号为" + childEquipment.cn_s_equi_no + "的关系记录中父项设备名称，详细信息为" + JsonConvert.SerializeObject(oldEquibom_parent);
                            equibomParentnameLog.cn_t_create = DateTime.Now;
                            logList.Add(equibomParentnameLog);
                        }
                        List<tn_dts_equirepair> oldRepairList = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldRepair in oldRepairList)
                        {
                            oldRepair.cn_s_equirepair_name = childEquipment.cn_s_equi_name;
                            oldRepair.cn_s_modify = user.UserCode;
                            oldRepair.cn_s_modify_by = user.UserName;
                            oldRepair.cn_t_modify = DateTime.Now;
                            newRepairList.Add(oldRepair);
                            tn_dts_logs equibomRepairLog = new tn_dts_logs();
                            equibomRepairLog.cn_guid = Guid.NewGuid().ToString();
                            equibomRepairLog.cn_s_logs_type = "操作";
                            equibomRepairLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equirepair表中设备编号为" + childEquipment.cn_s_equi_no + "的维修记录中设备名称，详细信息为" + JsonConvert.SerializeObject(oldRepair);
                            equibomRepairLog.cn_t_create = DateTime.Now;
                            logList.Add(equibomRepairLog);
                        }
                        List<tn_dts_equiupkeep> oldUpkeepList = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldUpkeep in oldUpkeepList)
                        {
                            oldUpkeep.cn_s_equiupkeep_name = childEquipment.cn_s_equi_name;
                            oldUpkeep.cn_s_modify = user.UserCode;
                            oldUpkeep.cn_s_modify_by = user.UserName;
                            oldUpkeep.cn_t_modify = DateTime.Now;
                            newUpkeepList.Add(oldUpkeep);
                            tn_dts_logs equibomUpkeepLog = new tn_dts_logs();
                            equibomUpkeepLog.cn_guid = Guid.NewGuid().ToString();
                            equibomUpkeepLog.cn_s_logs_type = "操作";
                            equibomUpkeepLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equiupkeep表中设备编号为" + childEquipment.cn_s_equi_no + "的保养记录中设备名称，详细信息为" + JsonConvert.SerializeObject(oldUpkeep);
                            equibomUpkeepLog.cn_t_create = DateTime.Now;
                            logList.Add(equibomUpkeepLog);
                        }
                        List<tn_dts_equiobject> oldObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_equi_no == childEquipment.cn_s_equi_no).ToList();
                        foreach (var oldObject in oldObjectList)
                        {
                            oldObject.cn_s_object_equi_name = childEquipment.cn_s_equi_name;
                            oldObject.cn_s_modify = user.UserCode;
                            oldObject.cn_s_modify_by = user.UserName;
                            oldObject.cn_t_modify = DateTime.Now;
                            newObjectList.Add(oldObject);
                            tn_dts_logs equibomObjectLog = new tn_dts_logs();
                            equibomObjectLog.cn_guid = Guid.NewGuid().ToString();
                            equibomObjectLog.cn_s_logs_type = "操作";
                            equibomObjectLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equiobject表中设备编号为" + childEquipment.cn_s_equi_no + "的对象记录中设备名称，详细信息为" + JsonConvert.SerializeObject(oldObject);
                            equibomObjectLog.cn_t_create = DateTime.Now;
                            logList.Add(equibomObjectLog);
                        }
                    }

                    itemGuid.cn_s_equi_name = childEquipment.cn_s_equi_name;
                    itemGuid.cn_s_equi_type = childEquipment.cn_s_equi_type;
                    itemGuid.cn_s_equi_model = childEquipment.cn_s_equi_model;
                    itemGuid.cn_s_equi_status = childEquipment.cn_s_equi_status;
                    itemGuid.cn_s_equi_buydate = childEquipment.cn_s_equi_buydate;
                    itemGuid.cn_s_equi_qadate = childEquipment.cn_s_equi_qadate;
                    itemGuid.cn_s_equi_firstdate = childEquipment.cn_s_equi_firstdate;
                    itemGuid.cn_s_equi_defentperiod = childEquipment.cn_s_equi_defentperiod;
                    itemGuid.cn_s_equi_dept = childEquipment.cn_s_equi_dept;
                    itemGuid.cn_s_equi_dutyman = childEquipment.cn_s_equi_dutyman;
                    itemGuid.cn_s_equi_dutyphone = childEquipment.cn_s_equi_dutyphone;
                    itemGuid.cn_s_equi_contractno = childEquipment.cn_s_equi_contractno;
                    itemGuid.cn_s_equi_beltline = childEquipment.cn_s_equi_beltline;
                    itemGuid.cn_s_equi_xpos = childEquipment.cn_s_equi_xpos;
                    itemGuid.cn_s_equi_ypos = childEquipment.cn_s_equi_ypos;
                    itemGuid.cn_s_equi_zpos = childEquipment.cn_s_equi_zpos;
                    itemGuid.cn_s_equi_remarks = childEquipment.cn_s_equi_remarks;
                    itemGuid.cn_s_modify = user.UserCode;
                    itemGuid.cn_s_modify_by = user.UserName;
                    itemGuid.cn_t_modify = DateTime.Now;
                    tn_dts_logs log = new tn_dts_logs();
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能修改了tn_dts_equipment表中设备编号为" + childEquipment.cn_s_equi_no + "的零部件，详细信息为" + JsonConvert.SerializeObject(itemGuid);
                    log.cn_t_create = DateTime.Now;
                    logList.Add(log);
                    ApiResult res = UseTransaction(dbTran =>
                    {
                        dbTran.Updateable<tn_dts_equipment>(itemGuid).ExecuteCommand();
                        if (ismodifyName || ismodifyStatus)
                        {
                            dbTran.Updateable<tn_dts_equibom>(newEquibomList).ExecuteCommand();
                        }
                        if (ismodifyName)
                        {
                            dbTran.Updateable<tn_dts_equirepair>(newRepairList).ExecuteCommand();
                            dbTran.Updateable<tn_dts_equiupkeep>(newUpkeepList).ExecuteCommand();
                            dbTran.Updateable<tn_dts_equiobject>(newObjectList).ExecuteCommand();
                        }
                        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                    });
                    if (!res.IsSuccess)
                    {
                        LogHelper.Info(DateTime.Now.ToString() + "5.2设备（Equipment&&Equibom）档案管理-保存接口修改零部件失败，详细信息为：" + res.Message);
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "修改零部件失败！";
                        return returnMessage;
                    }

                    returnMessage.IsSuccess = true;
                    returnMessage.Message = "修改零部件成功！";
                    return returnMessage;
                }
                else if (saveData.add_or_modify == "add")//新增零部件
                {
                    if (Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == childEquipment.cn_s_equi_no).ToList().Count != 0)
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "设备编号不能重复！";
                        return returnMessage;
                    }
                    if (childEquipment.cn_s_equi_parttype != "零部件")
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "新增记录的部件类型不为零部件！";
                        return returnMessage;
                    }
                    tn_dts_equipment parentEquipmentSelect = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == parentEquipment.cn_guid).First();
                    if (parentEquipmentSelect.cn_s_equi_parttype == "整机")
                    {
                        if (parentEquipmentSelect.cn_s_equi_status == "报废" && childEquipment.cn_s_equi_status == "正常")
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该零部件父级整机状态为报废，不允许插入正常零部件！";
                            return returnMessage;
                        }
                        if (parentEquipmentSelect.cn_s_equi_status == "正常" && childEquipment.cn_s_equi_status == "报废")
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该零部件父级整机状态为正常，不允许插入报废零部件！";
                            return returnMessage;
                        }
                    }
                    if (parentEquipmentSelect.cn_s_equi_parttype == "零部件")
                    {
                        if ((Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_childno == parentEquipmentSelect.cn_s_equi_no).First()) is null)
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "tn_dts_equibom表中找不到子项编号为插入父项设备编号的记录！";
                            return returnMessage;
                        }
                        if (!(Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_childno == parentEquipmentSelect.cn_s_equi_no && it.cn_s_equibom_status == "正常").First() is null) && childEquipment.cn_s_equi_status == "报废")
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该零部件父级零部件状态为正常，不允许插入报废零部件！";
                            return returnMessage;
                        }
                        if (!(Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_childno == parentEquipmentSelect.cn_s_equi_no && it.cn_s_equibom_status == "报废").First() is null) && childEquipment.cn_s_equi_status == "正常")
                        {
                            returnMessage.IsSuccess = false;
                            returnMessage.Message = "该零部件父级零部件状态为报废，不允许插入正常零部件！";
                            return returnMessage;
                        }
                    }
                    List<tn_dts_logs> logList = new List<tn_dts_logs>();
                    tn_dts_equipment equipment = new tn_dts_equipment();
                    equipment.cn_guid = Guid.NewGuid().ToString();
                    equipment.cn_s_equi_parttype = childEquipment.cn_s_equi_parttype;
                    equipment.cn_s_equi_no = childEquipment.cn_s_equi_no;
                    equipment.cn_s_equi_name = childEquipment.cn_s_equi_name;
                    equipment.cn_s_equi_type = childEquipment.cn_s_equi_type;
                    equipment.cn_s_equi_model = childEquipment.cn_s_equi_model;
                    equipment.cn_s_equi_status = childEquipment.cn_s_equi_status;
                    equipment.cn_s_equi_buydate = childEquipment.cn_s_equi_buydate;
                    equipment.cn_s_equi_qadate = childEquipment.cn_s_equi_qadate;
                    equipment.cn_s_equi_firstdate = childEquipment.cn_s_equi_firstdate;
                    equipment.cn_s_equi_defentperiod = childEquipment.cn_s_equi_defentperiod;
                    equipment.cn_s_equi_dept = childEquipment.cn_s_equi_dept;
                    equipment.cn_s_equi_dutyman = childEquipment.cn_s_equi_dutyman;
                    equipment.cn_s_equi_dutyphone = childEquipment.cn_s_equi_dutyphone;
                    equipment.cn_s_equi_contractno = childEquipment.cn_s_equi_contractno;
                    equipment.cn_s_equi_beltline = childEquipment.cn_s_equi_beltline;
                    equipment.cn_s_equi_xpos = childEquipment.cn_s_equi_xpos;
                    equipment.cn_s_equi_ypos = childEquipment.cn_s_equi_ypos;
                    equipment.cn_s_equi_zpos = childEquipment.cn_s_equi_zpos;
                    equipment.cn_s_equi_remarks = childEquipment.cn_s_equi_remarks;
                    equipment.cn_s_creator = user.UserCode;
                    equipment.cn_s_creator_by = user.UserName;
                    equipment.cn_t_create = DateTime.Now;
                    tn_dts_logs logEquipment = new tn_dts_logs();
                    logEquipment.cn_guid = Guid.NewGuid().ToString();
                    logEquipment.cn_s_logs_type = "操作";
                    logEquipment.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能向tn_dts_equipment表中新增一条零部件记录，详细信息为" + JsonConvert.SerializeObject(equipment);
                    logEquipment.cn_t_create = DateTime.Now;
                    logList.Add(logEquipment);

                    tn_dts_equibom equibom = new tn_dts_equibom();
                    equibom.cn_guid = Guid.NewGuid().ToString();
                    equibom.cn_s_equibom_parentno = parentEquipmentSelect.cn_s_equi_no;
                    equibom.cn_s_equibom_parentname = parentEquipmentSelect.cn_s_equi_name;
                    equibom.cn_s_equibom_childno = childEquipment.cn_s_equi_no;
                    equibom.cn_s_equibom_childname = childEquipment.cn_s_equi_name;
                    equibom.cn_t_equibom_effectuatetime = DateTime.Now;
                    if (childEquipment.cn_s_equi_status == "正常")
                    {
                        equibom.cn_t_equibom_lapsetime = DateTime.MaxValue;
                    }
                    else if (childEquipment.cn_s_equi_status == "报废")
                    {
                        equibom.cn_t_equibom_lapsetime = DateTime.Now;
                    }
                    else
                    {
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "前端接口传入的child_Equipment项中的cn_s_equi_status不为“正常”或“报废”！";
                        return returnMessage;
                    }
                    equibom.cn_s_equibom_status = childEquipment.cn_s_equi_status;
                    equibom.cn_s_equibom_remarks = null;
                    equibom.cn_s_creator = user.UserCode;
                    equibom.cn_s_creator_by = user.UserName;
                    equibom.cn_t_create = DateTime.Now;
                    tn_dts_logs logEquibom = new tn_dts_logs();
                    logEquibom.cn_guid = Guid.NewGuid().ToString();
                    logEquibom.cn_s_logs_type = "操作";
                    logEquibom.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备档案保存功能向tn_dts_equibom表中新增一条设备关系，详细信息为" + JsonConvert.SerializeObject(equibom);
                    logEquibom.cn_t_create = DateTime.Now;
                    logList.Add(logEquibom);
                    ApiResult res = UseTransaction(dbTran =>
                    {
                        dbTran.Insertable<tn_dts_equipment>(equipment).ExecuteCommand();
                        dbTran.Insertable<tn_dts_equibom>(equibom).ExecuteCommand();
                        dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                    });
                    if (!res.IsSuccess)
                    {
                        LogHelper.Info(DateTime.Now.ToString() + "5.2设备（Equipment&&Equibom）档案管理-保存接口新增零部件失败，详细信息为：" + res.Message);
                        returnMessage.IsSuccess = false;
                        returnMessage.Message = "新增零部件失败！";
                        return returnMessage;
                    }

                    returnMessage.IsSuccess = true;
                    returnMessage.Message = "新增零部件成功！";
                    return returnMessage;
                }
                else
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "前端传入的add_or_modify参数只能为add或modify！";
                    return returnMessage;
                }
            }
            else
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "前端传入的completemachine_and_part参数只能为completemachine或part！";
                return returnMessage;
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="equipment"></param>
        /// <returns></returns>
        public ReturnMessage DeleteData(tn_dts_equipment equipment)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equipment equipmentGuid = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == equipment.cn_guid).First();
            if(equipmentGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "数据库中找不到唯一标识为" + equipment.cn_guid + "的设备记录！";
                return returnMessage;
            }
            if (Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == equipmentGuid.cn_s_equi_no).ToList().Count != 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "设备维修表中有该整机/零部件维修记录，无法删除！";
                return returnMessage;
            }
            if (Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == equipmentGuid.cn_s_equi_no).ToList().Count != 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "设备保养表中有该整机/零部件保养记录，无法删除！";
                return returnMessage;
            }
            if (Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_equiguid == equipmentGuid.cn_guid).ToList().Count != 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "设备对象表中有该整机对象记录，无法删除！";
                return returnMessage;
            }
            if (Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == equipmentGuid.cn_s_equi_no).ToList().Count != 0)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "该整机/零部件有子项零部件未删除，请从子项开始删除！";
                return returnMessage;
            }
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            tn_dts_logs equipmentLog = new tn_dts_logs();
            equipmentLog.cn_guid = Guid.NewGuid().ToString();
            equipmentLog.cn_s_logs_type = "操作";
            equipmentLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能将tn_dts_equipment表中设备编号为" + equipmentGuid.cn_s_equi_no + "的设备记录，详细信息为" + JsonConvert.SerializeObject(equipmentGuid);
            equipmentLog.cn_t_create = DateTime.Now;
            logList.Add(equipmentLog);
            tn_dts_equibom equibomChildno = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_childno == equipmentGuid.cn_s_equi_no).First();
            if (!(equibomChildno is null))
            {
                tn_dts_logs equibomLog = new tn_dts_logs();
                equibomLog.cn_guid = Guid.NewGuid().ToString();
                equibomLog.cn_s_logs_type = "操作";
                equibomLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能将tn_dts_equibom表中子项设备编号为" + equipmentGuid.cn_s_equi_no + "的关系记录，详细信息为" + JsonConvert.SerializeObject(equibomChildno);
                equibomLog.cn_t_create = DateTime.Now;
                logList.Add(equibomLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equipment>(equipmentGuid).ExecuteCommand();
                if(!(equibomChildno is null))
                {
                    dbTran.Deleteable<tn_dts_equibom>(equibomChildno).ExecuteCommand();
                }
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "5.3设备（Equipment&&Equibom）档案管理删除接口删除设备失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        #region 复制整机
        /// <summary>
        /// 复制整机
        /// </summary>
        /// <param name="completemachineTree"></param>
        /// <param name="needCopyComponents"></param>
        /// <returns></returns>
        public ApiResult CopyEquipment(CompletemachineTree completemachineTree, bool needCopyComponents)
        {
            ApiResult apiResult = new ApiResult();
            UserSession user = GetSessionInfo();
            if (needCopyComponents && completemachineTree.children.Count != 0)
            {
                List<tn_dts_equipment> equipmentList = new List<tn_dts_equipment>();
                tn_dts_equipment completemachineEquipment = new tn_dts_equipment()
                {
                    cn_s_equi_parttype = completemachineTree.cn_s_equi_parttype,
                    cn_s_equi_no = completemachineTree.cn_s_equi_no,
                    cn_s_equi_name = completemachineTree.cn_s_equi_name,
                    cn_s_equi_type = completemachineTree.cn_s_equi_type,
                    cn_s_equi_model = completemachineTree.cn_s_equi_model,
                    cn_s_equi_status = completemachineTree.cn_s_equi_status,
                    cn_s_equi_buydate = completemachineTree.cn_s_equi_buydate,
                    cn_s_equi_qadate = completemachineTree.cn_s_equi_qadate,
                    cn_s_equi_firstdate = completemachineTree.cn_s_equi_firstdate,
                    cn_s_equi_defentperiod = completemachineTree.cn_s_equi_defentperiod,
                    cn_s_equi_dept = completemachineTree.cn_s_equi_dept,
                    cn_s_equi_dutyman = completemachineTree.cn_s_equi_dutyman,
                    cn_s_equi_dutyphone = completemachineTree.cn_s_equi_dutyphone,
                    cn_s_equi_contractno = completemachineTree.cn_s_equi_beltline,
                    cn_s_equi_beltline = completemachineTree.cn_s_equi_beltline,
                    cn_s_equi_xpos = completemachineTree.cn_s_equi_xpos,
                    cn_s_equi_ypos = completemachineTree.cn_s_equi_ypos,
                    cn_s_equi_zpos = completemachineTree.cn_s_equi_zpos,
                    cn_s_equi_remarks = completemachineTree.cn_s_equi_remarks,
                };
                equipmentList.Add(completemachineEquipment);
                List<tn_dts_equipment> childrenEquipmentList = new List<tn_dts_equipment>();
                GetAllChildrenEquipment_Recrusion(completemachineTree.children, ref childrenEquipmentList);
                equipmentList.AddRange(childrenEquipmentList);
                List<tn_dts_equipment> copyEquipmentList = new List<tn_dts_equipment>();
                List<tn_dts_logs> copyLogList = new List<tn_dts_logs>();
                foreach (var equipment in equipmentList)
                {
                    if (!(Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == equipment.cn_s_equi_no).First() is null))
                    {
                        apiResult.IsSuccess = true;
                        apiResult.StatusCode = 200;
                        apiResult.Message = "设备编号重复！";
                        apiResult.Data = equipment;
                        return apiResult;
                    }
                    equipment.cn_guid = Guid.NewGuid().ToString();
                    equipment.cn_s_creator = user.UserCode;
                    equipment.cn_s_creator_by = user.UserName;
                    equipment.cn_t_create = DateTime.Now;
                    copyEquipmentList.Add(equipment);
                    tn_dts_logs log = new tn_dts_logs();
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户为tn_dts_equipment表（复制）新增一条整机/零部件记录，详细信息为：" + JsonConvert.SerializeObject(equipment);
                    log.cn_t_create = DateTime.Now;
                    copyLogList.Add(log);
                }
                List<tn_dts_equibom> copyEquibomList = new List<tn_dts_equibom>();
                GetAllEquibom_Recrusion(completemachineTree, ref copyEquibomList);
                foreach (var copyEquibom in copyEquibomList)
                {
                    tn_dts_logs log = new tn_dts_logs();
                    log.cn_guid = Guid.NewGuid().ToString();
                    log.cn_s_logs_type = "操作";
                    log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户为tn_dts_equibom表（复制）新增一条关系记录，详细信息为：" + JsonConvert.SerializeObject(copyEquibom);
                    log.cn_t_create = DateTime.Now;
                    copyLogList.Add(log);
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_equipment>(copyEquipmentList).ExecuteCommand();
                    dbTran.Insertable<tn_dts_equibom>(copyEquibomList).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(copyLogList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "5.4设备（Equipment&&Equibom）档案管理复制接口新增设备失败，详细信息为：" + res.Message);
                    apiResult.IsSuccess = false;
                    apiResult.StatusCode = 500;
                    apiResult.Message = "复制失败！";
                    return apiResult;
                }
            }
            else
            {
                tn_dts_equipment equipment = new tn_dts_equipment()
                {
                    cn_s_equi_parttype = completemachineTree.cn_s_equi_parttype,
                    cn_s_equi_no = completemachineTree.cn_s_equi_no,
                    cn_s_equi_name = completemachineTree.cn_s_equi_name,
                    cn_s_equi_type = completemachineTree.cn_s_equi_type,
                    cn_s_equi_model = completemachineTree.cn_s_equi_model,
                    cn_s_equi_status = completemachineTree.cn_s_equi_status,
                    cn_s_equi_buydate = completemachineTree.cn_s_equi_buydate,
                    cn_s_equi_qadate = completemachineTree.cn_s_equi_qadate,
                    cn_s_equi_firstdate = completemachineTree.cn_s_equi_firstdate,
                    cn_s_equi_defentperiod = completemachineTree.cn_s_equi_defentperiod,
                    cn_s_equi_dept = completemachineTree.cn_s_equi_dept,
                    cn_s_equi_dutyman = completemachineTree.cn_s_equi_dutyman,
                    cn_s_equi_dutyphone = completemachineTree.cn_s_equi_dutyphone,
                    cn_s_equi_contractno = completemachineTree.cn_s_equi_beltline,
                    cn_s_equi_beltline = completemachineTree.cn_s_equi_beltline,
                    cn_s_equi_xpos = completemachineTree.cn_s_equi_xpos,
                    cn_s_equi_ypos = completemachineTree.cn_s_equi_ypos,
                    cn_s_equi_zpos = completemachineTree.cn_s_equi_zpos,
                    cn_s_equi_remarks = completemachineTree.cn_s_equi_remarks,
                };
                if (!(Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == completemachineTree.cn_s_equi_no).First() is null))
                {
                    apiResult.IsSuccess = true;
                    apiResult.StatusCode = 200;
                    apiResult.Message = "设备编号重复！";
                    apiResult.Data = equipment;
                    return apiResult;
                }
                equipment.cn_guid = Guid.NewGuid().ToString();
                equipment.cn_s_creator = user.UserCode;
                equipment.cn_s_creator_by = user.UserName;
                equipment.cn_t_create = DateTime.Now;
                tn_dts_logs log = new tn_dts_logs();
                log.cn_guid = Guid.NewGuid().ToString();
                log.cn_s_logs_type = "操作";
                log.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户为tn_dts_equipment表（复制）新增一条整机记录，详细信息为：" + JsonConvert.SerializeObject(equipment);
                log.cn_t_create = DateTime.Now;
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_equipment>(equipment).ExecuteCommand();
                    dbTran.Insertable<tn_dts_logs>(log).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "5.4设备（Equipment&&Equibom）档案管理复制接口新增设备失败，详细信息为：" + res.Message);
                    apiResult.IsSuccess = false;
                    apiResult.StatusCode = 500;
                    apiResult.Message = "复制失败！";
                    return apiResult;
                }
            }
            apiResult.IsSuccess = true;
            apiResult.StatusCode = 200;
            apiResult.Message = "复制成功！";
            return apiResult;
        }

        /// <summary>
        /// 读取一个设备树下所有零部件信息（递归）
        /// </summary>
        /// <param name="completemachineTreeList"></param>
        /// <param name="equipmentList"></param>
        public void GetAllChildrenEquipment_Recrusion(List<CompletemachineTree> completemachineTreeList, ref List<tn_dts_equipment> equipmentList)
        {
            if (completemachineTreeList.Count == 0)
            {

            }
            foreach (var completemachineTree in completemachineTreeList)
            {
                tn_dts_equipment equipment = new tn_dts_equipment()
                {
                    cn_s_equi_parttype = completemachineTree.cn_s_equi_parttype,
                    cn_s_equi_no = completemachineTree.cn_s_equi_no,
                    cn_s_equi_name = completemachineTree.cn_s_equi_name,
                    cn_s_equi_type = completemachineTree.cn_s_equi_type,
                    cn_s_equi_model = completemachineTree.cn_s_equi_model,
                    cn_s_equi_status = completemachineTree.cn_s_equi_status,
                    cn_s_equi_buydate = completemachineTree.cn_s_equi_buydate,
                    cn_s_equi_qadate = completemachineTree.cn_s_equi_qadate,
                    cn_s_equi_firstdate = completemachineTree.cn_s_equi_firstdate,
                    cn_s_equi_defentperiod = completemachineTree.cn_s_equi_defentperiod,
                    cn_s_equi_dept = completemachineTree.cn_s_equi_dept,
                    cn_s_equi_dutyman = completemachineTree.cn_s_equi_dutyman,
                    cn_s_equi_dutyphone = completemachineTree.cn_s_equi_dutyphone,
                    cn_s_equi_contractno = completemachineTree.cn_s_equi_beltline,
                    cn_s_equi_beltline = completemachineTree.cn_s_equi_beltline,
                    cn_s_equi_xpos = completemachineTree.cn_s_equi_xpos,
                    cn_s_equi_ypos = completemachineTree.cn_s_equi_ypos,
                    cn_s_equi_zpos = completemachineTree.cn_s_equi_zpos,
                    cn_s_equi_remarks = completemachineTree.cn_s_equi_remarks,
                };
                equipmentList.Add(equipment);
                GetAllChildrenEquipment_Recrusion(completemachineTree.children, ref equipmentList);
            }
        }

        /// <summary>
        /// 读取整机树所有关系（递归）
        /// 注：若传入整机树中设备状态不为“正常”或“报废”视为报废。
        /// </summary>
        /// <param name="completemachineTree"></param>
        /// <param name="equibomList"></param>
        public void GetAllEquibom_Recrusion(CompletemachineTree completemachineTree, ref List<tn_dts_equibom> equibomList)
        {
            if (completemachineTree.children.Count == 0)
            {

            }
            foreach (var child in completemachineTree.children)
            {
                UserSession user = GetSessionInfo();
                tn_dts_equibom equibom = new tn_dts_equibom();
                equibom.cn_guid = Guid.NewGuid().ToString();
                equibom.cn_s_equibom_parentno = completemachineTree.cn_s_equi_no;
                equibom.cn_s_equibom_parentname = completemachineTree.cn_s_equi_name;
                equibom.cn_s_equibom_childno = child.cn_s_equi_no;
                equibom.cn_s_equibom_childname = child.cn_s_equi_name;
                equibom.cn_t_equibom_effectuatetime = DateTime.Now;
                if (child.cn_s_equi_status == "正常")
                {
                    equibom.cn_t_equibom_lapsetime = DateTime.MaxValue;
                    equibom.cn_s_equibom_status = "正常";
                }
                else if (child.cn_s_equi_status == "报废")
                {
                    equibom.cn_t_equibom_lapsetime = DateTime.Now;
                    equibom.cn_s_equibom_status = "报废";
                }
                else
                {
                    equibom.cn_t_equibom_lapsetime = DateTime.Now;
                    equibom.cn_s_equibom_status = "报废";
                    //LogHelper.Info(DateTime.Now.ToString() + "5.4设备（Equipment&&Equibom）档案管理复制接口读取Equibom失败，存在零部件状态不为正常或报废，详细信息为：" + JsonConvert.SerializeObject(child));
                }
                equibom.cn_s_creator = user.UserCode;
                equibom.cn_s_creator = user.UserName;
                equibom.cn_t_create = DateTime.Now;
                equibomList.Add(equibom);
                GetAllEquibom_Recrusion(child, ref equibomList);
            }
        }
        #endregion

        #region 获取设备类型-设备树信息（按设备编号和设备名称混合模糊）
        /// <summary>
        /// 获取设备类型-设备树信息（按设备编号和设备名称混合模糊）
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<EquipmentTypeTree> GetEquipmentTypeTree(PageParm param)
        {
            string equiNoOrName = param.Parms["cn_s_equi_no_or_name"].ObjToString();
            string equiType = param.Parms["cn_s_equi_type"].ObjToString();
            List<tn_dts_equipment> equipmentTypeList = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_parttype == "整机")
                .WhereIF(!string.IsNullOrEmpty(equiNoOrName), it => it.cn_s_equi_no.Contains(equiNoOrName) || it.cn_s_equi_name.Contains(equiNoOrName))
                .WhereIF(!string.IsNullOrEmpty(equiType), it => it.cn_s_equi_type == equiType)
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                .ToList();
            PagedInfo<string> equipmentTypePagedList = equipmentTypeList.Select(it => it.cn_s_equi_type).Distinct().ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
            List<EquipmentTypeTree> equipmentTypeTreeList = new List<EquipmentTypeTree>();
            foreach (var equipmentType in equipmentTypePagedList.DataSource)
            {
                EquipmentTypeTree equipmentTypeTree = new EquipmentTypeTree();
                equipmentTypeTree.EquiType = equipmentType;
                equipmentTypeTree.EquipmentInformationList = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_type == equipmentType && (it.cn_s_equi_no.Contains(equiNoOrName) || it.cn_s_equi_name.Contains(equiNoOrName))).OrderBy(it => it.cn_t_modify)
                    .Select(it => new EquipmentInformation
                    {
                        EquiGuid = it.cn_guid,
                        EquiNo = it.cn_s_equi_no,
                        EquiName = it.cn_s_equi_name
                    }).ToList();
                equipmentTypeTreeList.Add(equipmentTypeTree);
            }
            PagedInfo<EquipmentTypeTree> equipmentReturnTypePagedList = new PagedInfo<EquipmentTypeTree>();
            equipmentReturnTypePagedList.PageIndex = equipmentTypePagedList.PageIndex;
            equipmentReturnTypePagedList.PageSize = equipmentTypePagedList.PageSize;
            equipmentReturnTypePagedList.TotalCount = equipmentTypePagedList.TotalCount;
            equipmentReturnTypePagedList.TotalPages = equipmentTypePagedList.TotalPages;
            equipmentReturnTypePagedList.DataSource = equipmentTypeTreeList;
            return equipmentReturnTypePagedList;
        }
        #endregion

        #region MyRegion

        #endregion

        #region 获取一个设备类型下所有设备信息
        /// <summary>
        /// 获取一个设备类型下所有设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<EquipmentInformation> GetEquipmentInformationByEquitype(PageParm param)
        {
            string equiType = param.Parms["cn_s_equi_type"].ObjToString();
            return Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_type == equiType && it.cn_s_equi_parttype == "整机")
                .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
                .Select(it => new EquipmentInformation
                {
                    EquiGuid = it.cn_guid,
                    EquiNo = it.cn_s_equi_no,
                    EquiName = it.cn_s_equi_name
                }).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

        #region 按设备类型、设备编码和设备名称混合模糊分页查询设备信息
        /// <summary>
        /// 按设备类型、设备编码和设备名称混合模糊分页查询设备信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public PagedInfo<EquipmentInformation> GetEquipment(PageParm param)
        {
            string equipmentType = param.Parms["cn_s_equipment_type"].ObjToString();
            string equipmentNoOrName = param.Parms["cn_s_equipment_no_or_name"].ObjToString();
            List<tn_dts_equipment> equipmentList = Db.Queryable<tn_dts_equipment>()
                .WhereIF(!string.IsNullOrEmpty(equipmentType), it => it.cn_s_equi_type == equipmentType)
                .WhereIF(!string.IsNullOrEmpty(equipmentNoOrName), it => it.cn_s_equi_no.Contains(equipmentNoOrName) || it.cn_s_equi_name.Contains(equipmentNoOrName))
               .OrderBy(string.IsNullOrEmpty(param.OrderBy) ? "cn_t_modify desc" : param.OrderBy)
               .ToList();
            return equipmentList.Select(it => new EquipmentInformation
            {
                EquiGuid = it.cn_guid,
                EquiNo = it.cn_s_equi_no,
                EquiName = it.cn_s_equi_name
            }).ToList().ToPageEnumerable(param.PageIndex, param.PageSize);
        }
        #endregion

    }
}
