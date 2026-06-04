using CSRedis;
using HZ.CommonUtil.Helpers;

namespace HZ.DbHelper
{
    public static class RedisServer
    {
        public static CSRedisClient Cache;
        public static CSRedisClient Sequence;
        public static CSRedisClient Session;
        public static CSRedisClient BarCode;
        public static CSRedisClient Equipment;

        public static void Initalize()
        {
            Cache = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Cache"));
            Sequence = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Sequence"));
            Session = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Session"));
            BarCode = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Cache"));
            Equipment = new CSRedisClient(AppSettings.GetValue<string>("RedisServer:Equipment"));

            //RedisHelper.Initialization(Session);
            //RedisHelper.Initialization(Sequence);
        }
    }
}
