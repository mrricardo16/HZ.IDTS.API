
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;
using SqlSugar;

namespace HZ.IDTSCore.Model
{
    public  class tn_mdg_location
    {
        public string stockCode { get; set; }
        public string areaCode { get; set; }
        public string locationCode { get; set; }
        public string locationType { get; set; }
        public string roadWay { get; set; }
        public string row { get; set; }
        public string column { get; set; }
        public string floor { get; set; }
        public string agvlocation { get; set; }

        //public string stockCode { get; set; }
        //public string locationCode { get; set; }
        //public string row { get; set; }
        //public string floor { get; set; }
        public string area_code { get; set; }             //(string)库区编号
        public string max_store_num { get; set; }
        public string roadway { get; set; }
        public string col { get; set; }
        public string type { get; set; }    //(string)货位/站点
        public string real_location { get; set; }           //(string)真实位置
        public string position { get; set; }             //(string)坐标
        public string location_state { get; set; }         //(string)状态
        public string location_name { get; set; }           //(string)货位名称

        public string beat { get; set; }                 //(int)配送节拍
    }
}
