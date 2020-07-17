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

namespace WCloud.IM.Server
{
    public class Startup : AppStartupBase<ContainerBuilder>
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(env, configuration) { }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<SignalModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.InitializeApplication();
            lifetime.ApplicationStopping.Register(() => Lib.ioc.IocContext.Instance.Dispose());
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
                        typeof(SignalModule).Assembly,
                    };
                    option.Interceptors.AddMethodMetric(ass);
                }
                option.ThrowAspectException = false;
            });
        }
    }
}
