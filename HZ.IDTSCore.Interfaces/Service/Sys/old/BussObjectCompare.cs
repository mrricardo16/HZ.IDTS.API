/// <summary>
/// 功能描述    ：业务对象比较  
/// 创 建 者    ：dbs
/// 创建日期    ：2021/2/23 16:59:04 
/// 最后修改者  ：dbs
/// 最后修改日期：2021/2/23 16:59:04 
/// </summary>
using HZ.CommonUtil.Model;
using HZ.CommonUtil.Utilities;
using HZ.DbHelper;
using HZ.IDTSCore.Model;
using HZ.IDTSCore.Model.Entity.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static HZ.IDTSCore.Common.Const.SysKeyword;

namespace HZ.IDTSCore.Interfaces.Service.Sys
{
    public static class BussObjectCompare
    {
        public static ApiResult MongoUpdateDetailExec(redis_update_detail_log mud)
        {
            if (mud == null)
                return ApiResult.Error("存入的数据异常！");

            string or = string.Empty;

            try
            {
                DbContext db = new DbContext(new SessionInfo(){ splitDbCode=mud.orgCode});

                //switch (StringUtil.GetEnumFromStr<OrderType>(mud.cn_s_order_type))
                switch (mud.cn_s_order_type)
                {
                    case "到货单":
                        var powerCof = db.Db.Queryable<tn_wms_view_table_conf>().Where(x => x.cn_s_power_code == "到货单列表").ToList();
                        //or = DetailOperateLog<tn_wms_arrival_mst, tn_wms_arrival_dtl>(powerCof, mud.cn_s_old_obj, mud.cn_s_new_obj, "DTLEntity", "到货单列表");
                        break;
                    //case OrderType.Check: break;
                    case "入库单":
                        powerCof = db.Db.Queryable<tn_wms_view_table_conf>().Where(x => x.cn_s_power_code == "入库单列表").ToList();
                       //or = DetailOperateLog<tn_wms_in_inventory_mst, tn_wms_in_inventory_dtl>(powerCof, mud.cn_s_old_obj, mud.cn_s_new_obj, "DTLEntity", "入库单列表");
                        break;
                    case "入库订单":
                        powerCof = db.Db.Queryable<tn_wms_view_table_conf>().Where(x => x.cn_s_power_code == "入库订单列表").ToList();
                        //or = DetailOperateLog<tn_wms_in_order_mst, tn_wms_in_order_dtl>(powerCof, mud.cn_s_old_obj, mud.cn_s_new_obj, "DTLEntity", "入库订单列表");
                        break;
                    case "发货通知单":
                        powerCof = db.Db.Queryable<tn_wms_view_table_conf>().Where(x => x.cn_s_power_code == "发货通知单列表").ToList();
                        //or = DetailOperateLog<tn_wms_out_advice_mst, tn_wms_out_advice_dtl>(powerCof, mud.cn_s_old_obj, mud.cn_s_new_obj, "DTLEntity", "发货通知单列表");
                        break;
                        //case OrderType.Inspect: break;
                        //case OrderType.OutInventory:
                        //    //or = UpdateDetailOperateLog<tn_wms_out_advice_mst, tn_wms_out_advice_dtl>(mud.cn_s_old_obj, mud.cn_s_new_obj, "OutDtlList", "发货通知单列表");
                        //    break;
                        //case OrderType.OutOrder:
                        //    //or = UpdateDetailOperateLog<TN_WM_REDUCE_INVENTORY_MSTEntity, TN_WM_REDUCE_INVENTORY_DTLEntity>(mud.CN_S_OLD_OBJ, mud.CN_S_NEW_OBJ, "DTLEntity");
                        //    break;
                }

                return ApiResult.Success("", new tn_wms_bill_exec()
                {
                    cn_s_guid = Guid.NewGuid().ToString(),
                    cn_s_executor = mud.cn_s_executor,
                    cn_s_executor_content = or,
                    cn_s_note = "",
                    cn_s_operation_type = mud.cn_s_active_type,
                    cn_s_op_no = mud.cn_s_op_no,
                    cn_t_execute = mud.cn_t_execute,
                    cn_s_order_type = mud.cn_s_order_type
                });
            }
            catch (Exception ex)
            {
                return ApiResult.Error(ex.Message);
            }
        }

