using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.extension;
using Microsoft.EntityFrameworkCore;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    public static class RepositoryExtension
    {

        public static async Task RemoveByUIDs<T>(this IEFRepository<T> repo, string[] uids) where T : EntityBase, ILogicalDeletion
        {
            uids.Should().NotBeNullOrEmpty();

            var db = repo.Database;

            var data = await db.Set<T>().Where(x => uids.Contains(x.Id)).ToArrayAsync();

            if (data.Any())
            {
                foreach (var m in data)
                {
                    m.IsDeleted = (int)YesOrNoEnum.YES;
                }

                await db.SaveChangesAsync();
            }
        }

        public static async Task RecoverByUIDs<T>(this IEFRepository<T> repo, string[] uids) where T : EntityBase, ILogicalDeletion
        {
            uids.Should().NotBeNullOrEmpty();

            var db = repo.Database;

            var data = await db.Set<T>().IgnoreQueryFilters()
                .Where(x => x.IsDeleted > 0)
                .Where(x => uids.Contains(x.Id)).ToArrayAsync();

            if (data.Any())
            {
                foreach (var m in data)
                {
                    m.IsDeleted = (int)YesOrNoEnum.No;
                }

                await db.SaveChangesAsync();
            }
        }

        public static async Task<IEnumerable<T>> QueryByMaxID<T>(this IQueryable<T> q, int max_id, int count)
            where T : IIncID
        {
            count.Should().BeInRange(1, 5000);

            var query = q;

            query = query.Where(x => x.IncID > max_id).OrderBy(x => x.IncID).Take(count);

            var res = await query.ToArrayAsync();

            return res;
        }

        public static async Task<IEnumerable<T>> QueryByMinID<T>(this IQueryable<T> q, int? min_id, int count)
            where T : IIncID
        {
            count.Should().BeInRange(1, 5000);

            var query = q;

            if (min_id != null)
            {
                query = query.Where(x => x.IncID < min_id.Value);
            }

            query = query.OrderByDescending(x => x.IncID).Take(count);

            var res = await query.ToArrayAsync();

            return res;
        }

        public static async IAsyncEnumerable<T> Batch<T>(this IQueryable<T> query, int batch_size)
            where T : IIncID
        {
            var max_id = -1L;
            while (true)
            {
                var list = await query
                                    .Where(x => x.IncID > max_id)
                                    .OrderBy(x => x.IncID).Take(batch_size).ToArrayAsync();
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
    }
}
