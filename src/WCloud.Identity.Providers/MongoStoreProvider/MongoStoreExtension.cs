﻿using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using WCloud.Identity.Providers.MongoStoreProvider.StoreProvider;

namespace WCloud.Identity.Providers.MongoStoreProvider
{
    public static class MongoStoreExtension
    {
        public static IIdentityServerBuilder AddMongoPersistedGrantStore(this IIdentityServerBuilder builder, string connection_string, string database_name)
        {
            var client = new MongoClient(MongoClientSettings.FromConnectionString(connection_string));
            builder.Services.AddDisposableSingleInstanceService(new IdsMongoConnectionWrapper(client, database_name));
            builder.Services.AddTransient(typeof(IIdsRepository<>), typeof(IdsRepository<>));

            builder.Services.RemoveAll<IPersistedGrantStore>();
            builder.AddPersistedGrantStore<MongoPersistedGrantStore>();
            return builder;
        }
    }
}
