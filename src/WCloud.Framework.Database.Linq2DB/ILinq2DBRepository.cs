using Lib.data;
using LinqToDB;
using WCloud.Framework.Database.Abstractions;

namespace WCloud.Framework.Database.Linq2DB
{
    public interface ILinq2DBRepository<T> : ILinqRepository<T>, IRepository<T> where T : class, IDBTable
    {
        IDataContext TryGetNewDatabaseOrThrow();

        IDataContext Database { get; }
    }
}
