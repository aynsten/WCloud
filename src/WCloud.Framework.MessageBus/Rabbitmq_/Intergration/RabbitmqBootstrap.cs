using FluentAssertions;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Intergration
{
    public static class RabbitmqBootstrap
    {
        internal static IServiceCollection AddRabbitmqMessageBus(this IServiceCollection services, Type[] consumer_types = null)
        {
            services.AddScoped<IMessagePublisher, RabbitmqMessagePublisher>();
            services.AddSingleton<IConsumerStartor, RabbitmqMessageConsumerStartor>();

            if (ValidateHelper.IsNotEmpty(consumer_types))
            {
                __reg_rabbitmq_consumers__(services, consumer_types);
            }

            return services;
        }

        static void __reg_rabbitmq_consumers__(this IServiceCollection services, Type[] all_types)
        {
            all_types.Should().NotBeNull();

            var message_types = all_types.Select(x => x.__get_message_type__()).ToArray();

            Type __build_redis_consumer__(Type message_type)
            {
                var res = typeof(RabbitmqMessageConsumer<>).MakeGenericType(message_type);
                return res;
            }

            //创建redis consumer
            var basic_redis_consumer_types = message_types.Select(__build_redis_consumer__).ToArray();

            //注册redis consumer
            foreach (var m in basic_redis_consumer_types)
            {
                services.AddSingleton(typeof(IRabbitMqConsumer), m);
                services.AddSingleton(m);
            }
        }
    }
}
