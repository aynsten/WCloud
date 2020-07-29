using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Intergration
{
    public class RabbitmqMessagePublisher : IMessagePublisher
    {
        private readonly IRabbitMqProducer rabbitMqProducer;
        private readonly IReadOnlyCollection<ConsumerDescriptor> mapping;
        public RabbitmqMessagePublisher(IRabbitMqProducer rabbitMqProducer, IReadOnlyCollection<ConsumerDescriptor> mapping)
        {
            this.rabbitMqProducer = rabbitMqProducer;
            this.mapping = mapping;
        }

        public async Task PublishAsync<T>(T model, CancellationToken cancellationToken = default) where T : class, IMessageBody
        {
            var maps = this.mapping.Where(x => x.MessageType == typeof(T)).ToArray();
            foreach (var m in maps)
            {
                var config = m.Config;
                await this.PublishAsync(key: config.GetRoutingKey(), model: model, cancellationToken: cancellationToken);
            }
        }

        public async Task PublishAsync<T>(string key, T model, CancellationToken cancellationToken = default) where T : class, IMessageBody
        {
            this.rabbitMqProducer.SendMessage(exchange: ConstConfig.ExchangeName, routeKey: key, model);

            await Task.CompletedTask;
        }
    }
}
