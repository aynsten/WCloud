﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
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
        typeof(Volo.Abp.Autofac.AbpAutofacModule),
        typeof(Volo.Abp.Uow.AbpUnitOfWorkModule),
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
            services.AddTransient<xx>();


            services.AddSwaggerDoc_(this.GetType().Assembly, "signal", new OpenApiInfo()
            {
                Title = "signal",
            });

#if DEBUG
            services.AddRouting().AddMvc(option => option.AddExceptionHandler()).AddJsonProvider_();
#endif
        }

        class xx
        {
            private readonly ILogger logger;
            public xx(ILogger<xx> logger)
            {
                this.logger = logger;
            }
            public async Task Log()
            {
                this.logger.LogInformation($"{nameof(SignalModule)}启动成功");
                await Task.CompletedTask;
            }
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

            if (_config.SwaggerEnabled() || _env.IsDevelopment())
            {
                app.UseSwaggerGatewayDefinitionJson();
                app.UseSwaggerWithUI(new[] { "signal" });
            }

            app.UseWebSockets();
            app.UseWebSocketEndpoint("/ws");

            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.StartHangfireJobs_();
            //创建一个测试定时任务
            using var s = app.ApplicationServices.CreateScope();
            var job_client = s.ServiceProvider.Resolve_<IBackgroundJobClient>();
            job_client.Schedule<xx>(x => x.Log(), TimeSpan.FromSeconds(30));

#if DEBUG
            app.UseRouting();
            app.UseEndpoints(e => e.MapDefaultControllerRoute());
#endif
        }
    }
}
