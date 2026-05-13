using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Basic
{
    [SugarTable("tn_wms_user")]
    public class tn_wms_user
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string cn_s_user_code { get; set; }

        public string cn_s_user_name { get; set; }

        public string cn_s_phone_id { get; set; }

        public DateTime cn_t_create { get; set; }

        public DateTime cn_t_modify { get; set; }
    }
}
