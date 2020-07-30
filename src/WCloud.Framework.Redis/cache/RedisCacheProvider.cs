using FluentAssertions;
using Lib.cache;
using Lib.extension;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using WCloud.Framework.Redis.implement;

namespace WCloud.Framework.Redis.cache
{
    /// <summary>
    /// ʹ��redis��Ϊ����
    /// </summary>
    public class RedisCacheProvider_ : ICacheProvider
    {
        private readonly RedisHelper _redis;
        private readonly IDatabase _db;
        private readonly ILogger _logger;

        public ILogger Logger => this._logger;

        public RedisCacheProvider_(RedisConnectionWrapper con, int db, ILogger<RedisCacheProvider_> logger)
        {
            this._redis = new RedisHelper(con.Connection, db);
            this._db = this._redis.Database;
            this._logger = logger;
        }

        #region Methods

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public bool IsSet(string key)
        {
            key.Should().NotBeNullOrEmpty();
            var res = this._db.KeyExists(key);
            return res;
        }

        public async Task<bool> IsSetAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();
            var res = await this._db.KeyExistsAsync(key);
            return res;
        }

        public void Remove(string key)
        {
            key.Should().NotBeNullOrEmpty();
            this._db.KeyDelete(key);
        }

        public async Task RemoveAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();
            await this._db.KeyDeleteAsync(key);
        }

        public CacheResult<T> Get_<T>(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var rValue = this._db.StringGet(key);
            if (rValue.HasValue)
            {
                var res = ((string)rValue).JsonToEntity<CacheResult<T>>(throwIfException: false);
                if (res != null)
                {
                    return res;
                }
            }

            return new CacheResult<T>();
        }

        public void Set_<T>(string key, T data, TimeSpan expire)
        {
            key.Should().NotBeNullOrEmpty();

            var res = new CacheResult<object>(data);
            var json = res.ToJson();

            this._db.StringSet(key, (string)json, expire);
        }

        public async Task<CacheResult<T>> GetAsync_<T>(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var rValue = await this._db.StringGetAsync(key);
            if (rValue.HasValue)
            {
                var res = ((string)rValue).JsonToEntity<CacheResult<T>>(throwIfException: false);
                if (res != null)
                {
                    return res;
                }
            }

            return new CacheResult<T>();
        }

        public async Task SetAsync_<T>(string key, T data, TimeSpan expire)
        {
            key.Should().NotBeNullOrEmpty();

            var res = new CacheResult<object>(data);
            var json = res.ToJson();

            await this._db.StringSetAsync(key, (string)json, expire);
        }

        #endregion
    }
}