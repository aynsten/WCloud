using FluentAssertions;
using Lib.extension;
using System;
using System.Linq;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus
{
    public class ConsumerDescriptor
    {
        public ConsumerDescriptor(Type t)
        {
            t.Should().NotBeNull();

            this.ConsumerType = t;
            this.MessageType = t.__get_message_type__();
            this.Config = this.__get_queue_config__(this.MessageType);
        }

        public Type ConsumerType { get; private set; }
        public Type MessageType { get; private set; }
        public QueueConfigAttribute Config { get; private set; }

        QueueConfigAttribute __get_queue_config__(Type t)
        {
            t.IsAssignableTo_<IMessageBody>().Should().BeTrue("队列配置的标签必须加载消息实体上");
            var queue_config = t.GetCustomAttributes_<QueueConfigAttribute>().FirstOrDefault();
            queue_config.Should().NotBeNull("consumer should have a config attribute");
            return queue_config;
        }
    }
}
