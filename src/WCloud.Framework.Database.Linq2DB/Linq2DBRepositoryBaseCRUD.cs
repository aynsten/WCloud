using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lib.extension;
using LinqToDB;

namespace WCloud.Framework.Database.Linq2DB
{
    public abstract partial class Linq2DBRepositoryBase<T>
    {
        public int Add(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.Insert(model);
        }

        public Task<int> AddAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.InsertAsync(model);
        }

        public int Delete(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.Delete(model);
        }

        public Task<int> DeleteAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.DeleteAsync(model);
        }

        public int DeleteWhere(Expression<Func<T, bool>> where)
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            return this.Database.Delete(where);
        }

        public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            if (where == null)
                throw new ArgumentNullException(nameof(where));

            return this.Database.DeleteAsync(where);
        }

        public bool Exist(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).Any();
        }

        public Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).AnyAsync();
        }

        public int GetCount(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).Count();
        }

        public Task<int> GetCountAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).CountAsync();
        }

        public T GetFirst(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).FirstOrDefault();
        }

        public Task<T> GetFirstAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).FirstOrDefaultAsync();
        }

        public List<T> GetList(Expression<Func<T, bool>> where, int? count = null)
        {
            return this.QueryList<object>(where: where, count: count);
        }

        public Task<List<T>> GetListAsync(Expression<Func<T, bool>> where, int? count = null)
        {
            return this.QueryListAsync<object>(where: where, count: count);
        }

        public List<T> QueryList<OrderByColumnType>(Expression<Func<T, bool>> where, Expression<Func<T, OrderByColumnType>> orderby = null, bool Desc = true, int? start = null, int? count = null)
        {
            var query = this.Queryable;
            query = query.WhereIfNotNull(where);
            if (orderby != null)
            {
                if (Desc)
                    query = query.OrderByDescending(orderby);
                else
                    query = query.OrderBy(orderby);
            }
            if (start != null)
            {
                if (orderby == null)
                    throw new ArgumentNullException("必须先排序");
                query = query.Skip(start.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }
            return query.ToList();
        }

        public Task<List<T>> QueryListAsync<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null,
            bool Desc = true, int? start = null, int? count = null)
        {
            var query = this.Queryable;
            query = query.WhereIfNotNull(where);
            if (orderby != null)
            {
                if (Desc)
                    query = query.OrderByDescending(orderby);
                else
                    query = query.OrderBy(orderby);
            }
            if (start != null)
            {
                if (orderby == null)
                    throw new ArgumentNullException("必须先排序");
                query = query.Skip(start.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }
            return query.ToListAsync();
        }

        public int Update(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.Update(model);
        }

        public Task<int> UpdateAsync(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return this.Database.UpdateAsync(model);
        }
    }
}
