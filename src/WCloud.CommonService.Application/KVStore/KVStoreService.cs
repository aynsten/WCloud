using FluentAssertions;
using Lib.extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.CommonService.Application.KVStore
{
    public class KVStoreService : BasicService<KVStoreEntity>, IKVStoreService
    {
        public KVStoreService(IServiceProvider provider, ICommonServiceRepository<KVStoreEntity> repo) : base(provider, repo)
        {
            //
        }

        public async Task<string> GetValue(string key)
        {
            key.Should().NotBeNullOrEmpty("kv get value");

            var res = await this._repo.Table.AsNoTracking().OrderByDescending(x => x.CreateTimeUtc).FirstOrDefaultAsync(x => x.Key == key);

            return res?.Value;
        }

        public async Task<T> GetValue<T>(string key) where T : class
        {
            var value = await this.GetValue(key);
            if (value == null)
            {
                return default;
            }
            else
            {
                try
                {
                    return value.JsonToEntity<T>() ?? throw new ArgumentException();
                }
                catch (Exception e)
                {
                    this._logger.AddWarningLog(msg: $"{value}不能转成{typeof(T).FullName}", e: e);
                    return default;
                }
            }
        }

        public async Task SetValue<T>(string key, T data) where T : class
        {
            key.Should().NotBeNullOrEmpty("kv set value key");
            data.Should().NotBeNull("kv set value data");

            var db = this._repo.Database;

            var set = db.Set<KVStoreEntity>();
            set.RemoveRange(set.Where(x => x.Key == key));
            await db.SaveChangesAsync();

            var entity = new KVStoreEntity()
            {
                Key = key,
                Value = data.ToJson()
            };
            entity.InitSelf();

            set.Add(entity);

            await db.SaveChangesAsync();
        }
    }
}
