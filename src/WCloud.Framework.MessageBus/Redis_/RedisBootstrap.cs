using FluentAssertions;
using Lib.helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace WCloud.Framework.MessageBus.Redis_
{
    public static class RedisBootstrap
    {
        internal static IServiceCollection AddRedisMessageBus(this IServiceCollection services, IConfiguration config, Type[] consumer_types = null)
        {
            services.AddScoped<IMessagePublisher, RedisMessagePublisher>();
            services.AddSingleton<IConsumerStartor, RedisConsumerStartor>();
            services.AddSingleton<IRedisDatabaseSelector, RedisDatabaseSelector>();

            if (ValidateHelper.IsNotEmpty(consumer_types))
            {
                __reg_redis_consumers__(services, consumer_types);
            }

            return services;
        }

        static void __reg_redis_consumers__(this IServiceCollection services, Type[] all_types)
        {
            all_types.Should().NotBeNull();

            var message_types = all_types.Select(x => x.__get_message_type__()).ToArray();

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
