using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Shared.Org;

namespace WCloud.Member.InternalApi.Client.Login
{
    public class OrgServiceClient : IAutoRegistered
    {
        public async Task<IEnumerable<OrgMemberDto>> GetMyOrgMap(string subject_id)
        {
            throw new NotImplementedException();
        }
    }
}
