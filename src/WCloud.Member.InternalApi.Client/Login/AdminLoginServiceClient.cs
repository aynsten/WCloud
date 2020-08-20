using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.InternalApi.Client.Login
{
    public class AdminLoginServiceClient : IAutoRegistered
    {
        private readonly IWCloudContext _context;
        private readonly HttpClient httpClient;
        public AdminLoginServiceClient(IWCloudContext<AdminLoginServiceClient> _context)
        {
            this._context = _context;
            this.httpClient = this._context.Provider.GetMemberInternalApiHttpClient();
        }

        public async Task<IEnumerable<string>> GetAdminPermissions(string admin_id)
        {
            var p = new { admin_id = admin_id };
            using var response = await this.httpClient.PostAsJsonAsync("account/get-admin-permission", p);

            var data = await response.Content.ReadAsStringAsync();

            var res = this._context.DataSerializer.DeserializeFromString<_<string[]>>(data);

            res.Should().NotBeNull();

            return res.Data ?? new string[] { };
        }

        public async Task<AdminDto> GetUserByUID(string admin_id)
        {
            var p = new { admin_id = admin_id };
            using var response = await this.httpClient.PostAsJsonAsync("account/get-admin-login-info", p);

            var data = await response.Content.ReadAsStringAsync();

            var res = this._context.DataSerializer.DeserializeFromString<_<AdminDto>>(data);
            res.Should().NotBeNull();

            return res.Data;
        }
    }
}
