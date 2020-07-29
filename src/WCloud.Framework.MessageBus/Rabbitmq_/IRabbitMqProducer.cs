namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public interface IRabbitMqProducer
    {
        /// <summary>
        /// 发送消息到exchange
        /// </summary>
        void SendMessage<T>(string exchange, string routeKey, T message, SendMessageOption option = null);
    }
}
