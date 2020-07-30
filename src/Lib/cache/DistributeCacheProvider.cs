using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Lib.cache
{
    public class DistributeCacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

        public ILogger Logger => this._logger;

        public DistributeCacheProvider(IDistributedCache _cache, ILogger<DistributeCacheProvider> _logger)
        {
            this._cache = _cache;
            this._logger = _logger;
        }

        public CacheResult<T> Get_<T>(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var json = this._cache.GetString(key);
            if (ValidateHelper.IsNotEmpty(json))
            {
                var res = json.JsonToEntity<CacheResult<T>>(throwIfException: false);
                if (res == null)
                {
                    throw new CacheException($"读取缓存错误，key:{key},data:{json},type:{typeof(T).FullName}");
                }
                return res.OK();
            }

            return new CacheResult<T>();
        }

        public void Set_<T>(string key, T data, TimeSpan expire)
        {
            key.Should().NotBeNullOrEmpty();

            var option = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expire
            };
            this._cache.SetString(key, new CacheResult<T>(data).ToJson(), option);
        }

        public async Task<CacheResult<T>> GetAsync_<T>(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var json = await this._cache.GetStringAsync(key);
            if (ValidateHelper.IsNotEmpty(json))
            {
                var res = json.JsonToEntity<CacheResult<T>>()?.OK();
                if (res == null)
                {
                    throw new CacheException($"读取缓存错误，key:{key},data:{json},type:{typeof(T).FullName}");
                }
                return res;
            }

            return new CacheResult<T>();
        }

        public async Task SetAsync_<T>(string key, T data, TimeSpan expire)
        {
            key.Should().NotBeNullOrEmpty();

            var option = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expire
            };
            await this._cache.SetStringAsync(key, new CacheResult<T>(data).ToJson(), option);
        }

        public void Remove(string key)
        {
            key.Should().NotBeNullOrEmpty();

            this._cache.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();

            await this._cache.RemoveAsync(key);
        }

        public bool IsSet(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var bs = this._cache.Get(key);
            return bs?.Length > 0;
        }

        public async Task<bool> IsSetAsync(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var bs = await this._cache.GetAsync(key);
            return bs?.Length > 0;
        }
    }
}
