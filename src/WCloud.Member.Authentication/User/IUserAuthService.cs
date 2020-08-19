using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Shared.User;

namespace WCloud.Member.Authentication.User
{
    public interface IUserAuthService : IAutoRegistered
    {
        Task<string[]> GetMyOrgPermission(string org_id, string subject_id);
        Task<bool> HasAllOrgPermission(string org_id, string subject_id, IEnumerable<string> permissions);
        Task<UserDto> GetUserByUID(string subject_id);
    }
}
