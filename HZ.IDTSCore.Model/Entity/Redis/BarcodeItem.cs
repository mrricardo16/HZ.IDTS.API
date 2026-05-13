using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Redis
{
    public class BarcodeItem
    {
        public string cn_s_item_code { get; set; }
        public string cn_s_item_name { get; set; }
        public string cn_s_model { get; set; }
        public string cn_s_unit { get; set; }
        public decimal cn_f_qty { get; set; }
        public DateTime? cn_t_production { get; set; }
        public DateTime? cn_t_in_storage_date { get; set; }
        public string cn_s_production_batch { get; set; }
        public string cn_s_other_lotcode { get; set; }
    }
}
