using IdentityServer4.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Modularity;
using WCloud.Core;
using WCloud.Framework.Apm;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.Wechat.Login;
using WCloud.Framework.Wechat.Models;
using WCloud.Identity.Providers;
using WCloud.Member.Authentication;

namespace WCloud.Identity
{
    [DependsOn(
        typeof(WCloud.CommonService.Application.CommonServiceModule),
        typeof(WCloud.Member.Application.MemberModule),
        typeof(Volo.Abp.AspNetCore.Mvc.AbpAspNetCoreMvcModule)
        )]
    public class IdentityModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var _config = services.GetConfiguration();
            var _env = services.GetHostingEnvironment();

            //基础配置
            services.AddBasicServices();
            //ids配置
            services.AddIdentityServer_(_env, _config);
            //认证配置
            var authBuilder = services.AddIdentityServerAuthentication_();
            authBuilder.AddGitHub(option =>
            {
                option.SignInScheme = ConfigSet.Identity.ExternalLoginScheme;
                option.ClientId = "65b5fa0f0c8fa1632674";
                option.ClientSecret = "2b6444fef59038cd9cfbfb27f683b97e81a03fc9";
            });
            //上下文
            services.AddScopedLoginContext();

            services.Configure<WxConfig>(_config.GetSection("wx"));
            services.AddSingleton(provider => provider.Resolve_<IOptions<WxConfig>>().Value);
            services.AddTransient<IUserWxLoginService, UserWxLoginService>();

            services.AddMessageBus_(_config);

            services.AddRouting().AddMvc(option => option.AddExceptionHandler()).AddJsonProvider_();

            services.ThrowWhenRepeatRegister();
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var _config = context.ServiceProvider.ResolveConfig_();
            var _env = context.GetEnvironment();

            app.ApplicationServices.SetAsRootServiceProvider();
            app.UseApm(_config);

            if (_config.InitDatabaseRequired())
            {
                //测试环境下尝试创建数据库
                app.EnsureIdentityDatabaseCreated();
            }
            if (_env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                //升级到v4后这publicorigin选项不见了，改为在中间件中设置publicorigin
                var config = context.RequestServices.ResolveConfig_();
                var publicOrigin = config["public_origin"];
                if (publicOrigin?.Length > 0)
                {
                    context.SetIdentityServerOrigin(publicOrigin);
                    context.SetIdentityServerBasePath(context.Request.PathBase.Value.TrimEnd('/'));
                }
                await next.Invoke();
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication().UseAuthorization();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
