using FluentAssertions;
using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using WCloud.Core.MessageBus;
using WCloud.Framework.MessageBus.Rabbitmq_;
using WCloud.Framework.MessageBus.Rabbitmq_.Intergration;
using WCloud.Framework.MessageBus.Redis_;

namespace WCloud.Framework.MessageBus
{
    public class TimeMessage : IMessageBody
    {
        public DateTime TimeUtc { get; set; }
    }

    public static class Bootstrap
    {
        public static IServiceCollection AddMessageBus_(this IServiceCollection services, IConfiguration config, Assembly[] consumer_ass = null)
        {
            config.Should().NotBeNull();

            consumer_ass ??= new Assembly[] { };

            //找到所有消费
            var all_consumer_types = consumer_ass.FindMessageConsumers();
            //注册消费
            services.AddMessageConsumers(all_consumer_types);

            var provider = config.GetMessageBusProvider();
            if (provider == "redis")
            {
                //使用redis
                services.AddRedisMessageBus(config, consumer_types: all_consumer_types);
            }
            else if (provider == "kafka")
            {
                throw new NotImplementedException();
            }
            else
            {
                //使用rabbitmq
                var rabbit_config = config.GetRabbitmqOrThrow();
                var option = new RabbitMqOption()
                {
                    HostName = rabbit_config.ServerAndPort,
                    UserName = rabbit_config.User,
                    Password = rabbit_config.Password
                };
                services.AddRabbitMq(option).AddRabbitmqMessageBus(consumer_types: all_consumer_types);
            }

            return services;
        }

        public static IServiceProvider StartMessageBusConsumer_(this IServiceProvider provider)
        {
            var logger = provider.Resolve_<ILogger<IConsumerStartor>>();

            var all_started = true;

            var startors = provider.ResolveAll_<IConsumerStartor>();
            foreach (var m in startors)
            {
                try
                {
                    m.StartComsume();
                }
                catch (Exception e)
                {
                    all_started = false;
                    logger.AddErrorLog("开启消费失败", e);
                }
            }

            if (all_started)
            {
                logger.LogInformation("消费启动完成");
            }
            else
            {
                logger.LogWarning("部分或全部消息消费启动失败");
            }

            return provider;
        }
    }
}
