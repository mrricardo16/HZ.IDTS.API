using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    /// <summary>
    /// 
    ///</summary>
    [SugarTable("tn_dts_camera")]
    public class tn_dts_camera
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_guid", IsPrimaryKey = true)]
        public string cn_guid { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_area_guid")]
        public string cn_s_camera_area_guid { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_serial")]
        public int? cn_s_camera_serial { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_code")]
        public string cn_s_camera_code { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_name")]
        public string cn_s_camera_name { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_enabledset")]
        public bool? cn_s_camera_enabledset { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_enabledmaxwin")]
        public bool? cn_s_camera_enabledmaxwin { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_posX")]
        public float? cn_s_camera_posX { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_posY")]
        public float? cn_s_camera_posY { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_posZ")]
        public float? cn_s_camera_posZ { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_angleX")]
        public float? cn_s_camera_angleX { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_angleY")]
        public float? cn_s_camera_angleY { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_angleZ")]
        public float? cn_s_camera_angleZ { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_camera_remarks")]
        public string cn_s_camera_remarks { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify")]
        public string cn_s_modify { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_modify_by")]
        public string cn_s_modify_by { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_modify")]
        public DateTime? cn_t_modify { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator")]
        public string cn_s_creator { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_s_creator_by")]
        public string cn_s_creator_by { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "cn_t_create")]
        public DateTime? cn_t_create { get; set; }
    }
}
