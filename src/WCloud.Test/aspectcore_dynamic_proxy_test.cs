using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using AspectCore.Configuration;

namespace WCloud.Test
{
    public class SampleInterceptor : AspectCore.DynamicProxy.AbstractInterceptorAttribute
    {
        private readonly Action<string> action;
        public SampleInterceptor(Action<string> action)
        {
            this.action = action;
        }

        public override Task Invoke(AspectCore.DynamicProxy.AspectContext context, AspectCore.DynamicProxy.AspectDelegate next)
        {
            this.action?.Invoke(context.ImplementationMethod?.Name);
            return context.Invoke(next);
        }
    }

    [TestClass]
    public class aspectcore_dynamic_proxy_test
    {
        public interface ISimple
        {
            string Name { get; set; }
        }

        public class SimpleSamepleEntity : ISimple
        {
            public virtual string Name { get; set; }

            //public void set_Name(string n) { }

            public virtual int Age { get; set; }
        }

        [TestMethod]
        public void aspectcore_proxy_with_target()
        {
            var change = 0;

            var proxyGeneratorBuilder = new ProxyGeneratorBuilder().Configure(option =>
            {
                option.Interceptors.AddTyped<SampleInterceptor>(new object[] { new Action<string>(x => ++change) });
            });
            var proxyGenerator = proxyGeneratorBuilder.Build();
            var entity = (SimpleSamepleEntity)proxyGenerator.CreateClassProxy<SimpleSamepleEntity>();
            entity.Name = "Richie";

            change.Should().Be(1);
        }

        [TestMethod]
        public void aspectcore_proxy_without_target()
        {
            var change = 0;

            var proxyGeneratorBuilder = new ProxyGeneratorBuilder().Configure(option =>
            {
                option.Interceptors.AddTyped<SampleInterceptor>(new object[] { new Action<string>(x => ++change) });
            });
            var proxyGenerator = proxyGeneratorBuilder.Build();
            var entity = (ISimple)proxyGenerator.CreateInterfaceProxy<ISimple>();
            entity.Name = "Richie";

            change.Should().Be(1);
        }

    }
}
