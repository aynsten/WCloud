using Lib.data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    public interface IEFRepository<T> : ILinqRepository<T>, IRepository<T>
        where T : class, IDBTable
    {
        DbContext Database { get; }
        DbSet<T> Table { get; }
        IQueryable<T> TrakingQueryable { get; }
        IQueryable<T> NoTrackingQueryable { get; }

        T GetFirstAsNoTrack(Expression<Func<T, bool>> where);

        Task<T> GetFirstAsNoTrackAsync(Expression<Func<T, bool>> where);

        int AddBulk(IEnumerable<T> models);

        Task<int> AddBulkAsync(IEnumerable<T> models);
    }
}
