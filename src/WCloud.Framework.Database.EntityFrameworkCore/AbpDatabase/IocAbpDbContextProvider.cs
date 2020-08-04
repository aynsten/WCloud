using Lib.core;
using Microsoft.Extensions.DependencyInjection;
using System;
using Volo.Abp.EntityFrameworkCore;

namespace WCloud.Framework.Database.EntityFrameworkCore.AbpDatabase
{
    public class IocAbpDbContextProvider<DbContextImpl> : IDbContextProvider<DbContextImpl>
        where DbContextImpl : AbpDbContext<DbContextImpl>
    {
        private readonly Lazy_<DbContextImpl> _lazy;

        public IocAbpDbContextProvider(IServiceProvider provider)
        {
            this._lazy = new Lazy_<DbContextImpl>(() => provider.Resolve_<DbContextImpl>());
        }

        public DbContextImpl GetDbContext()
        {
            var res = this._lazy.Value;
            return res;
        }
    }
}
