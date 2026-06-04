using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.DbHelper
{
    public static class SqlFuncExt
    {
        /// <summary>
        /// 获取Guid
        /// </summary>
        /// <returns></returns>
        public static string GetID()
        {
            //这里不能写任何实现代码，需要在下面的配置中实现
            //throw new NotSupportedException("Can only be used in expressions");
            return "";
        }
        public static bool DateIsNull(DateTime? s)
        {
            //这里不能写任何实现代码，需要在下面的配置中实现
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int ASCII(string s)
        {
            //这里不能写任何实现代码，需要在下面的配置中实现
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static List<SqlFuncExternal> GetSqlFuncExternal()
        {
            var expMethods = new List<SqlFuncExternal>();
            expMethods.Add(new SqlFuncExternal()
            {
                UniqueMethodName = "GetID",
                MethodValue = (expInfo, dbType, expContext) =>
                {
                    if (dbType == DbType.SqlServer)
                        return "NEWID()";
                    //return string.Format("CAST({0} AS VARCHAR(MAX))", expInfo.Args[0].MemberName);
                    else if (dbType == DbType.MySql)
                        return "md5(UUID())";
                    else if (dbType == DbType.Oracle)
                        return "sys_guid()";
                    else if (dbType == DbType.PostgreSQL)
                        return "uuid_generate_v4()";
                    else
                        throw new Exception("未实现");
                }
            });
            expMethods.Add(new SqlFuncExternal()
            {
                UniqueMethodName = "DateIsNull",
                MethodValue = (expInfo, dbType, expContext) =>
                {
                    return $"{expInfo.Args[0].MemberName} is null ";
                }
            });
            expMethods.Add(new SqlFuncExternal()
            {
                UniqueMethodName = "ASCII",
                MethodValue = (expInfo, dbType, expContext) =>
                {
                    if (dbType == DbType.SqlServer)
                        return $"ASCII({expInfo.Args[0].MemberName})";
                    else if (dbType == DbType.MySql)
                        return $"ASCII({expInfo.Args[0].MemberName})";
                    else if (dbType == DbType.Oracle)
                        throw new Exception("未实现");
                    else
                        throw new Exception("未实现");
                }
            });
            return expMethods;
        }
    }
}
