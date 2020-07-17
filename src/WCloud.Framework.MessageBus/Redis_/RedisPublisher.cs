using Lib.data;
using System.Threading;
using System.Threading.Tasks;

namespace WCloud.Framework.MessageBus.Redis_
{
    public class RedisMessagePublisher : IMessagePublisher
    {
        private readonly IRedisDatabaseSelector redisDatabaseSelector;
        private readonly ISerializeProvider serializeProvider;
        public RedisMessagePublisher(IRedisDatabaseSelector redisDatabaseSelector, ISerializeProvider serializeProvider)
        {
            this.redisDatabaseSelector = redisDatabaseSelector;
            this.serializeProvider = serializeProvider;
        }

        async Task IMessagePublisher.PublishAsync<T>(string key, T model, CancellationToken cancellationToken)
        {
            var bs = this.serializeProvider.Serialize(model);
            await this.redisDatabaseSelector.Database.ListLeftPushAsync(key: key, value: bs);
        }
    }
}
