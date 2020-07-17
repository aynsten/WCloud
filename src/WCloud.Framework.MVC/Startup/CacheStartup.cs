using FluentAssertions;
using Lib.cache;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core;

namespace WCloud.Framework.Startup
{
    public static class CacheStartup
    {
        public static IServiceCollection AddCacheProvider_(this IServiceCollection collection)
        {
            collection.AddTransient<ICacheProvider, DistributeCacheProvider>();

            return collection;
        }

        public static IServiceCollection AddRedisCacheProvider_(this IServiceCollection collection, string redis_str)
        {
            redis_str.Should().NotBeNullOrEmpty();

            collection.AddStackExchangeRedisCache(option =>
            {
                option.InstanceName = "rds-";
                //option.Configuration = redis_str;

                option.ConfigurationOptions ??= new StackExchange.Redis.ConfigurationOptions();

                option.ConfigurationOptions.EndPoints.Add(redis_str);

                var db = (int)ConfigSet.Redis.缓存;
                option.ConfigurationOptions.DefaultDatabase = db;
            });

            return collection;
        }
    }
}
