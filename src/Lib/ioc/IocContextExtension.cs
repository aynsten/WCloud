using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Lib.ioc
{
    public static class IocContextExtension
    {
        /// <summary>
        /// 作为顶级容器
        /// </summary>
        /// <param name="provider"></param>
        public static IServiceProvider SetAsRootServiceProvider(this IServiceProvider provider)
        {
            provider.Should().NotBeNull();

            IocContext.Instance.SetRootContainer(provider);
            return provider;
        }

        public static T Resolve_<T>(this IServiceProvider provider)
        {
            provider.Should().NotBeNull();

            var res = provider.GetRequiredService<T>();
            return res;
        }

        public static T ResolveOptional_<T>(this IServiceProvider provider)
        {
            provider.Should().NotBeNull();

            var res = provider.GetService<T>();
            return res;
        }

        public static T[] ResolveAll_<T>(this IServiceProvider provider)
        {
            provider.Should().NotBeNull();

            var res = provider.GetServices<T>().ToArray();
            return res;
        }

        public static IConfiguration ResolveConfig_(this IServiceProvider provider)
        {
            provider.Should().NotBeNull();

            var res = provider.Resolve_<IConfiguration>();
            return res;
        }

        //-------------------------------------------------------------------------

        public static IServiceCollection Remove_(this IServiceCollection collection, Func<ServiceDescriptor, bool> where)
        {
            where.Should().NotBeNull();

            var query = collection.AsEnumerable().Where(where);
            var remove_list = query.ToArray();

            //这里最好tolist，防止query中的值被修改
            foreach (var m in remove_list)
                collection.Remove(m);

            return collection;
        }

        /// <summary>
        /// 移除服务，两个参数至少指定一个
        /// </summary>
        public static IServiceCollection RemoveService_(this IServiceCollection collection, Type service)
        {
            service.Should().NotBeNull();

            return collection.Remove_(x => x.ServiceType == service);
        }

        /// <summary>
        /// 移除实现
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="implement"></param>
        /// <returns></returns>
        public static IServiceCollection RemoveImplement_(this IServiceCollection collection, Type implement)
        {
            implement.Should().NotBeNull();

            return collection.Remove_(x => x.ImplementationType == implement);
        }

        /// <summary>
        /// 存在service
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        public static bool ExistService_(this IServiceCollection collection, Type service)
        {
            var res = collection.Any(x => x.ServiceType == service);
            return res;
        }

        /// <summary>
        /// 存在实现
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="implement"></param>
        /// <returns></returns>
        public static bool ExistServiceImplement_(this IServiceCollection collection, Type implement)
        {
            var res = collection.Any(x => x.ImplementationType == implement);
            return res;
        }

        //-----------------------------------------------------------------------------

        /// <summary>
        /// 啦啦啦啦
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IServiceCollection AddDisposableSingleInstanceService<T>(this IServiceCollection collection, T instance)
            where T : class, ISingleInstanceService
        {
            collection.AddSingleton<ISingleInstanceService>(instance).AddSingleton(instance);
            return collection;
        }
    }
}
