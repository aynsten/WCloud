using WCloud.Framework.Database.MongoDB;

namespace WCloud.Framework.Storage.MongoDB
{
    public class MongoUploadProviderConnectionWrapper : MongoConnectionWrapper
    {
        public MongoUploadProviderConnectionWrapper(string connection_string, string database_name) : base(connection_string, database_name)
        { }
    }
}
