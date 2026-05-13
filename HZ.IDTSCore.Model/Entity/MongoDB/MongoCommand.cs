using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    public class MongoCommand
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public int number { get; set; }
        /// <summary>
        /// 流程次序
        /// </summary>
        public int procedureOrder { get; set; }
        /// <summary>
        /// 完整指令Json（不含通配符）
        /// </summary>
        public string commandJson { get; set; }
        /// <summary>
        /// 时间间隔（ms/毫秒）
        /// </summary>
        public int interval { get; set; }
    }
}
