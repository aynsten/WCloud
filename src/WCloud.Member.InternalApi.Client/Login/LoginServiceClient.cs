using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;

namespace WCloud.Member.InternalApi.Client.Login
{
    public class LoginServiceClient : IAutoRegistered
    {
        private readonly HttpClient httpClient;
        public LoginServiceClient(IServiceProvider provider)
        {
            this.httpClient = provider.GetMemberInternalApiHttpClient();
        }

        public async Task<string> GetAdminLoginInfo(string admin_id)
        {
            var p = new { admin_id = admin_id };
            using var response = await this.httpClient.PostAsJsonAsync("account/get-admin-login-info", p);

            var data = await response.Content.ReadAsStringAsync();
            data.Should().NotBeNullOrEmpty();
            var res = data.JsonToEntity<_<string>>();
            res.ThrowIfNotSuccess();

            return res.Data;
        }
    }
}
