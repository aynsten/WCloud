using Lib.data;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Core.Authentication.Model;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Framework.Database.EntityFrameworkCore.Repository
{
    /// <summary>
    /// 慎用，自动隔离租户信息
    /// 可能存在不确定性，需要长期测试
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrgRepository__<T> : IEFRepository<T>
        where T : class, IDBTable, IOrgRow
    {
        /// <summary>
        /// 上下文的租户id
        /// </summary>
        string OrgUID { get; }
    }

    public abstract class OrgRepository<T, DbContextType> : WCloudEFRepository<T, DbContextType>, IOrgRepository__<T>
        where T : EntityBase, IDBTable, IOrgRow
        where DbContextType : DbContext
    {
        public string OrgUID { get; private set; }
        public OrgRepository(IServiceProvider provider, WCloudUserInfo user) : base(provider)
        {
            this.OrgUID = user.Org?.UID;
            if (ValidateHelper.IsEmpty(this.OrgUID))
                throw new ArgumentException("租户仓储无法获取当前租户上下文");
        }

        void CheckOrgField(T model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (ValidateHelper.IsNotEmpty(model.OrgUID))
            {
                if (model.OrgUID != this.OrgUID)
                    throw new NotSupportedException("当前model的租户字段和上下文冲突");
            }
        }

        SAVE_HANDLE __AddBulk__<SAVE_HANDLE>(DbContext context, IEnumerable<T> models, Func<DbContext, SAVE_HANDLE> save_handle)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (ValidateHelper.IsEmpty(models))
                throw new ArgumentNullException(nameof(models));
            if (models.Any(x => x == null))
                throw new ArgumentNullException("model存在null值");
            if (save_handle == null)
                throw new ArgumentNullException(nameof(save_handle));
            //check
            foreach (var m in models)
                this.CheckOrgField(m);
            //fill field
            foreach (var m in models)
                m.OrgUID = this.OrgUID;

            context.ThrowIfHasChanges();

            var set = context.Set<T>() ?? throw new ArgumentNullException(nameof(T));

            foreach (var m in models)
                set.Add(m);

            var res = save_handle.Invoke(context);
            return res;
        }

        public override int Add(T model)
        {
            var res = this.__AddBulk__(this.Database, new[] { model }, context => context.SaveChanges());
            return res;
        }

        public override async Task<int> AddAsync(T model)
        {
            var res = this.__AddBulk__(this.Database, new[] { model }, context => context.SaveChangesAsync());
            return await res;
        }

        public override int AddBulk(IEnumerable<T> models)
        {
            var res = __AddBulk__(this.Database, models, context => context.SaveChanges());
            return res;
        }

        public override async Task<int> AddBulkAsync(IEnumerable<T> models)
        {
            var res = __AddBulk__(this.Database, models, context => context.SaveChangesAsync());
            return await res;
        }

        /// <summary>
        /// 强制条件
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IQueryable<T> __Query__(IQueryable<T> query) => query.Where(x => x.OrgUID == this.OrgUID);

        public override List<T> QueryList<OrderByColumnType>(Expression<Func<T, bool>> where, Expression<Func<T, OrderByColumnType>> orderby = null, bool Desc = true, int? start = null, int? count = null)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
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
        }

        public override async Task<List<T>> QueryListAsync<OrderByColumnType>(Expression<Func<T, bool>> where, Expression<Func<T, OrderByColumnType>> orderby = null, bool Desc = true, int? start = null, int? count = null)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
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
        }

        public override bool Exist(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.Any();
        }

        public override async Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.AnyAsync();
        }

        public override int GetCount(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.Count();
        }

        public override async Task<int> GetCountAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.CountAsync();
        }

        public override T GetByKeys(string key)
        {
            throw new NotSupportedException(nameof(GetByKeys));
        }

        public override Task<T> GetByKeysAsync(string key)
        {
            throw new NotSupportedException(nameof(GetByKeysAsync));
        }

        public override T GetFirst(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.TrakingQueryable);
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public override async Task<T> GetFirstAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.TrakingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }

        public override T GetFirstAsNoTrack(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public override async Task<T> GetFirstAsNoTrackAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }
    }
}
