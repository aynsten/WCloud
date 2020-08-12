using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WCloud.InternalGateway
{
    public class Startup : AppStartupBase<ContainerBuilder>
    {
        public Startup(IWebHostEnvironment env, IConfiguration config) : base(env, config)
        { }

        public override void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.InitializeApplication();
        }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            //
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<InternalGatewayModule>();
        }
    }
}
