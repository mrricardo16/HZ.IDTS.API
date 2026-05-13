using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 3d视图菜单表
    /// <summary>
    /// 3d视图菜单表
    /// </summary>
    [SugarTable("tn_dts_3dviewmenu")]
    public class tn_dts_3dviewmenu
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  父菜单编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_parentmenuid")]
        public string cn_s_3dview_parentmenuid { get; set; }
        /// <summary>
        ///  菜单编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_menuid")]
        public string cn_s_3dview_menuid { get; set; }
        /// <summary>
        ///  菜单名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_menuname")]
        public string cn_s_3dview_menuname { get; set; }
        /// <summary>
        ///  菜单图标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_menuico")]
        public string cn_s_3dview_menuico { get; set; }
        /// <summary>
        ///  菜单的导航地址
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_menuurl")]
        public string cn_s_3dview_menuurl { get; set; }
        /// <summary>
        ///  菜单排序
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_menusort")]
        public int cn_s_3dview_menusort { get; set; }
        /// <summary>
        ///  菜单是否显示
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_menuisshow")]
        public int cn_s_3dview_menuisshow { get; set; }
        /// <summary>
        ///  菜单下的模块权限
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_menumoduleauth")]
        public string cn_s_3dview_menumoduleauth { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3dview_remarks")]
        public string cn_s_3dview_remarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify_by")]
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  创建人
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator")]
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  创建人名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator_by")]
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
    }
    #endregion

   
}
