using System.Collections.Generic;

namespace HZ.IDTSCore.Model.SerializeEntity
{
    public class AccountEntity
    {
        /// <summary>
        /// code的值的含义为，0：成功，1：失败。
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Data { set; get; }

        /// <summary>
        /// 随机字符标记
        /// </summary>
        public string Flag { get; set; }
        /// <summary>
        /// 随机密钥
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// 用户真实名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户扩展属性字段
        /// </summary>
        public UserExt userExt = new UserExt();

        public List<string> AuthorizeStock => userExt.stockCode == null ? new List<string>() : new List<string>(userExt.stockCode.Split(','));
        /// <summary>
        /// 当前用户所有的菜单
        /// </summary>
        //public List<MenuListEntity> UserMenuList { get; set; }

        public string SplitDbCode { get; set; } = "";
        public List<SplitDb> SplitDbList = new List<SplitDb>();
    }

    public class UserExt
    {
        public string OrgName { get; set; }
        public string OrgCode { get; set; }
        public string OrgRank { get; set; }
        public string OrgFlag { get; set; }
        public string stockCode { get; set; }

        public string areaCode { get; set; }
    }

    public class SplitDb
    {
        public string cn_s_orgname { get; set; }
        public string cn_s_orgflag { get; set; }
    }
}
