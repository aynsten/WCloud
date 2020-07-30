using StackExchange.Redis;
using WCloud.Framework.Redis;

namespace WCloud.Framework.MessageBus.Redis_
{
    public interface IRedisDatabaseSelector
    {
        IDatabase Database { get; }
    }

    public class RedisDatabaseSelector : IRedisDatabaseSelector
    {
        private readonly IDatabase database;

        public RedisDatabaseSelector(RedisConnectionWrapper redisConnectionWrapper)
        {
            var db = (int)WCloud.Core.ConfigSet.Redis.消息队列;
            this.database = redisConnectionWrapper.Connection.GetDatabase(db);
        }

        public IDatabase Database => this.database;
    }
}
