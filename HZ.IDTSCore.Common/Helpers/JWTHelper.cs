using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;

namespace HZ.IDTSCore.Common.Helpers
{
    public static class JWTHelper
    {
        /// <summary>
        /// 生成jwt字符串
        /// </summary>
        /// <param name="exp">到期时间</param>
        /// <param name="secret">私密</param>
        /// <returns></returns>
        public static string Encode(long exp,string secret)
        {
            //JWT载荷数据对象
            var payload = new Dictionary<string, object>
            {
                //发行人
                { "iss","WMS"},
                //到期时间【设置过期时间为24小时】
                //{ "exp", DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds() },
                { "exp",exp },
                //主题
                //{ "sub", "WMSJWT" }, 
                //用户
                //{ "aud", "USER" }, 
                //发布时间 
                { "iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds()}, 
                //自定义载荷数据
                //{
                //    "data", new
                //    {
                //        UserId = 123456,
                //        UserName = "系统管理员"
                //    }
                //}
             };
            //私钥
            //var secret = "CHDY-2022";// "C4CCD2D2656D820062C11968C09E9175";

            //HMACSHA256加密
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            //序列化和反序列
            IJsonSerializer serializer = new JsonNetSerializer();
            //Base64编解码
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            //编码成JWT令牌
            var token = encoder.Encode(payload, secret);
            Console.WriteLine(token);
            return token;
        }
    }
}
