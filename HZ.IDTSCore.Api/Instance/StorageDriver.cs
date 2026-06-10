using HZ.IDTSCore.Model.Entity.OpenApi;
using HZ.CommonUtil.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class StorageDriver
    {
        private const int SlowPublishStocksInfoMilliseconds = 1000;
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
            var stopwatch = Stopwatch.StartNew();
            var stockCount = stocks == null ? 0 : stocks.Count;
            try
            {
                if (stocks == null)
                {
                    // 入参为空时原逻辑会进入异常并返回 false；这里提前记录，避免空引用异常被误认为业务处理异常。
                    LogHelper.Warn("PublishStocksInfo刷新货位信息失败，入参为空，耗时：" + stopwatch.ElapsedMilliseconds + "ms");
                    return false;
                }

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
            catch (Exception ex)
            {
                // 原逻辑遇到异常会直接吞掉，导致货位内存没有刷新却无法定位；这里记录货位数量和耗时。
                LogHelper.Error("PublishStocksInfo刷新货位信息失败，货位数量：" + stockCount + "，耗时：" + stopwatch.ElapsedMilliseconds + "ms，异常原因：" + ex.Message, ex);
            }
            finally
            {
                stopwatch.Stop();
                if (result && stopwatch.ElapsedMilliseconds > SlowPublishStocksInfoMilliseconds)
                {
                    LogHelper.Warn("PublishStocksInfo刷新货位信息耗时较长，货位数量：" + stockCount + "，耗时：" + stopwatch.ElapsedMilliseconds + "ms");
                }
            }
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
