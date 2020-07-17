using RabbitMQ.Client;
using System;

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
    }
}
