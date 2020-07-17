using AspectCore.Extensions.Autofac;
using Autofac;
using WCloud.Framework.Apm;
using WCloud.Member.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WCloud.Member.Domain;

namespace WCloud.MetroAd.Api
{
    public class Startup : AppStartupBase<ContainerBuilder>
    {
        public Startup(IWebHostEnvironment env, IConfiguration config) : base(env, config) { }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<MetroAdApiModule>();
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
