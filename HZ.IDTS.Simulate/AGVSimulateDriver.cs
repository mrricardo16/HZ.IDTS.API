using HZ.CommonUtil.Helpers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HZ.IDTS.Simulate
{
    /// <summary>
    /// IP地址配置
    /// </summary>
    public static class AGVSimulateDriverConfig
    {
        public static string IpAddress { get; set; }
        public static int Port { get; set; }
    }

    /// <summary>
    /// 构建AGV Socket 客户端类
    /// </summary>
    public class AGVSimulateDriver
    {
        private const int BufferSize = 4096;
        private static byte[] Buffer = new byte[BufferSize];
        private static Socket ClientSocket;
        private Thread heartbeatThread;
        private static bool IsConnected = false;
        private static byte[] EndDelimiter = { 36, 36 }; // $$结束符

        /// <summary>
        /// 数据包队列
        /// </summary>
        private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // 连接服务器
            ConnectToServer(AGVSimulateDriverConfig.IpAddress, AGVSimulateDriverConfig.Port);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // 关闭连接和心跳线程
            ClientSocket.Close();
            heartbeatThread.Abort();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 连接Socket服务端
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="serverPort"></param>
        private  void ConnectToServer(string serverAddress, int serverPort)
        {
            while (!IsConnected)
            {
                try
                {
                    // 创建客户端Socket
                    ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    // 尝试连接服务器
                    ClientSocket.Connect(serverAddress, serverPort);
                    IsConnected = true;
                    // 连接成功后，继续接收数据
                    ClientSocket.BeginReceive(Buffer, 0, BufferSize, SocketFlags.None, ReceiveCallback, null);
                    // 启动心跳线程
                    heartbeatThread = new Thread(Heartbeat);
                    heartbeatThread.Start();
                    Console.WriteLine("Connected to server.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to connect to server. Retrying...");
                    System.Threading.Thread.Sleep(5000); // 等待1秒后重试
                }
            }
        }

        private  void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int bytesRead = ClientSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // 查找结束符的位置
                    int endIndex = Array.IndexOf(Buffer, (byte)36, 0, bytesRead);

                    if (endIndex != -1)
                    {
                        // 获取消息内容（不包含结束符）
                        string receivedData = Encoding.UTF8.GetString(Buffer, 0, endIndex);

                        // 处理接收到的数据
                        //Console.WriteLine("Received: " + receivedData);
                        // 处理接收到的完整数据包
                        ProcessReceivedData(receivedData);

                        // 将接收缓冲区中的数据前移，准备接收下一条消息
                        Array.Copy(Buffer, endIndex + 2, Buffer, 0, bytesRead - (endIndex + 2));
                    }

                    // 继续接收数据
                    ClientSocket.BeginReceive(Buffer, bytesRead - (endIndex + 2), BufferSize - (bytesRead - (endIndex + 2)), SocketFlags.None, ReceiveCallback, null);
                }
                //else
                //{
                //    // 服务器断开连接，尝试重新连接
                //    CloseClient();
                //    Console.WriteLine("Disconnected from server. Trying to reconnect...");
                //    ConnectToServer(AGVSimulateDriverConfig.IpAddress, AGVSimulateDriverConfig.Port); // 重新连接服务器
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 向服务端发送消息
        /// </summary>
        /// <param name="message"></param>
        private  void SendToServer(string message)
        {
            try
            {
                // 将消息转换为字节数组，并添加结束符
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] dataWithEndDelimiter = new byte[data.Length + EndDelimiter.Length];
                Array.Copy(data, 0, dataWithEndDelimiter, 0, data.Length);
                Array.Copy(EndDelimiter, 0, dataWithEndDelimiter, data.Length, EndDelimiter.Length);

                // 发送消息给服务器
                ClientSocket.Send(dataWithEndDelimiter);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// 接收完整的数据包
        /// </summary>
        /// <param name="data"></param>
        private void ProcessReceivedData(string data)
        {
            messageQueue.Enqueue(data);
            // 在这里进行逻辑处理，处理接收到的完整数据包
            //Console.WriteLine("Received complete data package: " + data);
            //LogHelper.Info("Received complete data package:" + data);
        }

        /// <summary>
        /// 心跳包
        /// </summary>
        private  void Heartbeat()
        {
            try
            {
                while (true)
                {
                    // 发送心跳消息
                    byte[] heartbeatMessage = Encoding.UTF8.GetBytes("IDTS.Heartbeat");
                    int byteSize = ClientSocket.Send(heartbeatMessage);
                    Thread.Sleep(5000); // 每隔5秒发送一次心跳消息
                }
            }
            catch (ThreadAbortException)
            {
                // 心跳线程被终止，即退出程序，不需要处理
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in heartbeat thread: " + ex.Message);
                IsConnected = false;

                // 服务器断开连接，尝试重新连接
                CloseClient();
                ConnectToServer(AGVSimulateDriverConfig.IpAddress, AGVSimulateDriverConfig.Port); // 重新连接服务器
            }
        }

        /// <summary>
        /// 关闭客户端
        /// </summary>
        private void CloseClient()
        {
            // 关闭客户端Socket
            if (ClientSocket != null && ClientSocket.Connected)
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
                IsConnected = false;
            }
        }
    }
}
