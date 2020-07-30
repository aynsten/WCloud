using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Framework.Redis.implement
{
    /// <summary>
    /// sorted set
    /// </summary>
    public partial class RedisHelper
    {
        #region SortedSet
        public bool SortedSetAdd(string key, object value, double score) => 
            (this.Database.SortedSetAdd(key, this._serializer.Serialize(value), score));

        public bool SortedSetRemove<T>(string key, object value) => 
            (this.Database.SortedSetRemove(key, this._serializer.Serialize(value)));
        #endregion
    }
}
