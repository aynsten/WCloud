using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCloud.Framework.Redis
{
    public interface IRedisAll :
        IRedisString__,
        IRedisSortedSet__,
        IRedisKey__,
        IRedisHash__,
        IRedisList__
    {
        //
    }

    public interface IRedisString__
    {
        /// <summary>
        /// 保存一个对象
        /// </summary>
        bool StringSet(string key, object obj, TimeSpan? expiry = default(TimeSpan?));

        /// <summary>
        /// 获取一个key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T StringGet<T>(string key);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        double StringIncrement(string key, double val = 1);

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        double StringDecrement(string key, double val = 1);
    }

    public interface IRedisSortedSet__
    {
        bool SortedSetAdd(string key, object value, double score);
        bool SortedSetRemove<T>(string key, object value);
    }

    public interface IRedisKey__
    {
        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns>是否删除成功</returns>
        bool KeyDelete(string key);

        /// <summary>
        /// 判断key是否存储
        /// </summary>
        /// <param name="key">redis key</param>
        /// <returns></returns>
        bool KeyExists(string key);

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        bool KeyRename(string key, string newKey);

        /// <summary>
        /// 设置Key的时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?));
    }

    public interface IRedisHash__
    {
        bool HashSet(string key, string k, object value);

        bool HashDelete(string key, string k);
    }

    public interface IRedisList__
    {
        /// <summary>
        /// 移除指定ListId的内部List的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListRemove(string key, object value);

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<T> ListRange<T>(string key);

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void ListRightPush(string key, object value);

        /// <summary>
        /// 出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T ListRightPop<T>(string key);

        Task<T> BListRightPop<T>(string key, TimeSpan sleep, TimeSpan? timeout = null);

        /// <summary>
        /// 入栈
        /// </summary>
        long ListLeftPush(string key, object value);

        /// <summary>
        /// 出栈
        /// </summary>
        T ListLeftPop<T>(string key);

        Task<T> BListLeftPop<T>(string key, TimeSpan sleep, TimeSpan? timeout = null);

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long ListLength(string key);
    }
}
