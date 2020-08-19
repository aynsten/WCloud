using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Shared.User;

namespace WCloud.Member.InternalApi.Client.Login
{
    public class UserLoginServiceClient : IAutoRegistered
    {
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
