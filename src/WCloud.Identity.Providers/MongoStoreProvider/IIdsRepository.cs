using WCloud.Framework.Database.MongoDB;
using System;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Identity.Providers.MongoStoreProvider
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
}
