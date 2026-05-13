using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Common.Helpers
{
    public static class OpConvert
    {
        public static string NextOp(string opType, string orderType)
        {
            if (orderType == "入库单")
            {
                switch (opType)
                {
                    case "采购订单": return "采购入库";
                    case "生产入库": return "生产入库";
                    case "采购到货": return "采购入库";
                    default: return opType;
                }
            }
            else if (orderType == "出库单")
            {
                switch (opType)
                {
                    case "生产领料": return "生产领料";
                    case "销售订单": return "销售出库";
                    default: return opType;
                }
            }
            else if (orderType == "到货单")
            {
                switch (opType)
                {
                    case "采购订单": return "采购到货";
                    default: return opType;
                }
            }
            return "";
        }
    }
}
