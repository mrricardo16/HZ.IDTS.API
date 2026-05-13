using HZ.iWCS.Common.Core;
using Newtonsoft.Json;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HZ.IDTS.SimulateService
{
    public class FakeReceiveFilter : TerminatorReceiveFilter<StringPackageInfo>
    {
        private HttpClient hc;
        private readonly ConcurrentQueue<AgvRealMonitorViewModel> dataQueue;
        private bool isSending;
        public FakeReceiveFilter()
           : base(new byte[] { 0x24, 0x24 })
        {
            hc = new HttpClient();
            hc.Timeout = TimeSpan.FromSeconds(30);
            dataQueue = new ConcurrentQueue<AgvRealMonitorViewModel>();
            isSending = false;
        }


        //解压
        public byte[] MicrosoftDecompress(byte[] data)
        {
            MemoryStream compressed = new MemoryStream(data);
            MemoryStream decompressed = new MemoryStream();
            DeflateStream deflateStream = new DeflateStream(compressed, CompressionMode.Decompress);
            deflateStream.CopyTo(decompressed);
            byte[] result = decompressed.ToArray();
            return result;

        }
        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            var totalLength = (int)bufferStream.Length - 2;
            byte[] buffer = new byte[totalLength];
            int bytesRead = 0;
            foreach (var segment in bufferStream.Buffers)
            {
                int copyLength = Math.Min(segment.Count, totalLength - bytesRead);
                if (copyLength <= 0)
                {
                    break; // Avoid copying more data than available or necessary.
                }
                Buffer.BlockCopy(segment.Array, segment.Offset, buffer, bytesRead, copyLength);
                bytesRead += copyLength;
            }
            var bufferString = Encoding.UTF8.GetString(buffer);
            byte[] data = MicrosoftDecompress(buffer);
            var jsonString = Encoding.UTF8.GetString(data);
            bufferStream.Buffers.Clear();
            //Bootstrap._NamedPipeClient.SendDataToServer(json);
            //Console.WriteLine(json);
            //LogHelper.Info(json);
            AgvRealMonitorViewModel agvModel = new AgvRealMonitorViewModel();
            List<CarModel> list = JsonConvert.DeserializeObject<List<CarModel>>(jsonString);
            foreach (var model in list)
            {
                byte byte1;
                byte byte2;
                FromShort(model.s, out byte1, out byte2);
                string isOnline = (byte1 & 128) == 128 ? "在线" : "离线";
                string isCarryCargo = (byte1 & 8) == 8 ? "1" : "0";
                string carState = (byte2 & 1) == 1 ? "异常" : "正常";
                string errMsg = "";
                if (carState == "正常") errMsg = "0#正常";
                if (carState == "异常")
                {
                    errMsg = "1#异常[";
                    if ((byte1 & 64) == 64)
                    {
                        errMsg += "安全激光触发";
                    }
                    if ((byte1 & 32) == 32)
                    {
                        errMsg += "导航丢失";
                    }
                    if ((byte1 & 16) == 16)
                    {
                        errMsg += "低电量";
                    }
                    if((byte1 & 4) == 4)
                    {
                        errMsg += "触边触发";
                    }
                    if((byte1 & 2) == 2)
                    {
                        errMsg += "急停触发";
                    }
                    errMsg += "]";
                }

                agvModel.Agv.Add(new AgvModel()
                {
                    carCode = model.id.ToString(),
                    carName = model.n,
                    x = model.x.ToString(),
                    y = model.y.ToString(),
                    angle = model.th.ToString(),
                    onlineState = isOnline,
                    loadStatus = isCarryCargo,
                    liftHeight = model.h.ToString(),
                    carState = carState,
                    power = model.soc,//%
                    speed = model.v.ToString(),//米/S
                    errMsg = errMsg
                });
            }
            //string sendJSONString = JsonConvert.SerializeObject(agvModel);
            AddData(agvModel);
            //var result = hc.PostAsync(Bootstrap.dtsApiUrl + "api/dts/AgvSimulate", new StringContent(sendJSONString, System.Text.Encoding.UTF8, "application/json"));
            return null;
        }

        /// <summary>
        /// 解析方法
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string GetStatus(short number)
        {
            byte byte1;
            byte byte2;
            FromShort(number, out byte1, out byte2);
            var isOnline = (byte1 & 128) == 128 ? 1 : 0;//在线离线
            var safeLaser = (byte1 & 64) == 64 ? 1 : 0;//安全激光触发
            var navigationLost = (byte1 & 32) == 32 ? 1 : 0;//导航丢失
            var lowPower = (byte1 & 16) == 16 ? 1 : 0;//低电量
            var isCarryCargo = (byte1 & 8) == 8 ? 1 : 0;//有无货物
            var tentacleTrigger = (byte1 & 4) == 4 ? 1 : 0;//触边
            var emergencyStop = (byte1 & 2) == 2 ? 1 : 0;//急停
            var mode = (byte1 & 1) == 1 ? 1 : 0;//手自动
            var isException = (byte2 & 1) == 1 ? 1 : 0;//是否异常
            return "";

        }


        public void FromShort(short number, out byte byte1, out byte byte2)
        {
            byte2 = (byte)(number >> 8);
            byte1 = (byte)(number & 255);
        }

        public void AddData(AgvRealMonitorViewModel data)
        {
            dataQueue.Enqueue(data);
            // If not already sending data, start sending in a separate thread
            if (!isSending)
            {
                Task.Run(SendDataAsync);
            }
        }

        private async Task SendDataAsync()
        {
            isSending = true;

            while (dataQueue.TryDequeue(out var data))
            {
                try
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + "-->" + JsonConvert.SerializeObject(data));
                    // Send the data using POST request
                    //var content = new StringContent(data, Encoding.UTF8, "application/json");
                    //await hc.PostAsync(Bootstrap.dtsApiUrl + "api/dts/AgvSimulate", content);
                    //response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending data: " + ex.Message);
                }
                // Pause for 100ms before sending the next data (adjust this as needed)
                await Task.Delay(100);
            }

            isSending = false;
        }
    }
}
