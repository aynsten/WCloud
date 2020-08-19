using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.InternalApi.Client.Login;
using WCloud.Member.Shared.Org;

namespace WCloud.Member.Authentication.Org
{
    public class OrgAuthServiceFromApiClient : IOrgAuthService
    {
        private readonly IWCloudContext _context;
        private readonly OrgServiceClient _client;
        public OrgAuthServiceFromApiClient(IWCloudContext<OrgAuthServiceFromApiClient> _context, OrgServiceClient _client)
        {
            this._context = _context;
            this._client = _client;
        }

        public Task<IEnumerable<OrgMemberDto>> GetMyOrgMap(string subject_id)
        {
            throw new System.NotImplementedException();
        }
    }
}
