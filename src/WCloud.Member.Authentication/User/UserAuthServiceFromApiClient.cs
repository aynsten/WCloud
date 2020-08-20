using WCloud.Core.Cache;
using System;
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

        string __my_org_permission_cache_key__(string org_id, string user_id) => $"my_org_permission_{org_id}_{user_id}";

        public async Task<string[]> GetMyOrgPermission(string org_id, string subject_id)
        {
            var key = this.__my_org_permission_cache_key__(org_id, subject_id);
            var res = await this._context.CacheProvider.GetOrSetAsync_(key,
                () => this._client.GetMyOrgPermission(org_id, subject_id),
                TimeSpan.FromMinutes(1));
            return res ?? new string[] { };
        }

        public async Task RemoveMyOrgPermissionCacheKey(string org_id, string user_id)
        {
            var key = this.__my_org_permission_cache_key__(org_id, user_id);
            await this._context.CacheProvider.RemoveAsync(key);
        }

        string __user_login_info_cache_key__(string user_id) => $"user_login_info_{user_id}";

        public async Task<UserDto> GetUserByUID(string subject_id)
        {
            var key = this.__user_login_info_cache_key__(subject_id);
            var res = await this._context.CacheProvider.GetOrSetAsync_(key,
                () => this._client.GetUserByUID(subject_id),
                TimeSpan.FromMinutes(1));

            return res;
        }

        public async Task RemoveUserLoginInfoCacheKey(string user_id)
        {
            var key = this.__user_login_info_cache_key__(user_id);
            await this._context.CacheProvider.RemoveAsync(key);
        }
    }
}
