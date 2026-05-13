using HZ.IDTSCore.Model.Entity.OpenApi;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    public class MongoRealCollect : EQRealCollectModel
    {
        public ObjectId _id { get; set; }
    }
}
