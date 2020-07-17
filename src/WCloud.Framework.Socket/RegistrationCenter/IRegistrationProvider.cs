using System.Threading.Tasks;

namespace WCloud.Framework.Socket.RegistrationCenter
{
    public interface IRegistrationProvider
    {
        Task RegisterUserInfo(UserRegistrationInfo info);
        Task<string[]> GetUserServerInstances(string user_uid);
        Task RegisterGroupInfo(GroupRegistrationInfo info);
        Task RemoveRegisterInfo(string user_uid, string device);
    }
}
