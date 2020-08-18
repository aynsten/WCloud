using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Lib.core;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IocRegExtension
    {
        /// <summary>
        /// 无意义的接口，不要注册
        /// </summary>
        private static readonly IReadOnlyCollection<Type> ignore_interfaces = new List<Type>{
                typeof(IDisposable),
                typeof(IDBTable),
                typeof(IFinder),
                typeof(IAutoRegistered),
                typeof(ISingleInstance),
                typeof(IScopedInstance),
                }.AsReadOnly();

        /// <summary>
        /// 自动扫描可以注册的类
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="search_in_assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AutoRegister(this IServiceCollection collection, Assembly[] search_in_assembly)
        {
            search_in_assembly.Should().NotBeNullOrEmpty();

            var all_types = search_in_assembly
            .GetAllTypes()
            .Where(x => x.IsNormalClass())
            .Where(x => x.CanRegIoc())
            .Where(x => x.IsAssignableTo_<IAutoRegistered>())
            .ToArray();

            foreach (var t in all_types)
            {
                var ifs = t.GetInterfaces().Where(x => !ignore_interfaces.Contains(x)).ToArray();

                //删除之前的注册项目
                collection.RemoveAll(t);
                foreach (var i in ifs)
                {
                    collection.RemoveAll(i);
                }

                /*
                var lifetime = ServiceLifetime.Transient;
                collection.Add(new ServiceDescriptor(t, t, lifetime));
                foreach (var @interface in ifs)
                {
                    collection.Add(new ServiceDescriptor(@interface, t, lifetime));
                }*/

                if (t.IsSingleInstance())
                {
                    //单例
                    collection.AddSingleton(t);
                    foreach (var @interface in ifs)
                    {
                        collection.AddSingleton(@interface, t);
                    }
                }
                else if (t.IsScopedInstance())
                {
                    //容器作用域
                    collection.AddScoped(t);
                    foreach (var @interface in ifs)
                    {
                        collection.AddScoped(@interface, t);
                    }
                }
                else
                {
                    //代码作用域
                    collection.AddTransient(t);
                    foreach (var @interface in ifs)
                    {
                        collection.AddTransient(@interface, t);
                    }
                }
            }

            return collection;
        }

        /// <summary>
        /// 检查是否有不允许重复注册的service，如果有就抛出异常
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection ThrowWhenRepeatRegister(this IServiceCollection collection)
        {
            string GetImplementName(ServiceDescriptor d)
            {
                if (d.ImplementationType != null)
                    return d.ImplementationType.FullName;
                if (d.ImplementationInstance != null)
                    return d.ImplementationInstance.GetType().FullName;
                if (d.ImplementationFactory != null)
                {
                    var func = d.ImplementationFactory;
                    return func.Method?.ReturnType?.FullName ?? func.ToString();
                }
                return "未知实现";
            }

            var bad_reg = new Dictionary<string, string[]>();

            var items = collection.GroupBy(x => x.ServiceType).Where(x => x.Key.CheckRepeatRequired())
                .Select(x => new { x.Key, Impl = x.ToArray() });

            foreach (var m in items)
            {
                if (m.Impl.Length > 1)
                {
                    var serviceType = m.Key.FullName;
                    var impls = m.Impl.Select(x => GetImplementName(x)).ToArray();

                    bad_reg[serviceType] = impls;
                }
            }

            if (bad_reg.Any())
            {
                throw new RepeatRegException("存在不被允许的重复注册")
                {
                    BadReg = bad_reg
                };
            }
            return collection;
        }
    }
}