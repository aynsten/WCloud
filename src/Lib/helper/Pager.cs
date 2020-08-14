using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Lib.helper
{
    /// <summary>
    /// 分页用的数据模型，请不要返回空对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [DataContract]
    public class PagerData<T> : PagerData<T, object> { }

    /// <summary>
    /// 分页用的数据模型，请不要返回空对象
    /// </summary>
    [Serializable]
    [DataContract]
    public class PagerData<T, EXT>
    {
        /// <summary>
        /// 数据库记录总数
        /// </summary>
        [DataMember]
        public int ItemCount { get; set; }

        /// <summary>
        /// 总页数，通过itemcount和pagesize计算出
        /// </summary>
        [DataMember]
        public int PageCount
        {
            get
            {
                if (this.PageSize <= 0) { return -1; }
                return PagerHelper.GetPageCount(this.ItemCount, this.PageSize);
            }
        }

        /// <summary>
        /// 每页显示数量
        /// </summary>
        [DataMember]
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        [DataMember]
        public int Page { get; set; }

        /// <summary>
        /// 查出来的数据列表
        /// </summary>
        [DataMember]
        public IEnumerable<T> DataList { get; set; }

        /// <summary>
        /// 额外数据
        /// </summary>
        [DataMember]
        public EXT ExtData { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [DataMember]
        public bool Success { get; set; } = true;
    }

    /// <summary>
    /// 分页帮助类
    /// </summary>
    public static class PagerHelper
    {
#if DEBUG
        static string __render_pager_html__(int page, int pageCount, int span, string prefix, string post)
        {
            var builder = new StringBuilder();
            builder.Append(prefix);

            var head = page - span;
            var tail = page + span;

            //render 1
            if (head > 1 + 1)
            {
                //reader ...
            }
            for (var i = Math.Max(1 + 1, head); i <= Math.Min(pageCount - 1, tail); ++i)
            {
                //render i
            }
            if (tail < pageCount - 1)
            {
                //render ...
            }
            //render page count

            builder.Append(post);
            var res = builder.ToString();
            return res;
        }
#endif

        /// <summary>
        /// 计算总页数
        /// </summary>
        public static int GetPageCount(int item_count, int page_size)
        {
            if (item_count <= 0) { return 0; }
            page_size.Should().BeGreaterOrEqualTo(1, "pagesize不能小于1");

            var res = item_count % page_size == 0 ?
                (item_count / page_size) :
                (item_count / page_size + 1);
            return res;
        }

        /// <summary>
        /// 计算mysql 的limit参数
        /// </summary>
        public static (int skip, int take) GetQueryRange(int page, int page_size)
        {
            page.Should().BeGreaterOrEqualTo(1, "页码不能小于1");
            page_size.Should().BeGreaterOrEqualTo(1, "pagesize不能小于1");

            var skip = (page - 1) * page_size;
            var take = page_size;

            return (skip, take);
        }

        public static PagerData<Result> MapPagerData<T, Result>(this PagerData<T> data, Func<T, Result> selector)
        {
            data.DataList.Should().NotBeNull();

            var res = new PagerData<Result>()
            {
                Page = data.Page,
                PageSize = data.PageSize,
                ItemCount = data.ItemCount,
                ExtData = data.ExtData,
                DataList = data.DataList.Select(selector).ToArray()
            };

            return res;
        }
    }
}
