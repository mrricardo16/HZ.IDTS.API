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
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service.Equipment
{
    public class EquiObjectService : BaseService<tn_dts_equiobject>, IEquiObjectService
    {
        public EquiObjectService(SessionInfo session) : base(session)
        {

        }

        #region 分页模糊查询所属设备ID和对象名
        /// <summary>
        /// 分页模糊查询所属设备ID和对象名
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equiobject> GetListPages(PageParm parm)
        {
            string cn_s_object_equiguid = parm.Parms["cn_s_object_equiguid"].ObjToString();
            string cn_s_object_name = parm.Parms["cn_s_object_name"].ObjToString();
            return Db.Queryable<tn_dts_equiobject>().WhereIF(!string.IsNullOrEmpty(cn_s_object_equiguid), (s => s.cn_s_object_equiguid.Contains(cn_s_object_equiguid)))
            .WhereIF(!string.IsNullOrEmpty(cn_s_object_name), (s => s.cn_s_object_name.Contains(cn_s_object_name)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除设备模型对象
        /// <summary>
        /// 删除设备模型对象
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_equiobject>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 获取对象树
        /// <summary>
        /// 获取对象树
        /// </summary>
        /// <returns></returns>
        public List<ObjectTree> GetObjectTree()
        {
            List<ObjectTree> objectTreeList = new List<ObjectTree>();
            List<tn_dts_equipment> completeMachineTreeList = Db.Queryable<tn_dts_equipment>().Where(s => s.cn_s_equi_parttype == "整机").ToList();
            foreach (var completeMachineTree in completeMachineTreeList)
            {
                ObjectTree objectTree = new ObjectTree();
                objectTree.cn_guid = completeMachineTree.cn_guid;
                objectTree.cn_s_equi_parttype = completeMachineTree.cn_s_equi_parttype;
                objectTree.cn_s_equi_no = completeMachineTree.cn_s_equi_no;
                objectTree.cn_s_equi_name = completeMachineTree.cn_s_equi_name;
                objectTree.cn_s_equi_type = completeMachineTree.cn_s_equi_type;
                objectTree.cn_s_equi_model = completeMachineTree.cn_s_equi_model;
                objectTree.cn_s_equi_status = completeMachineTree.cn_s_equi_status;
                objectTree.cn_s_equi_buydate = completeMachineTree.cn_s_equi_buydate;
                objectTree.cn_s_equi_qadate = completeMachineTree.cn_s_equi_qadate;
                objectTree.cn_s_equi_firstdate = completeMachineTree.cn_s_equi_firstdate;
                objectTree.cn_s_equi_defentperiod = completeMachineTree.cn_s_equi_defentperiod;
                objectTree.cn_s_equi_dept = completeMachineTree.cn_s_equi_dept;
                objectTree.cn_s_equi_dutyman = completeMachineTree.cn_s_equi_dutyman;
                objectTree.cn_s_equi_dutyphone = completeMachineTree.cn_s_equi_dutyphone;
                objectTree.cn_s_equi_contractno = completeMachineTree.cn_s_equi_contractno;
                objectTree.cn_s_equi_beltline = completeMachineTree.cn_s_equi_beltline;
                objectTree.cn_s_equi_xpos = completeMachineTree.cn_s_equi_xpos;
                objectTree.cn_s_equi_ypos = completeMachineTree.cn_s_equi_ypos;
                objectTree.cn_s_equi_zpos = completeMachineTree.cn_s_equi_zpos;
                objectTree.cn_s_equi_remarks = completeMachineTree.cn_s_equi_remarks;
                objectTree.cn_s_modify = completeMachineTree.cn_s_modify;
                objectTree.cn_s_modify_by = completeMachineTree.cn_s_modify_by;
                objectTree.cn_t_modify = completeMachineTree.cn_t_modify;
                objectTree.cn_s_creator = completeMachineTree.cn_s_creator;
                objectTree.cn_s_creator_by = completeMachineTree.cn_s_creator_by;
                objectTree.cn_t_create = completeMachineTree.cn_t_create;
                objectTree.equipmentobject = Db.Queryable<tn_dts_equiobject>().Where(s => s.cn_s_object_equiguid == completeMachineTree.cn_guid).ToList();
                objectTreeList.Add(objectTree);
            }
            return objectTreeList;
        }
        #endregion

        #region 新增对象
        /// <summary>
        /// 新增对象
        /// </summary>
        /// <param name="equiobject"></param>
        /// <returns></returns>
        public ReturnMessage AddObject(tn_dts_equiobject equiobject)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            if (!(Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_name == equiobject.cn_s_object_name && it.cn_s_object_equi_no == equiobject.cn_s_object_equi_no).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "同一整机下的对象名不可重复！";
                return returnMessage;
            }
            equiobject.cn_guid = Guid.NewGuid().ToString();
            equiobject.cn_s_creator = user.UserCode;
            equiobject.cn_s_creator_by = user.UserName;
            equiobject.cn_t_create = DateTime.Now;
            tn_dts_logs newEquiobjectLog = new tn_dts_logs();
            newEquiobjectLog.cn_guid = Guid.NewGuid().ToString();
            newEquiobjectLog.cn_s_logs_type = "操作";
            newEquiobjectLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备对象新增功能向tn_dts_equiobject表中新增一条设备对象信息，详细信息为" + JsonConvert.SerializeObject(equiobject);
            newEquiobjectLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_equiobject>(equiobject).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(newEquiobjectLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "11.2设备（Object）对象管理新增接口新增对象记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "新增失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "新增成功！";
            return returnMessage;
        }
        #endregion

        #region 修改对象
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="equiobject"></param>
        /// <returns></returns>
        public ReturnMessage UpdateObject(tn_dts_equiobject equiobject)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equiobject objectGuid = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_guid == equiobject.cn_guid).First();
            if (objectGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "未找到唯一标识为" + equiobject.cn_guid + "的记录";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_equi_no == equiobject.cn_s_object_equi_no && it.cn_s_object_name == equiobject.cn_s_object_name && it.cn_guid != equiobject.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "该整机下已有对象名为" + equiobject.cn_s_object_name + "的对象，修改失败";
                return returnMessage;
            }

            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            List<tn_dts_equiobjectattr> newObjectattrList = new List<tn_dts_equiobjectattr>();
            bool isModifyObjectName = false;

            //if (itemGuid.cn_s_object_equi_name != equiobject.cn_s_object_equi_name)
            //{
            //    List<tn_dts_equiobject> itemObjectList = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_equi_no == equiobject.cn_s_object_equi_no && it.cn_guid != equiobject.cn_guid).ToList();
            //    if (itemObjectList.Count != 0)
            //    {
            //        foreach (var itemObject in itemObjectList)
            //        {
            //            itemObject.cn_s_object_equi_name = equiobject.cn_s_object_equi_name;
            //            itemObject.cn_s_modify = user.UserCode;
            //            itemObject.cn_s_modify_by = user.UserName;
            //            itemObject.cn_t_modify = DateTime.Now;
            //            int resOtherObject = Db.Updateable(itemObject).ExecuteCommand();
            //            if (resOtherObject <= 0)
            //            {
            //                returnMessage.IsSuccess = false;
            //                returnMessage.Message = "修改tn_dts_equiobject表中唯一标识为" + itemObject.cn_guid + "的数据中的设备名称失败!";
            //                return returnMessage;
            //            }
            //        }
            //    }

            //    tn_dts_equipment itemEquipment = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == equiobject.cn_s_object_equi_no).First();
            //    if (!(itemEquipment is null))
            //    {
            //        itemEquipment.cn_s_equi_name = equiobject.cn_s_object_equi_name;
            //        itemEquipment.cn_s_modify = user.UserCode;
            //        itemEquipment.cn_s_modify_by = user.UserName;
            //        itemEquipment.cn_t_modify = DateTime.Now;
            //        int resEquipment = Db.Updateable(itemEquipment).ExecuteCommand();
            //        if (resEquipment <= 0)
            //        {
            //            returnMessage.IsSuccess = false;
            //            returnMessage.Message = "修改tn_dts_equipment表设备名称字段失败!";
            //            return returnMessage;
            //        }
            //    }

            //    //tn_dts_equibom parentbom = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_childno == equiobject.cn_s_object_equi_no).First();
            //    //if (!(parentbom is null))
            //    //{
            //    //    parentbom.cn_s_equibom_childname = equiobject.cn_s_object_equi_name;
            //    //    parentbom.cn_s_modify = user.UserCode;
            //    //    parentbom.cn_s_modify_by = user.UserName;
            //    //    parentbom.cn_t_modify = DateTime.Now;
            //    //    int resParentbom = Db.Updateable(parentbom).ExecuteCommand();
            //    //    if (resParentbom <= 0)
            //    //    {
            //    //        returnMessage.IsSuccess = false;
            //    //        returnMessage.Message = "修改tn_dts_equibom表子项设备名称失败!";
            //    //        return returnMessage;
            //    //    }
            //    //}

            //    List<tn_dts_equibom> childbomList = Db.Queryable<tn_dts_equibom>().Where(it => it.cn_s_equibom_parentno == equiobject.cn_s_object_equi_no).ToList();
            //    if (childbomList.Count != 0)
            //    {
            //        foreach (var childbom in childbomList)
            //        {
            //            childbom.cn_s_equibom_parentname = equiobject.cn_s_object_equi_name;
            //            childbom.cn_s_modify = user.UserCode;
            //            childbom.cn_s_modify_by = user.UserName;
            //            childbom.cn_t_modify = DateTime.Now;
            //            int resChildbom = Db.Updateable(childbom).ExecuteCommand();
            //            if (resChildbom <= 0)
            //            {
            //                returnMessage.IsSuccess = false;
            //                returnMessage.Message = "修改tn_dts_equibom表唯一标识为" + childbom.cn_guid + "的数据中的父项设备名称失败!";
            //                return returnMessage;
            //            }
            //        }
            //    }

            //    List<tn_dts_equirepair> itemRepairList = Db.Queryable<tn_dts_equirepair>().Where(it => it.cn_s_equirepair_no == equiobject.cn_s_object_equi_no).ToList();
            //    if (itemRepairList.Count != 0)
            //    {
            //        foreach (var itemRepair in itemRepairList)
            //        {
            //            itemRepair.cn_s_equirepair_name = equiobject.cn_s_object_equi_name;
            //            itemRepair.cn_s_modify = user.UserCode;
            //            itemRepair.cn_s_modify_by = user.UserName;
            //            itemRepair.cn_t_modify = DateTime.Now;
            //            int resRepair = Db.Updateable(itemRepair).ExecuteCommand();
            //            if (resRepair <= 0)
            //            {
            //                returnMessage.IsSuccess = false;
            //                returnMessage.Message = "修改tn_dts_equirepair表唯一标识为" + itemRepair.cn_guid + "的数据中的设备名称失败!";
            //                return returnMessage;
            //            }
            //        }
            //    }

            //    List<tn_dts_equiupkeep> itemUpkeepList = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == equiobject.cn_s_object_equi_no).ToList();
            //    if (itemUpkeepList.Count != 0)
            //    {
            //        foreach (var itemUpkeep in itemUpkeepList)
            //        {
            //            itemUpkeep.cn_s_equiupkeep_name = equiobject.cn_s_object_equi_name;
            //            itemUpkeep.cn_s_modify = user.UserCode;
            //            itemUpkeep.cn_s_modify_by = user.UserName;
            //            itemUpkeep.cn_t_modify = DateTime.Now;
            //            int resUpkeep = Db.Updateable(itemUpkeep).ExecuteCommand();
            //            if (resUpkeep <= 0)
            //            {
            //                returnMessage.IsSuccess = false;
            //                returnMessage.Message = "修改tn_dts_equiupkeep表唯一标识为" + itemUpkeep.cn_guid + "的数据中的设备名称失败!";
            //                return returnMessage;
            //            }
            //        }
            //    }
            //}
            string oldObjectName = equiobject.cn_s_object_name;
            if (objectGuid.cn_s_object_name != equiobject.cn_s_object_name)
            {
                isModifyObjectName = true;
                List<tn_dts_equiobjectattr> objectattrObjectguidList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == equiobject.cn_guid).ToList();
                foreach (var objectattrName in objectattrObjectguidList)
                {
                    objectattrName.cn_s_objectattr_name = equiobject.cn_s_object_name;
                    objectattrName.cn_s_modify = user.UserCode;
                    objectattrName.cn_s_modify_by = user.UserName;
                    objectattrName.cn_t_modify = DateTime.Now;
                    newObjectattrList.Add(objectattrName);
                    tn_dts_logs newObjectattrLog = new tn_dts_logs();
                    newObjectattrLog.cn_guid = Guid.NewGuid().ToString();
                    newObjectattrLog.cn_s_logs_type = "操作";
                    newObjectattrLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用修改对象功能将tn_dts_equiobjectattr表中对象名为：" + oldObjectName + "的对象属性名为：" + objectattrName.cn_s_objectattr_attrname + "的对象属性记录进行修改，详细信息为" + JsonConvert.SerializeObject(objectattrName);
                    newObjectattrLog.cn_t_create = DateTime.Now;
                    logList.Add(newObjectattrLog);
                }
            }
            objectGuid.cn_s_object_item = equiobject.cn_s_object_item;
            objectGuid.cn_s_object_name = equiobject.cn_s_object_name;
            objectGuid.cn_s_object_type = equiobject.cn_s_object_type;
            objectGuid.cn_s_object_remarks = equiobject.cn_s_object_remarks;
            objectGuid.cn_s_modify = user.UserCode;
            objectGuid.cn_s_modify_by = user.UserName;
            objectGuid.cn_t_modify = DateTime.Now;
            tn_dts_logs newObjectLog = new tn_dts_logs();
            newObjectLog.cn_guid = Guid.NewGuid().ToString();
            newObjectLog.cn_s_logs_type = "操作";
            newObjectLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用修改对象功能将tn_dts_equiobject表中对象名为：" + oldObjectName + "的对象记录进行修改，详细信息为" + JsonConvert.SerializeObject(objectGuid);
            newObjectLog.cn_t_create = DateTime.Now;
            logList.Add(newObjectLog);
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_equiobject>(objectGuid).ExecuteCommand();
                if (isModifyObjectName)
                {
                    dbTran.Updateable<tn_dts_equiobjectattr>(newObjectattrList).ExecuteCommand();
                }
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "11.3设备（Object）对象管理修改接口修改对象记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改成功!";
            return returnMessage;
        }
        #endregion

        #region 删除对象
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="equiobject"></param>
        /// <returns></returns>
        public ReturnMessage DeleteObject(tn_dts_equiobject equiobject)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equiobject objectGuid = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_guid == equiobject.cn_guid).First();
            if (objectGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "tn_dts_equiobject表中找不到唯一标识为" + equiobject.cn_guid + "的记录";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == equiobject.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "要删除的对象有对象属性，请先删除对象属性!";
                return returnMessage;
            }
            tn_dts_logs objectLog = new tn_dts_logs();
            objectLog.cn_guid = Guid.NewGuid().ToString();
            objectLog.cn_s_logs_type = "操作";
            objectLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除对象功能将tn_dts_equiobject表中对象名为：" + objectGuid.cn_s_object_name + "的对象记录进行删除，详细信息为" + JsonConvert.SerializeObject(objectGuid);
            objectLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equiobject>(objectGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(objectLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "11.4设备（Object）对象管理删除对象接口删除对象记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功!";
            return returnMessage;
        }
        #endregion

        #region 粘贴对象接口
        /// <summary>
        /// 粘贴对象接口
        /// </summary>
        /// <param name="pasteObject"></param>
        /// <returns></returns>
        public ReturnMessage PasteObject(PasteObject pasteObject)
        {
            string completemachineguid = pasteObject.Completemachineguid;
            string latestCopyObjectGuid = pasteObject.LatestCopyObjectGuid;
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equipment equipmentGuid = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_guid == completemachineguid).First();
            if (equipmentGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "数据库中找不到唯一标识为" + completemachineguid + "的设备记录，无法粘贴！";
                return returnMessage;
            }
            if (string.IsNullOrEmpty(equipmentGuid.cn_s_equi_no))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "传入的唯一标识对应设备的设备编号为空，无法粘贴！";
                return returnMessage;
            }
            if (string.IsNullOrEmpty(latestCopyObjectGuid))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "系统中没有复制的对象信息，请先复制对象再尝试粘贴！";
                return returnMessage;
            }
            tn_dts_equiobject objectGuid = Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_guid == latestCopyObjectGuid).First();
            if (objectGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "原来复制的对象已删除，请重新复制！";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_equiobject>().Where(it => it.cn_s_object_name == objectGuid.cn_s_object_name && it.cn_s_object_equi_no == equipmentGuid.cn_s_equi_no).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "该整机下已有对象名为" + objectGuid.cn_s_object_name + "的对象，无法复制！";
                return returnMessage;
            }
            List<tn_dts_equiobjectattr> oldObjectattrList = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == latestCopyObjectGuid).ToList();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            List<tn_dts_equiobjectattr> newObjectattrList = new List<tn_dts_equiobjectattr>();

            tn_dts_equiobject newObject = new tn_dts_equiobject();
            newObject.cn_guid = Guid.NewGuid().ToString();
            newObject.cn_s_object_equiguid = completemachineguid;
            newObject.cn_s_object_equi_no = equipmentGuid.cn_s_equi_no;
            newObject.cn_s_object_equi_name = equipmentGuid.cn_s_equi_name;
            newObject.cn_s_object_item = objectGuid.cn_s_object_item;
            newObject.cn_s_object_name = objectGuid.cn_s_object_name;
            newObject.cn_s_object_type = objectGuid.cn_s_object_type;
            newObject.cn_s_object_remarks = objectGuid.cn_s_object_remarks;
            newObject.cn_s_creator = user.UserCode;
            newObject.cn_s_creator_by = user.UserName;
            newObject.cn_t_create = DateTime.Now;
            tn_dts_logs equiobjectLog = new tn_dts_logs();
            equiobjectLog.cn_guid = Guid.NewGuid().ToString();
            equiobjectLog.cn_s_logs_type = "操作";
            equiobjectLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用粘贴功能向tn_dts_equiobject表中新增对象名为" + newObject.cn_s_object_equi_name + "的对象记录，详细信息为" + JsonConvert.SerializeObject(newObject);
            equiobjectLog.cn_t_create = DateTime.Now;
            logList.Add(equiobjectLog);
            foreach (var oldObjectattr in oldObjectattrList)
            {
                tn_dts_equiobjectattr newObjectattr = new tn_dts_equiobjectattr();
                newObjectattr.cn_guid = Guid.NewGuid().ToString();
                newObjectattr.cn_s_objectattr_guid = newObject.cn_guid;
                newObjectattr.cn_s_objectattr_name = newObject.cn_s_object_name;
                newObjectattr.cn_s_objectattr_attrname = oldObjectattr.cn_s_objectattr_attrname;
                newObjectattr.cn_s_objectattr_attrtype = oldObjectattr.cn_s_objectattr_attrtype;
                newObjectattr.cn_s_objectattr_attrvalue = oldObjectattr.cn_s_objectattr_attrvalue;
                newObjectattr.cn_s_objectattr_remarks = oldObjectattr.cn_s_objectattr_remarks;
                newObjectattr.cn_s_creator = user.UserCode;
                newObjectattr.cn_s_creator_by = user.UserName;
                newObjectattr.cn_t_create = DateTime.Now;
                newObjectattrList.Add(newObjectattr);
                tn_dts_logs equiobjectattrLog = new tn_dts_logs();
                equiobjectattrLog.cn_guid = Guid.NewGuid().ToString();
                equiobjectattrLog.cn_s_logs_type = "操作";
                equiobjectattrLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用粘贴功能向tn_dts_equiobjectattr表中新增属性名为" + newObjectattr.cn_s_objectattr_attrname + "的对象属性记录，详细信息为" + JsonConvert.SerializeObject(newObjectattr);
                equiobjectattrLog.cn_t_create = DateTime.Now;
                logList.Add(equiobjectattrLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_equiobject>(newObject).ExecuteCommand();
                dbTran.Insertable<tn_dts_equiobjectattr>(newObjectattrList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "11.10设备（Object&&Objectattr）对象管理粘贴对象接口粘贴对象失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "粘贴对象失败！";
                return returnMessage;
            }

            returnMessage.IsSuccess = true;
            returnMessage.Message = "粘贴对象成功！";
            return returnMessage;
        }
        #endregion
    }
}
