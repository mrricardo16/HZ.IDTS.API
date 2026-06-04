using HZ.CommonUtil.Helpers;
using SuperSocket;
using SuperSocket.WebSocket.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HZ.IDTSCore.WebSocketServer
{
    public class SessionInstance
    {
        private static readonly SessionInstance instance = new SessionInstance();
        

        public List<WebSession> WCSSessionList = new List<WebSession>();
        private static readonly object lockObject = new object();

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
            lock(lockObject)
            {
                try
                {
                    LogHelper.Info("客户端已上线，相关信息：sessionIp：" + (session.RemoteEndPoint as IPEndPoint).Address + ":" + (session.RemoteEndPoint as IPEndPoint).Port);
                    if (!WCSSessionList.Exists(p => p.SessionID == session.SessionID))
                    {
                        WCSSessionList.Add(session);
                    }

                }
                catch (Exception exception)
                {
                    LogHelper.Info("AddSession异常，详细情况为：" + exception.Message);
                }
            } 
        }

        ///// <summary>
        ///// 增加会话到内存队列
        ///// </summary>
        ///// <param name="session"></param>
        //public void AddSession(WebSession session)
        //{
        //    object _lockobj = lockObject;
        //    bool _lockWasTaken = false;
        //    try
        //    {
        //        System.Threading.Monitor.Enter(_lockobj, ref _lockWasTaken);
        //        LogHelper.Info("客户端已上线，相关信息：sessionIp：" + (session.RemoteEndPoint as IPEndPoint).Address + ":" + (session.RemoteEndPoint as IPEndPoint).Port);
        //        if (!WCSSessionList.Exists(p => p.SessionID == session.SessionID))
        //        {
        //            WCSSessionList.Add(session);
        //        }

        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Info("AddSession异常，详细情况为：" + exception.Message);
        //    }
        //    finally
        //    {
        //        if (_lockWasTaken) System.Threading.Monitor.Exit(_lockobj);
        //    }
        //}

        /// <summary>
        /// 给指定客户端发送消息(只匹配IP地址)
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSend(string ip, int port, string msg)
        {
            lock(lockObject)
            {
                try
                {
                    List<WebSession> sessionList = new List<WebSession>();
                    sessionList = WCSSessionList.FindAll(p => p.RemoteEndPoint.ToString() == ip);
                    //对当前已连接的所有会话进行广播
                    foreach (var session in sessionList)
                    {
                        if (session.State == SessionState.Connected)
                        {
                            var data = Encoding.UTF8.GetBytes(msg);
                            session.SendAsync(data);
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.Info("PLCSend(ip, port, msg)异常，详细情况为：" + exception.Message);
                }
            }           
        }

        ///// <summary>
        ///// 给指定客户端发送消息(只匹配IP地址)
        ///// </summary>
        ///// <param name="ip"></param>
        ///// <param name="port"></param>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public async ValueTask PLCSend(string ip, int port, string msg)
        //{
        //    try
        //    {
        //        object _lockobj = lockObject;
        //        bool _lockWasTaken = false;
        //        List<WebSession> sessionList = new List<WebSession>();
        //        try
        //        {
        //            System.Threading.Monitor.Enter(_lockobj, ref _lockWasTaken);
        //            sessionList = WCSSessionList.FindAll(p => p.RemoteEndPoint.ToString() == ip);
        //        }
        //        catch (Exception exception)
        //        {
        //            throw exception;
        //        }
        //        finally
        //        {
        //            if (_lockWasTaken) System.Threading.Monitor.Exit(_lockobj);
        //        }
        //        //对当前已连接的所有会话进行广播
        //        foreach (var session in sessionList)
        //        {
        //            if (session.State == SessionState.Connected)
        //            {
        //                var data = Encoding.UTF8.GetBytes(msg);
        //                await session.SendAsync(data);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Info("PLCSend(ip, port, msg)异常，详细情况为：" + exception.Message);
        //    }
        //}

        /// <summary>
        /// 给指定客户端发送消息(匹配IP地址)
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSend(string ip, string msg)
        {
            lock(lockObject)
            {
                try
                {
                    List<WebSession> sessionList = new List<WebSession>();
                    sessionList = WCSSessionList.FindAll(p => p.RemoteEndPoint.ToString() == ip);
                    //对当前已连接的所有会话进行广播
                    foreach (var session in sessionList)
                    {
                        if (session.State == SessionState.Connected)
                        {
                            var data = Encoding.UTF8.GetBytes(msg);
                            session.SendAsync(data);
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.Info("PLCSend(ip, msg)异常，详细情况为：" + exception.Message);
                }
            }
            
        }

        ///// <summary>
        ///// 给指定客户端发送消息(匹配IP地址)
        ///// </summary>
        ///// <param name="ip">IP地址</param>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public async ValueTask PLCSend(string ip, string msg)
        //{
        //    try
        //    {
        //        object _lockobj = lockObject;
        //        bool _lockWasTaken = false;
        //        List<WebSession> sessionList = new List<WebSession>();
        //        try
        //        {
        //            System.Threading.Monitor.Enter(_lockobj, ref _lockWasTaken);
        //            sessionList = WCSSessionList.FindAll(p => p.RemoteEndPoint.ToString() == ip);
        //        }
        //        catch (Exception exception)
        //        {
        //            throw exception;
        //        }
        //        finally
        //        {
        //            if (_lockWasTaken) System.Threading.Monitor.Exit(_lockobj);
        //        }
        //        //对当前已连接的所有会话进行广播
        //        foreach (var session in sessionList)
        //        {
        //            if (session.State == SessionState.Connected)
        //            {
        //                var data = Encoding.UTF8.GetBytes(msg);
        //                await session.SendAsync(data);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Info("PLCSend(ip, msg)异常，详细情况为：" + exception.Message);
        //    }
        //}

        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSendAll(string msg)
        {
            lock (lockObject)
            {
                try
                {
                    //LogHelper.Info("WCSSessionList个数：" + WCSSessionList.Count);
                    //对当前已连接的所有会话进行广播
                    foreach (var session in WCSSessionList)
                    {
                        LogHelper.Info("sessionIp：" + (session.RemoteEndPoint as IPEndPoint).Address + ":" + (session.RemoteEndPoint as IPEndPoint).Port);
                        if (session.State == SessionState.Connected)
                        {
                            var data = msg;// Encoding.UTF8.GetBytes(msg);
                            session.SendAsync(data);
                        }
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.Info("PLCSendAll异常，详细情况为：" + exception.Message);
                }
            }
        }

        /// <summary>
        /// 广播数据 -20260507 优化性能，减少锁粒度和重复查询
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async ValueTask PLCSendAllV2(string msg)
        {
            try
            {
                List<WebSession> sessions;
                lock (lockObject)
                {
                    sessions = WCSSessionList
                        .Where(x => x.State == SessionState.Connected)
                        .ToList();
                }

                foreach (var session in sessions)
                {
                    await session.SendAsync(msg);
                }
            }
            catch (Exception exception)
            {
                LogHelper.Info("PLCSendAllV2异常，详细情况为：" + exception.Message);
            }
        }


        ///// <summary>
        ///// 广播数据
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public async ValueTask PLCSendAll(string msg)
        //{
        //    try
        //    {
        //        object _lockobj = lockObject;
        //        bool _lockWasTaken = false;
        //        List<WebSession> sessionList = new List<WebSession>();
        //        try
        //        {
        //            System.Threading.Monitor.Enter(_lockobj, ref _lockWasTaken);
        //            sessionList = WCSSessionList;
        //        }
        //        catch (Exception exception)
        //        {
        //            throw exception;
        //        }
        //        finally
        //        {
        //            if (_lockWasTaken) System.Threading.Monitor.Exit(_lockobj);
        //        }
        //        LogHelper.Info("WCSSessionList个数：" + sessionList.Count);
        //        //对当前已连接的所有会话进行广播
        //        foreach (var session in sessionList)
        //        {
        //            LogHelper.Info("sessionIp：" + (session.RemoteEndPoint as IPEndPoint).Address + ":" + (session.RemoteEndPoint as IPEndPoint).Port);
        //            if (session.State == SessionState.Connected)
        //            {
        //                var data = msg;// Encoding.UTF8.GetBytes(msg);
        //                await session.SendAsync(data);
        //            }
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Info("PLCSendAll异常，详细情况为：" + exception.Message);
        //    }
        //}



        /// <summary>
        /// 通过会话句柄ID找到客户端对象发送数据
        /// </summary>
        /// <param name="sessionID">会话句柄ID</param>
        /// <param name="msg">发送数据</param>
        /// <returns></returns>
        public async ValueTask PLCSendForSessionID(string sessionID, string msg)
        {
            lock(lockObject)
            {
                try
                {
                    WebSession session = new WebSession();
                    session = WCSSessionList.Find(p => p.SessionID == sessionID);
                    if (session != null)
                    {
                        var data = Encoding.UTF8.GetBytes(msg);
                        session.SendAsync(data);
                    }
                }
                catch (Exception exception)
                {
                    LogHelper.Info("PLCSendForSessionID异常，详细情况为：" + exception.Message);
                }
            }
        }

        ///// <summary>
        ///// 通过会话句柄ID找到客户端对象发送数据
        ///// </summary>
        ///// <param name="sessionID">会话句柄ID</param>
        ///// <param name="msg">发送数据</param>
        ///// <returns></returns>
        //public async ValueTask PLCSendForSessionID(string sessionID, string msg)
        //{
        //    try
        //    {
        //        object _lockobj = lockObject;
        //        bool _lockWasTaken = false;
        //        WebSession session = new WebSession();
        //        try
        //        {
        //            System.Threading.Monitor.Enter(_lockobj, ref _lockWasTaken);
        //            session = WCSSessionList.Find(p => p.SessionID == sessionID);
        //        }
        //        catch (Exception exception)
        //        {
        //            throw exception;
        //        }
        //        finally
        //        {
        //            if (_lockWasTaken) System.Threading.Monitor.Exit(_lockobj);
        //        }
        //        if (session != null)
        //        {
        //            var data = Encoding.UTF8.GetBytes(msg);
        //            await session.SendAsync(data);
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Info("PLCSendForSessionID异常，详细情况为：" + exception.Message);
        //    }

        //}

        /// <summary>
        /// 删除一个会话
        /// </summary>
        /// <param name="sessionID">会话ID</param>
        public void SessionRemove(WebSession session)
        {
            lock(lockObject)
            {
                try
                {
                    LogHelper.Info("客户端已下线，相关信息：sessionIp：" + (session.RemoteEndPoint as IPEndPoint).Address + ":" + (session.RemoteEndPoint as IPEndPoint).Port);
                    WCSSessionList.Remove(session);
                }
                catch (Exception exception)
                {
                    LogHelper.Info("SessionRemove异常，详细情况为：" + exception.Message);
                }
            }  
        }

        ///// <summary>
        ///// 删除一个会话
        ///// </summary>
        ///// <param name="sessionID">会话ID</param>
        //public void SessionRemove(WebSession session)
        //{
        //    object _lockobj = lockObject;
        //    bool _lockWasTaken = false;
        //    try
        //    {
        //        System.Threading.Monitor.Enter(_lockobj, ref _lockWasTaken);
        //        LogHelper.Info("客户端已下线，相关信息：sessionIp：" + (session.RemoteEndPoint as IPEndPoint).Address + ":" + (session.RemoteEndPoint as IPEndPoint).Port);
        //        WCSSessionList.Remove(session);
        //    }
        //    catch (Exception exception)
        //    {
        //        LogHelper.Info("SessionRemove异常，详细情况为：" + exception.Message);
        //    }
        //    finally
        //    {
        //        if (_lockWasTaken) System.Threading.Monitor.Exit(_lockobj);
        //    }
        //}
    }
}
