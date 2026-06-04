using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Helpers
{
    public class CommonHelper
    {
        /// <summary>
        /// 是否虚拟托盘
        /// </summary>
        /// <param name="trayCode"></param>
        /// <returns></returns>
        public static bool IsVirtualTray(string trayCode)
        {
            return trayCode.StartsWith("VT");
        }

        /// <summary>
        /// 获取虚拟托盘
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        public static string GetVirtualTray(string locationCode)
        {
            return "VT" + locationCode;
        }
    }
}
