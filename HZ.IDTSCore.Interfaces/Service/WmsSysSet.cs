using HZ.CommonUtil.Helpers;
using HZ.DbHelper;
using HZ.IDTSCore.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces.Service
{
   public static class WmsSysSet
    {
        //private static List<tn_wms_sys_set_value> SysSetValue = new List<tn_wms_sys_set_value>();

        //public static void SetAllValue(List<tn_wms_sys_set_value> values)
        //{
        //    SysSetValue = values;
        //}
        public static string GetStringValue(SessionInfo session, string stockCode, string key)
        {
            return GetValue(session,stockCode, key).ToString();
        }

        public static string GetStringValue(string key)
        {
            return GetValue(key).ToString();
        }
        public static bool GetBoolValue(SessionInfo session, string stockCode, string key)
        { 
            return Convert.ToBoolean(GetValue(session, stockCode, key));
        }

        public static bool GetBoolValue(SessionInfo session, string key)
        {
            return Convert.ToBoolean(GetValue(session,key));
        }

        //public static bool GetBoolValue(SessionInfo session, string key)
        //{
        //    return Convert.ToBoolean(GetValue(session, key));
        //}

        public static int GetIntValue(SessionInfo session, string stockCode, string key)
        {
            return Convert.ToInt32(GetValue(session, stockCode, key));
        }
        private static object GetValue(SessionInfo session,string stockCode, string key)
        {
            string keyValue = RedisServer.Cache.HGet(stockCode, key);
            if (string.IsNullOrEmpty(keyValue))
            {
                //var v = null;// new SysSetValueService(session).GetFirst(x =>x.cn_s_stock_code==stockCode &&x.cn_s_set_code==key);
                //if (v == null)
                //{
                //    var def = null;// new SysSetService(session).GetFirst(x => x.cn_s_set_code == key);
                //    if (def == null || string.IsNullOrEmpty(def.cn_s_default_value))
                //        throw new Exception($"仓库[{stockCode}]中找不到该系统策略[{key}]！");
                //    else
                //        v = new tn_wms_sys_set_value() { cn_s_set_value = def.cn_s_default_value };
                //}
                //else if(string.IsNullOrEmpty(v.cn_s_set_value))
                //{
                //    throw new Exception($"请为仓库[{stockCode}]配置系统策略[{key}]！");
                //}

                //keyValue = v.cn_s_set_value;
                SetValue(stockCode, key, keyValue);
            }
            return keyValue;
        }

        private static object GetValue(string key)
        {
            string appName = AppSettings.GetValue<string>("AppSet:AppName");
            
            string keyValue = RedisServer.Cache.HGet(appName, key);
            if (string.IsNullOrEmpty(keyValue))
            {
                //var v = new SysSetValueService(new SessionInfo() { }).GetFirst(x => SqlFunc.Trim(x.cn_s_set_code) == key);
                //if (v == null)
                //{
                //    var def = new SysSetService(new SessionInfo() { }).GetFirst(x => SqlFunc.Trim(x.cn_s_set_code) == key);

                //    if (def == null || string.IsNullOrEmpty(def.cn_s_default_value))
                //        throw new Exception($"请尽快配置系统策略[{key}]！");
                //    else
                //        v = new tn_wms_sys_set_value() { cn_s_set_value = def.cn_s_default_value };
                //}

                //keyValue = v.cn_s_set_value;
                SetValue(appName, key, keyValue);
            }
            return keyValue;
        }

        private static object GetValue(SessionInfo session, string key)
        {
            string appName = AppSettings.GetValue<string>("AppSet:AppName");

            string keyValue = RedisServer.Cache.HGet(appName, key);
            if (string.IsNullOrEmpty(keyValue))
            {
                //var v = new SysSetValueService(session).GetFirst(x => SqlFunc.Trim(x.cn_s_set_code) == key);
                //if (v == null)
                //{
                //    var def = new SysSetService(session).GetFirst(x => SqlFunc.Trim(x.cn_s_set_code) == key);

                //    if (def == null || string.IsNullOrEmpty(def.cn_s_default_value))
                //        throw new Exception($"请尽快配置系统策略[{key}]！");
                //    else
                //        v = new tn_wms_sys_set_value() { cn_s_set_value = def.cn_s_default_value };
                //}

                //keyValue = v.cn_s_set_value;
                SetValue(appName, key, keyValue);
            }
            return keyValue;
        }

        public static bool SetValue(string stockCode, string key, string val)
        {
            if (!RedisServer.Cache.HExists(stockCode, key))
                return RedisServer.Cache.HSet(stockCode, key, val);
            else
            {
                RedisServer.Cache.HDel(stockCode, key);
                return RedisServer.Cache.HSet(stockCode, key, val);
            }
        }
    }
}
