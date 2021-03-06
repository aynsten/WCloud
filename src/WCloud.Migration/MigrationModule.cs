﻿using Lib.extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Core;
using WCloud.Core.Helper;
using WCloud.Framework.Common.Validator;
using WCloud.Framework.HttpClient_;
using WCloud.Framework.Logging;
using WCloud.Framework.MVC;
using WCloud.Framework.Redis;
using WCloud.Member.Startup;

namespace WCloud.Migration
{
    [DependsOn(
        typeof(CoreModule),
        typeof(MemberStartupModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.Autofac.AbpAutofacModule),
        typeof(Volo.Abp.Uow.AbpUnitOfWorkModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class MigrationModule : AbpModule
    {
        readonly Assembly __this_ass__ = typeof(MigrationModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            var ass_to_scan = __this_ass__.FindWCloudAssemblies();

            services.AddWCloudBuilder()
                .AutoRegister(ass_to_scan)
                .AddFluentValidatorHelper().RegEntityValidators(ass_to_scan)
                .AddHttpClient()
                .AddRedisClient()
                .AddRedisHelper()
                .AddRedisDistributedCacheProvider_()
                .AddRedisDataProtectionKeyStore()
                .AddLoggingAll()
                .AddWCloudMvc();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();

            app.ApplicationServices.SetAsRootServiceProvider();

            this.__config_database__(app);

            var host = app.ApplicationServices.Resolve_<Microsoft.Extensions.Hosting.IHost>();
            host.StopAsync();
        }

        void __config_database__(IApplicationBuilder app)
        {
            //账户
            using (var s = app.ApplicationServices.CreateScope())
            {
                var logger = s.ServiceProvider.Resolve_<ILogger<MigrationModule>>();

                var services = s.ServiceProvider.ResolveAll_<IDataInitializeHelper>();

                Task.Run(async () =>
                {
                    foreach (var m in services)
                    {
                        try
                        {
                            await m.CreateDatabase();
                            await m.InitSeedData();

                            logger.LogInformation(m.GetType().FullName);
                        }
                        catch (Exception e)
                        {
                            logger.AddErrorLog(e.Message, e);
                        }
                    }
                }).Wait();

                logger.LogInformation("初始化成功");
            }
        }
    }
}
