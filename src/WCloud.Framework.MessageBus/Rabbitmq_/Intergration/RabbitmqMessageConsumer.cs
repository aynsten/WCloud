using Lib.data;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;
using WCloud.Framework.MessageBus.Rabbitmq_.Providers;

namespace WCloud.Framework.MessageBus.Rabbitmq_.Intergration
{
    internal class RabbitmqMessageConsumer<T> : RabbitMqConsumerBase<T> where T : class, IMessageBody
    {
        public RabbitmqMessageConsumer(IServiceProvider provider,
            ILogger<RabbitmqMessageConsumer<T>> logger,
            IConnection connection,
            ISerializeProvider _serializer) :
            base(provider, logger, connection, _serializer, new ConsumeOptionFromAttribute<T>())
        {
            //exchange
            this._channel.ExchangeDeclare(exchange: ConstConfig.ExchangeName, type: "topic", durable: true, autoDelete: false);
            //queue
            var queue_res = this._channel.QueueDeclare(this._option.QueueName, durable: true, exclusive: false, autoDelete: false);
            //route
            var args = new Dictionary<string, object>();
            this._channel.RouteFromExchangeToQueue(ConstConfig.ExchangeName, queue_res.QueueName, routing_key: this._option.QueueName, args);
        }

        public override async Task<bool> OnMessageReceived(ConsumerMessage<T> message)
        {
            using var s = this.provider.CreateScope();
            var consumers = s.ServiceProvider.ResolveConsumerOptional<T>();

            var model = message.MessageModel;

            foreach (var c in consumers)
            {
#if DEBUG
                this.logger.LogInformation($"消费：{model.ToJson()}");
#endif

                var data = new BasicMessageConsumeContext<T>(model);
                await c.Consume(data);
            }

            return true;
        }
    }
}
