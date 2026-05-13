using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Sys
{
    [SugarTable("tn_wms_menu_collect")]
    public class tn_wms_menu_collect
    {
        
        public string cn_s_menu_url { get; set; }
        
        public string cn_s_user_code { get; set; }
    }
}
