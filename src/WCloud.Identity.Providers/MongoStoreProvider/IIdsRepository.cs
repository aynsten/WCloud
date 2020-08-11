using WCloud.Framework.Database.MongoDB;
using System;

namespace WCloud.Identity.Providers.MongoStoreProvider
{
    public interface IIdsRepository<T> : IMongoRepository<T> where T : MongoEntityBase
    {
        //
    }

    internal class IdsRepository<T> : MongoRepository<T>, IIdsRepository<T> where T : MongoEntityBase
    {
        public IdsRepository(IServiceProvider provider, IdsMongoConnectionWrapper wrapper) : base(provider, wrapper)
        { }
    }
}
