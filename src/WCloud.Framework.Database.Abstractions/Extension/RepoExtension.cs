using FluentAssertions;
using Lib.core;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions;
using WCloud.Framework.Database.Abstractions.Entity;

namespace System.Linq
{
    public static class RepoExtension
    {
        /// <summary>
        /// 获取list
        /// </summary>
        public static List<T> QueryMany<T>(this IRepository<T> repo, Expression<Func<T, bool>> where, int? count = null) where T : class, IDBTable
        {
            var res = repo.QueryMany<object>(where: where, count: count);
            return res.ToList();
        }

        /// <summary>
        /// 异步获取list
        /// </summary>
        public static async Task<List<T>> QueryManyAsync<T>(this IRepository<T> repo, Expression<Func<T, bool>> where, int? count = null) where T : class, IDBTable
        {
            var res = await repo.QueryManyAsync<object>(where: where, count: count);
            return res.ToList();
        }

        /// <summary>
        /// check input=>delete by uids
        /// </summary>
        public static async Task DeleteByIds<T>(this IRepository<T> repo, string[] uids) where T : EntityBase
        {
            uids.Should().NotBeNullOrEmpty();
            foreach (var uid in uids)
            {
                uid.Should().NotBeNullOrEmpty("批量删除数据：uid为空");
            }

            await repo.DeleteWhereAsync(x => uids.Contains(x.Id));
        }
    }
}
