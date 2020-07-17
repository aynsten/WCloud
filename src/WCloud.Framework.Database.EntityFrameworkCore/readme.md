``` c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;

namespace Lib.entityframework
{
    public abstract partial class EFRepositoryBase<T>
    {
        #region 添加

        public int Add(T model)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            return PrepareSession(db =>
            {
                var set = db.Set<T>();
                set.Add(model);
                return db.SaveChanges();
            });
        }

        public async Task<int> AddAsync(T model)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            return await PrepareSessionAsync(async db =>
            {
                var set = db.Set<T>();
                set.Add(model);
                return await db.SaveChangesAsync();
            });
        }
        #endregion

        #region 删除

        public int Delete(T model)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            return PrepareSession(db =>
            {
                var set = db.Set<T>();
                //添加追踪（引用），如果多个追踪对象包含相同key就会抛出异常
                db.Entry(model).State = EntityState.Deleted;
                return db.SaveChanges();
            });
        }

        public async Task<int> DeleteAsync(T model)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            return await PrepareSessionAsync(async db =>
            {
                var set = db.Set<T>();
                //添加追踪（引用），如果多个追踪对象包含相同key就会抛出异常
                db.Entry(model).State = EntityState.Deleted;
                return await db.SaveChangesAsync();
            });
        }

        public int DeleteWhere(Expression<Func<T, bool>> where)
        {
            return PrepareSession(db =>
            {
                var set = db.Set<T>();
                var q = set.AsQueryable();

                q = q.WhereIfNotNull(where);

                set.RemoveRange(q);

                return db.SaveChanges();
            });
        }

        public async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            return await PrepareSessionAsync(async db =>
            {
                var set = db.Set<T>();
                var q = set.AsQueryable();

                q = q.WhereIfNotNull(where);

                set.RemoveRange(q);

                return await db.SaveChangesAsync();
            });
        }
        #endregion

        #region 修改

        public int Update(T model)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            return PrepareSession(db =>
            {
                var set = db.Set<T>();

                if (!set.Local.Any(x => x == model))
                    //添加追踪（引用），如果多个追踪对象包含相同key就会抛出异常
                    db.Entry(model).State = EntityState.Modified;

                return db.SaveChanges();
            });
        }

        public async Task<int> UpdateAsync(T model)
        {
            if (model == null)
                throw new ArgumentException("参数为空");

            return await PrepareSessionAsync(async db =>
            {
                var set = db.Set<T>();

                if (!set.Local.Any(x => x == model))
                    //添加追踪（引用），如果多个追踪对象包含相同key就会抛出异常
                    db.Entry(model).State = EntityState.Modified;

                return await db.SaveChangesAsync();
            });
        }

        #endregion

        #region 查询

        public T GetByKeys(string key)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentException("参数为空");

            return PrepareSession(db =>
            {
                return db.Set<T>().Find(key);
            });
        }

        public async Task<T> GetByKeysAsync(string key)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentException("参数为空");

            return await PrepareSessionAsync(async db =>
            {
                return await db.Set<T>().FindAsync(key);
            });
        }

        public List<T> QueryList<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null,
            bool Desc = true,
            int? start = null,
            int? count = null)
        {
            return PrepareIQueryable(query =>
            {
                query = query.WhereIfNotNull(where);

                if (orderby != null)
                {
                    query = query.OrderBy_(orderby, Desc);
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
                return query.ToList();
            });
        }

        public async Task<List<T>> QueryListAsync<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby = null,
            bool Desc = true,
            int? start = null,
            int? count = null)
        {
            return await PrepareIQueryableAsync(async query =>
            {
                query = query.WhereIfNotNull(where);

                if (orderby != null)
                {
                    query = query.OrderBy_(orderby, Desc);
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
                return await query.ToListAsync();
            });
        }

        public List<T> GetList(Expression<Func<T, bool>> where, int? count = null)
        {
            return QueryList<object>(where: where, count: count);
        }
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> where, int? count = null)
        {
            return await QueryListAsync<object>(where: where, count: count);
        }

        public T GetFirst(Expression<Func<T, bool>> where)
        {
            var list = GetList(where: where, count: 1);
            return list.FirstOrDefault();
        }
        public async Task<T> GetFirstAsync(Expression<Func<T, bool>> where)
        {
            var list = await GetListAsync(where: where, count: 1);
            return list.FirstOrDefault();
        }

        public int GetCount(Expression<Func<T, bool>> where)
        {
            return PrepareIQueryable(query =>
            {
                query = query.WhereIfNotNull(where);
                return query.Count();
            });
        }
        public async Task<int> GetCountAsync(Expression<Func<T, bool>> where)
        {
            return await PrepareIQueryableAsync(async query =>
            {
                query = query.WhereIfNotNull(where);
                return await query.CountAsync();
            });
        }

        public bool Exist(Expression<Func<T, bool>> where)
        {
            return PrepareIQueryable(query =>
            {
                query = query.WhereIfNotNull(where);
                return query.Any();
            });
        }
        public async Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            return await PrepareIQueryableAsync(async query =>
            {
                query = query.WhereIfNotNull(where);
                return await query.AnyAsync();
            });
        }
        #endregion

    }
}

```