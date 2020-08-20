using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WCloud.Identity.Providers
{
    public class IdentityOperationDbContext : PersistedGrantDbContext<IdentityOperationDbContext>
    {
        private readonly IConfiguration _config;
        public IdentityOperationDbContext(
            DbContextOptions<IdentityOperationDbContext> option,
            OperationalStoreOptions o,
            IConfiguration config) : base(option, o)
        {
            this._config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var con_str = this._config.GetIdentityConnectionString().GrantsDb;
            optionsBuilder.UseMySql(con_str);
        }
    }
}
