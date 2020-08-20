using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace WCloud.Identity.Providers
{
    public static class Bootstrap
    {
        /// <summary>
        /// 配置identity server
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddIdentityServer_(this IServiceCollection collection, IWebHostEnvironment env, IConfiguration config)
        {
            //使用通用的idistributedcache
            collection.AddTransient(typeof(IdentityServer4.Services.ICache<>), typeof(IdentityServerDistributedCache<>));
            var identityBuilder = collection.AddIdentityServer(option =>
            {
                //option.UserInteraction.ErrorUrl = "";
                /*
                 * 升级到v4后这个选项不见了，改为在中间件中设置publicorigin
                 var publicOrigin = config["public_origin"];
                if (publicOrigin?.Length > 0)
                {
                    option.PublicOrigin = publicOrigin;
                }
                 */
                option.Endpoints.EnableJwtRequestUri = true;
            });

            //identityBuilder.AddDeveloperSigningCredential();
            var pfx = Path.Combine(env.ContentRootPath, "idsrv4.pfx");
            File.Exists(pfx).Should().BeTrue($"ids证书不存在:{pfx}");

            identityBuilder.AddSigningCredential(new X509Certificate2(pfx, "1234"));

            //identityBuilder.AddConfigurationStore<IdentityConfigurationDbContext>(option => { });
            //使用写死的数据
            identityBuilder.AddConstantConfiguationStore();

            //微信授权登陆
            identityBuilder.AddExtensionGrantValidator<MyWechatGrantTypeValidator>();
            identityBuilder.AddExtensionGrantValidator<MyAdminPasswordGrantTypeValidator>();
            identityBuilder.AddResourceOwnerValidator<MyResourceOwnerPasswordValidator>();
            identityBuilder.AddProfileService<MyProfileService>();
            if (env.IsProduction())
            {
                identityBuilder.AddRedirectUriValidator<MyProductionRedirectUriValidator>();
            }
            else
            {
                identityBuilder.AddRedirectUriValidator<MyDevRedirectUriValidator>();
            }

            /*
             ids似乎对client resource提供了缓存支持
             但是对于token没有
             */
            //identityBuilder.AddClientStoreCache();
            identityBuilder.AddInMemoryCaching();

            return identityBuilder;
        }
    }
}
