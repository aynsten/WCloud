using FluentAssertions;
using Lib.cache;
using Lib.extension;
using Lib.ioc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Core.Helper;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.Application.PermissionValidator
{
    [ScopedInstance]
    public class OrgPermissionValidatorService : IOrgPermissionValidatorService
    {
        private readonly TimeSpan _cache_timeout = TimeSpan.FromMinutes(10);

        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager _keyManager;
        private readonly IMSRepository<OrgRoleEntity> _repo;
        private readonly IStringArraySerializer permissionSerializer;

        public OrgPermissionValidatorService(
            ICacheProvider _cache,
            ICacheKeyManager keyManager,
            IMSRepository<OrgRoleEntity> _repo,
            IStringArraySerializer permissionSerializer)
        {
            this._cache = _cache;
            this._keyManager = keyManager;
            this._repo = _repo;
            this.permissionSerializer = permissionSerializer;
        }

        void __check__(string org_uid, string subject_id, IEnumerable<string> permissions)
        {
            org_uid.Should().NotBeNullOrEmpty("org permission check org uid");
            subject_id.Should().NotBeNullOrEmpty("org permission check subject id");
            permissions.Should().NotBeNullOrEmpty("org permission check permissions");

            foreach (var m in permissions)
            {
                m.Should().NotBeNullOrEmpty();
            }
        }

        public async Task<bool> HasAllOrgPermission(string org_uid, string subject_id, IEnumerable<string> permission_uid)
        {
            this.__check__(org_uid, subject_id, permission_uid);

            var all = await this.LoadAllOrgPermission(org_uid, subject_id);

            return all.ContainsAll_(permission_uid);
        }

        public async Task<bool> HasAnyOrgPermission(string org_uid, string subject_id, IEnumerable<string> permission_uid)
        {
            this.__check__(org_uid, subject_id, permission_uid);

            var all = await this.LoadAllOrgPermission(org_uid, subject_id);

            return all.ContainsAny_(permission_uid);
        }

        public Task<bool> HasOrgPermission(string org_uid, string subject_id, string permission_uid)
        {
            return HasAllOrgPermission(org_uid, subject_id, new[] { permission_uid });
        }

        private string[] __permissions__ = null;

        public async Task<string[]> LoadAllOrgPermission(string org_uid, string subject_id)
        {
            async Task<string[]> my_permission()
            {
                var db = this._repo.Database;

                var user_role_map = db.Set<OrgMemberRoleEntity>().AsNoTracking();
                var role_query = db.Set<OrgRoleEntity>().AsNoTracking();

                var roles = from user_role in user_role_map.Where(x => x.OrgUID == org_uid && x.UserUID == subject_id)

                            join role_permission in role_query
                            on user_role.RoleUID equals role_permission.UID

                            select role_permission;

                var res = await roles.ToArrayAsync();

                return res.SelectMany(x => this.permissionSerializer.Deserialize(x.PermissionJson)).Distinct().ToArray();
            }

            if (this.__permissions__ == null)
            {
                var cache_key = this._keyManager.UserOrgPermission(org_uid, subject_id);

                var data = await this._cache.GetOrSetAsync_(cache_key, my_permission, this._cache_timeout);

                this.__permissions__ = data ?? new string[] { };
            }

            return this.__permissions__;
        }
    }
}
