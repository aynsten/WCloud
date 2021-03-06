﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Framework.Redis.implement
{
    /// <summary>
    /// string
    /// </summary>
    public partial class RedisHelper
    {
        #region String

        /// <summary>
        /// 保存一个对象
        /// </summary>
        public bool StringSet(string key, object obj, TimeSpan? expiry = default(TimeSpan?))
        {
            return (this.Database.StringSet(key, this._serializer.SerializeToBytes(obj), expiry));
        }

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T StringGet<T>(string key)
        {
            var data = this.Database.StringGet(key);
            if (data.HasValue)
            {
                var bs = (byte[])data;
                var res = this._serializer.DeserializeFromBytes<T>(bs);
                return res;
            }
            return default;
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val = 1)
        {
            return (this.Database.StringIncrement(key, val));
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double StringDecrement(string key, double val = 1)
        {
            return (this.Database.StringDecrement(key, val));
        }
        #endregion String
    }
}
