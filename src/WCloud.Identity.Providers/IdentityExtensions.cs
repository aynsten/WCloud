using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WCloud.Identity.Providers
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// 使用写死的配置数据
        /// </summary>
        /// <param name="identityBuilder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConstantConfiguationStore(this IIdentityServerBuilder identityBuilder)
        {
            identityBuilder.AddInMemoryApiScopes(IdentityConfig.TestApiScopes());
            identityBuilder.AddInMemoryApiResources(IdentityConfig.TestApiResource());
            identityBuilder.AddInMemoryIdentityResources(IdentityConfig.TestIdentityResource());
            identityBuilder.AddInMemoryClients(IdentityConfig.TestClients());
            return identityBuilder;
        }

        /// <summary>
        /// 获取ids数据库链接字符串
        /// 配置和授权分开了
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static (string ConfigDb, string GrantsDb) GetIdentityConnectionString(this IConfiguration config)
        {
            var config_str = config.GetConnectionString("db_oauth_config");
            config_str.Should().NotBeNullOrEmpty(nameof(config_str));

            var grants_str = config.GetConnectionString("db_oauth_grants");
            grants_str.Should().NotBeNullOrEmpty(nameof(grants_str));

            return (config_str, grants_str);
        }
    }
}
