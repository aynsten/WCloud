using FluentAssertions;
using Lib.extension;
using System.Collections.Generic;
using WCloud.Framework.Database.Abstractions.Entity;

namespace System.Linq
{
    public static class QueryableExtension
    {
        public static IQueryable<T> TakeUpTo5000<T>(this IQueryable<T> query)
        {
            query = query.Take(5000);
            return query;
        }

        public static IEnumerable<T> QueryByMaxID<T>(this IQueryable<T> q, int max_id, int count)
            where T : IIncID
        {
            count.Should().BeInRange(1, 5000);

            var query = q;

            query = query.Where(x => x.IncID > max_id).OrderBy(x => x.IncID).Take(count);

            var res = query.ToArray();

            return res;
        }

        public static IEnumerable<T> QueryByMinID<T>(this IQueryable<T> q, int? min_id, int count)
            where T : IIncID
        {
            count.Should().BeInRange(1, 5000);

            var query = q;

            if (min_id != null)
            {
                query = query.Where(x => x.IncID < min_id.Value);
            }

            query = query.OrderByDescending(x => x.IncID).Take(count);

            var res = query.ToArray();

            return res;
        }

        public static IEnumerable<T> BatchByMaxID<T>(this IQueryable<T> query, int batch_size) where T : IIncID
        {
            var max_id = -1;
            while (true)
            {
                var list = query
                    .Where(x => x.IncID > max_id)
                    .OrderBy(x => x.IncID).Take(batch_size).ToArray();
                if (!list.Any())
                {
                    break;
                }

                foreach (var m in list)
                {
                    yield return m;
                }

                max_id = list.Max(x => x.IncID);
            }
        }

        public static IEnumerable<T> BatchByPaging<T>(this IQueryable<T> query, int batch_size) where T : EntityBase
        {
            var page = 1;
            while (true)
            {
                var list = query.OrderByDescending(x => x.CreateTimeUtc).QueryPage(page, batch_size).ToArray();
                if (!list.Any())
                {
                    break;
                }

                foreach (var m in list)
                {
                    yield return m;
                }

                ++page;
            }
        }
    }
}
