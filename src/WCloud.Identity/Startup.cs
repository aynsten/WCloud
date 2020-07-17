using AspectCore.Configuration;
using AspectCore.Extensions.Autofac;
using Autofac;
using WCloud.Framework.Apm;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using WCloud.Framework.Aop;
using WCloud.Member.Domain;

namespace WCloud.Identity
{
    public class Startup : AppStartupBase<ContainerBuilder>
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) : base(env, configuration) { }

        // This method gets called by the runtime. Use this method to add services to the container.
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<IdentityModule>();
        }

        public override void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterDynamicProxy(option =>
            {
                if (this._config.IsApmServerConfigAvaliable())
                {
                    var ass = new[] { typeof(IMemberShipDBTable).Assembly };
                    //监控identity server性能
                    var methods = new[] {
                        typeof(IProfileService),
                        typeof(IResourceOwnerPasswordValidator),
                        typeof(IExtensionGrantValidator),
                        typeof(ITokenCreationService),
                        typeof(ITokenRequestValidator),
                        typeof(ITokenRevocationRequestValidator),
                        typeof(IRefreshTokenService),
                        typeof(ITokenService),
                        typeof(ITokenValidator),
                        typeof(ICustomTokenRequestValidator),
                        typeof(ICustomTokenValidator)
                    }.SelectMany(x => x.GetMethods()).ToArray();
                    option.Interceptors.AddMethodMetric(ass, force_include: x => methods.Contains(x));
                }
                option.ThrowAspectException = false;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public override void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            app.InitializeApplication();
        }
    }
}