        static List<string> filterField = new List<string>()
        {
           "cn_s_guid","cn_n_row_no",
            "cn_s_creator","cn_s_creator_by","cn_t_create",
            "cn_s_modify","cn_s_modify_by","cn_t_modify"
        };
        private static string DetailOperateLog<M, D>(List<tn_wms_view_table_conf> fieldName_zh, object oldObj, object newObj, string dtlField, string power = "")
        {
            try
            {
                if (oldObj == null)
                {
                    return AddDetailOperateLog<M, D>(fieldName_zh, newObj);
                }
                else
                {
                    return UpdateDetailOperateLog<M, D>(fieldName_zh, oldObj, newObj, dtlField, power);
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        private static string UpdateDetailOperateLog<M, D>(List<tn_wms_view_table_conf> fieldName_zh,object oldObj, object newObj, string dtlField, string power = "")
        {
            try
            {
                M oldM = JsonConvert.DeserializeObject<M>(oldObj.ToString());
                M newM = JsonConvert.DeserializeObject<M>(newObj.ToString());
                PropertyInfo[] mFields = typeof(M).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                StringBuilder mstSb = new StringBuilder();
                StringBuilder sb = new StringBuilder();

                if (!string.IsNullOrEmpty(power))
                {
                    //var powerMst = CreateDapperDAL<TN_WM_POWER_ENAB
                    //_MSTEntity>().GetSingleEntity(new
                    //{
                    //    CN_S_POWER = power
                    //});
                    //if (powerMst != null)
                    //{
                    //fieldName_zh = Db.Queryable<tn_wms_view_table_conf>().Where(x => x.cn_s_power_name == power).ToList();
                    //fieldName_zh = CreateDapperDAL<tn_wms_view_table_conf>().GetList(new
                    //    {
                    //        CN_S_PARENTID = powerMst.CN_GUID
                    //    });
                    //}
                }

                foreach (var mF in mFields)
                {
                    if (mF.Name == dtlField)
                    {
                        List<D> oldD = (List<D>)mF.GetValue(oldM, null);//修改前的子表集合
                        List<D> newD = (List<D>)mF.GetValue(newM, null);//修改后的子表集合
                        PropertyInfo[] dFields = typeof(D).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);//子表实体属性激活
                        PropertyInfo itemField = dFields.FirstOrDefault(d => d.Name == "cn_s_item_code");//寻找ItemCode属性

                        List<D> eqOObj = new List<D>();//记录没有变动的修改前行记录
                        List<D> eqNObj = new List<D>();//记录没有变动的修改后行记录

                        foreach (var _od in oldD)
                        {
                            //循环修改前的行
                            foreach (var _nd in newD)
                            {
                                int filedCount = dFields.Length;
                                int eqNum = 0;
                                //跟修改后的行比较
                                foreach (var dF in dFields)
                                {
                                    if (filterField.Contains(dF.Name)) continue;
                                    //循环所有属性
                                    //if (dF.GetValue(_od).Equals(dF.GetValue(_nd)))
                                    if (StringUtil.IsNull(dF.GetValue(_od)) == StringUtil.IsNull(dF.GetValue(_nd)))
                                    {
                                        //存在不等于的属性
                                        eqNum++;
                                    }
                                    //break;
                                }

                                if (eqNum == filedCount)
                                {
                                    //完全相同
                                    eqOObj.Add(_od);
                                    eqNObj.Add(_nd);
                                }
                            }
                        }

                        //排除掉完全相同的行记录
                        eqOObj.ForEach(x =>
                        {
                            oldD.Remove(x);
                        });
                        eqNObj.ForEach(x =>
                        {
                            newD.Remove(x);
                        });

                        List<D> add = new List<D>();
                        List<D> remove = new List<D>();
                        Dictionary<D, D> update = new Dictionary<D, D>();

                        foreach (var _od in oldD)
                        {
                            string oldItemCode = StringUtil.IsNull(itemField.GetValue(_od, null));

                            bool isExist = false;
                            for (int i = newD.Count - 1; i >= 0; i--)
                            {
                                string newItemCode = StringUtil.IsNull(itemField.GetValue(newD[i], null));
                                if (newItemCode.Equals(oldItemCode))
                                {
                                    isExist = true;
                                    update[_od] = newD[i];
                                    newD.Remove(newD[i]);
                                    break;
                                }
                            }
                            if (!isExist)
                            {
                                remove.Add(_od);
                            }
                        }
                        foreach (var _nd in newD)
                        {
                            add.Add(_nd);
                        }

                        foreach (var _remove in remove)
                        {
                            sb.AppendLine("【删除】:" + JsonConvert.SerializeObject(_remove));
                        }
                        foreach (var _add in add)
                        {
                            sb.AppendLine("【新增】:" + JsonConvert.SerializeObject(_add));
                        }
                        foreach (var _uKey in update.Keys)
                        {
                            string itemCode = StringUtil.IsNull(itemField.GetValue(_uKey, null));
                            sb.AppendLine("【" + itemCode + " 修改】:");
                            foreach (var dF in dFields)
                            {
                                if (filterField.Contains(dF.Name)) continue;
                                string oldValue = string.Empty;
                                string newValue = string.Empty;

                                if (dF.GetValue(_uKey) == null)
                                    oldValue = "null";
                                else if (StringUtil.IsNull(dF.GetValue(_uKey)) == "")
                                    oldValue = "''";
                                else
                                    oldValue = StringUtil.IsNull(dF.GetValue(_uKey));

                                if (dF.GetValue(update[_uKey]) == null)
                                    newValue = "";
                                else if (StringUtil.IsNull(dF.GetValue(update[_uKey])) == "")
                                    newValue = "''";
                                else
                                    newValue = StringUtil.IsNull(dF.GetValue(update[_uKey]));

                                //fieldName_zh
                                string fieldName = fieldName_zh.Find(f => f.cn_s_column_code == dF.Name) != null
                                    ? fieldName_zh.Find(f => f.cn_s_column_code == dF.Name).cn_s_column_name : dF.Name;

                                if (oldValue != newValue)
                                {
                                    sb.Append("[" + fieldName + "]：" + oldValue + "->" + newValue + "\t");
                                }
                            }
                            sb.AppendLine();
                        }
                    }
                    else
                    {
                        if (filterField.Contains(mF.Name)) continue;
                        string oldValue = string.Empty;
                        string newValue = string.Empty;

                        if (mF.GetValue(oldM, null) == null)
                            oldValue = "null";
                        else if (StringUtil.IsNull(mF.GetValue(oldM, null)) == "")
                            oldValue = "''";
                        else
                            oldValue = StringUtil.IsNull(mF.GetValue(oldM, null));

                        if (mF.GetValue(newM, null) == null)
                            newValue = "null";
                        else if (StringUtil.IsNull(mF.GetValue(newM, null)) == "")
                            newValue = "''";
                        else
                            newValue = StringUtil.IsNull(mF.GetValue(newM, null));


                        string fieldName = fieldName_zh.Find(f => f.cn_s_column_code == mF.Name) != null
                            ? fieldName_zh.Find(f => f.cn_s_column_code == mF.Name).cn_s_column_name : mF.Name;

                        if (oldValue != newValue)
                        {
                            mstSb.Append("[" + fieldName + "]：" + oldValue + "->" + newValue + ",");
                        }
                    }
                }
                StringBuilder result = new StringBuilder();
                string mstLog = mstSb.ToString();
                if (!string.IsNullOrEmpty(mstLog))
                {
                    result.AppendLine("单据修改信息：");
                    result.AppendLine(mstLog);
                }
                result.AppendLine("明细修改信息：");
                result.AppendLine(sb.ToString());
                return result.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string AddDetailOperateLog<M, D>(List<tn_wms_view_table_conf> fieldName_zh, object newObj)
        {
            try
            {
                M newM = JsonConvert.DeserializeObject<M>(newObj.ToString());
                PropertyInfo[] mFields = typeof(M).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                StringBuilder mstSb = new StringBuilder();
                StringBuilder sb = new StringBuilder();

                foreach (var mF in mFields)
                {
                    if (filterField.Contains(mF.Name)) continue;
                    string newValue = string.Empty;

                    if (mF.GetValue(newM, null) == null)
                        newValue = "";
                    else if (StringUtil.IsNull(mF.GetValue(newM, null)) == "")
                        newValue = "''";
                    else
                        newValue = StringUtil.IsNull(mF.GetValue(newM, null));

                    var field = fieldName_zh.Find(f => f.cn_s_column_code == mF.Name);
                    if (field != null)
                    {
                        string fieldName = fieldName_zh.Find(f => f.cn_s_column_code == mF.Name).cn_s_column_name;
                        mstSb.Append("[" + fieldName + "]：" + newValue??""+",");
                    }
                    //string fieldName = fieldName_zh.Find(f => f.cn_s_column_code == mF.Name) != null
                    //    ? fieldName_zh.Find(f => f.cn_s_column_code == mF.Name).cn_s_column_name : mF.Name;
                    //mstSb.Append("[" + fieldName + "]：" + newValue + "\t");

                }
                StringBuilder result = new StringBuilder();
                string mstLog = mstSb.ToString();
                if (!string.IsNullOrEmpty(mstLog))
                {
                    result.AppendLine("单据新增信息：");
                    result.AppendLine(mstLog);
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
