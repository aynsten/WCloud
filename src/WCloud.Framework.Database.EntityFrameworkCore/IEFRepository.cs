using Lib.core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions;

namespace WCloud.Framework.Database.EntityFrameworkCore
{
    public interface IEFRepository<T> : ILinqRepository<T>, IBulkInsertRepository<T>, IQueryByKeyRepository<T> where T : class, IDBTable
    {
        DbContext Database { get; }
        DbSet<T> Table { get; }
        IQueryable<T> TrakingQueryable { get; }
        IQueryable<T> NoTrackingQueryable { get; }

        T QueryOneAsNoTrack(Expression<Func<T, bool>> where);

        Task<T> QueryOneAsNoTrackAsync(Expression<Func<T, bool>> where);
    }
}
