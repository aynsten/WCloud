﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.Startup;
using WCloud.Framework.Wechat.Login;
using WCloud.Framework.Wechat.Models;
using WCloud.Framework.Wechat.Pay;
using WCloud.Member.Application;
using WCloud.Member.Authentication;

namespace WCloud.MetroAd.Api
{
    [DependsOn(
        typeof(MetroAdModule),
        typeof(MemberModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class MetroAdApiModule : AbpModule
    {
        private readonly Assembly __this_ass__ = typeof(MetroAdApiModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            services.Configure<WxConfig>(_config.GetSection("wx"));
            services.AddSingleton(provider => provider.Resolve_<IOptions<WxConfig>>().Value);
            services.AddTransient<IUserWxLoginService, UserWxLoginService>();
            services.AddTransient(provider =>
            {
                return new WxPayApi(
                   httpClientFactory: provider.Resolve_<IHttpClientFactory>(),
                   logger: provider.Resolve_<ILogger<WxPayApi>>(),
                   config: provider.Resolve_<WxConfig>());
            });

            services.AddBasicServices();

            services.AddIdentityServerTokenValidation(_config);

            services.AddScopedLoginContext();

            services.AddMessageBus_(_config);

            services.AddSwaggerDoc_(this.__this_ass__, MetroAdServiceRouteAttribute.ServiceName, new OpenApiInfo()
            {
                Title = "metro-ad",
                Description = "地铁广告"
            }, option => option.UseOauth_(_config));

            services.AbpReplaceEmbeddedByPhysical<MetroAdModule>();

            services.AddRouting().AddMvc(option => option.AddExceptionHandler()).AddJsonProvider_();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var _config = context.ServiceProvider.ResolveConfig_();
            var _env = context.GetEnvironment();

            app.ApplicationServices.SetAsRootServiceProvider();

            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (_config.SwaggerEnabled() || _env.IsDevelopment())
            {
                app.UseSwaggerGatewayDefinitionJson();
                app.UseSwaggerWithUI(gateway_template_services: new[] { MetroAdServiceRouteAttribute.ServiceName });
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
