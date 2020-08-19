using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Shared.Org;

namespace WCloud.Member.Authentication.Org
{
    public interface IOrgAuthService : IAutoRegistered
    {
        Task<IEnumerable<OrgMemberDto>> GetMyOrgMap(string subject_id);
    }
}
