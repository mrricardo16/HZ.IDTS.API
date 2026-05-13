using HZ.iWCS.Common.Core;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace HZ.IDTS.SimulateService
{
    public class Bootstrap
    {
        public static NamedPipeClient _NamedPipeClient { get; set; }

        public static string dtsApiUrl { get; set; }

        /// <summary>
        /// 心跳线程
        /// </summary>
        private Thread _HeartbeatThread { get; set; }

        /// <summary>
        /// 与AGV仿真通讯的客户端
        /// </summary>
        private EasyClient client { get; set; }

        private string serverAddress { get; set; }
        private int serverPort { get; set; }

        public void Initialize()
        {
            //_NamedPipeClient = new NamedPipeClient("MyNamedPipe");
            //_NamedPipeClient.Connect();
            serverAddress = AppConfigurtaionServices.Configuration["AppSettings:ServerAddress"];
            serverPort = Convert.ToInt32(AppConfigurtaionServices.Configuration["AppSettings:ServerPort"]);
            dtsApiUrl = AppConfigurtaionServices.Configuration["AppSettings:IdtsApi"];
            ConnectServer(serverAddress, serverPort);
            _HeartbeatThread = new Thread(HeartbeatHandle);
        }

        private void ConnectServer(string _serverAddress, int _serverPort)
        {
            client = new EasyClient();
            client.Initialize(new FakeReceiveFilter(), (p) =>
            {
                // do nothing
            });

            client.ConnectAsync(new DnsEndPoint(_serverAddress, _serverPort));
        }

        private void HeartbeatHandle()
        {
            while (true)
            {
                try
                {
                    if (client.IsConnected)
                    {
                        client.Send(Encoding.UTF8.GetBytes("IDTS_Client"));
                        Console.WriteLine("心跳正常");
                    }
                    else
                    {
                        Console.WriteLine("断线重连中...");
                        ConnectServer(serverAddress, serverPort);
                    }
                }
                catch { }
                Thread.Sleep(5000);
            }
        }

        public  void Start()
        {
            _HeartbeatThread.Start();
        }

        public  void Shutdown()
        {
            _HeartbeatThread.Abort();
        }
    }
}
