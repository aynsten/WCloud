using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Member.Application;
using WCloud.Member.Application.InitData;
using WCloud.Member.DataAccess.EF;

namespace WCloud.Admin.Consumer
{
    [DependsOn(
        typeof(MemberModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class AdminConsumerModule : AbpModule
    {
        readonly Assembly __this_ass__ = typeof(AdminConsumerModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            //基础配置
            services.AddBasicServices();

            this.__add_message_bus__(services, _config);

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

        void __add_message_bus__(IServiceCollection services, IConfiguration _config)
        {
            var ass = new System.Reflection.Assembly[] { __this_ass__ };

            services.AddMessageBus_(_config, ass);

            ////消息总线 
            //services.AddMasstransitMessageBus_(_config, option =>
            //{
            //    //optional
            //    option.ReceiveEndpoint("user_role_changed_subscriber", endpoint =>
            //    {
            //        //新建一个队列绑定exchange，实现一个消息多次订阅
            //        endpoint.Bind<UserRoleChangedMessage>();
            //        endpoint.Consumer<AdminRoleChangedConsumer>();
            //    });

            //    //optional：注册消费程序
            //    option.RegMasstransitBasicConsumer(all_consumer_types);
            //    //option.RegMasstransitConsumer(ass);
            //});
        }

        void __config_database__(IApplicationBuilder app)
        {
            //账户
            using var s = app.ApplicationServices.CreateScope();
            using (var db = s.ServiceProvider.Resolve_<CommonServiceDbContext>())
            {
                db.Database.EnsureCreated();
            }

            using (var db = s.ServiceProvider.Resolve_<MemberShipDbContext>())
            {
                db.Database.EnsureCreated();
                //尝试创建初始账户
                app.ApplicationServices.InitAdminRoles()
                   .InitAdminUsers()
                   .InitOrgInfo()
                   .AddRandomUserAndJoinOrg();
            }
        }
    }
}
