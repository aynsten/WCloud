using System;
using MongoDB.Driver;
using WCloud.Framework.Database.MongoDB;

namespace WCloud.Identity.Providers.MongoStoreProvider
{
    public class IdsMongoConnectionWrapper : MongoConnectionWrapper
    {
        public IdsMongoConnectionWrapper(IMongoClient client, string database_name) : base(client, database_name)
        {
        }
    }
}
