using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HZ.IDTSCore.Common
{
    /// <summary>
    /// 常用帮助类
    /// </summary>
    public class CHelper
    {
        private const char INTERVAL_SYMBOL = '$';           //拆分与合并的间隔符

        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static string Merge(List<string> list)
        {
            return string.Join(INTERVAL_SYMBOL, list);
        }

        /// <summary>
        /// 拆分
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> Split(string str)
        {
            return str.Split(INTERVAL_SYMBOL).ToList();
        }


        /// <summary>
        /// 拆分
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> SplitDict(string str)
        {
            if(string.IsNullOrEmpty(str))
                return new KeyValuePair<string, string>();

            var list = str.Split(INTERVAL_SYMBOL).ToList();
            if(list.Count() > 1)
            {
                return new KeyValuePair<string, string>(list[0], list[1]);
            }
            return new KeyValuePair<string, string>();
        }

    }
}
