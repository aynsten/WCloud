using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.MessageBus;
using WCloud.Framework.MessageBus.Memory;
using WCloud.Framework.MessageBus.Rabbitmq_;
using WCloud.Framework.MessageBus.Rabbitmq_.Intergration;
using WCloud.Framework.MessageBus.Redis_;

namespace WCloud.Framework.MessageBus
{
    [QueueConfig("timer_test")]
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
            else if (provider == "memory")
            {
                services.AddMemoryMeesageBus();
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
    }
}
