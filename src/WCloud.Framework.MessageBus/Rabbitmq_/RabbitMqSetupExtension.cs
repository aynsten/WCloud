using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public static class RabbitMqSetupExtension
    {
        public static void CreateExchange(this IModel channel,
            string exchange, ExchangeTypeEnum exchange_type, ExchangeOption option)
        {
            if (option == null)
                throw new ArgumentNullException($"{nameof(CreateExchange)}.{nameof(option)}");
            option.Args = option.Args ?? new Dictionary<string, object>();

            var exchange_type_name = exchange_type.GetExchangeTypeName();
            if (exchange_type == ExchangeTypeEnum.delay)
            {
                option.Args["x-delayed-type"] = exchange_type_name;
            }

            channel.ExchangeDeclare(exchange: exchange, type: exchange_type_name,
                durable: option.Durable, autoDelete: option.AutoDelete, arguments: option.Args);
        }

        public static void CreateQueue(this IModel channel, string queue, QueueOption option)
        {
            channel.QueueDeclare(queue: queue,
                durable: option.Durable,
                exclusive: option.Exclusive,
                autoDelete: option.AutoDelete,
                arguments: option.Args);
        }

        public static void RouteFromExchangeToExchange(this IModel channel, string from, string to, string routing_key, IDictionary<string, object> args)
        {
            channel.ExchangeBind(destination: to,
                source: from,
                routingKey: routing_key,
                arguments: args);
        }

        public static void RouteFromExchangeToQueue(this IModel channel, string exchange, string queue, string routing_key, IDictionary<string, object> args)
        {
            channel.QueueBind(queue: queue,
                exchange: exchange,
                routingKey: routing_key,
                arguments: args);
        }
    }
}
