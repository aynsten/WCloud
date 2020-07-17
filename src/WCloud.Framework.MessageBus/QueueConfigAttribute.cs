using System;

namespace WCloud.Framework.MessageBus
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueueConfigAttribute : Attribute
    {
        public string QueueName { get; private set; }

        public string ExchangeType { get; set; } = "topic";

        public bool AutoDelete { get; set; } = false;

        public bool Exclusive { get; set; } = false;

        public bool Durable { get; set; } = true;

        public QueueConfigAttribute(string name)
        {
            this.QueueName = name ?? throw new ArgumentNullException(nameof(QueueConfigAttribute));
        }
    }
}
