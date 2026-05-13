using HZ.CommonUtil.Helpers;
using HZ.IDTSCore.Common.Const;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Common.Helpers
{
    /// <summary>
    /// 测试日志 test-log
    /// </summary>
    public class TLog
    {
        /// <summary>
        /// 测试日志 - info
        /// </summary>
        /// <param name="content"></param>
        public static void Info(string content)
        {
            if (SysConst.PrintTestLog)
            {
                LogHelper.Info(content);
            }
        }

        /// <summary>
        /// 测试日志 - info
        /// </summary>
        /// <param name="content"></param>
        public static string Error(string content)
        {
            if (SysConst.PrintTestLog)
            {
                LogHelper.Error(content);
            }

            return content;
        }

    }
}
