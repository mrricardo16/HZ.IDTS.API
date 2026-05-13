using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 关于线程的一些时间设置
    /// </summary>
    public class MongoThreadEventSet
    {
        public ObjectId _id { get; set; }

        /// <summary>
        /// 线程类的名称
        /// </summary>
        public string threadClassName { get; set; }

        /// <summary>
        /// 线程名称
        /// </summary>
        public string threadName { get; set; }

        /// <summary>
        /// 事件ID
        /// </summary>
        public string eventId { get; set; }

        /// <summary>
        /// 事件名称
        /// </summary>
        public string eventName { get; set; }

        /// <summary>
        /// 最后一次处理时间
        /// </summary>
        public DateTime lastUpdateTime { get; set; }

        public string ext1 { get; set; }
        public string ext2 { get; set; }
        public string ext3 { get; set; }
        public string ext4 { get; set; }
        public string ext5 { get; set; }
        public string ext6 { get; set; }
    }
}
