using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;

namespace WCloud.Framework.Database.MongoDB
{
    public static class MongoBootstrap
    {
        public static IServiceCollection UseMongoDB(this IServiceCollection collection,
            string database_name, string connection_string)
        {
            var client = new MongoClient(MongoClientSettings.FromConnectionString(connection_string));
            var connection = new MongoConnectionWrapper(client, database_name);

            collection.AddDisposableSingleInstanceService(connection);

            return collection;
        }

        public static IServiceCollection UseMongoRepositoryFromIoc(this IServiceCollection collection) =>
            collection.UseMongoRepository(typeof(MongoRepository<>));

        public static IServiceCollection UseMongoRepository(this IServiceCollection collection, Type repoType)
        {
            if (repoType == null)
                throw new ArgumentNullException(nameof(repoType));
            if (!repoType.IsGenericType)
                throw new ArgumentException("mongo repository type must be generic type");

            collection.AddTransient(typeof(IMongoRepository<>), repoType);
            return collection;
        }
    }
}
