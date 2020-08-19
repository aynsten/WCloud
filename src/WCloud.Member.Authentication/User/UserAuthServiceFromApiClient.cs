using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.InternalApi.Client.Login;
using WCloud.Member.Shared.User;

namespace WCloud.Member.Authentication.User
{
    public class UserAuthServiceFromApiClient : IUserAuthService
    {
        private readonly IWCloudContext _context;
        private readonly UserLoginServiceClient _client;
        public UserAuthServiceFromApiClient(IWCloudContext<UserAuthServiceFromApiClient> _context, UserLoginServiceClient _client)
        {
            this._context = _context;
            this._client = _client;
        }

        public async Task<string[]> GetMyOrgPermission(string org_id, string subject_id)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> HasAllOrgPermission(string org_id, string subject_id, IEnumerable<string> permissions)
        {
            throw new NotImplementedException();
        }
        public async Task<UserDto> GetUserByUID(string subject_id)
        {
            throw new NotImplementedException();
        }
    }
}
