using MongoDB.Driver;
using WCloud.Framework.Database.MongoDB;

namespace WCloud.Identity.Store.MongoDB
{
    public class IdsMongoConnectionWrapper : MongoConnectionWrapper
    {
        public IdsMongoConnectionWrapper(IMongoClient client, string database_name) : base(client, database_name)
        {
        }
    }
}
