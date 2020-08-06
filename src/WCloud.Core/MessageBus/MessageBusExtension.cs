using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WCloud.Core.MessageBus
{
    public static class MessageBusExtension
    {
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

        public static string GetMessageBusProvider(this IConfiguration config)
        {
            var res = config["message_bus_provider"];
            return res?.ToLower();
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
                foreach (var i in all_ins)
                {
                    services.AddTransient(i, implementationType: m);
                }
            }
            return services;
        }
    }
}
