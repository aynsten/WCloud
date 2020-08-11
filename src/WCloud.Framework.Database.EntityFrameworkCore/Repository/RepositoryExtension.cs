using FluentAssertions;
using Lib.extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    public static class RepositoryExtension
    {

        public static async Task RemoveByUIDs<T>(this IEFRepository<T> repo, string[] uids) where T : EntityBase, ILogicalDeletion
        {
            uids.Should().NotBeNullOrEmpty();

            var db = repo.Database;

            var data = await db.Set<T>().Where(x => uids.Contains(x.UID)).ToArrayAsync();

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
                .Where(x => uids.Contains(x.UID)).ToArrayAsync();

            if (data.Any())
            {
                foreach (var m in data)
                {
                    m.IsDeleted = (int)YesOrNoEnum.No;
                }

                await db.SaveChangesAsync();
            }
        }
    }
}
