using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.Authentication.Admin
{
    public interface IAdminAuthService : IAutoRegistered
    {
        Task<IEnumerable<string>> GetAdminPermission(string admin_id);
        Task RemoveAdminPermissionCache(string admin_id);
        Task<AdminDto> GetAdminLoginInfoById(string subject_id);
        Task RemoveAdminLoginInfoCache(string admin_id);
    }
}
