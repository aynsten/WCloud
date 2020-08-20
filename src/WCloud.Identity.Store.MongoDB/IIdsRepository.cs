using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.MongoDB;
using WCloud.Identity.Providers;
using WCloud.Identity.Store.MongoDB.Entity;

namespace WCloud.Identity.Store.MongoDB
{
    public interface IIdsRepository<T> : IMongoRepository<T> where T : EntityBase
    {
        //
    }

    internal class IdsRepository<T> : MongoRepository<T, IdsMongoConnectionWrapper>, IIdsRepository<T> where T : EntityBase
    {
        public IdsRepository(IServiceProvider provider) : base(provider)
        { }
    }

    public class IdsMongoDatabaseHelper : IIdentityServerDatabaseHelper
    {
        private readonly IWCloudContext _context;
        public IdsMongoDatabaseHelper(IWCloudContext<IdsMongoDatabaseHelper> _context)
        {
            this._context = _context;
        }

        public async Task CreateDatabase()
        {
            var repo = this._context.Provider.Resolve_<IIdsRepository<MongoPersistedGrantEntity>>();

            var names = repo.Collection.GetAllIndexNames();

            var index_name = "__grants_key__";

            if (!names.Contains(index_name))
            {
                var index = Builders<MongoPersistedGrantEntity>.IndexKeys.Descending(x => x.Key);
                var option = new CreateIndexOptions()
                {
                    //Unique = true,
                    Name = index_name
                };
                await repo.Collection.Indexes.CreateOneAsync(new CreateIndexModel<MongoPersistedGrantEntity>(index, option));
            }
        }
    }
}
