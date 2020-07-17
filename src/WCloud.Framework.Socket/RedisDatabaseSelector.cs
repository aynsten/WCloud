using Lib.redis;
using StackExchange.Redis;

namespace WCloud.Framework.Socket
{
    public interface IRedisDatabaseSelector
    {
        IDatabase Database { get; }
        IConnectionMultiplexer Connection { get; }
    }

    public class RedisDatabaseSelector : IRedisDatabaseSelector
    {
        private readonly IDatabase database;

        public RedisDatabaseSelector(RedisConnectionWrapper redisConnectionWrapper)
        {
            var db = (int)WCloud.Core.ConfigSet.Redis.发布订阅;
            this.database = redisConnectionWrapper.Connection.GetDatabase(db);
            this.Connection = redisConnectionWrapper.Connection;
        }

        public IDatabase Database => this.database;
        public IConnectionMultiplexer Connection { get; }
    }
}
