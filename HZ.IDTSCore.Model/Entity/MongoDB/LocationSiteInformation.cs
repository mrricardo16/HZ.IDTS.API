using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 货位站点信息
    /// </summary>
    public class LocationSiteInformation
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 货位编码
        /// </summary>
        public string locationCode { get; set; }
        /// <summary>
        /// 最大容器数量（int类型）
        /// </summary>
        public int? max_store_num { get; set; }
        /// <summary>
        /// 所属巷道
        /// </summary>
        public string roadway { get; set; }
        /// <summary>
        /// 排
        /// </summary>
        public string row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public string col { get; set; }
        /// <summary>
        /// 层
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 货位/站点
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 真实位置
        /// </summary>
        public string real_location { get; set; }
        /// <summary>
        /// 坐标
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string location_state { get; set; }
        /// <summary>
        /// 货位名称
        /// </summary>
        public string location_name { get; set; }
        /// <summary>
        /// 库区编号
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 配送节拍（int类型）
        /// </summary>
        public int? beat { get; set; }
    }

    /// <summary>
    /// 货位信息（过渡）
    /// </summary>
    public class LocationInformation_Middle
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 货位编码（string）
        /// </summary>
        public string locationCode { get; set; }
        /// <summary>
        /// 最大容器数量
        /// </summary>
        public string max_store_num { get; set; }
        /// <summary>
        /// 所属巷道
        /// </summary>
        public string roadway { get; set; }
        /// <summary>
        /// 排
        /// </summary>
        public string row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public string col { get; set; }
        /// <summary>
        /// 层
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 货点/站点
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 真实位置
        /// </summary>
        public string real_location { get; set; }
        /// <summary>
        /// 坐标
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string location_state { get; set; }
        /// <summary>
        /// 货位名称
        /// </summary>
        public string location_name { get; set; }
        /// <summary>
        /// 库区编号
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 配送节拍（string）
        /// </summary>
        public string beat { get; set; }
    }

    /// <summary>
    /// 站点信息（过渡）
    /// </summary>
    public class SiteInformation_Middle
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 仓库编码
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 站点编码（string）
        /// </summary>
        public string siteCode { get; set; }
        /// <summary>
        /// 最大容器数量
        /// </summary>
        public string max_store_num { get; set; }
        /// <summary>
        /// 所属巷道
        /// </summary>
        public string roadway { get; set; }
        /// <summary>
        /// 排
        /// </summary>
        public string row { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public string col { get; set; }
        /// <summary>
        /// 层
        /// </summary>
        public string floor { get; set; }
        /// <summary>
        /// 货点/站点
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 真实位置
        /// </summary>
        public string real_location { get; set; }
        /// <summary>
        /// 坐标
        /// </summary>
        public string position { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string location_state { get; set; }
        /// <summary>
        /// 货位名称
        /// </summary>
        public string location_name { get; set; }
        /// <summary>
        /// 库区编号
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 配送节拍（string）
        /// </summary>
        public string beat { get; set; }
    }
}
