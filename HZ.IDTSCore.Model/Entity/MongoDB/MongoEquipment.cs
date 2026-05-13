using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 设备档案Mongodb缓存数据
    /// </summary>
    public class MongoEquipment
    {
        public ObjectId _id { get; set; }

        public string cn_guid { get; set; }

        public string cn_s_equi_parttype { get; set; }

        public string cn_s_equi_no { get; set; }

        public string cn_s_equi_name { get; set; }

        public string cn_s_equi_type { get; set; }

        public string cn_s_equi_model { get; set; }

        /// <summary>
        /// 这里存设备的实时状态
        /// </summary>
        public string cn_s_equi_status { get; set; }

        public DateTime? cn_s_equi_buydate { get; set; }
  
        public DateTime? cn_s_equi_qadate { get; set; }

        public DateTime? cn_s_equi_firstdate { get; set; }

        public int? cn_s_equi_defentperiod { get; set; }

        public string cn_s_equi_dept { get; set; }

        public string cn_s_equi_dutyman { get; set; }

        public string cn_s_equi_dutyphone { get; set; }

        public string cn_s_equi_contractno { get; set; }

        public string cn_s_equi_beltline { get; set; }

        public decimal? cn_s_equi_xpos { get; set; }

        public decimal? cn_s_equi_ypos { get; set; }

        public decimal? cn_s_equi_zpos { get; set; }

        public string cn_s_equi_remarks { get; set; }

        public string cn_s_modify { get; set; }

        public string cn_s_modify_by { get; set; }

        public DateTime? cn_t_modify { get; set; }

        public string cn_s_creator { get; set; }

        public string cn_s_creator_by { get; set; }

        public DateTime? cn_t_create { get; set; }
    }
}
