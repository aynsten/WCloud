using FluentAssertions;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.Abstractions.Service;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;

namespace WCloud.Framework.Database.EntityFrameworkCore.Service
{
    public abstract class BasicService<T> : IBasicService<T> where T : EntityBase
    {
        protected readonly _<string> SUCCESS = new _<string>().SetSuccessData(string.Empty);

        protected readonly ILogger _logger;
        protected readonly IServiceProvider _provider;
        protected readonly IEFRepository<T> _repo;

        public BasicService(IServiceProvider provider, IEFRepository<T> _repo)
        {
            provider.Should().NotBeNull();
            _repo.Should().NotBeNull();

            this._provider = provider;
            this._repo = _repo;
            this._logger = provider.Resolve_<ILogger<BasicService<T>>>();
        }

        protected virtual void ValidModel(T model) { }

        protected virtual Task CheckBeforeInsert(T model) => Task.CompletedTask;

        public virtual async Task Add(T data)
        {
            data.Should().NotBeNull();

            var model = data.InitSelf();

            this.ValidModel(model);

            await this.CheckBeforeInsert(model);

            await this._repo.AddAsync(model);
        }

        public virtual async Task Delete(params string[] uids)
        {
            uids.Should().NotBeNullOrEmpty();

            await this._repo.DeleteByIds(uids);
        }

        public virtual async Task<IEnumerable<T>> QueryTop<SortField>(int count, Expression<Func<T, SortField>> orderby, bool desc)
        {
            count.Should().BeInRange(1, 5000);

            var res = await this._repo.QueryListAsync(
                where: x => x.Id >= 0,
                orderby: orderby,
                desc: desc,
                count: count);

            return res;
        }

        public virtual async Task<IEnumerable<T>> QueryTop(int count)
        {
            count.Should().BeInRange(1, 5000);

            var res = await this._repo.GetListAsync(x => x.Id >= 0, count: count);
            return res;
        }

        protected virtual IQueryable<T> SearchKeyword(IQueryable<T> query, string q)
        {
            return query;
        }

        public virtual async Task<PagerData<T>> Query(string q, int page, int pagesize)
        {
            var res = await this.Query(q, page, pagesize, x => x.Id, true);
            return res;
        }

        public virtual async Task<PagerData<T>> Query<SortField>(string q, int page, int pagesize,
            Expression<Func<T, SortField>> orderby, bool desc)
        {
            page.Should().BeGreaterOrEqualTo(1);
            pagesize.Should().BeInRange(1, 5000);

            var db = this._repo.Database;

            var query = db.Set<T>().AsNoTrackingQueryable();

            if (ValidateHelper.IsNotEmpty(q))
            {
                query = this.SearchKeyword(query, q);
            }

            var res = await query.ToPagedListAsync(page, pagesize, orderby: orderby, desc: desc);

            return res;
        }

        protected virtual object UpdateField(T data)
        {
            return new { };
        }

        protected virtual Task CheckBeforeUpdate(T model) => Task.CompletedTask;

        public virtual async Task Update(T data)
        {
            data.Should().NotBeNull();
            data.UID.Should().NotBeNullOrEmpty();

            var model = await this._repo.GetFirstAsync(x => x.UID == data.UID);
            model.Should().NotBeNull();

            var update_fields = this.UpdateField(data);

            update_fields.Should().NotBeNull($"{nameof(this.UpdateField)}不能返回空");

            model.SetField(update_fields);

            this.ValidModel(model);

            await this.CheckBeforeUpdate(model);

            await this._repo.UpdateAsync(model);
        }

        public virtual async Task<T> GetByUID(string uid)
        {
            uid.Should().NotBeNullOrEmpty();

            var res = await this._repo.GetFirstAsNoTrackAsync(x => x.UID == uid);

            return res;
        }

        public virtual async Task<IEnumerable<T>> QueryByMaxID(int max_id, int count)
        {
            count.Should().BeInRange(1, 5000);

            var query = this._repo.NoTrackingQueryable;

            query = query.Where(x => x.Id > max_id).OrderBy(x => x.Id).Take(count);

            var res = await query.ToArrayAsync();

            return res;
        }

        public virtual async Task<IEnumerable<T>> QueryByMinID(int? min_id, int count)
        {
            count.Should().BeInRange(1, 5000);

            var query = this._repo.NoTrackingQueryable;

            if (min_id != null)
            {
                query = query.Where(x => x.Id < min_id.Value);
            }

            query = query.OrderByDescending(x => x.Id).Take(count);

            var res = await query.ToArrayAsync();

            return res;
        }
    }
}
