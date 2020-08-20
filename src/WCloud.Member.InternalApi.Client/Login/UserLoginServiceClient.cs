using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Shared.User;

namespace WCloud.Member.InternalApi.Client.Login
{
    public class UserLoginServiceClient : IAutoRegistered
    {
        private readonly IWCloudContext _context;
        private readonly HttpClient httpClient;
        public UserLoginServiceClient(IWCloudContext<AdminLoginServiceClient> _context)
        {
            this._context = _context;
            this.httpClient = this._context.Provider.GetMemberInternalApiHttpClient();
        }

        public async Task<string[]> GetMyOrgPermission(string org_id, string user_id)
        {
            var p = new { org_id = org_id, user_id = user_id };
            using var response = await this.httpClient.PostAsJsonAsync("account/get-org-user-permission", p);

            var data = await response.Content.ReadAsStringAsync();

            var res = this._context.DataSerializer.DeserializeFromString<_<string[]>>(data);

            res.Should().NotBeNull();

            return res.Data ?? new string[] { };
        }

        public async Task<UserDto> GetUserByUID(string user_id)
        {
            var p = new { user_id = user_id };

            using var response = await this.httpClient.PostAsJsonAsync("account/get-user-login-info", p);

            var data = await response.Content.ReadAsStringAsync();

            var res = this._context.DataSerializer.DeserializeFromString<_<UserDto>>(data);

            res.Should().NotBeNull();

            return res.Data;
        }
    }
}
