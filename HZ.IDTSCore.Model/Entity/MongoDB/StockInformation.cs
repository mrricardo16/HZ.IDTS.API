using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 仓库信息
    /// </summary>
    public class StockInformation
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 仓库编号
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 仓库名称
        /// </summary>
        public string stockName { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public string stockType { get; set; }
    }
}
