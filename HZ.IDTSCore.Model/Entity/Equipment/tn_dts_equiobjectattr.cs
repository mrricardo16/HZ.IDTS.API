using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备模型对象属性表
    /// <summary>
    /// 设备模型对象属性表
    ///</summary>
    [SugarTable("tn_dts_equiobjectattr")]
    public class tn_dts_equiobjectattr
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  对象ID
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_objectattr_guid")]
        public string cn_s_objectattr_guid { get; set; }
        /// <summary>
        ///  对象名
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_objectattr_name")]
        public string cn_s_objectattr_name { get; set; }
        /// <summary>
        ///  属性名
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_objectattr_attrname")]
        public string cn_s_objectattr_attrname { get; set; }
        /// <summary>
        ///  属性类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_objectattr_attrtype")]
        public string cn_s_objectattr_attrtype { get; set; }
        /// <summary>
        ///  属性值
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_objectattr_attrvalue")]
        public string cn_s_objectattr_attrvalue { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_objectattr_remarks")]
        public string cn_s_objectattr_remarks { get; set; }
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
