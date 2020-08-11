using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore.AbpDatabase
{
    /// <summary>
    /// abp的仓储
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAbpEfRepository<T> : IEfCoreRepository<T, string>
        where T : EntityBase
    {
        //
    }

    /// <summary>
    /// abp的仓储
    /// </summary>
    /// <typeparam name="DbContextImpl"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class AbpEfRepositoryBase<DbContextImpl, T> : EfCoreRepository<DbContextImpl, T, string>, IAbpEfRepository<T>
        where T : EntityBase
        where DbContextImpl : AbpDbContext<DbContextImpl>
    {
        public AbpEfRepositoryBase(IServiceProvider provider) : this(new IocAbpDbContextProvider<DbContextImpl>(provider))
        {
            //
        }

        public AbpEfRepositoryBase(IDbContextProvider<DbContextImpl> db) : base(db) { }
    }
}
