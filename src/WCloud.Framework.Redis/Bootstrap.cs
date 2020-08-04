using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using WCloud.Framework.Redis.implement;

namespace WCloud.Framework.Redis
{
    public static class RedisBootstrap
    {
        class RedisBootstrap__ { }

        static void __add_error_log__(Exception e = null, string msg = null)
        {
            using (var s = IocContext.Instance.Scope())
            {
                var logger = s.ServiceProvider.Resolve_<ILogger<RedisBootstrap__>>();
                logger.AddErrorLog(msg: msg, e: e);
            }
        }

        public static IServiceCollection AddRedisWrapper(this IServiceCollection collection, string connection_string, int db,
            Action<IConnectionMultiplexer> config = null)
        {
            var pool = ConnectionMultiplexer.Connect(connection_string);
            if (config == null)
            {
                config = (x) =>
                {
                    x.ConnectionFailed += (sender, e) => __add_error_log__(msg: "Redis连接失败:", e: e.Exception);
                    x.ConnectionRestored += (sender, e) => __add_error_log__(msg: "redis连接恢复正常");
                    x.ErrorMessage += (sender, e) => __add_error_log__(msg: "Redis-ErrorMessage");
                    x.InternalError += (sender, e) => __add_error_log__(msg: "Redis内部错误", e: e.Exception);
                };
            }
            config.Invoke(pool);

            var connection = new RedisConnectionWrapper(pool);
            collection.AddDisposableSingleInstanceService(connection);

            //all of them above
            collection.AddSingleton<IRedisAll>(x =>
            {
                var option = x.Resolve_<RedisConnectionWrapper>();
                return new RedisHelper(option.Connection, db);
            });

            return collection;
        }

        public static IServiceCollection AddRedisHelper(this IServiceCollection collection, int db)
        {
            return collection;
        }
    }
}
