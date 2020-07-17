using System.Threading.Tasks;

namespace WCloud.Framework.Socket.UserContext
{
    public interface IUserGroups
    {
        Task<string[]> GetUserGroups(string user_uid);
        Task<string[]> GetUsersGroups(string[] user_uids);
    }

    public class TestUserGroups : IUserGroups
    {
        public async Task<string[]> GetUserGroups(string user_uid)
        {
            await Task.CompletedTask;
            return new string[] { "group-a" };
        }

        public async Task<string[]> GetUsersGroups(string[] user_uids)
        {
            await Task.CompletedTask;
            return new string[] { "group-a" };
        }
    }
}
