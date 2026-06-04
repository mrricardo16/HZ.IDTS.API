using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace HZ.CommonUtil.Model
{
    public class PageParm
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页总条数
        /// </summary>
        public int PageSize { get; set; } = 20;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 配置power
        /// </summary>
        public string PowerCode { get; set; } = "";

        /// <summary>
        /// 是否导出
        /// </summary>
        public bool Export { get; set; } = false;

        /// <summary>
        /// 排序方式
        /// </summary>
        public string Sort { get; set; }

        public JObject Parms { get; set; } = new JObject();

        public JObject[] DynaParms { get; set; }
    }
}
