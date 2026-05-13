using HZ.IDTSCore.Model.Entity.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class StorageDriver
    {
        private static readonly StorageDriver instance = new StorageDriver();

        private StorageDriver() { }

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static StorageDriver Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 货位信息内存
        /// </summary>
        public List<StockViewModel> GoodsList = new List<StockViewModel>();

        /// <summary>
        /// 刷新信息
        /// </summary>
        /// <param name="stocks"></param>
        /// <returns></returns>
        public bool PublishStocksInfo(List<StockViewModel> stocks)
        {
            bool result = false;
            try
            {
                foreach (var stock in stocks)
                {
                    var cacheStock = GoodsList.FirstOrDefault(p => p.stockCode == stock.stockCode && p.areaCode== stock.areaCode && p.locationCode== stock.locationCode);
                    if (cacheStock == null) GoodsList.Add(stock);
                    else
                    {
                        cacheStock.storageState = stock.storageState;
                        cacheStock.state = stock.state;
                        cacheStock.itemRow = stock.itemRow;
                    }
                }
                result = true;
            }
            catch { }
            return result;
        }
    }
}
