using Lib.data;
using Microsoft.EntityFrameworkCore;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    /// <summary>
    /// 通过泛型指定dbcontext
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="DbContextType"></typeparam>
    public abstract class EFRepositoryFromNew<T, DbContextType> : EFRepositoryBase<T>
        where T : class, IDBTable
        where DbContextType : DbContext, new()
    {
        protected EFRepositoryFromNew() : base(() => new DbContextType()) { }
    }
}
