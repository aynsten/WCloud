using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core.Authentication.Model;
using WCloud.Member.Authentication.Midddlewares;
using WCloud.Member.Authentication.OrgSelector;

namespace WCloud.Member.Authentication
{
    public static class Bootstrap
    {
        /// <summary>
        /// 在中间件中加载用户和租户的信息
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCurrentUserMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<UserAuthenticationMiddleware>();
            return app;
        }

        public static IApplicationBuilder UseCurrentAdminMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<AdminAuthenticationMiddleware>();
            return app;
        }

        /// <summary>
        /// 用来获取当前登陆用户信息
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedLoginContext(this IServiceCollection collection)
        {
            collection.AddScoped<ICurrentOrgSelector, MyCurrentOrgSelector>();

            collection.AddScoped<WCloudAdminInfo>(provider => new WCloudAdminInfo());
            collection.AddScoped<WCloudUserInfo>(provider => new WCloudUserInfo());
            collection.AddScoped<OrgInfo>(provider => provider.Resolve_<WCloudUserInfo>().Org ?? new OrgInfo());

            return collection;
        }
    }
}
