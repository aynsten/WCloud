using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WCloud.Migration
{
    public class Startup : AppStartupBase<ContainerBuilder>
    {
        public Startup(IWebHostEnvironment env, IConfiguration config) : base(env, config) { }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<MigrationModule>();
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
