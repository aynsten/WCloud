﻿using FluentAssertions;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.InternalApi.Client.Login
{
    public class AdminLoginServiceClient : IAutoRegistered
    {
        private readonly HttpClient httpClient;
        public AdminLoginServiceClient(IServiceProvider provider)
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

        public async Task<bool> HasAllPermission(string subject_id, IEnumerable<string> permissions)
        {
            throw new NotImplementedException();
        }

        public async Task<AdminDto> GetUserByUID(string subject_id)
        {
            throw new NotImplementedException();
        }
    }
}