using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lib.threading;
using StackExchange.Redis;

namespace Lib.redis
{
    /// <summary>
    /// list
    /// </summary>
    public partial class RedisHelper
    {
        #region List

        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRemove(string key, object value)
        {
            this.Database.ListRemove(key, this._serializer.Serialize(value));
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IEnumerable<T> ListRange<T>(string key)
        {
            var values = this.Database.ListRange(key);
            return values.Where(x => x.HasValue).Select(x => this._serializer.Deserialize<T>(x)).ToList();
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void ListRightPush(string key, object value)
        {
            this.Database.ListRightPush(key, this._serializer.Serialize(value));
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ListRightPop<T>(string key)
        {
            var value = this.Database.ListRightPop(key);
            return this._serializer.Deserialize<T>(value);
        }

        /*
         * 异步流，要预览版才支持
        async Task<IEnumerable<T>> __PopOrBlock__<T>(TimeSpan sleep, Func<IDatabase, Task<RedisValue>> func, Func<bool> stop)
        {
            while (!stop.Invoke())
            {
                var value = await func.Invoke(this.Database);
                if (value.HasValue)
                {
                    var res = this._serializer.Deserialize<T>(value);
                    yield return res;
                }

                //sleep
                await sleep;
            }
        }*/

        async Task<T> PopOrBlock<T>(TimeSpan sleep, TimeSpan? timeout, Func<IDatabase, Task<RedisValue>> func)
        {
            var timeout_at = timeout == null ? default(DateTime?) : DateTime.Now.Add(timeout.Value);

            while (true)
            {
                var value = await func.Invoke(this.Database);
                if (value.HasValue)
                {
                    var res = this._serializer.Deserialize<T>(value);
                    return res;
                }

                if (timeout_at != null && DateTime.Now > timeout_at.Value)
                    throw new TimeoutException("等待超时");

                //sleep
                await sleep;
            }
        }

        public Task<T> BListRightPop<T>(string key, TimeSpan sleep, TimeSpan? timeout = null)
        {
            return PopOrBlock<T>(sleep, timeout, db => db.ListRightPopAsync(key));
        }

        /// <summary>
        /// 入栈
        /// </summary>
        public long ListLeftPush(string key, object value)
        {
            return (this.Database.ListLeftPush(key, this._serializer.Serialize(value)));
        }

        /// <summary>
        /// 出栈
        /// </summary>
        public T ListLeftPop<T>(string key)
        {
            var value = this.Database.ListLeftPop(key);
            return this._serializer.Deserialize<T>(value);
        }

        public Task<T> BListLeftPop<T>(string key, TimeSpan sleep, TimeSpan? timeout = null)
        {
            return PopOrBlock<T>(sleep, timeout, db => db.ListLeftPopAsync(key));
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            return (this.Database.ListLength(key));
        }

        #endregion List
    }
}
