using CSRedis;
using HZ.CommonUtil.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Common.Helpers
{
    public static class WMSRedisServer
    {
        public static CSRedisClient Cache;
        public static CSRedisClient Sequence;

        public static void Initalize()
        {
            Cache = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Cache"));
            Sequence = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Sequence"));
            //Session = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Session"));
            //BarCode = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Cache"));
            //Equipment = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Equipment"));

            RedisHelper.Initialization(Cache);
            RedisHelper.Initialization(Sequence);
        }
    }
}
