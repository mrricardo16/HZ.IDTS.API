using HZ.IDTSCore.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace HZ.IDTSCore.Model.SerializeEntity
{
    public class MagicQueryEntity
    {
        public string token { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// 功能点类型
        /// </summary>
        public string funcType { get; set; }

        /// <summary>
        /// 是否过程控制-必填
        /// </summary>
        public bool version_control { get; set; }

        /// <summary>
        /// 表名  必填
        /// </summary>
        public string tableName { get; set; }

        /// <summary>
        /// 是否树状显示
        /// </summary>
        public bool isTree { get; set; }

        /// <summary>
        /// 如果为树状显示  则必填 通过此字段进行链接
        /// </summary>
        public string joinNameTree { get; set; }

        /// <summary>
        /// 是否分页
        /// </summary>
        public bool isPageing { get; set; }

        /// <summary>
        /// 每页显示多少条   isPageing=true时必填
        /// </summary>
        public string pageSize { get; set; }

        /// <summary>
        /// 当前页  isPageing=true时必填
        /// </summary>
        public string pageIndex { get; set; }

        /// <summary>
        /// 初始化查询条件  可为空
        /// </summary>
        public string loadWhere { get; set; }

        /// <summary>
        /// 需要查询的字段名 以逗号分隔 不填则自动填充为*号 
        /// </summary>
        public string fileds { get; set; }

        /// <summary>
        /// 组织的查询数据 可为空
        /// </summary>
        public List<QueryEntity> lstQuery { get; set; }

        /// <summary>
        /// 排序  可为空
        /// </summary>
        public List<OrderByEntity> lstOrderBy { get; set; }
    }

    /// <summary>
    /// 排序实体
    /// </summary>
    public class OrderByEntity
    {
        /// <summary>
        /// 排序字段名称-必填
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 排序类型  必填 (枚举asc、desc)
        /// </summary>
        public string type { get; set; }
    }

    /// <summary>
    /// 用于查询的实体
    /// </summary>
    public class QueryEntity
    {
        /// <summary>
        /// 字段名   必填
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 查询关键字  = like  !=
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 查询的值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        public string type { get; set; }
    }


    public class MdgQueryResult<T>
    {
        public List<T> rows { get; set; }
        public int total { get; set; }
    }
}
