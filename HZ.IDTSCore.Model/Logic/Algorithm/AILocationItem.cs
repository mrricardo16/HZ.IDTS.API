using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Logic.Algorithm
{
    public class AILocationItem:tn_wms_location
    {
        public string cn_s_tray_code { get; set; }
        public int cn_n_depth { get; set; }
        
        public string cn_s_item_code { get; set; }
        public string cn_s_owner { get; set; }
        public string cn_s_ctl_lot_code { get; set; }
        public string cn_s_item_state { get; set; }
    }
}
