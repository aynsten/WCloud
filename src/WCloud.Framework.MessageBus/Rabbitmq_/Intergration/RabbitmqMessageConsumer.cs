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
    internal class RabbitmqMessageConsumer<MessageType> : RabbitMqConsumerBase<MessageType> where MessageType : class, IMessageBody
    {
        public RabbitmqMessageConsumer(IServiceProvider provider,
            ILogger<RabbitmqMessageConsumer<MessageType>> logger,
            IConnection connection) :
            base(provider, logger, connection, new ConsumeOptionFromAttribute<MessageType>(provider))
        {
            var map = this.provider.ResolveMessageTypeMapping<MessageType>();

            //exchange
            this._channel.ExchangeDeclare(exchange: ConstConfig.ExchangeName,
                type: "topic",
                durable: true,
                autoDelete: false);
            //queue
            var queue_res = this._channel.QueueDeclare(queue: this._option.QueueName,
                durable: map.Config.QueueDurable,
                exclusive: map.Config.QueueExclusive,
                autoDelete: map.Config.QueueAutoDelete);
            //route
            var args = new Dictionary<string, object>();
            this._channel.RouteFromExchangeToQueue(
                exchange: ConstConfig.ExchangeName,
                queue: queue_res.QueueName,
                routing_key: map.Config.GetRoutingKey(),
                args: args);
        }

        public override async Task OnMessageReceived(IMessageConsumeContext<MessageType> message)
        {
            var model = message.Message;

            using var s = this.provider.CreateScope();
            var consumers = s.ServiceProvider.ResolveConsumerOptional<MessageType>();

            foreach (var c in consumers)
            {
#if DEBUG
                this.logger.LogInformation($"消费：{model.ToJson()}");
#endif

                var data = new BasicMessageConsumeContext<MessageType>(model);
                await c.Consume(data);
            }
        }
    }
}
