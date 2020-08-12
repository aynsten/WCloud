using FluentAssertions;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.MongoDB.Extension
{
    public static class RepositoryExtension
    {
        public static async Task RemoveByIdAsync<T>(this IMongoRepository<T> repo, string[] uids) where T : EntityBase, ILogicalDeletion
        {
            uids.Should().NotBeNullOrEmpty();

            var collection = repo.Collection;

            var filter = Builders<T>.Filter.Where(x => uids.Contains(x.Id) && x.IsDeleted <= 0);
            var update = Builders<T>.Update.Set(x => x.IsDeleted, 1);

            var res = await collection.UpdateManyAsync(filter, update);
            res.IsAcknowledged.Should().BeTrue();
        }

        public static async Task RecoverByIdAsync<T>(this IMongoRepository<T> repo, string[] uids) where T : EntityBase, ILogicalDeletion
        {
            uids.Should().NotBeNullOrEmpty();

            var collection = repo.Collection;

            var filter = Builders<T>.Filter.Where(x => uids.Contains(x.Id) && x.IsDeleted > 0);
            var update = Builders<T>.Update.Set(x => x.IsDeleted, 0);

            var res = await collection.UpdateManyAsync(filter, update);
            res.IsAcknowledged.Should().BeTrue();
        }
    }
}
