using Lib.cache;
using Lib.helper;
using Lib.redis;
using System;
using System.Threading.Tasks;

namespace WCloud.Framework.Redis.KV
{
    /// <summary>
    /// kv存储，在数据允许丢失的情况下使用
    /// consumer计算出count，保存到kv store，前台接口直接读取kv获取计算好的数据
    /// </summary>
    public interface IKVStore
    {
        Task<T> GetAsync<T>(string key, T deft = default);

        Task SetAsync<T>(string key, T data);

        Task RemoveAsync(string key);
    }

    public class RedisKVStore : IKVStore
    {
        private readonly IRedisAll _redis;
        public RedisKVStore(IRedisAll redis)
        {
            this._redis = redis;
        }

        void __check_key__(string key)
        {
            if (ValidateHelper.IsEmpty(key))
                throw new ArgumentNullException(nameof(key));
        }

        public async Task<T> GetAsync<T>(string key, T deft = default)
        {
            this.__check_key__(key);
            await Task.CompletedTask;

            try
            {
                var res = this._redis.StringGet<CacheResult<T>>(key);
                return res.Data;
            }
            catch
            {
                return default;
            }
        }

        public async Task RemoveAsync(string key)
        {
            this.__check_key__(key);
            await Task.CompletedTask;
            this._redis.KeyDelete(key);
        }

        public async Task SetAsync<T>(string key, T data)
        {
            this.__check_key__(key);
            await Task.CompletedTask;
            this._redis.StringSet(key, new CacheResult<T>(data));
        }
    }
}
