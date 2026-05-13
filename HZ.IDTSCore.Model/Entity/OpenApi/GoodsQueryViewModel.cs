using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.Entity.OpenApi
{
    public class GoodsQueryViewModel
    {
        /// <summary>
        /// 物料编码（模糊）
        /// </summary>
        public string goodsCode { get; set; }

        /// <summary>
        /// 物料名称（模糊）
        /// </summary>
        public string goodsName { get; set; }
    }

    public class GoodsQueryResult
    {
        public List<GoodsQueryResultModel> goodsQueryList = new List<GoodsQueryResultModel>();
    }

    public class GoodsQueryResultModel
    {
        /// <summary>
        /// 物料编码（准确）
        /// </summary>
        public string goodsCode { get; set; }
        /// <summary>
        /// 物料名称（准确）
        /// </summary>
        public string goodsName { get; set; }
        /// <summary>
        /// 位置信息
        /// </summary>
        public string locationCode { get; set; }
    }

    public class GoodsQueryRackResult
    {
        public List<GoodsQuery> goodsQueryList = new List<GoodsQuery>();
    }

    public class GoodsQueryRackViewModel
    {
        /// <summary>
        /// 物料编码（模糊）
        /// </summary>
        public string goodsCode { get; set; }

        /// <summary>
        /// 物料名称（模糊）
        /// </summary>
        public string goodsName { get; set; }

        /// <summary>
        /// 项目编号
        /// </summary>
        public string pileofland { get; set; }
    }

    /// <summary>
    /// 物料查询返回类(含料架）
    /// </summary>
    public class GoodsQuery
    {
        /// <summary>
        /// 物料编码
        /// </summary>
        public string goodsCode { get; set; }

        /// <summary>
        /// 物料名称
        /// </summary>
        public string goodsName { get; set; }

        /// <summary>
        /// 地堆货位编码
        /// </summary>
        public string locationCode { get; set; }

        /// <summary>
        /// 料架信息
        /// </summary>
        public RackInfo rackInfo { get; set; }
    }

    public class RackInfo
    {
        /// <summary>
        /// 备注信息
        /// </summary>
        public string remarks { get; set; }

        /// <summary>
        /// 料架编号
        /// </summary>
        public string rackCode { get; set; }

        /// <summary>
        /// 料箱信息
        /// </summary>
        public List<BoxInfo> boxInfo { get; set; }
    }

    public class BoxInfo
    {
        /// <summary>
        /// 料箱编号
        /// </summary>
        public string boxCode { get; set; }

        /// <summary>
        /// 料箱位置 排-列-层
        /// </summary>
        public string boxLocation { get; set; }
    }

}
