using System;
using WCloud.Framework.Database.MongoDB;

namespace WCloud.Identity.Providers.MongoStoreProvider
{
    public interface IIdsRepository<T> : IMongoRepository<T> where T : MongoEntityBase
    {
        //
    }
}
