using Lib.data;
using Lib.ioc;
using Microsoft.EntityFrameworkCore;
using System;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    public class EFRepository<T, DbContextType> : EFRepositoryBase<T>
        where T : class, IDBTable
        where DbContextType : DbContext
    {
        public EFRepository(IServiceProvider provider) : base(() => provider.Resolve_<DbContextType>())
        {
            //
        }
    }
}
