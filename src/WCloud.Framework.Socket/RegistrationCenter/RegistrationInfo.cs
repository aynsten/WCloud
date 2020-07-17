using System;

namespace WCloud.Framework.Socket.RegistrationCenter
{
    public class UserRegistrationInfo
    {
        public string UserID { get; set; }
        public string DeviceType { get; set; }
        public UserRegistrationInfoPayload Payload { get; set; }
    }

    public class UserRegistrationInfoPayload
    {
        public string ServerInstanceID { get; set; }
        public DateTime PingTimeUtc { get; set; }
    }

    public class GroupRegistrationInfo
    {
        public string GroupUID { get; set; }
        public string ServerInstance { get; set; }
        public GroupRegistrationInfoPayload Payload { get; set; }
    }

    public class GroupRegistrationInfoPayload
    { }
}
