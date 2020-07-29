using FluentAssertions;
using System;
using System.Collections.Generic;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Rabbitmq_
{
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

    public class ExchangeOption
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

    public class QueueOption
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

    public class ConsumeOptionFromAttribute<T> : ConsumeOption where T : class, IMessageBody
    {
        public ConsumeOptionFromAttribute(IServiceProvider provider)
        {
            var config = provider.ResolveMessageTypeMapping<T>()?.Config;
            config.Should().NotBeNull();

            this.QueueName = config.QueueName;
        }
    }

    public class ConsumeOption
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

        public virtual void Valid()
        {
            this.QueueName.Should().NotBeNullOrEmpty();
        }
    }

    public class BatchConsumeOption : ConsumeOption
    {
        public int BatchSize { get; set; }

        public TimeSpan BatchTimeout { get; set; }

        public TimeSpan? DisposeTimeout { get; set; }

        public override void Valid()
        {
            this.BatchSize.Should().BeGreaterThan(0);

            (this.BatchTimeout != null && this.BatchTimeout.Seconds > 0).Should().BeTrue();
        }
    }

    [System.Obsolete]
    public class RabbitMQ
    {
        public string ServerAndPort { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// rabitmq，配置
    /// </summary>
    public class RabbitMqOption
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

        public void Valid()
        {
            this.HostName.Should().NotBeNullOrEmpty(nameof(this.HostName));
        }
    }
}
