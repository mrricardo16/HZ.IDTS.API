
using HZ.CommonUtil.Helpers;
using System.Collections.Generic;

namespace HZ.IDTSCore.Common.Const
{
    public class SysConst
    {
        public static string Token;

        /// <summary>
        /// 默认 SESSION 过期时间 (单位:小时)
        /// </summary>
        public static int DefaultSessionExpire;

        /// <summary>
        /// 是否启用Mongo作为mdg数据缓存
        /// </summary>
        public static bool UseMongoCache;

        /// <summary>
        /// 项目编号
        /// </summary>
        public static string ProjectCode = AppSettings.GetValue<string>("AppSettings:ProjectCode");

        /// <summary>
        /// 打印测试日志
        /// des: 默认不打印，配置打印；避免正式发布后，日志文件过大
        /// </summary>
        public static bool PrintTestLog = AppSettings.GetValue<bool>("AppSettings:PrintTestLog");

        /// <summary>
        /// 是否推送SAP
        /// </summary>
        public static bool PushSAP;

        /// <summary>
        /// 是否推送SAP
        /// </summary>
        public static bool StockSplit;

        public static bool IsTest = false;

        public static List<string> SplitDb = new List<string>();

        public static void Initalize()
        {
            Token = AppSettings.GetValue<string>("ProjectSet:Token");
            DefaultSessionExpire = AppSettings.GetValue<int>("AppSettings:DefaultSessionExpire");
            UseMongoCache = false;
            IsTest = AppSettings.GetValue<bool>("ProjectSet:IS_TEST");
            PrintTestLog = AppSettings.GetValue<bool>("ProjectSet:PrintTestLog");
            PushSAP = AppSettings.GetValue<bool>("ProjectSet:PUSH_SAP");
            //ryboHelp = new RyboHelp();

            SplitDb =new List<string>(AppSettings.GetValue<string>("AppSettings:SplitDbs").Split(','));
        }

        /// <summary>
        /// MDG API 地址
        /// </summary>
        public static string MDGApi;

    }
}
