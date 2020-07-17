using AspectCore.DynamicProxy;
using System;
using System.Threading.Tasks;
using Dapper;

namespace WCloud.Framework.Database.Dapper.Proxy
{
    public class EntityProxy
    {
        public EntityProxy()
        {
            ProxyGeneratorBuilder proxyGeneratorBuilder = new ProxyGeneratorBuilder();
            IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build();

            SampleInterface sampleInterface = proxyGenerator.CreateInterfaceProxy<SampleInterface, SampleClass>();
            Console.WriteLine(sampleInterface);
            sampleInterface.Foo();

            Console.ReadKey();

            System.Data.IDbConnection con = null;
            //proxyGenerator.CreateClassProxy();
        }

    }

    public class SampleInterceptor : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            await Task.CompletedTask;

            await context.Invoke(next);
        }
    }

    public class SampleClass : SampleInterface
    {
        public virtual void Foo() { }
    }

    [SampleInterceptor]
    public interface SampleInterface
    {
        void Foo();
    }
}
