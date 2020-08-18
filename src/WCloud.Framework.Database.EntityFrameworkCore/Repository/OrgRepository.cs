using FluentAssertions;
using Lib.core;
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
    [Obsolete]
    public interface IOrgRepository__<T> : IEFRepository<T> where T : class, IDBTable, IOrgRow
    {
        /// <summary>
        /// 上下文的租户id
        /// </summary>
        string OrgUID { get; }
    }

    [Obsolete]
    public abstract class OrgRepository__<T, DbContextType> : EFRepository<T, DbContextType>, IOrgRepository__<T>
        where T : EntityBase, IDBTable, IOrgRow
        where DbContextType : DbContext
    {
        public string OrgUID { get; private set; }
        public OrgRepository__(IServiceProvider provider, WCloudUserInfo user) : base(provider)
        {
            this.OrgUID = user.Org?.Id;
            this.OrgUID.Should().NotBeNullOrEmpty("租户仓储无法获取当前租户上下文");
        }

        protected override void __add_bulk__(DbContext context, IEnumerable<T> models)
        {
            //fill field
            foreach (var m in models)
            {
                if (ValidateHelper.IsNotEmpty(m.OrgUID))
                {
                    (m.OrgUID == this.OrgUID).Should().BeTrue("当前model的租户字段和上下文冲突");
                }
                m.OrgUID = this.OrgUID;
            }
            base.__add_bulk__(context, models);
        }

        /// <summary>
        /// 强制条件
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IQueryable<T> __get_query__(IQueryable<T> query) => query.Where(x => x.OrgUID == this.OrgUID);

        public override bool Exist(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.Any();
        }

        public override async Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.AnyAsync();
        }

        public override int QueryCount(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.Count();
        }

        public override async Task<int> QueryCountAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.CountAsync();
        }

        public override T QueryOne(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.TrakingQueryable);
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public override async Task<T> QueryOneAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.TrakingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }

        public override T QueryOneAsNoTrack(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return query.FirstOrDefault();
        }

        public override async Task<T> QueryOneAsNoTrackAsync(Expression<Func<T, bool>> where)
        {
            var query = this.__get_query__(this.NoTrackingQueryable);
            query = query.WhereIfNotNull(where);
            return await query.FirstOrDefaultAsync();
        }
    }
}
