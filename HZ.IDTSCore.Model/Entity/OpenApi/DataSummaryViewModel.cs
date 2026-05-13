using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class DataSummaryViewModel
    {
        public DataSummaryModel dataSummary = new DataSummaryModel();
    }

    public class DataSummaryModel
    {
        /// <summary>
        /// 货位总数
        /// </summary>
        public string goodsTotal { get; set; }

        /// <summary>
        /// 满货位数
        /// </summary>
        public string fullGoodsNum { get; set; }

        /// <summary>
        /// 空货位数
        /// </summary>
        public string emptyGoodsNum { get; set; }

        /// <summary>
        /// 货位利用率
        /// </summary>
        public string goodsUsage { get; set; }

        /// <summary>
        /// 货位故障
        /// </summary>
        public string faultNum { get; set; }

        /// <summary>
        /// 预入库
        /// </summary>
        public string dueInStockNum { get; set; }

        /// <summary>
        /// 预出库
        /// </summary>
        public string dueOutStockNum { get; set; }

        /// <summary>
        /// 正常满
        /// </summary>
        public string normalFullNum { get; set; }

        /// <summary>
        /// 正常空
        /// </summary>
        public string normalEmptyNum { get; set; }

        /// <summary>
        /// 当日入库业务
        /// </summary>
        public string currDaysInStockBussNum { get; set; }

        /// <summary>
        /// 当日入库任务
        /// </summary>
        public string currDaysInStockTaskNum { get; set; }

        /// <summary>
        /// 库存总量
        /// </summary>
        public string stockTotalNum { get; set; }

        /// <summary>
        /// 当日出库业务
        /// </summary>
        public string currDaysOutStockBussNum { get; set; }

        /// <summary>
        /// 当日出库任务
        /// </summary>
        public string currDaysOutStockTaskNum { get; set; }

        /// <summary>
        /// 库存总托数
        /// </summary>
        public string stockTotalTrayNum { get; set; }
    }
}
