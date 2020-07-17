using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Volo.Abp.DynamicProxy;

namespace WCloud.Gateway
{
    public interface idb
    {
        Task<string> xx();
    }
    class xx : idb
    {
        async Task<string> idb.xx()
        {
            return DateTime.Now.ToString();
        }
    }
    class log : Volo.Abp.DynamicProxy.IAbpInterceptor
    {
        public async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            await invocation.ProceedAsync();
        }
    }

    public class Startup : AppStartupBase<ContainerBuilder>
    {
        public Startup(IWebHostEnvironment env, IConfiguration config) : base(env, config) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<GatewayModule>();
            //使用abp提供的aop封装
            services.AddTransient<idb, xx>();
            services.AddTransient<log>();
            services.OnRegistred(context =>
            {
                if (context.ImplementationType == typeof(xx))
                {
                    context.Interceptors.Add<log>();
                }
            });
        }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            //
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.InitializeApplication();
        }
    }
}
