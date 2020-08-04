using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Lib.events
{
    public static class EventPublishBootstrap
    {
        /// <summary>
        /// 使用内存版本
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="search_in_assembly"></param>
        /// <returns></returns>
        public static IServiceCollection UseInMemoryEventBus(this IServiceCollection collection, Assembly[] search_in_assembly)
        {
            if (ValidateHelper.IsEmpty(search_in_assembly))
                throw new ArgumentNullException(nameof(search_in_assembly));

            collection.AddSingleton<IEventPublisher, EventPublisher>();

            foreach (var t in search_in_assembly.GetAllTypes())
            {
                if (!t.IsNormalClass())
                    continue;
                var interfaces = t.GetInterfaces();
                var matched_interfaces = interfaces.Where(x => x.IsGenericType_(typeof(IConsumer<>))).ToList();
                if (!matched_interfaces.Any())
                    continue;

                foreach (var c in matched_interfaces)
                    collection.AddSingleton(c, t);
            }

            return collection;
        }
    }
}
