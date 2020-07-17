using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.MemoryStorage;
using Lib.ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Framework.Apm;
using WCloud.Framework.Jobs;
using WCloud.Framework.MVC;
using WCloud.Framework.Socket;
using WCloud.Framework.Startup;
using WCloud.Member.Application;

namespace WCloud.IM.Server
{
    [DependsOn(
        typeof(MemberModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class SignalModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            //基础配置
            services.AddBasicServices();

            services.AddWebSockets(option =>
            {
                option.KeepAliveInterval = TimeSpan.FromSeconds(10);
                //option.ReceiveBufferSize = 4;
                option.AllowedOrigins.Add("http://coolaf.com");
                option.AllowedOrigins.Add("http://localhost:4001");
            });

            services.AddIMServer()
                .AddDefaultHubProvider()
                .AddRedisPersistenceProvider()
                .AddRedisRegistrationCenter()
                .AddRedisTransportProvider().AddDefaultUserContextProvider();

            services.AddHangfireJobs_(new Assembly[] { this.GetType().Assembly });
            services.AddHangfire((provider, config) =>
            {
                config.UseMemoryStorage();
                config.UseActivator(new AspNetCoreJobActivator(provider.Resolve_<IServiceScopeFactory>()));
                config.UseLogProvider(new AspNetCoreLogProvider(provider.Resolve_<ILoggerFactory>()));
            });
            services.AddHangfireServer();

#if DEBUG
            services.AddRouting().AddMvc(option => option.AddExceptionHandler()).AddJsonProvider_();
#endif
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var _config = context.ServiceProvider.ResolveConfig_();
            var _env = context.GetEnvironment();

            app.ApplicationServices.SetAsRootServiceProvider();
            app.UseApm(_config);

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseConfigAsKV();
            }
#if DEBUG
            app.UseStaticFiles();
#endif

            app.UseAuthentication();

            app.UseWebSockets();
            app.UseWebSocketEndpoint("/ws");

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.StartHangfireJobs_();

#if DEBUG
            app.UseRouting();
            app.UseEndpoints(e => e.MapDefaultControllerRoute());
#endif
        }
    }
}
