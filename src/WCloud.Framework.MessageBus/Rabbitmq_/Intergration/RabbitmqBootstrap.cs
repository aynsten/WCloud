using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Intergration
{
    public static class RabbitmqBootstrap
    {
        internal static IServiceCollection AddRabbitmqMessageBus(this IServiceCollection services)
        {
            services.AddScoped<IMessagePublisher, RabbitmqMessagePublisher>();
            services.AddSingleton<IConsumerStartor, RabbitmqMessageConsumerStartor>();

            __reg_rabbitmq_consumers__(services);

            return services;
        }

        static void __reg_rabbitmq_consumers__(this IServiceCollection services)
        {
            var mapping = services.ResolveConsumerMapping();

            var message_types = mapping.Select(x => x.MessageType).Distinct().ToArray();

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
