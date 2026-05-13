using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class InOutStockCategoryViewModel
    {
        public InOutStockCategoryModel bussCategory = new InOutStockCategoryModel();
    }

    public class InOutStockCategoryModel
    {
        /// <summary>
        /// 入库总数量
        /// </summary>
        public int inStockNum { get; set; }

        /// <summary>
        /// 入库分类业务统计
        /// </summary>

        public List<BussStockCategory> inStockCategory = new List<BussStockCategory>();

        /// <summary>
        /// 出库总数量
        /// </summary>
        public int outStockNum { get; set; }


        /// <summary>
        /// 出库分类业务统计
        /// </summary>

        public List<BussStockCategory> outStockCategory = new List<BussStockCategory>();
    }

    public  class BussStockCategory 
    {
        /// <summary>
        /// 业务类型
        /// </summary>
        public string bussType { get; set; }

        /// <summary>
        /// 业务数量
        /// </summary>
        public int bussNum { get; set; }
    }
}
