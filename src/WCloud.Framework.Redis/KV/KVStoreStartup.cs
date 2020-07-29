using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WCloud.Framework.Redis.KV
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
