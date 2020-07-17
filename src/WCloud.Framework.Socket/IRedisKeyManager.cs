namespace WCloud.Framework.Socket
{
    public interface IRedisKeyManager
    {
        string UserRegInfoKey(string user_uid);
        string UserDeviceRegHashKey(string device);
        string GroupRegInfoKey(string group_uid);
        string GroupServerHashKey(string server_id);
    }

    public class DefaultRedisKeyManager : IRedisKeyManager
    {
        public string GroupRegInfoKey(string group_uid)
        {
            var res = $"im:reg:group:{group_uid}";
            return res;
        }

        public string GroupServerHashKey(string server_id)
        {
            var res = $"hash_{server_id}";
            return res;
        }

        public string UserDeviceRegHashKey(string device)
        {
            var res = $"hash_{device}";
            return res;
        }

        public string UserRegInfoKey(string user_uid)
        {
            var res = $"im:reg:user:{user_uid}";
            return res;
        }
    }
}
