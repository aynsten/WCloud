using WCloud.Core.Cache;
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

        string __admin_info_cache_key__(string admin_id) => $"auth_admin_info_{admin_id}";

        public async Task<IEnumerable<string>> GetAdminPermission(string admin_id)
        {
            var key = this.__admin_permission_cache_key__(admin_id);

            var data = await this._context.CacheProvider.GetOrSetAsync_(key,
                () => this._client.GetAdminPermissions(admin_id),
                TimeSpan.FromMinutes(1));

            return data;
        }

        string __admin_permission_cache_key__(string admin_id) => $"auth_admin_permission_{admin_id}";

        public async Task<AdminDto> GetAdminLoginInfoById(string admin_id)
        {
            var key = this.__admin_info_cache_key__(admin_id);

            var data = await this._context.CacheProvider.GetOrSetAsync_(key,
                () => this._client.GetUserByUID(admin_id),
                TimeSpan.FromMinutes(1));

            return data;
        }

        public async Task RemoveAdminPermissionCache(string admin_id)
        {
            var key = this.__admin_permission_cache_key__(admin_id);
            await this._context.CacheProvider.RemoveAsync(key);
        }

        public async Task RemoveAdminLoginInfoCache(string admin_id)
        {
            var key = this.__admin_info_cache_key__(admin_id);
            await this._context.CacheProvider.RemoveAsync(key);
        }
    }
}
