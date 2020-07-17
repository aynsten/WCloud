using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Lib.ioc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus
{
    public static class MessageBusExtension
    {
        public static WCloud.Framework.MessageBus.Config.RabbitMQ GetRabbitmqOrThrow(this IConfiguration config)
        {
            var section = "rabbit";
            var rabbit = new WCloud.Framework.MessageBus.Config.RabbitMQ()
            {
                ServerAndPort = config[$"{section}:server"],
                User = config[$"{section}:user"],
                Password = config[$"{section}:pass"]
            };

            if (ValidateHelper.IsEmpty(rabbit.ServerAndPort))
                throw new ArgumentNullException(nameof(rabbit.ServerAndPort));

            if (ValidateHelper.IsEmpty(rabbit.User))
                throw new ArgumentNullException(nameof(rabbit.User));

            if (ValidateHelper.IsEmpty(rabbit.Password))
                throw new ArgumentNullException(nameof(rabbit.Password));

            return rabbit;
        }

        public static async Task PublishAsync<T>(this IMessagePublisher publisher, T model, CancellationToken cancellationToken = default) where T : class, IMessageBody
        {
            await publisher.PublishAsync(key: typeof(T).FullName, model, cancellationToken);
        }

        public static Type[] FindMessageConsumers(this IEnumerable<Assembly> ass)
        {
            var all_types = ass.GetAllTypes()
                .Where(x => x.IsNormalClass())
                .Where(x => !x.IsGenericType)
                .Where(x => x.IsAssignableTo_<IMessageConsumerFinder>())
                .Where(x => x.GetInterfaces().Any(d => d.IsGenericType_(typeof(IMessageConsumer<>))))
                .ToArray();
            return all_types;
        }

        public static ConsumerDescriptor[] FindMessageConsumers_(this IEnumerable<Assembly> ass)
        {
            var all_types = FindMessageConsumers(ass);

            var res = all_types.Select(x => new ConsumerDescriptor(x)).ToArray();

            return res;
        }

        public static Type __get_message_type__(this Type t)
        {
            var ins = t.GetInterfaces().Where(x => x.IsGenericType_(typeof(IMessageConsumer<>))).FirstOrDefault();
            ins.Should().NotBeNull("not a consumer type");
            var generic_type = ins.GetGenericArguments().FirstOrDefault();
            generic_type.Should().NotBeNull("oops,consumer type should have a generic parameter");
            return generic_type;
        }

        public static QueueConfigAttribute __get_config__(this Type t)
        {
            var queue_config = t.GetCustomAttributes_<QueueConfigAttribute>().FirstOrDefault();
            queue_config.Should().NotBeNull("consumer should have a config attribute");
            return queue_config;
        }

        public static IMessageConsumer<T>[] ResolveConsumerOptional<T>(this IServiceProvider provider) where T : class, IMessageBody
        {
            var res = provider.ResolveAll_<IMessageConsumer<T>>();
            return res;
        }

        public static IServiceCollection AddMessageConsumers(this IServiceCollection services, IEnumerable<Type> all_types)
        {
            //加入ioc
            foreach (var m in all_types)
            {
                services.AddTransient(m);
                var all_ins = m.GetInterfaces();
                if (all_ins.Any())
                {
                    foreach (var i in all_ins)
                    {
                        services.AddTransient(i, implementationType: m);
                    }
                }
            }
            return services;
        }
    }
}
