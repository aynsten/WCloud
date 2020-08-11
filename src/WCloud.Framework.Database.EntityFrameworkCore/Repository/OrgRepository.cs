using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lib.data;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
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
            this.OrgUID = user.Org?.Id;
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

        public override int Insert(T model)
        {
            var res = this.__AddBulk__(this.Database, new[] { model }, context => context.SaveChanges());
            return res;
        }

        public override async Task<int> InsertAsync(T model)
        {
            var res = this.__AddBulk__(this.Database, new[] { model }, context => context.SaveChangesAsync());
            return await res;
        }

        public override int InsertBulk(IEnumerable<T> models)
        {
            var res = __AddBulk__(this.Database, models, context => context.SaveChanges());
            return res;
        }

        public override async Task<int> InsertBulkAsync(IEnumerable<T> models)
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

        public override int QueryCount(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.Count();
        }

        public override async Task<int> QueryCountAsync(Expression<Func<T, bool>> where)
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

        public override T QueryOne(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.TrakingQueryable);
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public override async Task<T> QueryOneAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.TrakingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }

        public override T QueryOneAsNoTrack(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public override async Task<T> QueryOneAsNoTrackAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__Query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }
    }
}
