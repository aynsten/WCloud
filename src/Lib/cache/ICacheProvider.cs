using System;
using System.Threading.Tasks;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace Lib.cache
{
    public interface ICacheProvider
    {
        void Remove(string key);
        Task RemoveAsync(string key);

        CacheResult<T> Get_<T>(string key);
        void Set_<T>(string key, T data, TimeSpan expire);

        Task<CacheResult<T>> GetAsync_<T>(string key);
        Task SetAsync_<T>(string key, T data, TimeSpan expire);

        T GetOrSet_<T>(string key, Func<T> source, TimeSpan expire, Func<T, bool> cache_when = null);
        Task<T> GetOrSetAsync_<T>(string key, Func<Task<T>> source, TimeSpan expire, Func<T, bool> cache_when = null);
    }

    public class DistributeCacheProvider : ICacheProvider
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;

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
            var option = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expire
            };
            this._cache.SetString(key, new CacheResult<T>(data).ToJson(), option);
        }

        public async Task<CacheResult<T>> GetAsync_<T>(string key)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentNullException(nameof(key));

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
            var option = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expire
            };
            await this._cache.SetStringAsync(key, new CacheResult<T>(data).ToJson(), option);
        }

        public T GetOrSet_<T>(string key, Func<T> source, TimeSpan expire, Func<T, bool> cache_when = null)
        {
            //默认都缓存
            cache_when ??= (x => true);
            try
            {
                var res = this.Get_<T>(key);
                if (res.Success)
                    return res.Data;

                var data = source.Invoke();

                //判断是否需要写入缓存
                var cache_data = cache_when.Invoke(data);
                if (cache_data)
                    this.Set_(key, data, expire);

                return data;
            }
            catch (Exception e)
            {
                try
                {
                    //读取缓存失败就尝试移除缓存
                    this._cache.Remove(key);
                }
                catch (Exception ex)
                {
                    this._logger.AddErrorLog($"删除错误缓存key异常-缓存key:{key}", ex);
                }
                this._logger.AddErrorLog($"读取缓存异常-缓存key:{key}", e);
                //缓存错误
                return source.Invoke();
            }
        }

        public async Task<T> GetOrSetAsync_<T>(string key, Func<Task<T>> source, TimeSpan expire, Func<T, bool> cache_when = null)
        {
            //默认都缓存
            cache_when ??= (x => true);
            try
            {
                var res = await this.GetAsync_<T>(key);
                if (res.Success)
                    return res.Data;

                var data = await source.Invoke();

                //判断是否需要写入缓存
                var cache_data = cache_when.Invoke(data);
                if (cache_data)
                    await this.SetAsync_(key, data, expire);

                return data;
            }
            catch (Exception e)
            {
                try
                {
                    //读取缓存失败就尝试移除缓存
                    await this._cache.RemoveAsync(key);
                }
                catch (Exception ex)
                {
                    this._logger.AddErrorLog($"删除错误缓存key异常-缓存key:{key}", ex);
                }
                this._logger.AddErrorLog($"读取缓存异常-缓存key:{key}", e);
                //缓存错误
                return await source.Invoke();
            }
        }

        public void Remove(string key)
        {
            this._cache.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            await this._cache.RemoveAsync(key);
        }
    }
}
