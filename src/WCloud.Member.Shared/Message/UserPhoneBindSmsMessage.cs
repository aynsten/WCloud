using WCloud.Core.MessageBus;

namespace WCloud.Member.Shared.MessageBody
{
    public class UserPhoneBindSmsMessage : IMessageBody
    {
        public string Phone { get; set; }
        public string Code { get; set; }
    }
}
