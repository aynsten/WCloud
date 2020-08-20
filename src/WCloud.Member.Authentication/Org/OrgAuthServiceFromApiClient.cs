using Lib.cache;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.InternalApi.Client.Login;
using WCloud.Member.Shared.Org;

namespace WCloud.Member.Authentication.Org
{
    public class OrgAuthServiceFromApiClient : IOrgAuthService
    {
        private readonly IWCloudContext _context;
        private readonly OrgServiceClient _client;
        public OrgAuthServiceFromApiClient(IWCloudContext<OrgAuthServiceFromApiClient> _context, OrgServiceClient _client)
        {
            this._context = _context;
            this._client = _client;
        }

        string __my_org_mapping_cache_key__(string user_id) => $"my_org_mapping_{user_id}";

        public async Task<IEnumerable<OrgMemberDto>> GetMyOrgMap(string user_id)
        {
            var key = this.__my_org_mapping_cache_key__(user_id);

            var res = await this._context.CacheProvider.GetOrSetAsync_(key,
                () => this._client.GetMyOrgMap(user_id),
                TimeSpan.FromMinutes(1));

            return res ?? new OrgMemberDto[] { };
        }

        public async Task RemoveMyOrgMappingCacheKey(string user_id)
        {
            var key = this.__my_org_mapping_cache_key__(user_id);
            await this._context.CacheProvider.RemoveAsync(key);
        }
    }
}
