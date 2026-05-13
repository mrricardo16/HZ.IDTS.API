using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    public class tn_wms_location_state
    {
        public ObjectId _id { get; set; }

        public string locationCode { get; set; }

        public string locationState { get; set; }

        public string lastUpdateTime { get; set; }
    }
}
