using FluentAssertions;
using System;

namespace WCloud.Framework.MessageBus
{
    public class ConsumerDescriptor
    {
        public ConsumerDescriptor(Type t)
        {
            t.Should().NotBeNull();

            this.ConsumerType = t;
            this.MessageType = t.__get_message_type__();
            this.Config = t.__get_config__();
        }

        public Type ConsumerType { get; private set; }
        public Type MessageType { get; private set; }
        public QueueConfigAttribute Config { get; private set; }
    }
}
