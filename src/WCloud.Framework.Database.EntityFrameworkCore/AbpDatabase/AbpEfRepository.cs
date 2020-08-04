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
    public interface IAbpEfRepository<T> : IEfCoreRepository<T, int>
        where T : BaseEntity
    {
        //
    }

    /// <summary>
    /// abp的仓储
    /// </summary>
    /// <typeparam name="DbContextImpl"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract class AbpEfRepositoryBase<DbContextImpl, T> : EfCoreRepository<DbContextImpl, T, int>, IAbpEfRepository<T>
        where T : BaseEntity
        where DbContextImpl : AbpDbContext<DbContextImpl>
    {
        public AbpEfRepositoryBase(IServiceProvider provider) : this(new IocAbpDbContextProvider<DbContextImpl>(provider))
        {
            //
        }

        public AbpEfRepositoryBase(IDbContextProvider<DbContextImpl> db) : base(db) { }
    }
}
