using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using WCloud.Core.Cache;
using WCloud.Core.DataSerializer;
using WCloud.Core.PollyExtension;

namespace WCloud.Core
{
    public static class CoreExtensions
    {
        public static IServiceCollection AddWCloudCore(this IServiceCollection collection)
        {
            //上下文
            var deft = typeof(DefaultWCloudContext<>).MakeGenericType(typeof(CoreModule));
            collection.AddScoped(typeof(IWCloudContext), deft);
            collection.AddScoped(typeof(IWCloudContext<>), typeof(DefaultWCloudContext<>));
            //序列化
            collection.AddSingleton<IDataSerializer, DefaultDataSerializer>();
            collection.AddSingleton<IStrategyFactory, StrategyFactory>();
            //缓存
            collection.AddScoped<ICacheKeyManager, CacheKeyManager>();
            collection.RemoveAll<ICacheProvider>().AddTransient<ICacheProvider, DistributeCacheProvider>();
            collection.AddMemoryCache().AddDistributedMemoryCache();
            return collection;
        }

        public static IWCloudContext<T> ResolveWCloudContext<T>(this IServiceProvider provider)
        {
            var res = provider.Resolve_<IWCloudContext<T>>();
            return res;
        }

        public static string GetAppName(this IConfiguration config) => config["app_name"];

        public static bool InitDatabaseRequired(this IConfiguration config)
        {
            var res = config["init_db"]?.ToBool() ?? false;
            return res;
        }

        public static string GetIdentityServerAddressOrThrow(this IConfiguration config)
        {
            var identity_server = config["identity_server"];
            identity_server.Should().NotBeNullOrEmpty($"请配置{nameof(identity_server)}");

            identity_server.EndsWith("/").Should().BeFalse("identity server后面不要加斜杠");

            return identity_server;
        }

        public static string GetInternalApiGatewayAddressOrThrow(this IConfiguration config)
        {
            var res = config["internal_api_gateway"];
            res.Should().NotBeNullOrEmpty();
            res.StartsWith("http", StringComparison.OrdinalIgnoreCase).Should().BeTrue();
            return res;
        }
    }
}
