using Lib.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lib.redis
{
    /// <summary>
    /// hash
    /// </summary>
    public partial class RedisHelper
    {
        public bool HashSet(string key, string k, object value) =>
            this.Database.HashSet(key, k, this._serializer.Serialize(value));

        public bool HashDelete(string key, string k) =>
            this.Database.HashDelete(key, k);
    }
}
