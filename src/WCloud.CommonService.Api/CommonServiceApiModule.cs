using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Framework.Apm;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.Startup;
using WCloud.Member.Application;
using WCloud.Member.Authentication;

namespace WCloud.CommonService.Api
{
    [DependsOn(
        typeof(MemberModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.Autofac.AbpAutofacModule),
        typeof(Volo.Abp.Uow.AbpUnitOfWorkModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class CommonServiceApiModule : AbpModule
    {
        private readonly Assembly __this_ass__ = typeof(CommonServiceApiModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            services.AddBasicServices();

            services.AddIdentityServerTokenValidation(_config);

            services.AddScopedLoginContext();

            services.AddMessageBus_(_config);

            services.AddSwaggerDoc_(this.__this_ass__, CommonServiceRouteAttribute.ServiceName, new OpenApiInfo()
            {
                Title = "common-service",
                Description = "公共服务"
            }, option => option.UseOauth_(_config));

            services.AddRouting().AddMvc(option => option.AddExceptionHandler()).AddJsonProvider_();
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
            }

            if (_config.SwaggerEnabled() || _env.IsDevelopment())
            {
                app.UseSwaggerGatewayDefinitionJson();
                app.UseSwaggerWithUI(gateway_template_services: new[] { CommonServiceRouteAttribute.ServiceName });
            }

            app.UseRouting();

            app.UseAuthentication().UseAuthorization();

            app.UseCurrentAdminMiddleware().UseCurrentUserMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
