using System;
using System.Linq;
using Lib.data;
using Microsoft.EntityFrameworkCore;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    public abstract partial class EFRepositoryBase<T> : IEFRepository<T> where T : class, IDBTable
    {
        private DbContext __context__ { get; set; }

        private readonly object _lock = new object();
        private readonly Func<DbContext> _db_getter;

        /// <summary>
        /// 懒加载，只有调用才会生成dbcontext
        /// </summary>
        public DbContext Database
        {
            get
            {
                if (this.__context__ == null)
                {
                    lock (this._lock)
                    {
                        if (this.__context__ == null)
                        {
                            this.__context__ = this._db_getter.Invoke();
                        }
                    }
                }

                return this.__context__ ?? throw new NotSupportedException("无法获取EF Context");
            }
        }

        public DbSet<T> Table => this.Database.Set<T>();
        public IQueryable<T> TrakingQueryable => this.Table.AsQueryableTrackingOrNot(true);
        public IQueryable<T> NoTrackingQueryable => this.Table.AsQueryableTrackingOrNot(false);
        public IQueryable<T> Queryable => this.NoTrackingQueryable;

        protected EFRepositoryBase(Func<DbContext> db_context_getter)
        {
            this._db_getter = db_context_getter ?? throw new ArgumentNullException(nameof(db_context_getter));
        }

        public virtual void Dispose()
        {
            this.__context__?.Dispose();
        }
    }
}
