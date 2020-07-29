using Lib.extension;
using Lib.redis;
using StackExchange.Redis;
using System;

namespace Lib.cache
{
    /// <summary>
    /// ʹ��redis��Ϊ����
    /// </summary>
    public class RedisCacheProvider_
    {
        private readonly RedisHelper _redis;
        private readonly IDatabase _db;

        public RedisCacheProvider_(RedisConnectionWrapper con, int db)
        {
            this._redis = new RedisHelper(con.Connection, db);
            this._db = this._redis.Database;
        }

        #region Methods

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual CacheResult<T> Get<T>(string key)
        {
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

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        public virtual void Set(string key, object data, TimeSpan expire)
        {
            var res = new CacheResult<object>(data);
            var json = res.ToJson();

            this._db.StringSet(key, (string)json, expire);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsSet(string key)
        {
            var ret = this._db.KeyExists(key);
            return ret;
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            this._db.KeyDelete(key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var _muxer = this._redis.Connection;
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                var keys = server.Keys(pattern: "*" + pattern + "*");
                foreach (var key in keys)
                {
                    this._db.KeyDelete(key);
                }
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            var _muxer = this._redis.Connection;
            foreach (var ep in _muxer.GetEndPoints())
            {
                var server = _muxer.GetServer(ep);
                //we can use the code belwo (commented)
                //but it requires administration permission - ",allowAdmin=true"
                //server.FlushDatabase();

                //that's why we simply interate through all elements now
                var keys = server.Keys();
                foreach (var key in keys)
                {
                    this._db.KeyDelete(key);
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            //do nothing
        }

        #endregion
    }
}