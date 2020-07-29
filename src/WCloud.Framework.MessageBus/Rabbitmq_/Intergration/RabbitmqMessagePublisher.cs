using System;
using System.Threading;
using System.Threading.Tasks;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Intergration
{
    public class RabbitmqMessagePublisher : IMessagePublisher
    {
        private readonly IRabbitMqProducer rabbitMqProducer;
        public RabbitmqMessagePublisher(IRabbitMqProducer rabbitMqProducer)
        {
            this.rabbitMqProducer = rabbitMqProducer;
        }

        async Task IMessagePublisher.PublishAsync<T>(string key, T model, CancellationToken cancellationToken)
        {
            var options = new ConsumeOptionFromAttribute<T>();

            this.rabbitMqProducer.SendMessage(exchange: ConstConfig.ExchangeName, routeKey: options.QueueName, model);

            await Task.CompletedTask;
        }
    }
}
