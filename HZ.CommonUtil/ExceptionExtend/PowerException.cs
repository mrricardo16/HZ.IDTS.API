using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.ExceptionExtend
{
    public class PowerException:Exception
    {
        public PowerException(string message) : base(message)
        { }
    }
}
