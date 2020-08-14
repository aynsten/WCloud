using FluentAssertions;
using Lib.data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using WCloud.Core;
using WCloud.Framework.Redis.implement;

namespace WCloud.Framework.Redis
{
    public static class RedisBootstrap
    {
        public static RedisConnectionWrapper GetRedisClientWrapper(this IServiceCollection collection, bool nullable = false)
        {
            var res = collection.GetSingletonInstanceOrNull<RedisConnectionWrapper>();
            if (!nullable)
            {
                res.Should().NotBeNull("请先注册redis链接");
            }
            return res;
        }

        public static RedisConnectionWrapper GetRedisClientWrapper(this IServiceProvider provider)
        {
            var res = provider.Resolve_<RedisConnectionWrapper>();
            res.Should().NotBeNull();
            return res;
        }

        public static IServiceCollection AddRedisClient(this IServiceCollection collection, IConfiguration config)
        {
            var redis_wrapper = collection.GetRedisClientWrapper(true);
            redis_wrapper.Should().NotBeNull();

            var connection_string = config.GetRedisConnectionString();

            var connection = new RedisConnectionWrapper(connection_string);
            collection.AddDisposableSingleInstanceService(connection);

            return collection;
        }

        public static IServiceCollection AddRedisHelper(this IServiceCollection collection)
        {
            collection.RemoveAll<IRedisAll>();
            collection.AddScoped<IRedisAll>(provider =>
            {
                var redis_wrapper = provider.GetRedisClientWrapper();
                var kv_db = (int)ConfigSet.Redis.KV存储;
                var serializer = provider.Resolve_<ISerializeProvider>();
                var helper = new RedisHelper(redis_wrapper.Connection, kv_db, serializer);
                return helper;
            });
            return collection;
        }

        public static IServiceCollection AddRedisDistributedCacheProvider_(this IServiceCollection collection)
        {
            var redis_wrapper = collection.GetRedisClientWrapper();

            var redis_str = redis_wrapper.ConnectionString;
            redis_str.Should().NotBeNullOrEmpty();

            collection.RemoveAll<IDistributedCache>();

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

        public static IServiceCollection AddRedisDataProtectionKeyStore(this IServiceCollection collection, IConfiguration config)
        {
            var redis_wrapper = collection.GetRedisClientWrapper();

            var app_name = config.GetAppName() ?? "shared_app";

            collection.RemoveAll<IDataProtectionBuilder>()
                .RemoveAll<IDataProtectionProvider>()
                .RemoveAll<IDataProtector>();

            var builder = collection
                .AddDataProtection()
                .SetApplicationName(applicationName: app_name)
                .AddKeyManagementOptions(option =>
                {
                    option.AutoGenerateKeys = true;
                    option.NewKeyLifetime = TimeSpan.FromDays(1000);
                });

            //加密私钥
            var db = (int)ConfigSet.Redis.加密KEY;
            builder.PersistKeysToStackExchangeRedis(
                () => redis_wrapper.Connection.SelectDatabase(db),
                $"data_protection_key:{app_name}");

            return collection;
        }
    }
}
