using HZ.IDTSCore.Model.Entity.OpenApi;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api.Instance
{
    public class LocationRealMonitorDriver
    {
        private static readonly LocationRealMonitorDriver instance = new LocationRealMonitorDriver();

        private LocationRealMonitorDriver() { }

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static LocationRealMonitorDriver Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 货位同步方法(提供给API和Redis订阅的通用方法)
        /// 默认策略：每1000个货位推送一批，每批之间等待10ms。
        /// 适用场景：API接口直接推送大批量货位，例如4万货位初始化，避免前端/WebSocket瞬间收到超大包。
        /// </summary>
        /// <param name="model">货位实时数据。</param>
        public async Task LocationRealMonitorProc(LocationRealMonitorViewModel model)
        {
            await LocationRealMonitorProc(model, 1000, 10);
        }

        /// <summary>
        /// 货位同步方法(提供给API和Redis订阅的通用方法)
        /// 处理内容：先通过WebSocket分批推送给前端，再刷新StorageDriver中的货位内存缓存。
        /// </summary>
        /// <param name="model">货位数据</param>
        /// <param name="batchSize">WebSocket每批发送的货位数量</param>
        /// <param name="intervalMs">每批WebSocket发送间隔，0表示不等待</param>
        public async Task LocationRealMonitorProc(LocationRealMonitorViewModel model, int batchSize, int intervalMs)
        {
            // 空数据直接返回，兼容API和Redis订阅两种入口，避免空引用异常。
            if (model?.stock == null || model.stock.Count == 0)
            {
                return;
            }

            // 防御性处理，避免配置传入异常值导致死循环或Task.Delay异常。
            batchSize = batchSize <= 0 ? 1000 : batchSize;
            intervalMs = intervalMs < 0 ? 0 : intervalMs;

            int total = model.stock.Count;
            for (int i = 0; i < total; i += batchSize)
            {
                // i为当前批次起始下标，take为本批实际数量；最后一批可能不足batchSize。
                int take = Math.Min(batchSize, total - i);

                // GetRange只取当前批次，避免一次WebSocket消息携带过多货位造成前端解析压力。
                LocationRealMonitorViewModel batchModel = new LocationRealMonitorViewModel
                {
                    stock = model.stock.GetRange(i, take)
                };

                // 每批单独序列化，前端仍按原LocationRealMonitorViewModel结构接收，不改变原消息格式。
                string sendJSONString = JsonConvert.SerializeObject(batchModel);
                await WebSocketServer.SessionInstance.Instance.PLCSendAllV2(sendJSONString);

                // Redis订阅聚合场景会传入0，追求1s内快速推送；API大包场景默认10ms，给前端留出处理间隔。
                if (intervalMs > 0 && i + take < total)
                {
                    await Task.Delay(intervalMs);
                }
            }

            // WebSocket推送完成后刷新服务端内存货位状态，保证后续查询/展示拿到最新数据。
            if (model.stock.Count > 0)
            {
                StorageDriver.Instance.PublishStocksInfoV2(model.stock);
            }
        }
    }
}
