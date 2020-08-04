using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    public abstract partial class EFRepositoryBase<T>
    {
        #region 添加
        public virtual int Insert(T model)
        {
            var res = this.__AddBulk(this.Database, new[] { model }, context => context.SaveChanges());
            return res;
        }

        public virtual async Task<int> InsertAsync(T model)
        {
            var res = this.__AddBulk(this.Database, new[] { model }, context => context.SaveChangesAsync());
            return await res;
        }

        RES __AddBulk<RES>(DbContext context, IEnumerable<T> models, Func<DbContext, RES> save_handle)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (ValidateHelper.IsEmpty(models))
                throw new ArgumentNullException(nameof(models));
            if (models.Any(x => x == null))
                throw new ArgumentNullException("model存在null值");
            if (save_handle == null)
                throw new ArgumentNullException(nameof(save_handle));

            //保证当前上下文干净，避免保存了额外的数据
            context.ThrowIfHasChanges();

            var set = context.Set<T>();

            set.AddRange(models);

            var res = save_handle.Invoke(context);
            return res;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repo"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public virtual int AddBulk(IEnumerable<T> models)
        {
            var res = this.__AddBulk(this.Database, models, context => context.SaveChanges());
            return res;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repo"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public virtual async Task<int> AddBulkAsync(IEnumerable<T> models)
        {
            var res = this.__AddBulk(this.Database, models, context => context.SaveChangesAsync());
            return await res;
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

        R __delete_where__<R>(Expression<Func<T, bool>> where, Func<DbContext, R> handler)
        {
            this.Database.ThrowIfHasChanges();

            var range = this.Table.AsQueryable();
            range = range.WhereIfNotNull(where);

            this.Table.RemoveRange(range);

            return handler.Invoke(this.Database);
        }

        public virtual int DeleteWhere(Expression<Func<T, bool>> where)
        {
            var res = this.__delete_where__(where, db => db.SaveChanges());
            return res;
        }

        public virtual async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            var res = this.__delete_where__(where, db => db.SaveChangesAsync());
            return await res;
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

        R __update__<R>(T model, Func<DbContext, R> handler)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            //this.Database.ThrowIfHasChanges();

            this.__TrackEntity__(model);

            return handler.Invoke(this.Database);
        }

        public virtual int Update(T model)
        {
            var res = this.__update__(model, db => db.SaveChanges());
            return res;
        }

        public virtual async Task<int> UpdateAsync(T model)
        {
            var res = this.__update__(model, db => db.SaveChangesAsync());
            return await res;
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

        R __query_list__<OrderByColumnType, R>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby,
            bool desc,
            int? start,
            int? count,
            Func<IQueryable<T>, R> handler)
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

            return handler.Invoke(query);
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

        public virtual async Task<List<T>> QueryListAsync<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null,
            bool Desc = true,
            int? start = null,
            int? count = null)
        {
            var res = this.__query_list__(where, orderby, Desc, start, count, query => query.ToListAsync());
            return await res;
        }

        public virtual List<T> QueryMany(Expression<Func<T, bool>> where, int? count = null)
        {
            return QueryMany<object>(where: where, count: count).ToList();
        }

        public virtual async Task<List<T>> QueryManyAsync(Expression<Func<T, bool>> where, int? count = null)
        {
            return await QueryListAsync<object>(where: where, count: count);
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

        public virtual T GetFirstAsNoTrack(Expression<Func<T, bool>> where)
        {
            var query = this.NoTrackingQueryable;
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public virtual async Task<T> GetFirstAsNoTrackAsync(Expression<Func<T, bool>> where)
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

    }
}
