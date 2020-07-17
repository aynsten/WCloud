using WCloud.Core.MessageBus;

namespace WCloud.MetroAd.Shared.Message
{
    public class OrderPayedMessage : IMessageBody
    {
        public string OrderUID { get; set; }
    }
}
