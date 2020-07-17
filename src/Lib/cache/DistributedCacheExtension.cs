using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lib.cache
{
    public static class DistributedCacheExtension
    {
        [Obsolete]
        public static async Task<CacheResult<T>> GetAsync_<T>(this IDistributedCache cache, string key)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var json = await cache.GetStringAsync(key);
            if (ValidateHelper.IsNotEmpty(json))
                return json.JsonToEntity<CacheResult<T>>()?.OK() ??
                    throw new CacheException($"读取缓存错误，key:{key}");

            return new CacheResult<T>();
        }

        [Obsolete]
        public static async Task SetAsync_<T>(this IDistributedCache cache, string key, T data, TimeSpan expire)
        {
            var option = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = expire
            };
            await cache.SetStringAsync(key, new CacheResult<T>(data).ToJson(), option);
        }


        /// <summary>
        /// 节流
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="range"></param>
        /// <param name="max_retry"></param>
        /// <returns></returns>
        public static async Task<ThrottleResponse<T>> Throttle_<T>(this IDistributedCache cache,
            string key, Func<Task<ThrottleResponse<T>>> func,
            TimeSpan range, int max_retry)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentNullException(nameof(key));

            var now = DateTime.Now;
            var data_after = now - range;
            var data = (await cache.GetAsync_<List<DateTime>>(key)).Data ?? new List<DateTime>();

            data = data.Where(x => x > data_after).ToList();
            if (data.Count > max_retry)
                return ThrottleResponse<T>.Error();

            var response = await func.Invoke();
            if (!response.Success)
                data.Add(now);
            //更新记录
            await cache.SetAsync_(key, data, range);

            return response;
        }

    }

    public class ThrottleResponse<T> : IDataContainer<T>
    {
        public bool Success { get; set; } = false;

        public T Data { get; set; }

        public static ThrottleResponse<T> FromSuccess(T data) =>
            new ThrottleResponse<T>()
            {
                Success = true,
                Data = data
            };

        public static ThrottleResponse<T> Error() => new ThrottleResponse<T>();
    }
}
