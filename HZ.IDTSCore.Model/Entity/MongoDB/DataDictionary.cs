using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 数据字典
    /// </summary>
    public class DataDictionary
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 父项编码
        /// </summary>
        public string parentCode { get; set; }
        /// <summary>
        /// 父项名称
        /// </summary>
        public string parentName { get; set; }
        /// <summary>
        /// 字典编码
        /// </summary>
        public string dicCode { get; set; }
        /// <summary>
        /// 字典名称
        /// </summary>
        public string dicName { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public string idDefault { get; set; }
    }
}
