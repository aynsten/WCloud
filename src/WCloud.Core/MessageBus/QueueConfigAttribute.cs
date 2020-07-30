using FluentAssertions;
using System;

namespace WCloud.Core.MessageBus
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueueConfigAttribute : Attribute
    {
        public string QueueName { get; private set; }

        /// <summary>
        /// 自动删除
        /// </summary>
        public bool QueueAutoDelete { get; set; } = false;

        /// <summary>
        /// 排他，客户端断开就删除队列
        /// </summary>
        public bool QueueExclusive { get; set; } = false;

        /// <summary>
        /// 持久化
        /// </summary>
        public bool QueueDurable { get; set; } = true;

        public QueueConfigAttribute(string name)
        {
            name.Should().NotBeNullOrEmpty();
            this.QueueName = name;
        }
    }
}
