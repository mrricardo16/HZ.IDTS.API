using SqlSugar;
using System.Collections.Generic;

namespace HZ.IDTSCore.Model.Entity.Basic
{
    [SugarTable("tn_wms_bc", "标签码管理")]
    public class tn_wms_bc
    {
        public tn_wms_bc()
        {
        }

        /// <summary>
        /// 条码
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string cn_s_bc { get; set; }

        /// <summary>
        /// 父项条码
        /// </summary>
        public string cn_s_pbc { get; set; }

        /// <summary>
        /// 包装单位
        /// </summary>
        public string cn_s_pack_unit { get; set; }

        /// <summary>
        /// 物料编号
        /// </summary>
        public string cn_s_item_code { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string cn_s_item_name { get; set; }

        /// <summary>
        /// 规格型号
        /// </summary>
        public string cn_s_model { get; set; }

        public string cn_s_item_state { get; set; }

        /// <summary>
        /// 供应商编码
        /// </summary>
        public string cn_s_supplier_code { get; set; }


        /// <summary>
        /// 供应商名称
        /// </summary>
        public string cn_s_supplier_name { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string cn_s_unit { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal cn_f_qty { get; set; }
        /// <summary>
        /// 初始化原数量
        /// </summary>
        public decimal cn_f_init_qty { get; set; }

        /// <summary>
        /// sn号
        /// </summary>
        public string cn_s_sn { get; set; }

        /// <summary>
        /// 是否已拆包
        /// </summary>
        public bool cn_b_unpack { get; set; }

        /// <summary>
        /// 是否已出包
        /// </summary>
        public bool cn_b_outpack { get; set; }

        /// <summary>
        /// 生产批次
        /// </summary>
        public string cn_s_ctl_lot_code { get; set; }

        /// <summary>
        /// 根条码
        /// </summary>
        public string cn_s_root_bc { get; set; }

        /// <summary>
        /// 入库批次
        /// </summary>
        public string cn_s_in_lot_code { get; set; }

        /// <summary>
        /// 托盘号
        /// </summary>
        public string cn_s_tray_code { get; set; }
        /// <summary>
        /// 入库单行guid
        /// </summary>
        public string cn_s_inve_row_guid { get; set; }

        /// <summary>
        /// 是否检废
        /// </summary>
        public bool cn_b_fail { get; set; }

        public List<tn_wms_bc> children = new List<tn_wms_bc>();

        [SugarColumn(IsIgnore = true)]
        public bool hasChildren { get; set; }
        /// <summary>
        /// 生产批次
        /// </summary>
        public string cn_s_product_batch { get; set; }



        [SugarColumn(IsIgnore = true)]
        public tn_wms_bc parentBc { get; set; }


        [SugarColumn(IsIgnore = true)]
        public decimal cn_f_pick_qty { get; set; }
    }
}
