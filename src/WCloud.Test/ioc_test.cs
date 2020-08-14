using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WCloud.Framework.Aop.AutofacProvider;

namespace WCloud.Test
{
    [TestClass]
    public class ioc_test
    {
        [TestMethod]
        public void single_instance()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton(provider => new service_());

            collection.GetSingletonInstanceOrNull<service_>().Should().BeNull();

            collection.RemoveAll<service_>();
            collection.AddSingleton(new service_());

            collection.GetSingletonInstanceOrNull<service_>().Should().NotBeNull();
        }

        //[TestMethod]
        public void autofac_test()
        {
            //创建builder
            var builder = new ContainerBuilder();
            //必须把servicecollection中的注册项copy过来，不然无法用iserviceprovider创建scope
            builder.Populate(new ServiceCollection());

            using (var context = builder.Build())
            {
                var provider = context.AsServiceProvider();

                try
                {
                    using (var scope = provider.CreateScope())
                    {
                        //scope:Autofac.Extensions.DependencyInjection.AutofacServiceScope
                    }
                }
                catch (Exception e)
                {
                    /*
                 Autofac.Core.Registration.ComponentNotRegisteredException:
                 “The requested service 'Microsoft.Extensions.DependencyInjection.IServiceScopeFactory' has not been registered. 
                 To avoid this exception, either register a component to provide the service, 
                 check for service registration using IsRegistered(), or use the ResolveOptional() method to resolve an optional dependency.”    
                 */
                    Console.Out.Write(e.Message);
                }
            }
        }

        interface transit : IDisposable { Action _cb { get; set; } }
        interface scoped : IDisposable { Action _cb { get; set; } }
        interface single : IDisposable { Action _cb { get; set; } }

        class service_ : transit, scoped, single
        {
            public Action _cb { get; set; }

            public void Dispose()
            {
                this._cb?.Invoke();
            }
        }

        /// <summary>
        /// 印证我的想法
        /// </summary>
        [TestMethod]
        public void di_test_transient()
        {
            var provider = new ServiceCollection()
                .AddTransient<transit, service_>()
                .AddScoped<scoped, service_>()
                .AddSingleton<single, service_>()
                .BuildServiceProvider().SetAsRootServiceProvider();

            var single_dispose = 0;
            var scoped_dispose = 0;
            var transit_dispose = 0;

            Action add_single = () => ++single_dispose;
            Action add_scoped = () => ++scoped_dispose;
            Action add_transit = () => ++transit_dispose;

            var single_object = provider.Resolve_<single>();
            single_object._cb = add_single;

            //作用域1
            using (var s = provider.CreateScope())
            {
                var another_single_object = s.ServiceProvider.Resolve_<single>();
                another_single_object._cb = add_single;
                single_object.Should().Be(another_single_object);
            }

            //作用域2
            using (var s = provider.CreateScope())
            {
                var another_single_object = s.ServiceProvider.Resolve_<single>();
                another_single_object._cb = add_single;
                single_object.Should().Be(another_single_object);

                var scoped_object = s.ServiceProvider.Resolve_<scoped>();
                scoped_object._cb = add_scoped;
                var another_scoped_object = s.ServiceProvider.Resolve_<scoped>();
                another_scoped_object._cb = add_scoped;
                scoped_object.Should().Be(another_scoped_object);

                var transit_object = s.ServiceProvider.Resolve_<transit>();
                transit_object._cb = add_transit;
                var another_transit_object = s.ServiceProvider.Resolve_<transit>();
                another_transit_object._cb = add_transit;
                transit_object.Should().NotBe(another_transit_object);
            }
            //出了作用域
            single_dispose.Should().Be(0);
            scoped_dispose.Should().Be(1);
            transit_dispose.Should().Be(2);
        }

        [TestMethod]
        public void di_test_service_provider()
        {
            var collection = new ServiceCollection();
            collection.AddTransient<transit, service_>();
            var provider = collection.BuildServiceProvider();
            using (var s = provider.CreateScope())
            {
                var _provider = s.ServiceProvider.GetService<IServiceProvider>();
                _provider.Should().Be(s.ServiceProvider);
            }
        }
    }
}
