using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lib.redis
{
    /// <summary>
    /// set
    /// </summary>
    public partial class RedisHelper
    {
        #region Set
        public bool SetAdd(string key, object value) => (this.Database.SetAdd(key, this._serializer.Serialize(value)));
        public List<T> SetMembers<T>(string key) => (this.Database.SetMembers(key).Select(x => this._serializer.Deserialize<T>(x)).ToList());
        public bool SetRemove<T>(string key, object value) => (this.Database.SetRemove(key, this._serializer.Serialize(value)));
        public List<T> SetCombine<T>(SetOperation operation, params string[] keys)
        {
            var redis_keys = keys.Select(x =>
            {
                RedisKey k = x;
                return k;
            }).ToArray();
            return (this.Database.SetCombine(operation, redis_keys).Select(x => this._serializer.Deserialize<T>(x)).ToList());
        }
        #endregion
    }
}
