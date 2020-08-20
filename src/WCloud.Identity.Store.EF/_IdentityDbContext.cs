using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace WCloud.Identity.Providers
{
    [Obsolete]
    public class _IdentityDbContext : DbContext,
        IdentityServer4.EntityFramework.Interfaces.IConfigurationDbContext,
        IdentityServer4.EntityFramework.Interfaces.IPersistedGrantDbContext
    {
        public _IdentityDbContext() { }

        public _IdentityDbContext(DbContextOptions options) : base(options)
        { }

        public DbSet<Client> Clients { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DbSet<IdentityResource> IdentityResources { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DbSet<ApiResource> ApiResources { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DbSet<ApiScope> ApiScopes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DbSet<PersistedGrant> PersistedGrants { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<int> SaveChangesAsync()
        {
            return this.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var con_str = "";
            optionsBuilder.UseMySql(con_str);
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder
            IdentityServer4.EntityFramework.Extensions.ModelBuilderExtensions.ConfigureClientContext(modelBuilder, null);
        }
    }
}
