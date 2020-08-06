using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Redis_
{
    public static class RedisBootstrap
    {
        internal static IServiceCollection AddRedisMessageBus(this IServiceCollection services)
        {
            services.AddScoped<IMessagePublisher, RedisMessagePublisher>();
            services.AddSingleton<IConsumerStartor, RedisConsumerStartor>();
            services.AddSingleton<IRedisDatabaseSelector, RedisDatabaseSelector>();

            __reg_redis_consumers__(services);

            return services;
        }

        static void __reg_redis_consumers__(this IServiceCollection services)
        {
            var mapping = services.ResolveConsumerMapping();

            var message_types = mapping.Select(x => x.MessageType).Distinct().ToArray();

            Type __build_redis_consumer__(Type message_type)
            {
                var res = typeof(RedisConsumer<>).MakeGenericType(message_type);
                return res;
            }

            //创建redis consumer
            var basic_redis_consumer_types = message_types.Select(__build_redis_consumer__).ToArray();

            //注册redis consumer
            foreach (var m in basic_redis_consumer_types)
            {
                services.AddSingleton(typeof(IRedisConsumer), m);
                services.AddSingleton(m);
            }
        }

    }
}
