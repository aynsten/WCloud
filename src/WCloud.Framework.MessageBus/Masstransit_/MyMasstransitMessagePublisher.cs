using MassTransit;
using System.Threading;
using System.Threading.Tasks;
using WCloud.Core.MessageBus;

namespace WCloud.Framework.MessageBus.Masstransit_
{
    public class MyMasstransitMessagePublisher : IMessagePublisher
    {
        private readonly IBus _bus;

        public MyMasstransitMessagePublisher(IBus bus)
        {
            this._bus = bus;
        }

        public async Task PublishAsync<T>(string key, T model, CancellationToken cancellationToken = default) where T : class, IMessageBody
        {
            if (cancellationToken == null)
            {
                await this._bus.Publish(model);
            }
            else
            {
                await this._bus.Publish(model, cancellationToken: cancellationToken);
            }
        }
    }
}
