using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Model
{
    /// <summary>
    /// 贵阳料箱机器人接口返回结果
    /// </summary>
    public class EssResult
    {
        /// <summary>
        ///  0 代表正常，其余代表异常
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        public string msg { get; set; }

        /// <summary>
        /// 返回数据对象，如果需要
        /// </summary>
        public Object data { get; set; }
    }
}
