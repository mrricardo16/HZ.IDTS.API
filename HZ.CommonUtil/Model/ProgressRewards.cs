using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.CommonUtil.Model
{
    /// <summary>
    /// 缓存数据更新，进度回报
    /// </summary>
    public class ProgressRewards
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string usid { get; set; }
        /// <summary>
        /// 缓存数据类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 缓存数据总数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 已缓存的数量
        /// </summary>
        public int countOk { get; set; }

        /// <summary>
        /// 完成进度百分比
        /// </summary>
        public int percent { get; set; }
    }
}
