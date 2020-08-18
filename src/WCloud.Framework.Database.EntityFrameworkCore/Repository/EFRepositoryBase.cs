using FluentAssertions;
using Lib.data;
using Lib.extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    /// <summary>
    /// 永远不会修改iid，uid，create time
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="DbContextType"></typeparam>
    public abstract class EFRepository<T, DbContextType> : EFRepositoryBase<T>
        where T : EntityBase
        where DbContextType : DbContext
    {
        private readonly IWCloudContext _context;
        public EFRepository(IServiceProvider provider) : base(() => provider.Resolve_<DbContextType>())
        {
            this._context = provider.Resolve_<IWCloudContext<EFRepository<T, DbContextType>>>();
        }
        public EFRepository(IWCloudContext<EFRepository<T, DbContextType>> _context) : base(() => _context.Provider.Resolve_<DbContextType>())
        {
            this._context = _context;
        }
        public EFRepository(IWCloudContext _context) : base(() => _context.Provider.Resolve_<DbContextType>())
        {
            this._context = _context;
        }

        protected override EntityEntry<T> __update__(ref DbContext db, T model)
        {
            var tracker = base.__update__(ref db, model);

            tracker.Property(x => x.Id).IsModified = false;
            tracker.Property(x => x.CreateTimeUtc).IsModified = false;

            return tracker;
        }
    }

    /// <summary>
    /// 通过泛型指定dbcontext
    /// </summary>
    [Obsolete]
    public abstract class EFRepositoryFromNew<T, DbContextType> : EFRepositoryBase<T>
        where T : class, IDBTable
        where DbContextType : DbContext, new()
    {
        protected EFRepositoryFromNew() : base(() => new DbContextType()) { }
    }

    public abstract partial class EFRepositoryBase<T> : IEFRepository<T> where T : class, IDBTable
    {
        private readonly Lazy<DbContext> _lazy_db;

        /// <summary>
        /// 懒加载，只有调用才会生成dbcontext
        /// </summary>
        public DbContext Database => this._lazy_db.Value;

        public DbSet<T> Table => this.Database.Set<T>();
        public IQueryable<T> TrakingQueryable => this.Table.AsQueryableTrackingOrNot(true);
        public IQueryable<T> NoTrackingQueryable => this.Table.AsQueryableTrackingOrNot(false);
        public IQueryable<T> Queryable => this.NoTrackingQueryable;

        protected EFRepositoryBase(Func<DbContext> db_context_getter)
        {
            db_context_getter.Should().NotBeNull();
            this._lazy_db = new Lazy<DbContext>(db_context_getter);
        }

        #region 添加
        public virtual int Insert(T model)
        {
            var db = this.Database;
            this.__add_bulk__(db, new[] { model });
            var res = db.SaveChanges();
            return res;
        }

        public virtual async Task<int> InsertAsync(T model)
        {
            var db = this.Database;
            this.__add_bulk__(db, new[] { model });
            var res = await db.SaveChangesAsync();
            return res;
        }

        protected virtual void __add_bulk__(DbContext context, IEnumerable<T> models)
        {
            context.Should().NotBeNull();
            models.Should().NotBeNull();
            models.Any(x => x == null).Should().BeFalse();

            //保证当前上下文干净，避免保存了额外的数据
            context.ThrowIfHasChanges();

            var set = context.Set<T>();

            set.AddRange(models);
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repo"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public virtual int InsertBulk(IEnumerable<T> models)
        {
            var db = this.Database;
            this.__add_bulk__(db, models);
            var res = db.SaveChanges();
            return res;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repo"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public virtual async Task<int> InsertBulkAsync(IEnumerable<T> models)
        {
            var db = this.Database;
            this.__add_bulk__(db, models);
            var res = await db.SaveChangesAsync();
            return res;
        }
        #endregion

        #region 删除

        protected virtual void __delete__(DbContext db, T model)
        {
            model.Should().NotBeNull();

            db.ThrowIfHasChanges();

            db.AttachIfNot(model).State = EntityState.Deleted;

            //db.Entry(model).State = EntityState.Deleted;
        }

        public virtual int Delete(T model)
        {
            var db = this.Database;
            this.__delete__(db, model);
            var res = db.SaveChanges();
            return res;
        }

        public virtual async Task<int> DeleteAsync(T model)
        {
            var db = this.Database;
            this.__delete__(db, model);
            var res = await db.SaveChangesAsync();
            return res;
        }

        protected virtual void __delete_where__(ref DbContext db, Expression<Func<T, bool>> where)
        {
            db.ThrowIfHasChanges();

            var range = this.Table.AsQueryable();
            range = range.WhereIfNotNull(where);

            db.Set<T>().RemoveRange(range);
        }

        public virtual int DeleteWhere(Expression<Func<T, bool>> where)
        {
            var db = this.Database;
            this.__delete_where__(ref db, where);
            var res = db.SaveChanges();
            return res;
        }

        public virtual async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            var db = this.Database;
            this.__delete_where__(ref db, where);
            var res = await db.SaveChangesAsync();
            return res;
        }
        #endregion

        #region 修改

        protected virtual EntityEntry<T> __update__(ref DbContext db, T model)
        {
            var entry = db.AttachIfNot(model);

            if (entry.State != EntityState.Modified)
            {
                entry.State = EntityState.Modified;
            }

            return entry;
        }

        public virtual int Update(T model)
        {
            var db = this.Database;
            this.__update__(ref db, model);
            var res = db.SaveChanges();
            return res;
        }

        public virtual async Task<int> UpdateAsync(T model)
        {
            var db = this.Database;
            this.__update__(ref db, model);
            var res = await db.SaveChangesAsync();
            return res;
        }

        #endregion

        #region 查询

        public virtual T QueryOneByKey(string key)
        {
            key.Should().NotBeNullOrEmpty("参数为空");

            return this.Table.Find(key);
        }

        public virtual async Task<T> QueryOneByKeyAsync(string key)
        {
            key.Should().NotBeNullOrEmpty("参数为空");

            return await this.Table.FindAsync(key);
        }

        protected virtual IQueryable<T> __query_many__<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby,
            bool desc,
            int? start,
            int? count)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);

            if (orderby != null)
            {
                query = query.OrderBy_(orderby, desc);
            }
            if (start != null)
            {
                orderby.Should().NotBeNull("使用skip前必须先排序");
                query = query.Skip(start.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }

            return query;
        }

        public T[] QueryMany<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var query = this.__query_many__(where, orderby: order_by, desc: desc, start: skip, count: count);
            var res = query.ToArray();
            return res;
        }

        public async Task<T[]> QueryManyAsync<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var query = this.__query_many__(where, orderby: order_by, desc: desc, start: skip, count: count);
            var res = await query.ToArrayAsync();
            return res;
        }

        public virtual T QueryOne(Expression<Func<T, bool>> where)
        {
            var query = this.TrakingQueryable;
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public virtual async Task<T> QueryOneAsync(Expression<Func<T, bool>> where)
        {
            var query = this.TrakingQueryable;
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }

        public virtual T QueryOneAsNoTrack(Expression<Func<T, bool>> where)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public virtual async Task<T> QueryOneAsNoTrackAsync(Expression<Func<T, bool>> where)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }

        public virtual int QueryCount(Expression<Func<T, bool>> where)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);
            return query.Count();
        }

        public virtual async Task<int> QueryCountAsync(Expression<Func<T, bool>> where)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);
            return await query.CountAsync();
        }

        public virtual bool Exist(Expression<Func<T, bool>> where)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);
            return query.Any();
        }

        public virtual async Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);
            return await query.AnyAsync();
        }
        #endregion

        public virtual void Dispose()
        {
            this.Database?.Dispose();
        }
    }
}
