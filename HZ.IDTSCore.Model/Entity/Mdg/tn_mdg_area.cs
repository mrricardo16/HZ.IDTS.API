using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model
{
    public class MdgAreaResult {
        public List<tn_mdg_area> rows = new List<tn_mdg_area>();
        public int total = 0;
    }
    public class tn_mdg_area
    {
        public string CN_S_STOCK_CODE { get; set; }
        public string CN_S_AREA_CODE { get; set; }
        public string CN_S_AREA_NAME { get; set; }
        public string CN_S_CREATOR { get; set; }
        public string CN_S_CREATOR_BY { get; set; }
        public DateTime? CN_T_CREATE { get; set; }
        public string CN_S_MODIFY { get; set; }
        public string CN_S_MODIFY_BY { get; set; }
        public DateTime? CN_T_MODIFY { get; set; }
        public int CN_N_TYPE { get; set; }
        public int CN_N_ORDER { get; set; }
        public string CN_S_STRUCTURE { get; set; }


        public string stock_code { get; set; }
        public string area_code { get; set; }
        public string area_name { get; set; }
        public string area_type { get; set; }
        public int order { get; set; }
        public string area_struct { get; set; }
        public string area_class { get; set; }
        public string is_inventory { get; set; }
        public string is_auto { get; set; }
        public string control_leve { get; set; }
        public string control_qty { get; set; }
        public string codedisk_model { get; set; }
        public string is_tray { get; set; }
        public string is_checktray { get; set; }
        public string is_merge { get; set; }
        public string is_outarea { get; set; }
        public string is_inout_model { get; set; }
        public string inout_model { get; set; }
    }
}
