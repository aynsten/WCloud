using System;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public interface IRabbitMqConsumer : IDisposable
    {
        void StartConsume();
    }
}
