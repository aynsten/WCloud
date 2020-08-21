using Lib.extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Core;
using WCloud.Framework.Apm;
using WCloud.Framework.Common.Validator;
using WCloud.Framework.HttpClient;
using WCloud.Framework.Logging;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.Redis;
using WCloud.Framework.Startup;
using WCloud.Member.Authentication;
using WCloud.Member.Domain;
using WCloud.Member.Startup;

namespace WCloud.Admin
{
    [DependsOn(
        typeof(CoreModule),
        typeof(MemberStartupModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.Autofac.AbpAutofacModule),
        typeof(Volo.Abp.Uow.AbpUnitOfWorkModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class AdminModule : AbpModule
    {
        readonly Assembly __this_ass__ = typeof(AdminModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
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
                .AddMessageBus_()
                .AddWCloudMvc();
            //基础配置
            services.AddSession(option => { });

            //认证配置
            services.AddMemberAuthentication().AddAdminAuth();
            services.AddSwaggerDoc_(__this_ass__, AdminServiceRouteAttribute.ServiceName, new OpenApiInfo()
            {
                Title = "后台管理",
                Description = "后台管理接口，这里的接口使用cookie认证",
            });
            //使用swagger文档
            //services.AddSwagger_(this.GetType().Assembly, option => option.UseOauth_(_config));
            this.Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                var service_name = AdminServiceRouteAttribute.ServiceName;

                options
                    .ConventionalControllers
                    .Create(typeof(AdminModule).Assembly, option =>
                    {
                        option.RootPath = service_name;
                    });
            });

            services.ThrowWhenRepeatRegister();
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
                app.UseDbTableAsJson(new Assembly[] { typeof(IMemberShipDBTable).Assembly });
                app.UseAttributePermission(new Assembly[] { this.__this_ass__ });
            }

            if (_config.SwaggerEnabled() || _env.IsDevelopment())
            {
                app.UseSwaggerGatewayDefinitionJson();
                app.UseSwaggerWithUI(new[] { AdminServiceRouteAttribute.ServiceName });
            }

            //app.UseStaticFiles();
            //使用abp的虚拟文件，建议放在静态文件中间件之后
            //app.UseVirtualFiles();

            //app.UseSession();
            app.UseAuthentication();

            //app.UseAbpRequestLocalization();

            //app.UseCurrentUserAndOrgMiddleware();
            //app.UseMsgExceptionHandlerMiddleware();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
