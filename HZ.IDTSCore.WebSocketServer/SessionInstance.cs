using HZ.CommonUtil.Helpers;
using SuperSocket;
using SuperSocket.WebSocket.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTSCore.WebSocketServer
{
    public class SessionInstance
    {
        private static readonly SessionInstance instance = new SessionInstance();

        public List<WebSession> WCSSessionList = new List<WebSession>();
        private static readonly object lockObject = new object();

        #region WebSocket发送优化 - 2026-06-09
        // 优化内容：
        // 1. 发送前只在锁内复制会话快照，不在全局锁内执行网络IO。
        // 2. 每个Session独立加发送锁，避免多个接口同时向同一个客户端并发SendAsync。
        // 3. 广播发送使用有限并发，避免慢客户端拖慢全部客户端，也避免一次性创建过多发送压力。
        // 4. 发送失败或检测到断开连接时自动清理失效Session，降低长期运行后的无效遍历成本。
        // 5. 日志只记录摘要和异常，不再在群发过程中逐个输出session信息。
        // 说明：按照本次要求暂不处理“WCSSessionList public List”问题，避免影响外部现有访问方式。
        private const int MaxConcurrentBroadcastSends = 16;
        private const int SlowBroadcastLogMilliseconds = 1000;
        private const int SlowSessionSendMilliseconds = 1000;
        private const int ConsecutiveSlowSessionLogThreshold = 3;
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> SessionSendLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        #region WebSocket慢客户端诊断日志 - 2026-06-09
        // 优化内容：
        // 1. 正常发送不写日志，避免高频推送被日志拖慢。
        // 2. 单客户端发送超过SlowSessionSendMilliseconds才计数。
        // 3. 只有连续慢发送达到阈值才写摘要日志，帮助定位慢客户端或客户端处理积压。
        private static readonly ConcurrentDictionary<string, int> SlowSessionSendCounts = new ConcurrentDictionary<string, int>();
        private static readonly ConcurrentDictionary<string, long> LastSlowSessionElapsedMilliseconds = new ConcurrentDictionary<string, long>();
        #endregion
        #endregion

        /// <summary>
        /// 获取单实例
        /// </summary>
        public static SessionInstance Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public SessionInstance()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {

        }

        /// <summary>
        /// 增加会话到内存队列
        /// </summary>
        /// <param name="session"></param>
        public void AddSession(WebSession session)
        {
            string endpoint = GetRemoteEndpointText(session);
            try
            {
                lock (lockObject)
                {
                    if (!WCSSessionList.Exists(p => p.SessionID == session.SessionID))
                    {
                        WCSSessionList.Add(session);
                    }
                }

                // 2026-06-09优化：日志移出全局锁，避免日志IO阻塞其他连接增删或推送。
                LogHelper.Info("客户端已上线，相关信息：sessionIp：" + endpoint);
            }
            catch (Exception exception)
            {
                LogHelper.Info("AddSession异常，详细情况为：" + exception.Message);
            }
        }


        /// <summary>
        /// 给指定客户端发送消息(只匹配IP地址)
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSend(string ip, int port, string msg)
        {
            try
            {
                // 2026-06-09优化：使用快照发送，真正await发送结果，并修正port参数未参与匹配的问题。
                List<WebSession> sessionList = GetConnectedSessionsSnapshot(p => IsMatchRemoteEndPoint(p, ip, port));
                await SendSessionsAsync(sessionList, msg, "PLCSend(ip, port, msg)");
            }
            catch (Exception exception)
            {
                LogHelper.Info("PLCSend(ip, port, msg)异常，详细情况为：" + exception.Message);
            }
        }


        /// <summary>
        /// 给指定客户端发送消息(匹配IP地址)
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSend(string ip, string msg)
        {
            try
            {
                // 2026-06-09优化：使用快照发送，避免锁内网络IO，兼容纯IP和EndPoint字符串两种匹配方式。
                List<WebSession> sessionList = GetConnectedSessionsSnapshot(p => IsMatchRemoteEndPoint(p, ip, null));
                await SendSessionsAsync(sessionList, msg, "PLCSend(ip, msg)");
            }
            catch (Exception exception)
            {
                LogHelper.Info("PLCSend(ip, msg)异常，详细情况为：" + exception.Message);
            }
        }

        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSendAll(string msg)
        {
            try
            {
                // 2026-06-09优化：老接口保留名称，但内部切换到优化后的快照+有限并发发送逻辑。
                await SendSessionsAsync(GetConnectedSessionsSnapshot(), msg, "PLCSendAll");
            }
            catch (Exception exception)
            {
                LogHelper.Info("PLCSendAll异常，详细情况为：" + exception.Message);
            }
        }

        /// <summary>
        /// 广播数据 -20260507 优化性能，减少锁粒度和重复查询
        /// 2026-06-09继续优化：增加单Session发送锁、有限并发、失效Session清理、慢发送摘要日志。
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSendAllV2(string msg)
        {
            try
            {
                await SendSessionsAsync(GetConnectedSessionsSnapshot(), msg, "PLCSendAllV2");
            }
            catch (Exception exception)
            {
                LogHelper.Info("PLCSendAllV2异常，详细情况为：" + exception.Message);
            }
        }



        /// <summary>
        /// 通过会话句柄ID找到客户端对象发送数据
        /// </summary>
        /// <param name="sessionID">会话句柄ID</param>
        /// <param name="msg">发送数据</param>
        /// <returns></returns>
        public async ValueTask PLCSendForSessionID(string sessionID, string msg)
        {
            try
            {
                WebSession session;
                lock (lockObject)
                {
                    session = WCSSessionList.Find(p => p.SessionID == sessionID);
                }

                // 2026-06-09优化：不在全局锁内发送，真正await发送结果。
                if (session != null && session.State == SessionState.Connected)
                {
                    await SendToSessionAsync(session, msg, "PLCSendForSessionID");
                }
            }
            catch (Exception exception)
            {
                LogHelper.Info("PLCSendForSessionID异常，详细情况为：" + exception.Message);
            }
        }

        /// <summary>
        /// 给指定Session发送消息。
        /// 2026-06-10优化：统一场景测试回复、连接提示等点对点发送入口，避免绕过单Session发送锁。
        /// </summary>
        public async ValueTask SendForSessionV2(WebSocketSession session, string msg, string operationName = "SendForSessionV2")
        {
            try
            {
                await SendToSessionAsync(session, msg, operationName);
            }
            catch (Exception exception)
            {
                LogHelper.Info(operationName + "点对点发送异常，详细情况为：" + exception.Message);
            }
        }

        /// <summary>
        /// 删除一个会话
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        public void SessionRemove(WebSession session)
        {
            string endpoint = GetRemoteEndpointText(session);
            try
            {
                RemoveSessionInternal(session);

                // 2026-06-09优化：日志移出全局锁，避免日志IO阻塞其他连接增删或推送。
                LogHelper.Info("客户端已下线，相关信息：sessionIp：" + endpoint);
            }
            catch (Exception exception)
            {
                LogHelper.Info("SessionRemove异常，详细情况为：" + exception.Message);
            }
        }

        #region WebSocket发送优化方法 - 2026-06-09
        /// <summary>
        /// 只在锁内复制当前可发送会话，避免锁内执行网络IO。
        /// </summary>
        private List<WebSession> GetConnectedSessionsSnapshot(Func<WebSession, bool> predicate = null)
        {
            lock (lockObject)
            {
                RemoveDisconnectedSessionsNoLock();
                return WCSSessionList
                    .Where(p => p.State == SessionState.Connected && (predicate == null || predicate(p)))
                    .ToList();
            }
        }

        /// <summary>
        /// 广播发送使用有限并发；单个Session发送仍由SendToSessionAsync保证串行。
        /// </summary>
        private async ValueTask SendSessionsAsync(List<WebSession> sessions, string msg, string operationName)
        {
            if (sessions == null || sessions.Count == 0)
            {
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            int successCount = 0;
            using (var throttle = new SemaphoreSlim(MaxConcurrentBroadcastSends))
            {
                var tasks = sessions.Select(async session =>
                {
                    await throttle.WaitAsync();
                    try
                    {
                        if (await SendToSessionAsync(session, msg, operationName))
                        {
                            Interlocked.Increment(ref successCount);
                        }
                    }
                    finally
                    {
                        throttle.Release();
                    }
                }).ToArray();

                await Task.WhenAll(tasks);
            }

            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds >= SlowBroadcastLogMilliseconds)
            {
                LogHelper.Info(operationName + "发送耗时：" + stopwatch.ElapsedMilliseconds + "ms，目标客户端数：" + sessions.Count + "，成功数：" + successCount + "，失败数：" + (sessions.Count - successCount) + "，消息长度：" + (msg == null ? 0 : msg.Length));
            }
        }

        /// <summary>
        /// 同一个Session的发送串行化，避免多个接口同时向同一WebSocket连接并发SendAsync。
        /// </summary>
        private async Task<bool> SendToSessionAsync(WebSocketSession session, string msg, string operationName)
        {
            if (session == null)
            {
                return false;
            }

            if (session.State != SessionState.Connected)
            {
                RemoveSessionInternal(session);
                return false;
            }

            var sessionLock = SessionSendLocks.GetOrAdd(session.SessionID, _ => new SemaphoreSlim(1, 1));
            await sessionLock.WaitAsync();
            try
            {
                if (session.State != SessionState.Connected)
                {
                    RemoveSessionInternal(session);
                    return false;
                }

                long sendStartTimestamp = Stopwatch.GetTimestamp();
                await session.SendAsync(msg);
                long elapsedMilliseconds = GetElapsedMilliseconds(sendStartTimestamp);
                RecordSessionSendResult(session, operationName, msg, elapsedMilliseconds);
                return true;
            }
            catch (Exception exception)
            {
                RemoveSessionInternal(session);
                LogHelper.Info(operationName + "发送到客户端异常，sessionId：" + session.SessionID + "，sessionIp：" + GetRemoteEndpointText(session) + "，异常：" + exception.Message);
                return false;
            }
            finally
            {
                sessionLock.Release();
            }
        }

        /// <summary>
        /// 清理失效连接，降低长期运行后广播遍历无效Session的成本。
        /// </summary>
        private void RemoveDisconnectedSessionsNoLock()
        {
            var disconnectedSessions = WCSSessionList
                .Where(p => p == null || p.State != SessionState.Connected)
                .ToList();

            foreach (var session in disconnectedSessions)
            {
                WCSSessionList.Remove(session);
                if (session != null)
                {
                    SessionSendLocks.TryRemove(session.SessionID, out _);
                    SlowSessionSendCounts.TryRemove(session.SessionID, out _);
                    LastSlowSessionElapsedMilliseconds.TryRemove(session.SessionID, out _);
                }
            }
        }

        private void RemoveSessionInternal(WebSocketSession session)
        {
            if (session == null)
            {
                return;
            }

            lock (lockObject)
            {
                // 2026-06-10优化：兼容 WebSession 和基类 WebSocketSession，
                // 所有点对点发送失败后都能按 SessionID 清理，避免失效连接残留。
                var webSession = session as WebSession;
                if (webSession != null)
                {
                    WCSSessionList.Remove(webSession);
                }
                else
                {
                    WCSSessionList.RemoveAll(p => p != null && p.SessionID == session.SessionID);
                }

                SessionSendLocks.TryRemove(session.SessionID, out _);
                SlowSessionSendCounts.TryRemove(session.SessionID, out _);
                LastSlowSessionElapsedMilliseconds.TryRemove(session.SessionID, out _);
            }
        }

        /// <summary>
        /// 记录单客户端连续慢发送。正常发送不写日志，避免高频推送被日志拖慢。
        /// </summary>
        private static void RecordSessionSendResult(WebSocketSession session, string operationName, string msg, long elapsedMilliseconds)
        {
            if (session == null || string.IsNullOrEmpty(session.SessionID))
            {
                return;
            }

            if (elapsedMilliseconds < SlowSessionSendMilliseconds)
            {
                SlowSessionSendCounts.TryRemove(session.SessionID, out _);
                LastSlowSessionElapsedMilliseconds.TryRemove(session.SessionID, out _);
                return;
            }

            LastSlowSessionElapsedMilliseconds[session.SessionID] = elapsedMilliseconds;
            int slowCount = SlowSessionSendCounts.AddOrUpdate(session.SessionID, 1, (_, oldValue) => oldValue + 1);

            if (slowCount == ConsecutiveSlowSessionLogThreshold || slowCount % ConsecutiveSlowSessionLogThreshold == 0)
            {
                LogHelper.Info(operationName + "单客户端连续慢发送，sessionId：" + session.SessionID
                    + "，sessionIp：" + GetRemoteEndpointText(session)
                    + "，连续慢发送次数：" + slowCount
                    + "，本次耗时：" + elapsedMilliseconds + "ms"
                    + "，最近慢发送耗时：" + LastSlowSessionElapsedMilliseconds[session.SessionID] + "ms"
                    + "，消息长度：" + (msg == null ? 0 : msg.Length));
            }
        }

        private static long GetElapsedMilliseconds(long startTimestamp)
        {
            return (Stopwatch.GetTimestamp() - startTimestamp) * 1000 / Stopwatch.Frequency;
        }

        private static bool IsMatchRemoteEndPoint(WebSession session, string ip, int? port)
        {
            if (session == null || session.RemoteEndPoint == null)
            {
                return false;
            }

            string remoteText = session.RemoteEndPoint.ToString();
            var remoteIpEndPoint = session.RemoteEndPoint as IPEndPoint;
            if (remoteIpEndPoint == null)
            {
                return remoteText == ip;
            }

            if (port.HasValue)
            {
                return (remoteIpEndPoint.Address.ToString() == ip && remoteIpEndPoint.Port == port.Value) || remoteText == ip;
            }

            return remoteIpEndPoint.Address.ToString() == ip || remoteText == ip;
        }

        private static string GetRemoteEndpointText(WebSocketSession session)
        {
            if (session == null || session.RemoteEndPoint == null)
            {
                return string.Empty;
            }

            var remoteIpEndPoint = session.RemoteEndPoint as IPEndPoint;
            if (remoteIpEndPoint == null)
            {
                return session.RemoteEndPoint.ToString();
            }

            return remoteIpEndPoint.Address + ":" + remoteIpEndPoint.Port;
        }
        #endregion
    }
}