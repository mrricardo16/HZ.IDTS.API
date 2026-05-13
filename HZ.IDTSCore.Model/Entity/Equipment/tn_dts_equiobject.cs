using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Equipment
{
    #region 设备模型对象表
    /// <summary>
    /// 设备模型对象表
    ///</summary>
    [SugarTable("tn_dts_equiobject")]
    public class tn_dts_equiobject
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  设备唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_object_equiguid")]
        public string cn_s_object_equiguid { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_object_equi_no")]
        public string cn_s_object_equi_no { get; set; }
        /// <summary>
        ///  设备名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_object_equi_name")]
        public string cn_s_object_equi_name { get; set; }
        /// <summary>
        ///  设置项目
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_object_item")]
        public string cn_s_object_item { get; set; }
        /// <summary>
        ///  对象名
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_object_name")]
        public string cn_s_object_name { get; set; }
        /// <summary>
        ///  对象类型
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_object_type")]
        public string cn_s_object_type { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_object_remarks")]
        public string cn_s_object_remarks { get; set; }
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

    #region 对象树数据类
    /// <summary>
    /// 对象树数据类
    /// </summary>
    public class ObjectTree
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        public string cn_guid { get; set; }
        /// <summary>
        /// 部件类型
        /// </summary>
        public string cn_s_equi_parttype { get; set; }
        /// <summary>
        ///  设备编号
        ///</summary>
        public string cn_s_equi_no { get; set; }
        /// <summary>
        ///  设备名称
        ///</summary>
        public string cn_s_equi_name { get; set; }
        /// <summary>
        ///  设备类型
        ///</summary>
        public string cn_s_equi_type { get; set; }
        /// <summary>
        ///  设备型号
        ///</summary>
        public string cn_s_equi_model { get; set; }
        /// <summary>
        ///  设备状态
        ///</summary>
        public string cn_s_equi_status { get; set; }
        /// <summary>
        ///  购买日期
        ///</summary>
        public DateTime? cn_s_equi_buydate { get; set; }
        /// <summary>
        ///  质保日期
        ///</summary>
        public DateTime? cn_s_equi_qadate { get; set; }
        /// <summary>
        ///  首保日期
        ///</summary>
        public DateTime? cn_s_equi_firstdate { get; set; }
        /// <summary>
        ///  保养周期
        ///</summary>
        public int? cn_s_equi_defentperiod { get; set; }
        /// <summary>
        ///  所属部门
        ///</summary>
        public string cn_s_equi_dept { get; set; }
        /// <summary>
        ///  设备负责人
        ///</summary>
        public string cn_s_equi_dutyman { get; set; }
        /// <summary>
        ///  设备负责人电话
        ///</summary>
        public string cn_s_equi_dutyphone { get; set; }
        /// <summary>
        ///  合同编号
        ///</summary>
        public string cn_s_equi_contractno { get; set; }
        /// <summary>
        ///  所属产线
        ///</summary>
        public string cn_s_equi_beltline { get; set; }
        /// <summary>
        ///  x坐标
        ///</summary>
        public decimal? cn_s_equi_xpos { get; set; }
        /// <summary>
        ///  y坐标
        ///</summary>
        public decimal? cn_s_equi_ypos { get; set; }
        /// <summary>
        /// z坐标
        /// </summary>
        public decimal? cn_s_equi_zpos { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        public string cn_s_equi_remarks { get; set; }
        /// <summary>
        ///  修改人
        ///</summary>
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  修改人名称
        ///</summary>
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  修改日期
        ///</summary>
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  创建人
        ///</summary>
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  创建人名称
        ///</summary>
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  创建日期
        ///</summary>
        public DateTime? cn_t_create { get; set; }
        /// <summary>
        /// 该整机下对象列表
        /// </summary>
        public List<tn_dts_equiobject> equipmentobject { get; set; }
    }
    #endregion

    #region 粘贴对象接口传入数据类
    /// <summary>
    /// 粘贴对象接口传入数据类
    /// </summary>
    public class PasteObject
    {
        /// <summary>
        /// 对象要粘贴到的整机唯一标识
        /// </summary>
        public string Completemachineguid { get; set; }
        /// <summary>
        /// 复制的对象唯一标识
        /// </summary>
        public string LatestCopyObjectGuid { get; set; }
    }
    #endregion
}
