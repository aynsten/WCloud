using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lib.data;
using WCloud.Framework.Database.Abstractions;

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
    }
}
