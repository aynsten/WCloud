using FluentAssertions;
using System;

namespace WCloud.Framework.MessageBus
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueueConfigAttribute : Attribute
    {
        public string QueueName { get; private set; }

        public string ExchangeType { get; set; } = "topic";

        public string RoutingKey { get; set; }

        /// <summary>
        /// 自动删除
        /// </summary>
        public bool AutoDelete { get; set; } = false;

        /// <summary>
        /// 排他，客户端断开就删除队列
        /// </summary>
        public bool Exclusive { get; set; } = false;

        /// <summary>
        /// 持久化
        /// </summary>
        public bool Durable { get; set; } = true;

        public int? Concurrency { get; set; }

        public QueueConfigAttribute(string name)
        {
            name.Should().NotBeNullOrEmpty();
            this.QueueName = name;
        }
    }
}
