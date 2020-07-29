using FluentAssertions;
using Lib.extension;
using Lib.ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
            //注册消息和配置的mapping
            var mapping = all_consumer_types.Select(x => new ConsumerDescriptor(x)).ToList().AsReadOnly();
            services.AddSingleton(mapping);
            services.AddSingleton<IReadOnlyCollection<ConsumerDescriptor>>(mapping);

            //注册消费实现
            var provider = config.GetMessageBusProvider();
            if (provider == "redis")
            {
                //使用redis
                services.AddRedisMessageBus();
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
                //注册所有消费
                //services.AddRabbitmqConsumers(new Assembly[] { });
                services.AddRabbitMq(option).AddRabbitmqMessageBus();
            }

            return services;
        }

        public static IReadOnlyCollection<ConsumerDescriptor> ResolveConsumerMapping(this IServiceProvider provider)
        {
            var res = provider.Resolve_<IReadOnlyCollection<ConsumerDescriptor>>();
            return res;
        }

        public static ConsumerDescriptor ResolveMessageTypeMapping<MessageType>(this IServiceProvider provider) where MessageType : class, IMessageBody
        {
            var mapping = provider.ResolveConsumerMapping();
            var res = mapping.FirstOrDefault(x => x.MessageType == typeof(MessageType));
            res.Should().NotBeNull();
            return res;
        }

        public static IReadOnlyCollection<ConsumerDescriptor> ResolveConsumerMapping(this IServiceCollection services)
        {
            var res = services.GetSingletonInstanceOrNull<IReadOnlyCollection<ConsumerDescriptor>>();
            res.Should().NotBeNull();
            return res;
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
