using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HZ.IDTSCore.Model
{
    [SugarTable("tn_wms_bill_exec")]
    public class tn_wms_bill_exec
    {
        
        public string cn_s_guid { get; set; }
        
        public string cn_s_order_type { get; set; }
        
        public string cn_s_op_no { get; set; }
        
        public string cn_s_executor { get; set; }
        
        public string cn_s_executor_by { get; set; }
        
        public string cn_s_note { get; set; }
        
        public DateTime cn_t_execute { get; set; }
        
        public string cn_s_operation_type { get; set; }
        
        public string cn_s_executor_content { get; set; }
    }
}
