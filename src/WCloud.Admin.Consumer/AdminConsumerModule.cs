using Lib.extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Core;
using WCloud.Core.MessageBus;
using WCloud.Framework.Common.Validator;
using WCloud.Framework.HttpClient;
using WCloud.Framework.Logging;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.Redis;
using WCloud.Member.Initialization;
using WCloud.Member.Shared.Helper;
using WCloud.Member.Startup;

namespace WCloud.Admin.Consumer
{
    [DependsOn(
        typeof(CoreModule),
        typeof(MemberStartupModule),
        typeof(MemberInitiallizationModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.Autofac.AbpAutofacModule),
        typeof(Volo.Abp.Uow.AbpUnitOfWorkModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class AdminConsumerModule : AbpModule
    {
        readonly Assembly __this_ass__ = typeof(AdminConsumerModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _env = services.GetHostingEnvironment();

            var ass_to_scan = __this_ass__.FindWCloudAssemblies();
            var nlog_config_file = _env.NLogConfigFilePath();

            services.AddWCloudBuilder()
                .AutoRegister(ass_to_scan)
                .AddHttpClient()
                .AddFluentValidatorHelper().RegEntityValidators(ass_to_scan)
                .AddRedisClient()
                .AddRedisHelper()
                .AddRedisDistributedCacheProvider_()
                .AddRedisDataProtectionKeyStore()
                .AddLoggingAll(nlog_config_file)
                .AddMessageBus_(new[] { this.__this_ass__ })
                .AddWCloudMvc();

            services.ThrowWhenRepeatRegister();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var _config = context.ServiceProvider.ResolveConfig_();

            app.ApplicationServices.SetAsRootServiceProvider();

            if (_config.InitDatabaseRequired())
            {
                this.__config_database__(app);
            }

            //开启消息消费
            app.ApplicationServices.StartMessageBusConsumer_();
        }

        void __config_database__(IApplicationBuilder app)
        {
            //账户
            using var s = app.ApplicationServices.CreateScope();
            using (var db = s.ServiceProvider.Resolve_<CommonServiceDbContext>())
            {
                db.Database.EnsureCreated();
            }

            //尝试创建初始账户
            Task.Run(async () =>
            {
                var s = app.ApplicationServices;
                await s.Resolve_<IMemberDatabaseHelper>().CreateDatabase();
                await s.InitAdminRoles()
                   ; await s.InitAdminUsers()
                     ; await s.InitOrgInfo()
                       ; await s.AddRandomUserAndJoinOrg();
            }).Wait();
        }
    }
}
