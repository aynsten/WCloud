using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lib.extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus
{
    public static class MessageBusExtension
    {
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
