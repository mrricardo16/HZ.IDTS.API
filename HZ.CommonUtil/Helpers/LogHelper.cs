using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;

namespace HZ.CommonUtil.Helpers
{
    public class LogHelper
    {
        private const string repositoryName = "NETCoreRepository";
        private const string configFile = "log4net.config";
        
        private static ILoggerRepository repository { get; set; }

        private static readonly ILog _loginfo = LogManager.GetLogger(repositoryName, "loginfo");
        private static readonly ILog _logerror = LogManager.GetLogger(repositoryName, "logerror");
        private static readonly ILog _logwarn = LogManager.GetLogger(repositoryName, "logwarn");
        private static readonly ILog _logfatal = LogManager.GetLogger(repositoryName, "logfatal");
        private static readonly ILog _mysqllog = LogManager.GetLogger(repositoryName, "mysqllog");
        private static readonly ILog _mssqllog = LogManager.GetLogger(repositoryName, "mssqllog");

        private static string dataType = "MYSQL";
        public static void Configure()
        {
            repository = LogManager.CreateRepository(repositoryName);
            XmlConfigurator.Configure(repository, new FileInfo(configFile));
            dataType = AppSettings.GetValue<string>("DbConnection:DataType")?.ToUpper();
        }

        /// <summary>
        /// 保存到txt
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-09-14 12:17
        /// </remarks>
        /// <param name="msg">消息</param>
        /// <param name="logId">logid</param>
        /// <param name="otherCode">flag标记</param>
        public static void Info(string msg,string logId="", params string[] otherCode)
        {
            ThreadContext.Properties["cn_s_id"] = logId;
            if (otherCode.Length > 0)
                ThreadContext.Properties["cn_s_flag_num"] = string.Join(',', otherCode);
            _loginfo.Info(msg);
        }

        public static void Warn(string msg)
        {
            _logwarn.Warn(msg);
        }

        public static void Error(string msg, Exception exp = null, string logId = "", params string[] otherCode)
        {
            ThreadContext.Properties["cn_s_id"] = logId;
            if (otherCode.Length > 0)
                ThreadContext.Properties["cn_s_flag_num"] = string.Join(',', otherCode);
            if (exp == null)
                _logerror.Error(msg);
            else
                _logerror.Error(msg, exp);
        }

        public static void ErrorCache(int cacheSecond, string msg, Exception exp = null)
        {
            if (exp == null)
                _logerror.Error(msg);
            else
                _logerror.Error(msg, exp);
        }

        /// <summary>
        /// 保存到txt和db数据库
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-09-14 12:17
        /// </remarks>
        /// <param name="msg">消息</param>
        /// <param name="logId">logid</param>
        /// <param name="otherCode">flag标记</param>
        public static void LogDb(string msg, string logId = "", params string[] otherCode)
        {
            ThreadContext.Properties["cn_s_id"] = logId;
            if (otherCode.Length > 0)
                ThreadContext.Properties["cn_s_flag_num"] = string.Join(',', otherCode);

            Info(msg, logId, otherCode);
            switch (dataType)
            {
                case "MYSQL": _mysqllog.Info(msg); break;
                case "MSSQL": _mssqllog.Info(msg); break;
            }
        }

        public static void Fatal(string msg)
        {
            _logfatal.Fatal(msg);
        }
    }
}
