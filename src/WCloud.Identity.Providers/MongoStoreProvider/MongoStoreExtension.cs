using System;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WCloud.Identity.Providers.MongoStoreProvider.StoreProvider;

namespace WCloud.Identity.Providers.MongoStoreProvider
{
    public static class MongoStoreExtension
    {
        public static IIdentityServerBuilder AddMongoPersistedGrantStore(this IIdentityServerBuilder builder)
        {
            builder.Services.RemoveAll<IPersistedGrantStore>();
            builder.AddPersistedGrantStore<MongoPersistedGrantStore>();
            return builder;
        }
    }
}
