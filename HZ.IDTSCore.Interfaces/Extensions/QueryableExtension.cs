using HZ.CommonUtil.Model;
using HZ.IDTSCore.Model;
using HZ.iWCS.MData.Core;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HZ.IDTSCore.Interfaces
{
    public static class QueryableExtension
    {
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedInfo<T>> ToPageAsync<T>(this ISugarQueryable<T> source, int pageIndex, int pageSize)
        {
            var page = new PagedInfo<T>();
            var total = await source.CountAsync();
            page.TotalCount = total;
            page.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                page.TotalPages++;

            page.PageSize = pageSize;
            page.PageIndex = pageIndex;

            page.DataSource = await source.ToPageListAsync(pageIndex, pageSize);
            return page;
        }

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static async Task<PagedInfo<T>> ToPageAsync<T>(this ISugarQueryable<T> source, List<ITotalField> totalField, int pageIndex, int pageSize)
        {
            var page = new PagedInfo<T>();
            var total = await source.CountAsync();
            page.TotalCount = total;
            page.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                page.TotalPages++;

            page.PageSize = pageSize;
            page.PageIndex = pageIndex;

            if (totalField != null && totalField.Count > 0)
            {
                foreach (var field in totalField)
                {
                    field.value = source.Sum<decimal>(field.Name);
                }
            }

            page.TotalField = totalField;

            page.DataSource = await source.ToPageListAsync(pageIndex, pageSize);
            return page;
        }

        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedInfo<T> ToPage<T>(this ISugarQueryable<T> source, int pageIndex, int pageSize)
        {
            var page = new PagedInfo<T>();
            var total =  source.Count();
            page.TotalCount = total;
            page.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                page.TotalPages++;

            page.PageSize = pageSize;
            page.PageIndex = pageIndex;

            int t = 0;
            page.DataSource = source.ToPageList(pageIndex, pageSize,ref t);
            return page;
        }

        /// <summary>
        /// Mongo读取列表
        /// </summary>SELECT DISTINCT, ORDER BY expressions must appear in select list”

        /// <typeparam name="T"></typeparam>
        /// <param name="mongoDBSingleton"></param>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedInfo<T> ToPageMongo<T>(this MongoDBSingleton mongoDBSingleton, MongoDB.Driver.FilterDefinition<T> filter, int pageIndex, int pageSize)
        {
            var page = new PagedInfo<T>();
            var total = mongoDBSingleton.Count<T>(filter);
            page.TotalCount = Convert.ToInt32(total);
            page.TotalPages = Convert.ToInt32(total / pageSize);

            if (total % pageSize > 0)
            {
                page.TotalPages++;
            }
            page.PageSize = pageSize;
            page.PageIndex = pageIndex;
            page.DataSource = mongoDBSingleton.FindList<T>(filter).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList();
            return page;
        }

        /// <summary>
        /// List读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedInfo<T> ToPageEnumerable<T>(this IEnumerable<T> enumerable, int pageIndex, int pageSize)
        {
            var page = new PagedInfo<T>();
            var total = enumerable.Count();
            page.TotalCount = total;
            page.TotalPages = total / pageSize;

            if(total % pageSize > 0)
            {
                page.TotalPages++;
            }
            page.PageSize = pageSize;
            page.PageIndex = pageIndex;
            List<T> tList = enumerable as List<T>;
            page.DataSource = !(tList is null) ? tList.Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize).ToList() : new List<T>();
            return page;
        }

        public static PagedInfo<T> ToPageDistinct<T>(this ISugarQueryable<T> source, int pageIndex, int pageSize, string distinctFiled)
        {

            var page = new PagedInfo<T>();

            int t = 0;
            page.DataSource = source.Distinct().ToPageList(pageIndex, pageSize, ref t);

            var total = source.Select<int>("count(distinct "+ distinctFiled + ")").Single();
            page.TotalCount = total;
            page.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                page.TotalPages++;

            page.PageSize = pageSize;
            page.PageIndex = pageIndex;
            return page;
        }

        public static PagedDataTable ToPageDataTable<T>(this ISugarQueryable<T> source, int pageIndex, int pageSize)
        {
            var page = new PagedDataTable();
            var total = source.Count();
            page.TotalCount = total;
            page.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                page.TotalPages++;

            page.PageSize = pageSize;
            page.PageIndex = pageIndex;

            page.DataSource = source.ToDataTablePage(pageIndex, pageSize, ref total);
            return page;
        }
        /// <summary>
        /// 读取列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedInfo<T> ToPage<T>(this ISugarQueryable<T> source, List<ITotalField> totalField, int pageIndex, int pageSize)
        {
            var page = new PagedInfo<T>();
            var total = source.Count();
            page.TotalCount = total;
            page.TotalPages = total / pageSize;

            if (total % pageSize > 0)
                page.TotalPages++;

            page.PageSize = pageSize;
            page.PageIndex = pageIndex;

            if (totalField != null && totalField.Count > 0)
            {
                foreach (var field in totalField)
                {
                    field.value = source.Sum<decimal>(field.Name);
                }
            }

            page.TotalField = totalField;

            page.DataSource = source.ToPageList(pageIndex, pageSize);

            return page;
        }
    }
}
