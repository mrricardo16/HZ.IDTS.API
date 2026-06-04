using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Model
{
    public class QueryResult<T>
    {
        public int Total = 0;
        public List<T> Rows;
    }
}
