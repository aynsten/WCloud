using FluentAssertions;
using System;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus
{
    public interface IMessageConsumeContext<T>
    {
        T Message { get; }
        Task Ack(bool ack);
    }

    public class BasicMessageConsumeContext<T> : IMessageConsumeContext<T>
    {
        private readonly T _message;
        public Func<bool, Task> AckHandler { get; set; }
        public BasicMessageConsumeContext(T message)
        {
            message.Should().NotBeNull();
            this._message = message;
        }

        public T Message => this._message;

        private bool ack_handled = false;
        public async Task Ack(bool ack)
        {
            if (this.AckHandler != null)
            {
                if (!this.ack_handled)
                {
                    await this.AckHandler.Invoke(ack);
                    this.ack_handled = true;
                }
            }
        }
    }

    /// <summary>
    /// masstransit好变态
    /// 建议每个消息对应一个新的consumer
    /// 也可以共享，但是要consumer线程安全
    /// </summary>
    public interface IMessageConsumerFinder : Lib.core.IFinder { }

    public interface IMessageConsumer<T> : IMessageConsumerFinder where T : class, IMessageBody
    {
        Task Consume(IMessageConsumeContext<T> context);
    }
}
