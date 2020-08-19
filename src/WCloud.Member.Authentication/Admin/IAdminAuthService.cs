using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.Authentication.Admin
{
    public interface IAdminAuthService : IAutoRegistered
    {
        Task<string> GetAdminLoginInfo(string admin_id);
        Task<bool> HasAllPermission(string subject_id, IEnumerable<string> permissions);
        Task<AdminDto> GetUserByUID(string subject_id);
    }
}
