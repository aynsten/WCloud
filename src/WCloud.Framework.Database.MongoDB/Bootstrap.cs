using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace WCloud.Framework.Database.MongoDB
{
    public static class MongoBootstrap
    {
        public static IServiceCollection UseMongoDB(this IServiceCollection collection,
            string database_name, string connection_string)
        {
            var client = new MongoClient(MongoClientSettings.FromConnectionString(connection_string));

            UseMongoDB(collection, database_name, client);

            return collection;
        }

        public static IServiceCollection UseMongoDB(this IServiceCollection collection, string database_name, IMongoClient client)
        {
            var connection = new MongoConnectionWrapper(client, database_name);

            collection.AddDisposableSingleInstanceService(connection);

            return collection;
        }
    }
}
