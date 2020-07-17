using FluentAssertions;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus
{
    public interface IMessageConsumeContext<T>
    {
        T Message { get; }
    }

    public class BasicMessageConsumeContext<T> : IMessageConsumeContext<T>
    {
        private readonly T _message;
        public BasicMessageConsumeContext(T message)
        {
            message.Should().NotBeNull();
            this._message = message;
        }

        public T Message => this._message;
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
