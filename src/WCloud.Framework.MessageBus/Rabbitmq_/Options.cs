using Lib.core;
using Lib.data;
using Lib.helper;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
    public abstract class MessageBase<T, Message>
    {
        public Message MessageArgs { get; protected set; }

        public T MessageModel { get; protected set; }
    }

    public class ConsumerMessage<T> : MessageBase<T, BasicDeliverEventArgs>
    {
        public ConsumerMessage(ISerializeProvider _serializer, BasicDeliverEventArgs res)
        {
            this.MessageModel = _serializer.Deserialize<T>(res.Body.ToArray());
            this.MessageArgs = res;
        }
    }

    public class BasicGetMessage<T> : MessageBase<T, BasicGetResult>
    {
        public BasicGetMessage(ISerializeProvider _serializer, BasicGetResult res)
        {
            this.MessageModel = _serializer.Deserialize<T>(res.Body.ToArray());
            this.MessageArgs = res;
        }
    }

    /// <summary>
    /// exchange类型
    /// </summary>
    public enum ExchangeTypeEnum : byte
    {
        /// <summary>
        /// 如果routingKey匹配，那么Message就会被传递到相应的queue中。
        /// http://blog.csdn.net/anzhsoft/article/details/19630147
        /// </summary>
        direct = 1,

        /// <summary>
        /// 会向所有响应的queue广播。
        /// http://blog.csdn.net/anzhsoft/article/details/19617305
        /// </summary>
        fanout = 2,

        /// <summary>
        /// 对key进行模式匹配，比如ab.* 可以传递到所有ab.*的queue。* (星号) 代表任意 一个单词；# (hash) 0个或者多个单词。
        /// http://blog.csdn.net/anzhsoft/article/details/19633079
        /// </summary>
        topic = 3,

        /// <summary>
        /// 还没弄懂
        /// </summary>
        headers = 4,

        /// <summary>
        /// 用插件延迟消息发送
        /// </summary>
        delay = 5,
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

    public class ExchangeOption : OptionBase
    {
        /// <summary>
        /// 持久化
        /// </summary>
        public virtual bool Durable { get; set; }

        /// <summary>
        /// 没有消息没有客户端自动删除
        /// </summary>
        public virtual bool AutoDelete { get; set; }

        /// <summary>
        /// 自动删除
        /// </summary>
        public virtual IDictionary<string, object> Args { get; set; }
    }

    public class QueueOption : OptionBase
    {
        /// <summary>
        /// 队列是否持久化
        /// </summary>
        public virtual bool Durable { get; set; }

        /// <summary>
        /// 客户端下线自动删除队列
        /// </summary>
        public virtual bool AutoDelete { get; set; }

        /// <summary>
        /// 排他
        /// </summary>
        public virtual bool Exclusive { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public virtual IDictionary<string, object> Args { get; set; }
    }

    public class SendMessageOption : OptionBase
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

    public class ConsumeOption<T> : ConsumeOption
    {
        public ConsumeOption()
        {
            this.QueueName = "message_bus_" + typeof(T).FullName.Replace('.', '_');
        }
    }

    public class ConsumeOption : OptionBase
    {
        /// <summary>
        /// 队列
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 自动确认消息
        /// </summary>
        public bool AutoAck { get; set; } = false;

        /// <summary>
        /// 并发数
        /// </summary>
        public ushort? ConcurrencySize { get; set; }

        /// <summary>
        /// 消费名称，标识
        /// </summary>
        public string ConsumerName { get; set; }

        public override void Valid()
        {
            if (ValidateHelper.IsEmpty(this.QueueName))
                throw new ArgumentNullException(nameof(this.QueueName));
        }
    }

    public class BatchConsumeOption : ConsumeOption
    {
        public int BatchSize { get; set; }

        public TimeSpan BatchTimeout { get; set; }

        public TimeSpan? DisposeTimeout { get; set; }

        public override void Valid()
        {
            base.Valid();

            if (this.BatchSize <= 0)
                throw new ArgumentException(nameof(this.BatchSize));

            if (this.BatchTimeout != null && this.BatchTimeout.Seconds <= 0)
                throw new ArgumentException(nameof(this.BatchTimeout));
        }
    }

    /// <summary>
    /// rabitmq，配置
    /// </summary>
    public class RabbitMqOption : OptionBase
    {
        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string VirtualHost { get; set; }

        /// <summary>
        /// 默认3000ms
        /// </summary>
        public TimeSpan? ContinuationTimeout { get; set; }

        /// <summary>
        /// 默认2000ms
        /// </summary>
        public TimeSpan? SocketTimeout { get; set; }

        public override void Valid()
        {
            if (ValidateHelper.IsEmpty(this.HostName))
                throw new ArgumentNullException(nameof(this.HostName));
        }
    }
}
