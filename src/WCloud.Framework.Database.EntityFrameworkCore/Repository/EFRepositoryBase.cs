using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.data;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
            this.__add_bulk__(this.Database, new[] { model });
            var res = this.Database.SaveChanges();
            return res;
        }

        public virtual async Task<int> InsertAsync(T model)
        {
            this.__add_bulk__(this.Database, new[] { model });
            var res = await this.Database.SaveChangesAsync();
            return res;
        }

        void __add_bulk__(DbContext context, IEnumerable<T> models)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (ValidateHelper.IsEmpty(models))
                throw new ArgumentNullException(nameof(models));
            if (models.Any(x => x == null))
                throw new ArgumentNullException("model存在null值");

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
            this.__add_bulk__(this.Database, models);
            var res = this.Database.SaveChanges();
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
            this.__add_bulk__(this.Database, models);
            var res = await this.Database.SaveChangesAsync();
            return res;
        }
        #endregion

        #region 删除

        R __delete__<R>(T model, Func<DbContext, R> handler)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            this.Database.ThrowIfHasChanges();

            this.Database.Entry(model).State = EntityState.Deleted;

            return handler.Invoke(this.Database);
        }

        public virtual int Delete(T model)
        {
            var res = this.__delete__(model, db => db.SaveChanges());
            return res;
        }

        public virtual async Task<int> DeleteAsync(T model)
        {
            var res = this.__delete__(model, db => db.SaveChangesAsync());
            return await res;
        }

        void __delete_where__(Expression<Func<T, bool>> where)
        {
            this.Database.ThrowIfHasChanges();

            var range = this.Table.AsQueryable();
            range = range.WhereIfNotNull(where);

            this.Table.RemoveRange(range);
        }

        public virtual int DeleteWhere(Expression<Func<T, bool>> where)
        {
            this.__delete_where__(where);
            var res = this.Database.SaveChanges();
            return res;
        }

        public virtual async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            this.__delete_where__(where);
            var res = await this.Database.SaveChangesAsync();
            return res;
        }
        #endregion

        #region 修改

        protected virtual EntityEntry<T> __TrackEntity__(T model)
        {
            var tracker = this.Database.Entry(model) ?? throw new NotSupportedException(nameof(model));

            if (tracker.State != EntityState.Modified)
            {
                tracker.State = EntityState.Modified;
                //warning here
            }

            return tracker;
        }

        void __update__(T model)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            //this.Database.ThrowIfHasChanges();

            this.__TrackEntity__(model);
        }

        public virtual int Update(T model)
        {
            this.__update__(model);
            var res = this.Database.SaveChanges();
            return res;
        }

        public virtual async Task<int> UpdateAsync(T model)
        {
            this.__update__(model);
            var res = await this.Database.SaveChangesAsync();
            return res;
        }

        #endregion

        #region 查询

        public virtual T GetByKeys(string key)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentException("参数为空");

            return this.Table.Find(key);
        }

        public virtual async Task<T> GetByKeysAsync(string key)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentException("参数为空");

            return await this.Table.FindAsync(key);
        }

        IQueryable<T> __query_many__<OrderByColumnType>(
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
                if (orderby == null)
                    throw new ArgumentException("使用skip前必须先排序");
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
