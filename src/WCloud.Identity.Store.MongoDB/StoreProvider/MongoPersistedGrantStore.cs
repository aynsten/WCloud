using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Lib.helper;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.MongoDB;
using WCloud.Identity.Store.MongoDB.Entity;

namespace WCloud.Identity.Store.MongoDB.StoreProvider
{
    public class MongoPersistedGrantStore : IPersistedGrantStore
    {
        private readonly IWCloudContext<MongoPersistedGrantStore> wcloud_context;
        private readonly IIdsRepository<MongoPersistedGrantEntity> _repo;

        public MongoPersistedGrantStore(
            IWCloudContext<MongoPersistedGrantStore> wcloud_context,
            IIdsRepository<MongoPersistedGrantEntity> _repo)
        {
            this.wcloud_context = wcloud_context;
            this._repo = _repo;
        }

        PersistedGrant __map__(MongoPersistedGrantEntity data)
        {
            data.Should().NotBeNull();
            var res = this.wcloud_context.ObjectMapper.Map<MongoPersistedGrantEntity, PersistedGrant>(data);
            return res;
        }

        MongoPersistedGrantEntity __map__(PersistedGrant data)
        {
            data.Should().NotBeNull();
            var res = this.wcloud_context.ObjectMapper.Map<PersistedGrant, MongoPersistedGrantEntity>(data);
            return res;
        }

        IFindFluent<MongoPersistedGrantEntity, MongoPersistedGrantEntity> __query__(PersistedGrantFilter filter)
        {
            filter.Should().NotBeNull();
            var query = Builders<MongoPersistedGrantEntity>.Filter.Empty;

            query = query.WhereIf(ValidateHelper.IsNotEmpty(filter.SessionId), x => x.SessionId == filter.SessionId);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(filter.SubjectId), x => x.SubjectId == filter.SubjectId);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(filter.ClientId), x => x.ClientId == filter.ClientId);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(filter.Type), x => x.Type == filter.Type);

            var res = this._repo.Collection.Find(query);
            return res;
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var query = this.__query__(filter);

            var data = await query.Take(5000).ToListAsync();

            var res = data.Select(x => this.__map__(x)).ToArray();

            return res;
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var data = await this._repo.QueryOneAsync(x => x.Key == key);

            if (data == null)
            {
                this.wcloud_context.Logger.LogInformation($"grant dose not exists:{key}");
                return null;
            }

            var res = this.__map__(data);
            return res;
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            //批量可以在极端环境下避免数据库阻塞
            var batch_size = 1000;
            while (true)
            {
                var query = this.__query__(filter);
                var data = await query.Take(batch_size).ToListAsync();
                if (!data.Any())
                    break;
                var ids = data.Select(x => x.Id).ToArray();
                await this._repo.DeleteWhereAsync(x => ids.Contains(x.Id));
                if (ids.Length < batch_size)
                    return;
            }
        }

        public async Task RemoveAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();
            await this._repo.DeleteWhereAsync(x => x.Key == key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            grant.Should().NotBeNull();

            var entity = this.__map__(grant);
            entity.CreateTimeUtc = DateTime.UtcNow;

            await this._repo.InsertAsync(entity);
        }
    }
}
