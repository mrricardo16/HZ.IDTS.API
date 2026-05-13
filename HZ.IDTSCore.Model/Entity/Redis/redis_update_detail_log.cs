using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.Redis
{
    /// <summary>
    /// 作为队列缓存在redis中的修改记录
    /// </summary>
    public class redis_update_detail_log
    {
        public string cn_s_op_no { get; set; }
        public string cn_s_order_type { get; set; }
        public string cn_s_update_id { get; set; }
        public object cn_s_old_obj { get; set; }
        public object cn_s_new_obj { get; set; }

        public string cn_s_executor { get; set; }
        public DateTime cn_t_execute { get; set; }
        public string cn_s_active_type { get; set; }

        public string orgCode { get; set; }
    }
}