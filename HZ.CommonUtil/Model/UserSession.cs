using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.CommonUtil.Model
{
    public class UserSession
    {
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string DepartmentCode { get; set; }
        public string OrgCode { get; set; }
        public string OrgFlag { get; set; }
        public string OrgName { get; set; }
        public string TokenId { get; set; }

        public bool OneSession { get; set; }
        /// <summary>
        /// 用户可访问仓库
        /// </summary>
        public List<string> AuthorizeStock = new List<string>();
        public string SplitDbCode { get; set; }
        public int UserLocal { get; set; }
        public string UserPost { get; set; }
    }
}
