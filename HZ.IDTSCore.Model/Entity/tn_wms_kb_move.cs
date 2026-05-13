using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity
{
    public class tn_wms_kb_move
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string cn_s_item_code { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string cn_s_item_name { get; set; }

        /// <summary>
        /// 生产批次
        /// </summary>
        public string cn_s_product_batch { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal cn_f_qty { get; set; }

        /// <summary>
        /// 入库批次
        /// </summary>
        public string cn_s_question { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string cn_s_type { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string cn_s_creator { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string cn_s_creator_by { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime cn_t_create { get; set; }

        /// <summary>
        /// 凭证号
        /// </summary>
        public string cn_s_voucher { get; set; }

    }
}
