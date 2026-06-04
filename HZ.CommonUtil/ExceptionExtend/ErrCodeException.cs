using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.ExceptionExtend
{
    public class ErrCodeException : Exception
    {
        private int errCode = 0;
        public int ErrCode { get
            {
                return errCode;
            }
        }
        private string msgLogCode = "";
        public string MsgLogCode
        {
            get
            {
                return msgLogCode;
            }
        }

        /// <summary>
        /// 抛出异常
        /// </summary>
        /// <remarks>
        /// <para/>Author : DBS
        /// <para/>Date : 2023-09-18 17:47
        /// </remarks>
        /// <param name="message">错误类型</param>
        /// <param name="errCode">错误码</param>
        /// <param name="msgLogCode">logid</param>
        public ErrCodeException(string message, int errCode,string msgLogCode="") : base(message)
        {
            this.errCode = errCode;
            this.msgLogCode = msgLogCode;
        }
    }
}
