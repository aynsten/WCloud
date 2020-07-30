using WCloud.Core;
using WCloud.Core.MessageBus;

namespace WCloud.Member.Shared.MessageBody
{
    [QueueConfig(ConfigSet.MessageBus.Queue.User)]
    public class UserPhoneBindSmsMessage : IMessageBody
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }
}
