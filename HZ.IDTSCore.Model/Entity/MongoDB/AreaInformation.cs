using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.MongoDB
{
    /// <summary>
    /// 库区信息
    /// </summary>
    public class AreaInformation
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 所属仓库编码
        /// </summary>
        public string stock_code { get; set; }
        /// <summary>
        /// 库区编码
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 库区名称
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 库区类型（int类型：1:库区 2:作业区）
        /// </summary>
        public int? area_type { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 库区结构
        /// </summary>
        public string area_struct { get; set; }
        /// <summary>
        /// 作业区类型
        /// </summary>
        public string area_class { get; set; }
        /// <summary>
        /// 是否管控库存
        /// </summary>
        public string is_inventory { get; set; }
        /// <summary>
        /// 是否自动化库
        /// </summary>
        public string is_auto { get; set; }
        /// <summary>
        /// 管控维度
        /// </summary>
        public string control_leve { get; set; }
        /// <summary>
        /// 是否管控数量
        /// </summary>
        public string control_qty { get; set; }
        /// <summary>
        /// 库区码盘方式
        /// </summary>
        public string codedisk_model { get; set; }
        /// <summary>
        /// 有无托盘
        /// </summary>
        public string is_tray { get; set; }
        /// <summary>
        /// 是否校验托盘
        /// </summary>
        public string is_checktray { get; set; }
        /// <summary>
        /// 是否合并
        /// </summary>
        public string is_merge { get; set; }
        /// <summary>
        /// 是否可出库
        /// </summary>
        public string is_outarea { get; set; }
        /// <summary>
        /// 是否管控出入库模式
        /// </summary>
        public string is_inout_model { get; set; }
        /// <summary>
        /// 出入库模式
        /// </summary>
        public string inout_model { get; set; }
    }

    /// <summary>
    /// 库区信息过渡
    /// </summary>
    public class AreaInformation_Middle
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 所属仓库编码
        /// </summary>
        public string stock_code { get; set; }
        /// <summary>
        /// 库区编码
        /// </summary>
        public string area_code { get; set; }
        /// <summary>
        /// 库区名称
        /// </summary>
        public string area_name { get; set; }
        /// <summary>
        /// 库区类型（string类型）
        /// </summary>
        public string area_type { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public string order { get; set; }
        /// <summary>
        /// 库区结构
        /// </summary>
        public string area_struct { get; set; }
        /// <summary>
        /// 作业区类型
        /// </summary>
        public string area_class { get; set; }
        /// <summary>
        /// 是否管控库存
        /// </summary>
        public string is_inventory { get; set; }
        /// <summary>
        /// 是否自动化库
        /// </summary>
        public string is_auto { get; set; }
        /// <summary>
        /// 管控维度
        /// </summary>
        public string control_leve { get; set; }
        /// <summary>
        /// 是否管控数量
        /// </summary>
        public string control_qty { get; set; }
        /// <summary>
        /// 库区码盘方式
        /// </summary>
        public string codedisk_model { get; set; }
        /// <summary>
        /// 有无托盘
        /// </summary>
        public string is_tray { get; set; }
        /// <summary>
        /// 是否校验托盘
        /// </summary>
        public string is_checktray { get; set; }
        /// <summary>
        /// 是否合并
        /// </summary>
        public string is_merge { get; set; }
        /// <summary>
        /// 是否可出库
        /// </summary>
        public string is_outarea { get; set; }
        /// <summary>
        /// 是否管控出入库模式
        /// </summary>
        public string is_inout_model { get; set; }
        /// <summary>
        /// 出入库模式
        /// </summary>
        public string inout_model { get; set; }
    }
}
