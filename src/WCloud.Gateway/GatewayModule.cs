using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.Core;
using WCloud.Framework.Apm;
using WCloud.Framework.MVC;
using WCloud.Framework.Startup;
using WCloud.Gateway.Middlewares;
using WCloud.Member.Application;
using WCloud.Member.Authentication;

namespace WCloud.Gateway
{
    [DependsOn(
        typeof(CoreModule),
        typeof(MemberModule),
        typeof(Volo.Abp.Autofac.AbpAutofacModule),
        typeof(Volo.Abp.Uow.AbpUnitOfWorkModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class GatewayModule : AbpModule
    {
        private readonly Assembly __this_ass__ = typeof(GatewayModule).Assembly;

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            services.AddBasicServices();

            services.AddOcelot(_config);

            services.AddSwaggerDoc_(this.__this_ass__, GatewayRouteAttribute.ServiceName, new Microsoft.OpenApi.Models.OpenApiInfo()
            {
                Title = "网关",
                Description = "网关相关接口，不建议调用"
            }, option => option.UseOauth_(_config));

            //services.AddAlwaysAllowAuthorization();
            services.AddIdentityServerTokenValidation(_config);

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
                app.UseDevCors();
            }
            else
            {
                app.UseProductionCors(_config);
            }

            app.Map("/test", builder =>
            {
                builder.Run(async context =>
                {
                    var s = context.RequestServices.GetService<idb>();
                    var d = await s.xx();
                    await context.Response.WriteAsync(d);
                });
            });
#if DEBUG
            //source file
            app.UseStaticFiles();
            var apps = new[] { "water", "user", "app", "RM" };
            app.UseAppHtmlRender(apps);
#endif

            if (_config.SwaggerEnabled() || _env.IsDevelopment())
            {
                var micro_services = (_config["micro_services"] ?? string.Empty).Split(',').ToArray();
                micro_services.HasDuplicateItems().Should().BeFalse("微服务配置错误");

                //暴露json文件
                app.UseSwaggerDefaultDefinitionJson();
                app.UseSwaggerWithUI(
                    gateway_template_services: micro_services,
                    default_template_services: new[] { GatewayRouteAttribute.ServiceName },
                    optionBuilder: option =>
                    {
                        option.OAuthClientId("wx-imp");
                        option.OAuthClientSecret("123");
                        //option.OAuth2RedirectUrl("http://localhost:8888/swagger/token-callback");
                    });
            }

            //注册controller
            app.UseRouting();
            app.UseAuthentication().UseAuthorization();

            //app.UseWebSocketProxy();proxykit代理ws需要额外配置的
            //注册网关
            Task.Run(() => app.UseOcelot(DynamicDownStreamMiddlewareHelper.__config_ocelot__)).Wait();
        }

        public override void OnApplicationShutdown(ApplicationShutdownContext context)
        {
            base.OnApplicationShutdown(context);
            IocContext.Instance.Dispose();
        }
    }
}
