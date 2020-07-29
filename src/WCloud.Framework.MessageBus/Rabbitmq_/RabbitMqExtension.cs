using FluentAssertions;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Linq;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public static class RabbitMqExtension
    {
        public static string GetExchangeTypeName(this ExchangeTypeEnum type)
        {
            switch (type)
            {
                case ExchangeTypeEnum.direct:
                    return ExchangeType.Direct;
                case ExchangeTypeEnum.fanout:
                    return ExchangeType.Fanout;
                case ExchangeTypeEnum.topic:
                    return ExchangeType.Topic;
                case ExchangeTypeEnum.headers:
                    return ExchangeType.Headers;
                case ExchangeTypeEnum.delay:
                    return "x-delayed-message";
            }
            throw new NotSupportedException();
        }

        public static RabbitMQ GetRabbitmqOrThrow(this IConfiguration config)
        {
            var section = "rabbit";
            var rabbit = new RabbitMQ()
            {
                ServerAndPort = config[$"{section}:server"],
                User = config[$"{section}:user"],
                Password = config[$"{section}:pass"]
            };

            rabbit.ServerAndPort.Should().NotBeNullOrEmpty(nameof(rabbit.ServerAndPort));
            rabbit.User.Should().NotBeNullOrEmpty(nameof(rabbit.User));
            rabbit.Password.Should().NotBeNullOrEmpty(nameof(rabbit.Password));

            return rabbit;
        }

        public static string GetRoutingKey(this QueueConfigAttribute config)
        {
            var res = new[] { config.RoutingKey, config.QueueName }.FirstOrDefault();
            res.Should().NotBeNullOrEmpty();
            return res;
        }
    }
}
