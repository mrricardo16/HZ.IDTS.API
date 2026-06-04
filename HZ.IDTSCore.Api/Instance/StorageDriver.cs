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


        #region PublishStocksInfo V2 -20260507 优化性能，减少锁粒度和重复查询
        private readonly object _goodsLock = new object();
        private readonly Dictionary<string, StockViewModel> _goodsMap = new Dictionary<string, StockViewModel>();
        private static string GetKey(StockViewModel stock)
        {
            return $"{stock.stockCode}|{stock.areaCode}|{stock.locationCode}";
        }

        public bool PublishStocksInfoV2(List<StockViewModel> stocks)
        {
            if (stocks == null || stocks.Count == 0) return true;

            lock (_goodsLock)
            {
                foreach (var stock in stocks)
                {
                    var key = GetKey(stock);
                    if (_goodsMap.TryGetValue(key, out var cacheStock))
                    {
                        cacheStock.storageState = stock.storageState;
                        cacheStock.state = stock.state;
                        cacheStock.itemRow = stock.itemRow;
                        cacheStock.rackInfo = stock.rackInfo; // 如果前端需要料架/料箱变化，这里也应同步
                    }
                    else
                    {
                        _goodsMap[key] = stock;
                    }
                }

                GoodsList = _goodsMap.Values.ToList();
            }

            return true;
        }

        #endregion
    }
}
