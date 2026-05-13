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
    public class EquiupkeepService : BaseService<tn_dts_equiupkeep>, IEquiupkeepService
    {
        public EquiupkeepService(SessionInfo session) : base(session)
        {

        }

        #region 按cn_s_equiupkeep_isfirst、cn_s_equiupkeep_date、cn_s_equi_parttype、cn_s_equiupkeep_name(模糊)分页查询
        /// <summary>
        /// 按cn_s_equirepair_category、cn_s_equirepair_date、cn_s_equi_parttype、cn_s_equirepair_name(模糊)分页查询
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        public PagedInfo<tn_dts_equiupkeep> GetListPages(PageParm parm)
        {
            string cn_s_equiupkeep_no = parm.Parms["cn_s_equiupkeep_no"].ObjToString();
            string cn_n_equiupkeep_isfirst = parm.Parms["cn_n_equiupkeep_isfirst"].ObjToString();
            string cn_s_equiupkeep_starttime = parm.Parms["cn_s_equiupkeep_starttime"].ObjToString();
            string cn_s_equiupkeep_endtime = parm.Parms["cn_s_equiupkeep_endtime"].ObjToString();
            string cn_s_equi_parttype = parm.Parms["cn_s_equi_parttype"].ObjToString();
            string cn_s_equiupkeep_name = parm.Parms["cn_s_equiupkeep_name"].ObjToString();
            return Db.Queryable<tn_dts_equiupkeep>()
            .LeftJoin<tn_dts_equipment>((eu, e) => eu.cn_s_equiupkeep_no == e.cn_s_equi_no)
            .WhereIF(!String.IsNullOrEmpty(cn_s_equiupkeep_no), eu => eu.cn_s_equiupkeep_no == cn_s_equiupkeep_no)
            .WhereIF(!String.IsNullOrEmpty(cn_n_equiupkeep_isfirst), eu => eu.cn_n_equiupkeep_isfirst == cn_n_equiupkeep_isfirst)
            .WhereIF((!String.IsNullOrEmpty(cn_s_equiupkeep_starttime)) && (!String.IsNullOrEmpty(cn_s_equiupkeep_endtime)), eu => (eu.cn_t_equiupkeep_date >= DateTime.Parse(cn_s_equiupkeep_starttime)) && (eu.cn_t_equiupkeep_date <= DateTime.Parse(cn_s_equiupkeep_endtime)))
            .WhereIF(!String.IsNullOrEmpty(cn_s_equi_parttype), (eu, e) => e.cn_s_equi_parttype == cn_s_equi_parttype)
            .WhereIF(!String.IsNullOrEmpty(cn_s_equiupkeep_name), eu => eu.cn_s_equiupkeep_name.Contains(cn_s_equiupkeep_name))
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
                trans.Deleteable<tn_dts_equiupkeep>().In(x => x.cn_guid, cn_s_guid).ExecuteCommand();
            });
        }
        #endregion

        #region 保存设备保养
        /// <summary>
        /// 保存设备保养
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        public ReturnMessage SaveDataUpkeep(SaveData_Upkeep saveData)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            tn_dts_equiupkeep equiupkeep = saveData.equiupkeep;
            UserSession user = GetSessionInfo();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            tn_dts_matter newMatter = new tn_dts_matter();
            bool isNewMatter = false;
            if (saveData.add_or_modify == "add")
            {
                if (!(Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == equiupkeep.cn_s_equiupkeep_no && it.cn_s_equiupkeep_item == equiupkeep.cn_s_equiupkeep_item && it.cn_t_equiupkeep_date == equiupkeep.cn_t_equiupkeep_date).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "同一个设备同一时间在同一个保养项目上的保养记录只能有一个！";
                    return returnMessage;
                }
                tn_dts_equiupkeep newUpkeep = new tn_dts_equiupkeep();
                newUpkeep.cn_guid = Guid.NewGuid().ToString();
                newUpkeep.cn_s_equiupkeep_no = equiupkeep.cn_s_equiupkeep_no;
                newUpkeep.cn_s_equiupkeep_name = equiupkeep.cn_s_equiupkeep_name;
                newUpkeep.cn_s_equiupkeep_item = equiupkeep.cn_s_equiupkeep_item;
                newUpkeep.cn_s_equiupkeep_cause = equiupkeep.cn_s_equiupkeep_cause;
                if (Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == equiupkeep.cn_s_equiupkeep_no).First() is null)
                {
                    newUpkeep.cn_n_equiupkeep_isfirst = "首保";
                }
                else
                {
                    newUpkeep.cn_n_equiupkeep_isfirst = "非首保";
                }
                newUpkeep.cn_t_equiupkeep_nextdate = equiupkeep.cn_t_equiupkeep_nextdate;
                newUpkeep.cn_d_equiupkeep_cost = equiupkeep.cn_d_equiupkeep_cost;
                newUpkeep.cn_s_equiupkeep_material = equiupkeep.cn_s_equiupkeep_material;
                newUpkeep.cn_s_equiupkeep_man = equiupkeep.cn_s_equiupkeep_man;
                newUpkeep.cn_s_equiupkeep_phone = equiupkeep.cn_s_equiupkeep_phone;
                newUpkeep.cn_t_equiupkeep_date = equiupkeep.cn_t_equiupkeep_date;
                newUpkeep.cn_s_equiupkeep_remarks = equiupkeep.cn_s_equiupkeep_remarks;
                newUpkeep.cn_s_creator = user.UserCode;
                newUpkeep.cn_s_creator_by = user.UserName;
                newUpkeep.cn_t_create = DateTime.Now;
                tn_dts_logs newUpkeepLog = new tn_dts_logs();
                newUpkeepLog.cn_guid = Guid.NewGuid().ToString();
                newUpkeepLog.cn_s_logs_type = "操作";
                newUpkeepLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备保养保存功能向tn_dts_upkeep表中新增一条保养信息，详细信息为" + JsonConvert.SerializeObject(newUpkeep);
                newUpkeepLog.cn_t_create = DateTime.Now;
                logList.Add(newUpkeepLog);
                if (Db.Queryable<tn_dts_matter>().Where(it => it.cn_s_matter_name == equiupkeep.cn_s_equiupkeep_item && it.cn_s_matter_type == "保养").First() is null)
                {
                    isNewMatter = true;
                    newMatter.cn_guid = Guid.NewGuid().ToString();
                    newMatter.cn_s_matter_type = "保养";
                    newMatter.cn_s_matter_sourceid = equiupkeep.cn_s_equiupkeep_no;//事项来源id里存设备编号
                    newMatter.cn_s_matter_name = equiupkeep.cn_s_equiupkeep_item;
                    newMatter.cn_s_matter_remarks = "";
                    newMatter.cn_s_creator = user.UserCode;
                    newMatter.cn_s_creator_by = user.UserName;
                    newMatter.cn_t_create = DateTime.Now;
                    tn_dts_logs newMatterLog = new tn_dts_logs();
                    newMatterLog.cn_guid = Guid.NewGuid().ToString();
                    newMatterLog.cn_s_logs_type = "操作";
                    newMatterLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备保养保存功能向tn_dts_matter表中新增一条保养事项信息，详细信息为" + JsonConvert.SerializeObject(newMatter);
                    newMatterLog.cn_t_create = DateTime.Now;
                    logList.Add(newMatterLog);
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Insertable<tn_dts_equiupkeep>(newUpkeep).ExecuteCommand();
                    if (isNewMatter)
                    {
                        dbTran.Insertable<tn_dts_matter>(newMatter).ExecuteCommand();
                    }
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "7.3设备（Equiupkeep）保养管理保存接口新增保养记录失败，详细信息为：" + res.Message);
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
                if (!(Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == equiupkeep.cn_s_equiupkeep_no && it.cn_s_equiupkeep_item == equiupkeep.cn_s_equiupkeep_item && it.cn_t_equiupkeep_date == equiupkeep.cn_t_equiupkeep_date && it.cn_guid != equiupkeep.cn_guid).First() is null))
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "同一个设备同一时间在同一个保养项目上的保养记录只能有一个！";
                    return returnMessage;
                }
                tn_dts_equiupkeep upkeepGuid = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_guid == equiupkeep.cn_guid).First();
                if (upkeepGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "找不到唯一标识为：" + equiupkeep.cn_guid + "的保养记录，保存失败！";
                    return returnMessage;
                }
                upkeepGuid.cn_s_equiupkeep_name = equiupkeep.cn_s_equiupkeep_name;
                string oldUpkeepItem = upkeepGuid.cn_s_equiupkeep_item;
                upkeepGuid.cn_s_equiupkeep_item = equiupkeep.cn_s_equiupkeep_item;
                upkeepGuid.cn_s_equiupkeep_cause = equiupkeep.cn_s_equiupkeep_cause;
                upkeepGuid.cn_t_equiupkeep_nextdate = equiupkeep.cn_t_equiupkeep_nextdate;
                upkeepGuid.cn_d_equiupkeep_cost = equiupkeep.cn_d_equiupkeep_cost;
                upkeepGuid.cn_s_equiupkeep_material = equiupkeep.cn_s_equiupkeep_material;
                upkeepGuid.cn_s_equiupkeep_man = equiupkeep.cn_s_equiupkeep_man;
                upkeepGuid.cn_s_equiupkeep_phone = equiupkeep.cn_s_equiupkeep_phone;
                DateTime? oldUpkeepDate = upkeepGuid.cn_t_equiupkeep_date;
                upkeepGuid.cn_t_equiupkeep_date = equiupkeep.cn_t_equiupkeep_date;
                upkeepGuid.cn_s_equiupkeep_remarks = equiupkeep.cn_s_equiupkeep_remarks;
                upkeepGuid.cn_s_modify = user.UserCode;
                upkeepGuid.cn_s_modify_by = user.UserName;
                upkeepGuid.cn_t_modify = DateTime.Now;
                tn_dts_logs newUpkeepLog = new tn_dts_logs();
                newUpkeepLog.cn_guid = Guid.NewGuid().ToString();
                newUpkeepLog.cn_s_logs_type = "操作";
                newUpkeepLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备保养保存功能将tn_dts_upkeep表中设备编号为：" + upkeepGuid.cn_s_equiupkeep_no + "，保养项目为：" + oldUpkeepItem + ",保养时间为：" + oldUpkeepDate + "的保养记录进行修改，详细信息为" + JsonConvert.SerializeObject(upkeepGuid);
                newUpkeepLog.cn_t_create = DateTime.Now;
                logList.Add(newUpkeepLog);
                if (Db.Queryable<tn_dts_matter>().Where(it => it.cn_s_matter_name == equiupkeep.cn_s_equiupkeep_item && it.cn_s_matter_type == "保养").First() is null)
                {
                    isNewMatter = true;
                    newMatter.cn_guid = Guid.NewGuid().ToString();
                    newMatter.cn_s_matter_type = "保养";
                    newMatter.cn_s_matter_sourceid = equiupkeep.cn_s_equiupkeep_no;//事项来源id里存设备编号
                    newMatter.cn_s_matter_name = equiupkeep.cn_s_equiupkeep_item;
                    newMatter.cn_s_matter_remarks = "";
                    newMatter.cn_s_creator = user.UserCode;
                    newMatter.cn_s_creator_by = user.UserName;
                    newMatter.cn_t_create = DateTime.Now;
                    tn_dts_logs newMatterLog = new tn_dts_logs();
                    newMatterLog.cn_guid = Guid.NewGuid().ToString();
                    newMatterLog.cn_s_logs_type = "操作";
                    newMatterLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用设备保养保存功能向tn_dts_matter表中新增一条保养事项信息，详细信息为" + JsonConvert.SerializeObject(newMatter);
                    newMatterLog.cn_t_create = DateTime.Now;
                    logList.Add(newMatterLog);
                }
                ApiResult res = UseTransaction(dbTran =>
                {
                    dbTran.Updateable<tn_dts_equiupkeep>(upkeepGuid).ExecuteCommand();
                    if (isNewMatter)
                    {
                        dbTran.Insertable<tn_dts_matter>(newMatter).ExecuteCommand();
                    }
                    dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
                });
                if (!res.IsSuccess)
                {
                    LogHelper.Info(DateTime.Now.ToString() + "7.3设备（Equiupkeep）保养管理保存接口修改保养记录失败，详细信息为：" + res.Message);
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

        #region 删除设备保养
        /// <summary>
        /// 删除设备保养
        /// </summary>
        /// <param name="equiupkeepList"></param>
        /// <returns></returns>
        public ReturnMessage DeleteDataUpkeep(List<tn_dts_equiupkeep> equiupkeepList)
        {
            ReturnMessage returnMessage = new ReturnMessage();
            UserSession user = new UserSession();
            List<tn_dts_equiupkeep> latestUpkeepList = new List<tn_dts_equiupkeep>();
            List<tn_dts_logs> logList = new List<tn_dts_logs>();
            foreach (var equiupkeep in equiupkeepList)
            {
                tn_dts_equiupkeep equiupkeepGuid = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_guid == equiupkeep.cn_guid).First();
                if (equiupkeepGuid is null)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "tn_dts_equiupkeep表中找不到该唯一标识为：" + equiupkeep.cn_guid + "的记录！";
                    return returnMessage;
                }
                if (!equiupkeepGuid.cn_t_equiupkeep_date.HasValue)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "tn_dts_equiupkeep表中唯一标识为：" + equiupkeep.cn_guid + "的保养记录的保养时间为空，删除失败！";
                    return returnMessage;
                }
                DateTime? newTimeN = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == equiupkeepGuid.cn_s_equiupkeep_no && it.cn_s_equiupkeep_item == equiupkeepGuid.cn_s_equiupkeep_item).OrderBy(it => it.cn_t_equiupkeep_date, OrderByType.Desc).Select(it => it.cn_t_equiupkeep_date).First();
                if (newTimeN.HasValue == false)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "保养记录中的设备编号为：" + equiupkeepGuid.cn_s_equiupkeep_no + "的设备在保养项目为：" + equiupkeepGuid.cn_s_equiupkeep_item + "的最新保养记录的保养时间为空!";
                    return returnMessage;
                }
                if (newTimeN.Value != equiupkeepGuid.cn_t_equiupkeep_date.Value)
                {
                    returnMessage.IsSuccess = false;
                    returnMessage.Message = "当前删除的保养记录不是设备编号为：" + equiupkeep.cn_s_equiupkeep_no + "的设备在保养项目为：" + equiupkeepGuid.cn_s_equiupkeep_item + "的最新保养记录，最新保养时间为：" + newTimeN.Value + "，请先删除该记录，删除失败！";
                    return returnMessage;
                }
                latestUpkeepList.Add(equiupkeepGuid);
                tn_dts_logs equiupkeepLog = new tn_dts_logs();
                equiupkeepLog.cn_guid = Guid.NewGuid().ToString();
                equiupkeepLog.cn_s_logs_type = "操作";
                equiupkeepLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能将tn_dts_equiupkeep表中设备编号为" + equiupkeepGuid.cn_s_equiupkeep_no + "的设备在保养项目为" + equiupkeepGuid.cn_s_equiupkeep_item + "的最新保养记录删除，详细信息为" + JsonConvert.SerializeObject(equiupkeepGuid);
                equiupkeepLog.cn_t_create = DateTime.Now;
                logList.Add(equiupkeepLog);
            }
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_equiupkeep>(latestUpkeepList).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(logList).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "7.4设备（Equiupkeep）保养管理删除接口删除保养记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        #region 查看详情
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public tn_dts_equiupkeep Detail(string guid)
        {
            return Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_guid == guid).First();
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
            .Where(it => it.cn_s_matter_type == "保养")
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

        #region 删除保养项目
        /// <summary>
        /// 删除保养项目
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
                returnMessage.Message = "tn_dts_matter表中找不到唯一标识为;" + guid + "的保养项目记录！";
                return returnMessage;
            }
            if (matterGuid.cn_s_matter_type != "保养")
            {
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除事项记录的事项类型不是保养，不能删除！";
                return returnMessage;
            }
            tn_dts_logs matterLog = new tn_dts_logs();
            matterLog.cn_guid = Guid.NewGuid().ToString();
            matterLog.cn_s_logs_type = "操作";
            matterLog.cn_s_logs_remarks = "用户编号为" + user.UserCode + "的用户使用删除功能将tn_dts_matter表中事项名称为" + matterGuid.cn_s_matter_name + "的保养事项记录删除，详细信息为" + JsonConvert.SerializeObject(matterGuid);
            matterLog.cn_t_create = DateTime.Now;
            ApiResult res = UseTransaction(dbTran =>
            {
                dbTran.Deleteable<tn_dts_matter>(matterGuid).ExecuteCommand();
                dbTran.Insertable<tn_dts_logs>(matterLog).ExecuteCommand();
            });
            if (!res.IsSuccess)
            {
                LogHelper.Info(DateTime.Now.ToString() + "7.7设备（Equiupkeep&&matter）保养管理删除保养项目接口删除保养项目记录失败，详细信息为：" + res.Message);
                returnMessage.IsSuccess = false;
                returnMessage.Message = "删除失败！";
                return returnMessage;
            }
            returnMessage.IsSuccess = true;
            returnMessage.Message = "删除成功！";
            return returnMessage;
        }
        #endregion

        #region 返回指定设备的前指定个数个保养提醒项
        /// <summary>
        /// 返回指定设备的前指定个数个保养提醒项
        /// </summary>
        /// <param name="returnNum"></param>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public List<EQUpkeepCollect> GetEQUpkeepCollectList(int returnNum, string deviceCode)
        {
            List<EQUpkeepCollect> eQUpkeepCollectList = new List<EQUpkeepCollect>();
            //PagedInfo<tn_dts_equiupkeep> upkeepPagedList = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == deviceCode).OrderBy(it => it.cn_t_equiupkeep_date, OrderByType.Desc).ToPage(1, returnNum);
            //List<tn_dts_equiupkeep> equiupkeepList = upkeepPagedList.DataSource;
            List<tn_dts_equiupkeep> equiupkeepList = Db.Queryable<tn_dts_equiupkeep>().Where(it => it.cn_s_equiupkeep_no == deviceCode).OrderBy(it => it.cn_t_equiupkeep_date, OrderByType.Desc).Take(returnNum).ToList();
            EQUpkeepCollect eQUpkeepCollect = new EQUpkeepCollect();
            eQUpkeepCollect.deviceNo = deviceCode;
            eQUpkeepCollect.upkeepItemCount = equiupkeepList.Count().ToString();
            List<UpkeepItemModel> upkeepItemModelList = new List<UpkeepItemModel>();
            //if (upkeepPagedList.DataSource.Count == 1)
            //{
            //    UpkeepItemModel upkeepItemModel = new UpkeepItemModel();
            //    upkeepItemModel.
            //}
            //else
            //{
            int count = equiupkeepList.Count;
            int? defentperiod = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == deviceCode).Select(it => it.cn_s_equi_defentperiod).First();
            string parttype = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == deviceCode).Select(it => it.cn_s_equi_parttype).First();
            if (count == 0)
            {
                DateTime? buydate = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == deviceCode).Select(it => it.cn_s_equi_buydate).First();
                if (buydate.HasValue && (!string.IsNullOrEmpty(parttype)) && defentperiod.HasValue)
                {
                    UpkeepItemModel upkeepItemModel = new UpkeepItemModel();
                    DateTime upkeepTime = DateTime.Parse(buydate.Value.AddDays(defentperiod.Value).ToString());
                    upkeepItemModel.upkeepTime = upkeepTime.ToString();
                    upkeepItemModel.upkeepItemName = parttype;
                    upkeepItemModel.lastUpkeepTime = "首次";
                    upkeepItemModel.defentperiod = defentperiod.Value.ToString();
                    TimeSpan difference = upkeepTime - DateTime.Now;
                    if (difference.Days > 7)
                    {
                        upkeepItemModel.ext1 = "否";
                    }
                    else if (difference.Days >= 0)
                    {
                        upkeepItemModel.ext1 = "是（" + difference.Days.ToString() + "）";
                    }
                    else
                    {
                        upkeepItemModel.ext1 = "是（超期" + Math.Abs(difference.Days).ToString() + "）";
                    }
                    upkeepItemModel.ext2 = "";
                    upkeepItemModelList.Add(upkeepItemModel);
                }
            }
            for (int i = 0; i < count; i++)
            {

                if (defentperiod.HasValue == false)
                {
                    LogHelper.Info("tn_dts_equipment表中设备编码为：" + deviceCode + "的设备记录中cn_s_equi_defentperiod字段值为空。");
                    break;
                }
                UpkeepItemModel upkeepItemModel = new UpkeepItemModel();
                if (i == 0)
                {
                    DateTime? date = equiupkeepList[0].cn_t_equiupkeep_date;
                    if (date.HasValue == false)
                    {
                        LogHelper.Info("tn_dts_equiupkeep表中设备编码为：" + deviceCode + "的记录中，有记录的cn_t_equiupkeep_date字段值为空。");
                        continue;
                    }
                    //if (count == 1)
                    //{
                    DateTime upkeepTime = DateTime.Parse(date.Value.AddDays(defentperiod.Value).ToString());
                    upkeepItemModel.upkeepTime = upkeepTime.ToString();
                    upkeepItemModel.upkeepItemName = equiupkeepList[0].cn_s_equiupkeep_item;
                    upkeepItemModel.lastUpkeepTime = date.Value.ToString();
                    upkeepItemModel.defentperiod = defentperiod.Value.ToString();
                    TimeSpan difference = upkeepTime - DateTime.Now;
                    if (difference.Days > 7)
                    {
                        upkeepItemModel.ext1 = "否";
                    }
                    else if (difference.Days >= 0)
                    {
                        upkeepItemModel.ext1 = "是（" + difference.Days.ToString() + "）";
                    }
                    else
                    {
                        upkeepItemModel.ext1 = "是（超期" + Math.Abs(difference.Days).ToString() + "）";
                    }
                    upkeepItemModel.ext2 = "";
                    //}
                    //else
                    //{
                    //    DateTime upkeepTime = DateTime.Parse(date.Value.AddDays(defentperiod.Value).ToString());
                    //    upkeepItemModel.upkeepTime = upkeepTime.ToString();
                    //    upkeepItemModel.upkeepItemName = equiupkeepList[0].cn_s_equiupkeep_item;
                    //    upkeepItemModel.lastUpkeepTime = date.Value.ToString();
                    //    upkeepItemModel.defentperiod = defentperiod.Value.ToString();
                    //    TimeSpan difference = upkeepTime - DateTime.Now;
                    //    if (difference.Days > 7)
                    //    {
                    //        upkeepItemModel.ext1 = "否";
                    //    }
                    //    else if (difference.Days >= 0)
                    //    {
                    //        upkeepItemModel.ext1 = "是（" + difference.Days.ToString() + "）";
                    //    }
                    //    else
                    //    {
                    //        upkeepItemModel.ext1 = "是（超期" + Math.Abs(difference.Days).ToString() + "）";
                    //    }
                    //    upkeepItemModel.ext2 = "";
                    //}
                }
                else
                {
                    DateTime? daterear = equiupkeepList[i - 1].cn_t_equiupkeep_date;
                    DateTime? datefront = equiupkeepList[i].cn_t_equiupkeep_date;
                    if (daterear.HasValue == false || datefront.HasValue == false)
                    {
                        continue;
                    }
                    upkeepItemModel.upkeepTime = daterear.Value.ToString();
                    upkeepItemModel.upkeepItemName = equiupkeepList[i].cn_s_equiupkeep_item;
                    upkeepItemModel.lastUpkeepTime = datefront.Value.ToString();
                    upkeepItemModel.defentperiod = defentperiod.Value.ToString();
                    upkeepItemModel.ext1 = "";
                    upkeepItemModel.ext2 = "";
                }
                upkeepItemModelList.Add(upkeepItemModel);
            }
            eQUpkeepCollect.upkeepItem = upkeepItemModelList;
            eQUpkeepCollectList.Add(eQUpkeepCollect);
            return eQUpkeepCollectList;
        }
        #endregion

        //public List<EQUpkeepCollect> GetEQRepairCollectList(int returnNum)
        //{
        //    List<tn_dts_equiupkeep> equiupkeepList = Db.Queryable<tn_dts_equiupkeep>().OrderBy(it => it.cn_t_equiupkeep_date, OrderByType.Desc).ToList();
        //    List<string> equiupkeepRepeatNoList = Db.Queryable<tn_dts_equiupkeep>().OrderBy(it => it.cn_t_equiupkeep_date, OrderByType.Desc).Select(it => it.cn_s_equiupkeep_no).ToList();
        //    List<string> equiupkeepNoList = equiupkeepRepeatNoList.Distinct().ToList();
        //    List<EQUpkeepCollect> eQUpkeepCollectList = new List<EQUpkeepCollect>();
        //    for (int i = 0; i < returnNum; i++)
        //    {
        //        EQUpkeepCollect eQUpkeepCollect = new EQUpkeepCollect();
        //        if (equiupkeepNoList.Count <= i)
        //        {
        //            continue;
        //        }
        //        string equino = equiupkeepNoList[i];
        //        eQUpkeepCollect.deviceNo = equino;
        //        eQUpkeepCollect.upkeepItemCount = equiupkeepList.Where(it => it.cn_s_equiupkeep_no == equino).Count().ToString();
        //        List<tn_dts_equiupkeep> equiupkeepByEquinoList = equiupkeepList.Where(it => it.cn_s_equiupkeep_no == equino).OrderByDescending(it => it.cn_t_equiupkeep_date).ToList();
        //        List<UpkeepItemModel> upkeepItemList = new List<UpkeepItemModel>();
        //        for (int i = 0; i < ; i++)
        //        {

        //        }
        //        {
        //            int? defentperiod = Db.Queryable<tn_dts_equipment>().Where(it => it.cn_s_equi_no == equiupkeepByEquino.cn_s_equiupkeep_no).Select(it => it.cn_s_equi_defentperiod).First();
        //            if (defentperiod.HasValue == false)
        //            {
        //                continue;
        //            }
        //            UpkeepItemModel upkeepItem = new UpkeepItemModel();
        //            upkeepItem.upkeepTime = equiupkeepByEquino.cn_s_equiupkeep_date.ToString();
        //            upkeepItem.upkeepItemName = equiupkeepByEquino.cn_s_equirepair_item;
        //            upkeepItem.lastUpkeepTime =
        //            upkeepItem.defentperiod = equiupkeepByEquino.cn_s_equirepair_item;
        //            upkeepItem.ext1 = "";
        //            upkeepItem.ext2 = "";
        //            upkeepItemList.Add(upkeepItem);
        //        }
        //        eQRepairCollect.repairItem = repairItemList;
        //        eQRepairCollectList.Add(eQRepairCollect);
        //    }
        //    return eQRepairCollectList;
        //}
    }
}
