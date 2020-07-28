using System;
using System.Threading.Tasks;
using Lib.distributed;
using Lib.helper;
using Polly;
using StackExchange.Redis;

namespace Lib.redis
{
    /// <summary>
    /// Redis分布式锁
    /// https://github.com/KidFashion/redlock-cs
    /// 必须考虑到【锁的续租】，redis key是会过期的，如果拿到锁后未能很快处理完任务，那么要对锁进行续期。
    /// 否则别人就会拿到锁，抢占你的资源
    /// </summary>
    public class RedisDistributedLock : IDistributedLock
    {
        private readonly string _key;
        private readonly byte[] _value;

        private readonly RedisHelper _redis;
        private readonly TimeSpan _expired;
        private readonly AsyncPolicy _retryAsync;

        public RedisDistributedLock(IConnectionMultiplexer connection, int db,
            string source_name, TimeSpan? expired = null, int retryCount = 50, Func<int, TimeSpan> interval = null)
        {
            this._key = source_name ?? throw new ArgumentNullException(nameof(source_name));
            this._value = Guid.NewGuid().ToByteArray();

            this._redis = new RedisHelper(connection, db);
            interval = interval ?? (i => TimeSpan.FromMilliseconds(100 * i));
            this._retryAsync = Policy.Handle<Exception>().WaitAndRetryAsync(retryCount, interval);
            this._expired = expired ?? TimeSpan.FromMinutes(10);
        }

        public async Task LockOrThrow()
        {
            await this._retryAsync.ExecuteAsync(async () =>
            {
                var success = await this._redis.StringSetWhenNotExist(this._key, this._value, expire: this._expired);
                if (!success)
                    throw new Exception("没有拿到redis锁，再试一次");
            });

            //续租
            await this.ExtendLock();
        }

        async Task ExtendLock()
        {
            await Task.CompletedTask;
        }

        public async Task ReleaseLock()
        {
            await this._redis.DeleteKeyWithValueAsync(this._key, this._value);
        }

        public void Dispose()
        {
            AsyncHelper.RunSync(this.ReleaseLock);
        }

    }
}
