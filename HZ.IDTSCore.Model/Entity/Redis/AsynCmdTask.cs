using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Redis
{
    public class AsynCmdTask<T>
    {
        /// <summary>
        /// 指令类型，
        /// </summary>
        public CmdType cmdType { get; set; }
        public bool isQueue { get; set; }
        public T obj { get; set; }
        public DateTime cn_t_time { get; set; }
        public string cn_s_token { get; set; }
        public string org { get; set; }
    }

    public enum CmdType
    {
        TaskComplete,
        InCancel,
        AutoCpfr
    }

    public class TaskCompleteCmd
    {
        public string cn_s_stock_code { get; set; }
        public string cn_s_task_no { get; set; }
    }
    public class InCancelCmd
    {
        public string cn_s_stock_code { get; set; }
        /// <summary>
        /// 指令系统(wms、mes、other)
        /// </summary>
        public string handleSys { get; set; }
        public string cn_s_op_no { get; set; }
        public string note { get; set; }
        //public List<tn_wms_in_inventory_dtl> cancelQty{ get; set; }
    }
    public class AutoCpfrCmd
    {
        public string cn_s_stock_code { get; set; }
        public string cn_s_op_no { get; set; }
    }
}
