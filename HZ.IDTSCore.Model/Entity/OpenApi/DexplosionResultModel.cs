using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class DexplosionResultModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 错误代码
        /// </summary>
        public string ErrCode { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }
}
