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
        private readonly Func<bool, Task> _ack_handler;
        public BasicMessageConsumeContext(T message, Func<bool, Task> _ack_handler = null)
        {
            message.Should().NotBeNull();
            this._message = message;
            this._ack_handler = _ack_handler;
        }

        public T Message => this._message;

        public async Task Ack(bool ack)
        {
            if (this._ack_handler != null)
            {
                await this._ack_handler.Invoke(ack);
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
