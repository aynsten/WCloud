using System;
using System.Collections.Generic;
using System.Text;
using Lib.helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WCloud.Framework.Redis
{
    public static class KVStoreStartup
    {
        public static IServiceCollection AddRedisKVStore(this IServiceCollection collection, IConfiguration config)
        {
            collection.AddSingleton<IKVStore, RedisKVStore>();
            return collection;
        }
    }
}
