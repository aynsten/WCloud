using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WCloud.Member.Shared.User;

namespace WCloud.Member.Authentication.User
{
    public interface IUserAuthService : IAutoRegistered
    {
        Task<string[]> GetMyOrgPermission(string org_id, string subject_id);
        Task RemoveMyOrgPermissionCacheKey(string org_id, string user_id);
        Task<UserDto> GetUserByUID(string subject_id);
        Task RemoveUserLoginInfoCacheKey(string user_id);
    }
}
