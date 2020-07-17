using AspectCore.Extensions.Autofac;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WCloud.Framework.Apm;
using WCloud.Member.Application;
using WCloud.Member.Domain;

namespace WCloud.Member.Api
{
    public class Startup : AppStartupBase<ContainerBuilder>
    {
        public Startup(IWebHostEnvironment env, IConfiguration config) : base(env, config)
        {
            //
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<MemberApiModule>();
        }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterDynamicProxy(option =>
            {
                if (this._config.IsApmServerConfigAvaliable())
                {
                    var ass = new[]
                    {
                        typeof(IMemberShipDBTable).Assembly,
                    };
                    option.Interceptors.AddMethodMetric(ass);
                }
                option.ThrowAspectException = false;
            });
        }

        public override void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.InitializeApplication();
        }
    }
}
