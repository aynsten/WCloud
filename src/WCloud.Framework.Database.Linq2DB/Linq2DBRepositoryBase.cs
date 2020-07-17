using Lib.data;
using LinqToDB;
using System;
using System.Linq;

namespace WCloud.Framework.Database.Linq2DB
{
    public abstract partial class Linq2DBRepositoryBase<T> : ILinq2DBRepository<T>
        where T : class, IDBTable
    {
        private readonly Func<IDataContext> _db_getter;
        private readonly IDataContext _context;

        protected Linq2DBRepositoryBase(Func<IDataContext> db_getter)
        {
            this._db_getter = db_getter ?? throw new ArgumentNullException(nameof(db_getter));
            this._context = this._db_getter.Invoke() ?? throw new ArgumentNullException(nameof(this._context));
        }

        public IDataContext TryGetNewDatabaseOrThrow()
        {
            var db = this._db_getter.Invoke();
            if (object.ReferenceEquals(db, this.Database))
                throw new NotSupportedException("当前实现不支持创建新的连接实例");

            return db;
        }

        public IDataContext Database => this._context;
        public IQueryable<T> Queryable => this._context.GetTable<T>();

        public virtual void Dispose()
        {
            this._context?.Dispose();
        }
    }
}
