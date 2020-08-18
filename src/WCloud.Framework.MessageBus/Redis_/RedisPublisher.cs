using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Core.DataSerializer;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Redis_
{
    public class RedisMessagePublisher : IMessagePublisher
    {
        private readonly IRedisDatabaseSelector redisDatabaseSelector;
        private readonly IDataSerializer serializeProvider;
        private readonly IReadOnlyCollection<ConsumerDescriptor> mapping;
        public RedisMessagePublisher(IRedisDatabaseSelector redisDatabaseSelector, IDataSerializer serializeProvider,
            IReadOnlyCollection<ConsumerDescriptor> mapping)
        {
            this.redisDatabaseSelector = redisDatabaseSelector;
            this.serializeProvider = serializeProvider;
            this.mapping = mapping;
        }

        public async Task PublishAsync<T>(string key, T model, CancellationToken cancellationToken) where T : class, IMessageBody
        {
            var bs = this.serializeProvider.Serialize(model);
            await this.redisDatabaseSelector.Database.ListLeftPushAsync(key: key, value: bs);
        }

        public async Task PublishAsync<T>(T model, CancellationToken cancellationToken) where T : class, IMessageBody
        {
            var maps = this.mapping.Where(x => x.MessageType == typeof(T)).ToArray();

            foreach (var m in maps)
            {
                var config = m.Config;
                await this.PublishAsync(config.QueueName, model, cancellationToken);
            }
        }
    }
}
