using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Shared.Org;

namespace WCloud.Member.InternalApi.Client.Login
{
    public class OrgServiceClient : IAutoRegistered
    {
        private readonly IWCloudContext _context;
        private readonly HttpClient httpClient;
        public OrgServiceClient(IWCloudContext<AdminLoginServiceClient> _context)
        {
            this._context = _context;
            this.httpClient = this._context.Provider.GetMemberInternalApiHttpClient();
        }

        public async Task<IEnumerable<OrgMemberDto>> GetMyOrgMap(string user_id)
        {
            var p = new { user_id = user_id };
            using var response = await this.httpClient.PostAsJsonAsync("account/get-user-org-mapping", p);

            var data = await response.Content.ReadAsStringAsync();

            var res = this._context.DataSerializer.DeserializeFromString<_<OrgMemberDto[]>>(data);

            res.Should().NotBeNull();

            return res.Data ?? new OrgMemberDto[] { };
        }
    }
}
