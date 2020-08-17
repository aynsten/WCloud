using FluentAssertions;
using Lib.cache;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.Application.PermissionValidator
{
    [ScopedInstance]
    public class OrgPermissionValidatorService : IOrgPermissionValidatorService
    {
        private readonly TimeSpan _cache_timeout = TimeSpan.FromMinutes(10);

        private readonly IWCloudContext _context;
        private readonly IOrgRoleRepository _repo;

        public OrgPermissionValidatorService(
            IWCloudContext<OrgPermissionValidatorService> _context,
            IOrgRoleRepository _repo)
        {
            this._context = _context;
            this._repo = _repo;
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
            /*
            async Task<string[]> QueryMyPermission(string org_uid, string subject_id)
            {
                var db = this._repo.Database;

                var user_role_map = db.Set<OrgMemberRoleEntity>().AsNoTracking();
                var role_query = db.Set<OrgRoleEntity>().AsNoTracking();

                var roles = from user_role in user_role_map.Where(x => x.OrgUID == org_uid && x.UserUID == subject_id)

                            join role_permission in role_query
                            on user_role.RoleUID equals role_permission.Id

                            select role_permission;

                var res = await roles.ToArrayAsync();

                return res.SelectMany(x => this.permissionSerializer.Deserialize(x.PermissionJson)).Distinct().ToArray();
            }
             */

            if (this.__permissions__ == null)
            {
                var cache_key = this._context.CacheKeyManager.UserOrgPermission(org_uid, subject_id);

                var data = await this._context.CacheProvider.GetOrSetAsync_(cache_key,
                    () => this._repo.QueryMyPermission(org_uid, subject_id),
                    this._cache_timeout);

                this.__permissions__ = data ?? new string[] { };
            }

            return this.__permissions__;
        }
    }
}
