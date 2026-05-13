using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    #region 建模视角区域表
    /// <summary>
    /// 建模视角区域表
    /// </summary>
    [SugarTable("tn_dts_3danglearea")]
    public class tn_dts_3danglearea
    {
        /// <summary>
        ///  唯一标识
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  序号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_serial")]
        public int? cn_s_3danglearea_serial { get; set; }
        /// <summary>
        ///  视角区域编号
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_code")]
        public string cn_s_3danglearea_code { get; set; }
        /// <summary>
        ///  视角区域名称
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_name")]
        public string cn_s_3danglearea_name { get; set; }
        /// <summary>
        ///  相机X坐标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_posX")]
        public float? cn_s_3danglearea_posX { get; set; }
        /// <summary>
        ///  相机Y坐标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_posY")]
        public float? cn_s_3danglearea_posY { get; set; }
        /// <summary>
        ///  相机Z坐标
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_posZ")]
        public float? cn_s_3danglearea_posZ { get; set; }
        /// <summary>
        ///  角度X
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_angleX")]
        public float? cn_s_3danglearea_angleX { get; set; }
        /// <summary>
        ///  角度Y
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_angleY")]
        public float? cn_s_3danglearea_angleY { get; set; }
        /// <summary>
        ///  角度Z
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_angleZ")]
        public float? cn_s_3danglearea_angleZ { get; set; }
        /// <summary>
        ///  备注
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_3danglearea_remarks")]
        public string cn_s_3danglearea_remarks { get; set; }
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

    #region 建模视角区域保存数据类
    /// <summary>
    /// 建模视角区域保存数据类
    /// </summary>
    public class SaveThreeDimensionAngleArea
    {
        /// <summary>
        /// 新增修改
        /// </summary>
        public string AddOrModify { get; set; }
        /// <summary>
        /// 建模视角区域
        /// </summary>
        public tn_dts_3danglearea NewThreeDimensionAngleArea { get; set; }
    }
    #endregion
}
