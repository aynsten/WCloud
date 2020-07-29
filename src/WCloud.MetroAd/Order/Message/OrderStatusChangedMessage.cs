using WCloud.Core;
using WCloud.Core.MessageBus;
using WCloud.MetroAd.Order;

namespace WCloud.MetroAd.Message
{
    [QueueConfig(ConfigSet.MessageBus.Queue.MetroAd)]
    public class OrderStatusChangedMessage : OrderHistoryEntity, IMessageBody
    {
        //
    }
}
