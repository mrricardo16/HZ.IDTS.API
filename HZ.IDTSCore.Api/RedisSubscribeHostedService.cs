using CSRedis;
using Microsoft.Extensions.Configuration;
using HZ.IDTSCore.Api.Instance;
using HZ.IDTSCore.Model.Entity.OpenApi;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Api
{
    /// <summary>
    /// Redis订阅类
    /// </summary>
    public class RedisSubscribeHostedService : BackgroundService
    {
        private readonly ILogger<RedisSubscribeHostedService> _logger;
        private readonly IConfiguration _configuration;

        #region LocationRealMonitor订阅聚合参数
        /// <summary>
        /// 货位订阅消息聚合等待窗口。
        /// 作用：让同一瞬间进入的多条Redis消息先合并，再统一推送WebSocket，减少推送次数。
        /// 说明：窗口越小延迟越低，窗口越大合并效果越好；当前50ms用于兼顾实时性和1000条并发消息合并效果。
        /// </summary>
        private const int LocationRealMonitorBatchWindowMs = 50;

        /// <summary>
        /// 单次最多合并的Redis订阅消息数量。
        /// 作用：避免某一轮处理过多消息导致后台消费者长期占用，影响后续消息处理。
        /// 目标：现场要求1000条订阅消息同时进来时，尽量在1s内完成WebSocket推送。
        /// </summary>
        private const int LocationRealMonitorMaxMessageCount = 1000;

        /// <summary>
        /// WebSocket每批发送的货位数量。
        /// 说明：Redis侧已经先按消息数量聚合；这里再按货位数量分批，避免单个WebSocket包过大。
        /// </summary>
        private const int LocationRealMonitorWebSocketBatchSize = 1000;

        /// <summary>
        /// Redis订阅聚合后，WebSocket分批发送之间的等待时间。
        /// 说明：这里设置为0，是为了满足1000条订阅消息在1s内尽快推送出去。
        /// API接口默认仍可使用10ms间隔，避免一次性推送4万货位时前端压力过大。
        /// </summary>
        private const int LocationRealMonitorWebSocketIntervalMs = 0;

        /// <summary>
        /// LocationRealMonitor订阅消息队列。
        /// 设计意图：Redis回调线程只负责TryWrite快速入队，真正的JSON反序列化、数据合并、WebSocket推送都交给后台消费者处理。
        /// 好处：1000条订阅消息同时进入时，不会在Redis回调线程里同步等待1000次WebSocket发送。
        /// </summary>
        private readonly Channel<string> _locationRealMonitorQueue = Channel.CreateUnbounded<string>(
            new UnboundedChannelOptions
            {
                // 只有ProcessLocationRealMonitorQueueAsync一个后台消费者读取，减少Channel内部同步开销。
                SingleReader = true,

                // Redis订阅回调可能被多个线程触发，所以写入端不能声明为单写。
                SingleWriter = false,

                // 避免写入线程直接执行后续延续逻辑，保证Redis回调尽快返回。
                AllowSynchronousContinuations = false
            });
        #endregion

        public RedisSubscribeHostedService(
            ILogger<RedisSubscribeHostedService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Redis 订阅后台服务启动");

            #region 注册Redis订阅频道
            // 注意：RedisHelper.Subscribe只负责注册订阅回调。
            // LocationRealMonitor的耗时业务不在回调里执行，而是进入_locationRealMonitorQueue后由后台消费者统一处理。
            // RedisHelper.Subscribe在部分版本/场景下会长时间占用当前线程，所以放到独立后台任务中注册。
            // ExecuteAsync本身返回LocationRealMonitor消费者任务，保证队列消费者一定会启动。
            _ = Task.Run(() =>
            {
                RedisHelper.Subscribe(
                ("LocationRealMonitor", msg =>
                {
                    try
                    {
                        #region 货位订阅业务处理
                        if (string.IsNullOrWhiteSpace(msg.Body))
                        {
                            _logger.LogWarning("Redis LocationRealMonitor消息体为空，已忽略");
                            return;
                        }

                        // 订阅回调只负责快速入队，不在Redis回调线程里做反序列化和WebSocket发送。
                        // 1000条消息同时进入时，后台消费者会在短时间窗口内合并处理，避免1000次同步阻塞推送。
                        // TryWrite为非阻塞写入，正常情况下几乎只产生一次内存写入成本。
                        if (!_locationRealMonitorQueue.Writer.TryWrite(msg.Body))
                        {
                            _logger.LogWarning("Redis LocationRealMonitor消息入队失败，消息长度：{Length}", msg.Body.Length);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "处理 Redis LocationRealMonitor消息异常");
                    }
                }
            ),
                ("ConveyorRealCollect", msg =>
                {
                    try
                    {
                        _logger.LogInformation(
                            "收到 Redis 消息，ConveyorRealCollect频道：{Channel}，内容：{Body}",
                            msg.Channel,
                            msg.Body
                        );

                        //输送线订阅
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "处理 Redis ConveyorRealCollect消息异常");
                    }
                }
            ),
                ("RGVRealCollect", msg =>
                {
                    try
                    {
                        _logger.LogInformation(
                            "收到 Redis 消息，RGVRealCollect频道：{Channel}，内容：{Body}",
                            msg.Channel,
                            msg.Body
                        );

                        //RGV订阅
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "处理 Redis RGVRealCollect消息异常");
                    }
                }
            ),
                ("StackerRealCollect", msg =>
                {
                    try
                    {
                        _logger.LogInformation(
                            "收到 Redis 消息，StackerRealCollect频道：{Channel}，内容：{Body}",
                            msg.Channel,
                            msg.Body
                        );

                        //堆垛机订阅
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "处理 Redis StackerRealCollect消息异常");
                    }
                }
            )
            );
            }, stoppingToken);
            #endregion

            #region 启动LocationRealMonitor后台消费者
            // BackgroundService需要返回一个长期运行的Task。
            // 该Task负责持续读取队列、聚合消息、批量推送WebSocket，直到应用停止。
            return ProcessLocationRealMonitorQueueAsync(stoppingToken);
            #endregion
        }

        /// <summary>
        /// 后台消费LocationRealMonitor订阅消息。
        /// 核心目的：把短时间内并发进来的多条Redis消息合并成一批，减少WebSocket推送次数。
        /// </summary>
        /// <param name="stoppingToken">应用停止信号，用于优雅退出后台消费循环。</param>
        private async Task ProcessLocationRealMonitorQueueAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (await _locationRealMonitorQueue.Reader.WaitToReadAsync(stoppingToken))
                {
                    // 每一轮最多处理LocationRealMonitorMaxMessageCount条消息。
                    // 如果队列里还有更多消息，会在下一轮继续处理，避免单轮任务过重。
                    List<string> messageBodies = new List<string>(LocationRealMonitorMaxMessageCount);

                    #region 首次快速取出当前队列消息
                    // 先把当前已经排队的消息全部取出，减少后续Channel读取次数。
                    DrainLocationRealMonitorQueue(messageBodies);
                    if (messageBodies.Count == 0)
                    {
                        continue;
                    }
                    #endregion

                    #region 短窗口聚合并发消息
                    // 等待一个很短的窗口，让同一时间涌入的消息有机会合并到同一批。
                    // 50ms窗口下，即使加上反序列化和WebSocket发送，也更容易满足1s内推送1000条订阅消息的目标。
                    if (messageBodies.Count < LocationRealMonitorMaxMessageCount)
                    {
                        await Task.Delay(LocationRealMonitorBatchWindowMs, stoppingToken);

                        // 延迟窗口结束后再次读取，吸收窗口期内新到达的并发消息。
                        DrainLocationRealMonitorQueue(messageBodies);
                    }
                    #endregion

                    #region 批量处理货位消息
                    try
                    {
                        await FlushLocationRealMonitorMessagesAsync(messageBodies);
                    }
                    catch (Exception ex)
                    {
                        // 单批处理异常不能让整个后台消费者退出，否则后续Redis订阅消息将无法继续推送。
                        _logger.LogError(ex, "Redis LocationRealMonitor批量处理异常，消息数：{MessageCount}", messageBodies.Count);
                    }
                    #endregion
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Redis LocationRealMonitor订阅消费任务已停止");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis LocationRealMonitor订阅消费任务异常退出");
            }
        }

        /// <summary>
        /// 从队列中尽可能多地读取消息，但单批最多读取1000条，避免单次批处理过大影响后续消息。
        /// </summary>
        /// <param name="messageBodies">本轮待处理的Redis消息体集合。</param>
        private void DrainLocationRealMonitorQueue(List<string> messageBodies)
        {
            while (messageBodies.Count < LocationRealMonitorMaxMessageCount
                && _locationRealMonitorQueue.Reader.TryRead(out string body))
            {
                // 这里只保存原始JSON，不立即反序列化；反序列化统一放到Flush阶段，方便统计无效消息数量。
                messageBodies.Add(body);
            }
        }

        /// <summary>
        /// 批量反序列化Redis消息并复用货位处理逻辑统一推送WebSocket。
        /// </summary>
        /// <param name="messageBodies">一批Redis消息体，通常是50ms窗口内聚合得到的数据。</param>
        private async Task FlushLocationRealMonitorMessagesAsync(List<string> messageBodies)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // 聚合后的货位集合。多个Redis消息里的stock会合并到这里，最后只触发一次LocationRealMonitorProc。
            List<StockViewModel> stocks = new List<StockViewModel>();

            // 统计空消息、格式错误消息、没有stock数据的消息，便于现场排查上游推送质量。
            int invalidMessageCount = 0;

            #region 合并Redis消息中的货位数据
            foreach (string body in messageBodies)
            {
                try
                {
                    LocationRealMonitorViewModel? model = JsonConvert.DeserializeObject<LocationRealMonitorViewModel>(body);
                    if (model?.stock == null || model.stock.Count == 0)
                    {
                        invalidMessageCount++;
                        continue;
                    }

                    // 逐条过滤null货位，避免上游数组里出现null元素时影响整批推送。
                    foreach (StockViewModel stock in model.stock)
                    {
                        if (stock != null)
                        {
                            stocks.Add(stock);
                        }
                    }
                }
                catch (Exception ex)
                {
                    invalidMessageCount++;
                    _logger.LogWarning(ex, "Redis LocationRealMonitor消息反序列化失败，消息长度：{Length}", body?.Length ?? 0);
                }
            }
            #endregion

            if (stocks.Count == 0)
            {
                _logger.LogWarning(
                    "Redis LocationRealMonitor批量处理完成，但没有有效货位数据。消息数：{MessageCount}，无效消息数：{InvalidMessageCount}",
                    messageBodies.Count,
                    invalidMessageCount);
                return;
            }

            #region WebSocket批量推送和内存缓存刷新
            // Redis订阅已经做过50ms聚合，这里不再增加批次间隔，尽量保证1000条订阅消息在1s内推送出去。
            // LocationRealMonitorProc内部会继续按LocationRealMonitorWebSocketBatchSize拆分WebSocket包，并刷新StorageDriver内存货位缓存。
            LocationRealMonitorViewModel batchModel = new LocationRealMonitorViewModel
            {
                stock = stocks
            };
            await LocationRealMonitorDriver.Instance.LocationRealMonitorProc(
                batchModel,
                LocationRealMonitorWebSocketBatchSize,
                LocationRealMonitorWebSocketIntervalMs);
            #endregion

            stopwatch.Stop();
            _logger.LogInformation(
                "Redis LocationRealMonitor批量处理完成，消息数：{MessageCount}，无效消息数：{InvalidMessageCount}，货位数量：{StockCount}，耗时：{ElapsedMilliseconds}ms",
                messageBodies.Count,
                invalidMessageCount,
                stocks.Count,
                stopwatch.ElapsedMilliseconds);

            // 本地压测和现场排查使用：每批只记录一次Debug日志，避免恢复成每条订阅消息一条日志造成IO压力。
            _logger.LogDebug(
                "Redis LocationRealMonitor批量处理完成，消息数：{MessageCount}，无效消息数：{InvalidMessageCount}，货位数量：{StockCount}，耗时：{ElapsedMilliseconds}ms",
                messageBodies.Count,
                invalidMessageCount,
                stocks.Count,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
