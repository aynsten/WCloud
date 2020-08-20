using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace WCloud.Identity.Providers
{
    [Obsolete("现阶段使用写死的配置信息")]
    public class IdentityConfigurationDbContext : ConfigurationDbContext<IdentityConfigurationDbContext>
    {
        private readonly IConfiguration _config;
        public IdentityConfigurationDbContext(
            DbContextOptions<IdentityConfigurationDbContext> option,
            ConfigurationStoreOptions o, IConfiguration config) : base(option, o)
        {
            this._config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var con_str = this._config.GetIdentityConnectionString().ConfigDb;
            optionsBuilder.UseMySql(con_str);
        }
    }
}
