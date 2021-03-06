﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.CommonService.Application;
using WCloud.Core;
using WCloud.Framework.Apm;
using WCloud.Framework.Common.Validator;
using WCloud.Framework.HttpClient_;
using WCloud.Framework.Logging;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.Redis;
using WCloud.Framework.Startup;
using WCloud.Framework.Wechat.Login;
using WCloud.Framework.Wechat.Models;
using WCloud.Member.Authentication;
using WCloud.Member.Startup;

namespace WCloud.Member.Api
{
    [DependsOn(
        typeof(CoreModule),
        typeof(MemberStartupModule),
        typeof(CommonServiceModule),
        typeof(Volo.Abp.Autofac.AbpAutofacModule),
        typeof(Volo.Abp.Uow.AbpUnitOfWorkModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class MemberApiModule : AbpModule
    {
        private readonly Assembly __this_ass__ = typeof(MemberApiModule).Assembly;

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
                .AddMessageBus_()
                .AddWCloudMvc();

            services.AddMemberAuthentication().AddIdentityServerTokenValidation(_config);

            services.AddSwaggerDoc_(this.__this_ass__, MemberServiceRouteAttribute.ServiceName, new OpenApiInfo()
            {
                Title = "member",
                Description = "用户账号"
            }, option => option.UseOauth_(_config));

            services.Configure<WxConfig>(_config.GetSection("wx"));
            services.AddSingleton(provider => provider.Resolve_<IOptions<WxConfig>>().Value);
            services.AddTransient<IUserWxLoginService, UserWxLoginService>();
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
                app.UseSwaggerWithUI(gateway_template_services: new[] { MemberServiceRouteAttribute.ServiceName });
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
