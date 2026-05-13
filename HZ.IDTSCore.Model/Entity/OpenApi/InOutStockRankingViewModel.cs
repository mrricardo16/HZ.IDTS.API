using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class InOutStockRankingViewModel
    {
        public List<InStockModel> inStock = new List<InStockModel>();

        public List<OutStockModel> outStock = new List<OutStockModel>();
    }

    public class InStockModel
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// 入库数量
        /// </summary>
        public string itemNum { get; set; }
    }

    public class OutStockModel
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string itemCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// 出库数量
        /// </summary>
        public string itemNum { get; set; }
    }
}
