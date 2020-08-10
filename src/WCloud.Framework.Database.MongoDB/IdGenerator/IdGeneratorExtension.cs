using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;

namespace WCloud.Framework.Database.MongoDB.IdGenerator
{
    public static class IdGeneratorExtension
    {
        public static async Task EnsureIdGeneratorIndex<T>(this IMongoRepository<T> repo, string index_name) where T : IdGeneratorEntityBase
        {
            index_name.Should().NotBeNullOrEmpty();

            await repo.Collection.Indexes.DropAllAsync();

            var keys = Builders<T>.IndexKeys.Ascending(x => x.Category);
            var options = new CreateIndexOptions()
            {
                Name = index_name,
                Unique = true,
            };

            await repo.Collection.Indexes.CreateOneAsync(new CreateIndexModel<T>(keys, options));
        }

        public static async Task<int> GetMaxID<T>(this IMongoRepository<T> repo, string category)
            where T : IdGeneratorEntityBase
        {
            category.Should().NotBeNullOrEmpty();

            var builder = new UpdateDefinitionBuilder<T>();
            var update = builder.Inc(x => x.MaxID, 1);
            var res = await repo.Collection.FindOneAndUpdateAsync(x => x.Category == category, update);

            res.Should().NotBeNull($"记录maxid的记录不存在:{category}");

            return res.MaxID;
        }
    }
}
