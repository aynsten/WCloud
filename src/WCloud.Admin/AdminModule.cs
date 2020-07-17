using Lib.ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Framework.Apm;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.Startup;
using WCloud.Member.Application;
using WCloud.Member.Authentication;
using WCloud.Member.Domain;
using WCloud.MetroAd;

namespace WCloud.Admin
{
    [DependsOn(
        typeof(MetroAdModule),
        typeof(MemberModule),
        typeof(CommonServiceModule),
        typeof(AbpLocalizationModule),
        typeof(AbpAutofacModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class AdminModule : AbpModule
    {
        readonly Assembly __this_ass__ = typeof(AdminModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            //基础配置
            this.__add_basic__(services, _config, _env);

            this.__add_auth__(services);

            this.__add_message_bus__(services, _config);

            this.__add_swagger__(services);

            this.__add_mvc__(services);

            this.__config_auto_api__();

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
                app.UseUndefinedPermission(new Assembly[] { this.__this_ass__ });
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

        void __add_basic__(IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddBasicServices();
            services.AddSession(option => { });
        }

        void __config_auto_api__()
        {
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
        }

        void __add_auth__(IServiceCollection services)
        {
            //认证配置
            services.AddAdminAuth();
            //上下文
            services.AddScopedLoginContext();
        }

        void __add_message_bus__(IServiceCollection services, IConfiguration _config)
        {
            //消息总线 
            services.AddMessageBus_(_config);
        }

        void __add_mvc__(IServiceCollection services)
        {
            services.AddRouting().AddMvc(option => option.AddExceptionHandler()).AddJsonProvider_();
        }

        void __add_swagger__(IServiceCollection services)
        {
            services.AddSwaggerDoc_(__this_ass__, AdminServiceRouteAttribute.ServiceName, new OpenApiInfo()
            {
                Title = "后台管理",
                Description = "后台管理接口，这里的接口使用cookie认证",
            });
            //使用swagger文档
            //services.AddSwagger_(this.GetType().Assembly, option => option.UseOauth_(_config));
        }
    }
}
