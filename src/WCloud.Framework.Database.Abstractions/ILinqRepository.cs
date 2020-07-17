using Lib.data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Framework.Database.Abstractions
{
    public interface ILinqRepository<T> : IDisposable where T : IDBTable
    {
        IQueryable<T> Queryable { get; }
    }
}
