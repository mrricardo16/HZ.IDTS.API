using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Common.Helpers
{
    public class StockHelper
    {
        //private readonly static string default_factory  = "Z201";

        ///// <summary>
        ///// 工厂与仓库间隔
        ///// </summary>
        //private readonly static string interval_sign = "_";

        //public static string GetFactoryCode(string stockMixCode)
        //{
        //    return GetStockInfo(stockMixCode).factoryCode;
        //}

        //public static string GetStockCode(string stockMixCode)
        //{
        //    return GetStockInfo(stockMixCode).stockCode;
        //}

        //public static string GetMixStockCode(string factoryCode, string stockCode)
        //{
        //    if (default_factory.Equals(factoryCode))
        //        return stockCode;

        //    return factoryCode + interval_sign + stockCode;
        //}

        //public static FactoryStockBean GetStockInfo(string stockMixCode)
        //{
        //    var bean = new FactoryStockBean();
        //    var mixCode = stockMixCode.Split(interval_sign);

        //    if(mixCode.Length == 1)
        //    {
        //        bean.stockCode = mixCode[0];
        //    } else if (mixCode.Length > 1)
        //    {
        //        bean.factoryCode = mixCode[0];
        //        bean.stockCode = mixCode[1];
        //    }
                
        //    return bean;
        //}

        //public class FactoryStockBean
        //{
        //    public FactoryStockBean()
        //    {
        //        factoryCode = default_factory;
        //    }

        //    public string factoryCode { get; set; }

        //    public string stockCode { get; set; }
        //}
    }
}
