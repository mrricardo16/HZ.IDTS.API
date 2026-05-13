using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    #region 菜单模块权限数据
    /// <summary>
    /// 菜单模块权限数据
    /// </summary>
    public class MenuAuthorityModel
    {
        /// <summary>
        /// 菜单权限列表
        /// </summary>
        public List<Navigate> Navigate { get; set; }
    }
    #endregion

    #region 菜单权限项
    /// <summary>
    /// 菜单权限项
    /// </summary>
    public class Navigate
    {
        /// <summary>
        /// 父菜单编号
        /// </summary>
        public string ParentMenuid { get; set; }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string Menuid { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单图标
        /// </summary>
        public string MenuIco { get; set; }
        /// <summary>
        /// 菜单的导航地址
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 菜单排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 菜单是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 权限列表
        /// </summary>
        public List<Authority> ModuleAuthority { get; set; }
    }
    #endregion

    #region 系统权限项
    /// <summary>
    /// 系统权限项
    /// </summary>
    public class Authority
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string AuthorityName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsTrue { get; set; }
    }
    #endregion
}
