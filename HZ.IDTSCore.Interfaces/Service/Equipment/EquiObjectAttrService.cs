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
    public class EquiObjectAttrService : BaseService<tn_dts_equiobjectattr>, IEquiObjectAttrService
    {
        public EquiObjectAttrService(SessionInfo session) : base(session)
        {

        }

        #region 按对象名、属性名（模糊）分页查询
        /// <summary>
        /// 按对象名、属性名（模糊）分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equiobjectattr> GetListPages(PageParm parm)
        {
            string cn_s_objectattr_name = parm.Parms["cn_s_objectattr_name"].ObjToString();
            string cn_s_objectattr_equiguid = parm.Parms["cn_s_objectattr_equiguid"].ObjToString();
            string cn_s_objectattr_attrname = parm.Parms["cn_s_objectattr_attrname"].ObjToString();
            return Db.Queryable<tn_dts_equiobjectattr>().LeftJoin<tn_dts_equiobject>((oa, o) => oa.cn_s_objectattr_guid == o.cn_guid)
            .WhereIF(!string.IsNullOrEmpty(cn_s_objectattr_equiguid), (oa, o) => o.cn_s_object_equiguid == cn_s_objectattr_equiguid)
            .WhereIF(!string.IsNullOrEmpty(cn_s_objectattr_name), (oa => oa.cn_s_objectattr_name == cn_s_objectattr_name))
            .WhereIF(!string.IsNullOrEmpty(cn_s_objectattr_attrname), (oa => oa.cn_s_objectattr_attrname.Contains(cn_s_objectattr_attrname)))
            .OrderBy(string.IsNullOrEmpty(parm.OrderBy) ? " cn_t_modify desc" : parm.OrderBy)
            .ToPage(parm.PageIndex, parm.PageSize);
        }
        #endregion

        #region 删除设备模型对象属性
        /// <summary>
        /// 删除设备模型对象属性
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        public ApiResult Delete(string[] cn_s_guid)
        {
            return UseTransaction(trans =>
            {
                trans.Deleteable<tn_dts_equiobjectattr>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 新增对象属性
        /// <summary>
        /// 新增对象属性
        /// </summary>
        /// <param name="equiobjectattr"></param>
        /// <returns></returns>
        public ReturnMessage AddObjectAttr(tn_dts_equiobjectattr equiobjectattr)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equiobjectattr objectattrGuid = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == equiobjectattr.cn_s_objectattr_guid && it.cn_s_objectattr_attrname == equiobjectattr.cn_s_objectattr_attrname).First();
            if (!(objectattrGuid is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "同一对象下属性名不可重复！";
                return returnMessage;
            }
            equiobjectattr.cn_guid = Guid.NewGuid().ToString();
            equiobjectattr.cn_s_creator = user.UserCode;
            equiobjectattr.cn_s_creator_by = user.UserName;
            equiobjectattr.cn_t_create = DateTime.Now;
            tn_dts_logs newEquiobjectattrLog = new tn_dts_logs();
            newEquiobjectattrLog.cn_guid = Guid.NewGuid().ToString();
            newEquiobjectattrLog.cn_s_logs_type = "操作";
            newEquiobjectattrLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备对象属性新增功能向tn_dts_equiobjectattr表中新增一条设备对象属性信息，详细信息为" + JsonConvert.SerializeObject(equiobjectattr);
            newEquiobjectattrLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Insertable<tn_dts_equiobjectattr>(equiobjectattr).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(newEquiobjectattrLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "11.6设备（Objectattr）对象管理新增对象属性接口新增对象属性记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "新增失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "新增成功！";
            return returnMessage;
        }
        #endregion

        #region 修改对象属性
        /// <summary>
        /// 修改对象属性
        /// </summary>
        /// <param name="equiobjectattr"></param>
        /// <returns></returns>
        public ReturnMessage UpdateObjectAttr(tn_dts_equiobjectattr equiobjectattr)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            tn_dts_equiobjectattr objectattrGuid = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_guid == equiobjectattr.cn_guid).First();
            if (objectattrGuid is null)
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "未找到唯一标识为" + equiobjectattr.cn_guid + "的记录";
                return returnMessage;
            }
            if (!(Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_s_objectattr_guid == equiobjectattr.cn_s_objectattr_guid && it.cn_s_objectattr_attrname == equiobjectattr.cn_s_objectattr_attrname && it.cn_guid != equiobjectattr.cn_guid).First() is null))
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "该对象下已有属性名为" + equiobjectattr.cn_s_objectattr_name + "的对象属性，修改失败";
                return returnMessage;
            }

            string oldAttrname = equiobjectattr.cn_s_objectattr_attrname;
            objectattrGuid.cn_s_objectattr_attrname = equiobjectattr.cn_s_objectattr_attrname;
            objectattrGuid.cn_s_objectattr_attrtype = equiobjectattr.cn_s_objectattr_attrtype;
            objectattrGuid.cn_s_objectattr_attrvalue = equiobjectattr.cn_s_objectattr_attrvalue;
            objectattrGuid.cn_s_objectattr_remarks = equiobjectattr.cn_s_objectattr_remarks;
            objectattrGuid.cn_s_modify = user.UserCode;
            objectattrGuid.cn_s_modify_by = user.UserName;
            objectattrGuid.cn_t_modify = DateTime.Now;
            tn_dts_logs equiobjectattrLog = new tn_dts_logs();
            equiobjectattrLog.cn_guid = Guid.NewGuid().ToString();
            equiobjectattrLog.cn_s_logs_type = "操作";
            equiobjectattrLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备对象属性修改功能将tn_dts_equiobjectattr表中唯一标识为：" + equiobjectattr.cn_guid + "的对象属性名为" + oldAttrname + "的对象属性记录进行修改，详细信息为" + JsonConvert.SerializeObject(objectattrGuid);
            equiobjectattrLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Updateable<tn_dts_equiobjectattr>(objectattrGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(equiobjectattrLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "11.7设备（Objectattr）对象管理修改对象属性接口修改对象属性记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "修改失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "修改成功!";
            return returnMessage;
        }
        #endregion

        #region 批量删除对象属性
        /// <summary>
        /// 批量删除对象属性
        /// </summary>
        /// <param name="guidList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteObjectAttr(List<string> guidList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            List<tn_dts_equiobjectattr> objectattrList = new List<tn_dts_equiobjectattr>();
            foreach (string guid in guidList)
            {
                tn_dts_equiobjectattr objectattrGuid = Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_guid == guid).First();
                if (objectattrGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "选中的对象属性中有对象属性有对象属性的唯一标识不存在！";
                    return returnMessage;
                }
                objectattrList.Add(objectattrGuid);
                tn_dts_logs objectattrLog = new tn_dts_logs();
                objectattrLog.cn_guid = Guid.NewGuid().ToString();
                objectattrLog.cn_s_logs_type = "操作";
                objectattrLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备对象属性批量删除功能将tn_dts_equiobjectattr表中唯一标识为：" + guid + "的对象属性记录进行删除，详细信息为：" + JsonConvert.SerializeObject(objectattrGuid);
                objectattrLog.cn_t_create = DateTime.Now;
                logList.Add(objectattrLog);
            }

            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equiobjectattr>(objectattrList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "11.8设备（objectattr）对象管理删除对象属性（批量）接口删除对象属性记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除对象属性失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除对象属性成功！";
            return returnMessage;
        }
        #endregion

        //    /// <summary>
        //    /// 删除对象属性
        //    /// </summary>
        //    /// <param name="equiobjectattr"></param>
        //    /// <returns></returns>
        //    public ReturnMessage DeleteObjectAttr(tn_dts_equiobjectattr equiobjectattr)
        //    {
        //        ReturnMessage returnMessage = new ReturnMessage();
        //        if (Db.Queryable<tn_dts_equiobjectattr>().Where(it => it.cn_guid == equiobjectattr.cn_guid).First() is null)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "tn_dts_equiobjectattr表中找不到唯一标识为" + equiobjectattr.cn_guid + "的记录";
        //            return returnMessage;
        //        }

        //        int resEquiobjectattr = Db.Deleteable<tn_dts_equiobjectattr>().Where(it => it.cn_guid == equiobjectattr.cn_guid).ExecuteCommand();
        //        if (resEquiobjectattr <= 0)
        //        {
        //            returnMessage.IsSuccess = false;
        //            returnMessage.Message = "从tn_dts_equiobjectattr表中删除记录失败！";
        //            return returnMessage;
        //        }
        //        returnMessage.IsSuccess = true;
        //        returnMessage.Message = "删除成功!";
        //        return returnMessage;
        //    }
    }
}
