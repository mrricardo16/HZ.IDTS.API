using MongoDB.Driver;
using MongoDB.Driver.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.iWCS.MData.Core
{
    /// <summary>
    /// mongodb 创建database
    /// <history>CREATE[FJ] TIME[20190702]</history>
    /// </summary>
    internal class MongodbClient//<T> where T : class
    {

        private static MongoClient client;

        #region +MongodbInfoClient 获取mongodb实例
        /// <summary>
        /// 获取mongodb实例
        /// </summary>
        /// <param name="host">连接字符串，库，表</param>
        /// <returns></returns>
        public static IMongoDatabase MongodbDatabase(string mDataConnection, string database)
        {
            client = new MongoClient(new MongoUrl(mDataConnection + @"/" + database));
            var dataBase = client.GetDatabase(database);
            return dataBase;
        }

        /// <summary>
        /// 版本2;解决windows服务链接字符串报错问题
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="database"></param>
        /// <param name="userName"></param>
        /// <param name="passworde"></param>
        /// <returns></returns>
        public static IMongoDatabase MongodbDatabase(string host, int port, string database, string userName, string password)
        {
            MongoCredential credential = MongoCredential.CreateCredential(database, userName, password);
            MongoClientSettings settings = new MongoClientSettings
            {
                Credential = credential,
                Server = new MongoServerAddress(host, port)
            };
            client = new MongoClient(settings);
            var dataBase = client.GetDatabase(database);
            return dataBase;
        }

        /// <summary>
        /// 检测数据库是否连接成功
        /// </summary>
        /// <returns></returns>
        public static bool Connectioned()
        {
            bool result = false;
            try
            {
                var databases = client.ListDatabases().ToList();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        #endregion
    }
}
