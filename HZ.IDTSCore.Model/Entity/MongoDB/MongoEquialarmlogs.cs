using HZ.IDTSCore.Model.Entity.Equipment;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    public class MongoEquialarmlogs
    {
        public ObjectId _id { get; set; }
        public string deviceCode { get; set; }
        public string deviceName { get; set; }
        public string errCode { get; set; }
        public string errMsg { get; set; }
        public DateTime timestamp { get; set; }
    }
}
