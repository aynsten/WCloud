using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.InternalApi.Client.Login;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.Authentication.Admin
{
    public class AdminAuthServiceFromApiClient : IAdminAuthService
    {
        private readonly IWCloudContext _context;
        private readonly AdminLoginServiceClient _client;
        public AdminAuthServiceFromApiClient(IWCloudContext<AdminAuthServiceFromApiClient> _context, AdminLoginServiceClient _client)
        {
            this._context = _context;
            this._client = _client;
        }
        public async Task<string> GetAdminLoginInfo(string admin_id)
        {
            throw new NotImplementedException();
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
