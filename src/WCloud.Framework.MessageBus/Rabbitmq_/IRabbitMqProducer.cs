using System;
using System.Collections.Generic;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public interface IRabbitMqProducer
    {
        /// <summary>
        /// 发送消息到exchange
        /// </summary>
        void SendMessage<T>(string exchange, string routeKey, T message, SendMessageOption option = null);
    }

    /// <summary>
    /// 优先级
    /// </summary>
    public enum MessagePriority : byte
    {
        /// <summary>
        /// 优先级0
        /// </summary>
        None = 0,
        /// <summary>
        /// 优先级1
        /// </summary>
        Lowest = 1,
        /// <summary>
        /// 优先级2
        /// </summary>
        AboveLowest = 2,
        /// <summary>
        /// 优先级3
        /// </summary>
        Low = 3,
        /// <summary>
        /// 优先级4
        /// </summary>
        BelowNormal = 4,
        /// <summary>
        /// 优先级5
        /// </summary>
        Normal = 5,
        /// <summary>
        /// 优先级6
        /// </summary>
        AboveNormal = 6,
        /// <summary>
        /// 优先级7
        /// </summary>
        Hight = 7,
        /// <summary>
        /// 优先级8
        /// </summary>
        BelowHighest = 8,
        /// <summary>
        /// 优先级9
        /// </summary>
        Highest = 9
    }

    public class SendMessageOption
    {
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Persistent { get; set; } = true;

        /// <summary>
        /// 优先级
        /// </summary>
        public MessagePriority? Priority { get; set; }

        /// <summary>
        /// 消息延迟
        /// </summary>
        public TimeSpan? Delay { get; set; }

        /// <summary>
        /// 其他参数
        /// </summary>
        public IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// 是否使用事务，确认消息发送成功
        /// </summary>
        public bool Confirm { get; set; } = false;

        /// <summary>
        /// 消息确认超时时间
        /// </summary>
        public TimeSpan? ConfirmTimeout { get; set; } = TimeSpan.FromSeconds(3);
    }
}
