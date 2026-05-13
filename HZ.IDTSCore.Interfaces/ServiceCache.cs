using HZ.DbHelper;
using HZ.IDTSCore.Common.Const;
using HZ.IDTSCore.Interfaces.Service;
using HZ.IDTSCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Interfaces
{
    public class ServiceCache
    {
        #region 获取默认货主
        /// <summary>
        /// 获取默认货主
        /// </summary>
        /// <param name="stockCode"></param>
        /// <returns></returns>
        public static string GetDefaultOwner(SessionInfo session,string stockCode)
        {
            bool multiOwner = WmsSysSet.GetBoolValue(session, SysKeyword.SysSet_EnableMultiOwner);
            if (!multiOwner)
                return "";
            string defaultOwner = RedisServer.Cache.HGet(stockCode, "DefaultOwner");
            if (string.IsNullOrEmpty(defaultOwner))
            {
                tn_wms_owner o = null;// new OwnerService(new SessionInfo() { }).GetFirst(x =>x.cn_b_is_default == true);
                if (o == null)
                    throw new Exception("请维护默认货主！");
                RedisServer.Cache.HSet(stockCode, "DefaultOwner", o.cn_s_owner_name);
                return o.cn_s_owner_name;
            }
            return defaultOwner;
        }
        #endregion

        public static void SetDefaultOwner(string stockCode, string defaultOwner)
        {
            bool result = RedisServer.Cache.HSet(stockCode, "DefaultOwner", defaultOwner);
        }
    }
}
