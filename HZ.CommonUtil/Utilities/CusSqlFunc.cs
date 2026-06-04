using Newtonsoft.Json.Linq;
using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Utilities
{
    public static class CusSqlFunc
    {
        public static string IsNull(SqlSugarClient db, string field, string defValue = "''")
        {
            if (db.CurrentConnectionConfig.DbType == DbType.SqlServer)
                return $" isnull({field},{defValue}) ";
            else if (db.CurrentConnectionConfig.DbType == DbType.MySql)
                return $" ifnull({field},{defValue}) ";
            else if (db.CurrentConnectionConfig.DbType == DbType.PostgreSQL)
                return $" COALESCE({field},{defValue}) ";
            return "";
        }

        public static string GetWhere(JObject[] parms)
        {
            if (parms == null)
                return "";
            string where = "";
            string temp;
            for (var i = 0; i < parms.Length; i++)
            {
                if (i > 0)
                    where += " and ";
                temp = parms[i]["key"].ObjToString();
                
                where += $"{temp}=@{temp}";
            }
            return where;
        }

        public static SugarParameter[] GetParam(JObject[] parms)
        {
            if (parms == null)
                return null;
            string temp;
            SugarParameter[] arr = new SugarParameter[parms.Length];
            for (var i = 0; i < arr.Length; i++)
            {
                temp = parms[i]["key"].ObjToString();
                arr[i] = new SugarParameter(temp, parms[i]["val"].ObjToString());
            }
            return arr;
        }
    }
}
