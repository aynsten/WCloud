using Lib.helper;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Lib.extension
{
    /// <summary>
    /// 对Linq的扩展
    /// </summary>
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf_<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> where)
        {
            if (condition)
            {
                query = query.Where(predicate: where);
            }
            return query;
        }

        /// <summary>
        /// 如果条件不为空就使用条件
        /// </summary>
        public static IQueryable<T> WhereIfNotNull<T>(this IQueryable<T> query, Expression<Func<T, bool>> where)
        {
            var res = WhereIf_(query, where != null, where);
            return res;
        }

        /// <summary>
        /// 分页
        /// </summary>
        public static IQueryable<T> QueryPage<T>(this IOrderedQueryable<T> query, int page, int pagesize)
        {
            var pager = PagerHelper.GetQueryRange(page, pagesize);

            IQueryable<T> q = query;

            if (pager.skip > 0)
            {
                q = q.Skip(pager.skip);
            }
            var res = q.Take(pager.take);
            return res;
        }

        /// <summary>
        /// 获取记录总数和分页总数
        /// </summary>
        public static (int item_count, int page_count) QueryRowCountAndPageCount<T>(this IQueryable<T> query, int page_size)
        {
            var item_count = query.Count();
            var page_count = PagerHelper.GetPageCount(item_count, page_size);
            return (item_count, page_count);
        }

        /// <summary>
        /// 自动分页
        /// </summary>
        public static PagerData<T> ToPagedList<T, SortColumn>(this IQueryable<T> query,
            int page, int pagesize, Expression<Func<T, SortColumn>> orderby, bool desc = true)
        {
            var data = new PagerData<T>()
            {
                Page = page,
                PageSize = pagesize
            };

            data.ItemCount = query.Count();
            data.DataList = query.OrderBy_(orderby, desc).QueryPage(page, pagesize).ToList();

            return data;
        }

        /// <summary>
        /// 排序
        /// </summary>
        public static IOrderedQueryable<T> OrderBy_<T, SortColumn>(this IQueryable<T> query,
            Expression<Func<T, SortColumn>> orderby, bool desc = true)
        {
            var res = desc ?
            query.OrderByDescending(orderby) :
            query.OrderBy(orderby);
            return res;
        }
    }
}
