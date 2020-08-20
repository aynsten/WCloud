using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Identity.Providers;

namespace WCloud.Identity.Store.EF
{
    public static class EFStoreExtension
    {
        public static IIdentityServerBuilder AddEFOperationDbContext(this IIdentityServerBuilder builder)
        {
            builder.AddOperationalStore<IdentityOperationDbContext>(option =>
            {
                //自动清理无用token
                option.EnableTokenCleanup = true;
            });
            return builder;
        }

        /// <summary>
        /// 创建identity的数据库
        /// </summary>
        public static IApplicationBuilder EnsureIdentityDatabaseCreated(this IApplicationBuilder app)
        {
            using (var s = app.ApplicationServices.CreateScope())
            {
                //尝试创建client，resource等配置数据库
                var config_db = s.ServiceProvider.ResolveOptional_<IdentityConfigurationDbContext>();
                if (config_db != null)
                    using (config_db)
                        config_db.Database.EnsureCreated();
                //尝试创建token的授权数据库
                var grants_db = s.ServiceProvider.ResolveOptional_<IdentityOperationDbContext>();
                if (grants_db != null)
                    using (grants_db)
                        grants_db.Database.EnsureCreated();
            }
            return app;
        }
    }
}
