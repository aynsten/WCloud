using System;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public interface IRabbitMqProducer
    {
        /// <summary>
        /// 发送消息到exchange
        /// </summary>
        void SendMessage<T>(string exchange, string routeKey, T message, SendMessageOption option = null);

        /// <summary>
        /// 发送消息到queue
        /// </summary>
        [Obsolete("建议使用exchange")]
        void SendMessage<T>(string queue, T message, SendMessageOption option = null);
    }
}
