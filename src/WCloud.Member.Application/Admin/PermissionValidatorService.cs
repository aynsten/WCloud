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
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Application.PermissionValidator
{
    [ScopedInstance]
    public class PermissionValidatorService : IPermissionValidatorService
    {
        private readonly TimeSpan _cache_timeout = TimeSpan.FromMinutes(10);

        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager _keyManager;
        private readonly IMSRepository<RoleEntity> _repo;
        private readonly IStringArraySerializer permissionSerializer;

        public PermissionValidatorService(
            ICacheProvider _cache,
            ICacheKeyManager keyManager,
            IMSRepository<RoleEntity> _repo,
            IStringArraySerializer permissionSerializer)
        {
            this._cache = _cache;
            this._keyManager = keyManager;
            this._repo = _repo;
            this.permissionSerializer = permissionSerializer;
        }

        void __check__(string subject_id, IEnumerable<string> permissions)
        {
            subject_id.Should().NotBeNullOrEmpty("permission check subject id");
            permissions.Should().NotBeNullOrEmpty("permission check permissions");

            foreach (var m in permissions)
            {
                m.Should().NotBeNullOrEmpty();
            }
        }

        public async Task<bool> HasAllPermission(string subject_id, IEnumerable<string> permissions)
        {
            this.__check__(subject_id, permissions);

            var all = await this.LoadAllPermissionsName(subject_id);

            return all.ContainsAny_(permissions);
        }

        public async Task<bool> HasAnyPermission(string subject_id, IEnumerable<string> permissions)
        {
            this.__check__(subject_id, permissions);

            var all = await this.LoadAllPermissionsName(subject_id);

            return all.ContainsAny_(permissions);
        }

        public async Task<bool> HasPermission(string subject_id, string permission)
        {
            return await this.HasAllPermission(subject_id, new string[] { permission });
        }

        private string[] __permissions__ = null;

        public async Task<string[]> LoadAllPermissionsName(string subject_id)
        {
            /// <summary>
            /// 用户->角色->权限
            /// </summary>
            /// <param name="subject_id"></param>
            /// <returns></returns>
            async Task<string[]> __my_permission__()
            {
                var db = this._repo.Database;

                var user_role_query = db.Set<AdminRoleEntity>().AsNoTracking();
                var role_query = db.Set<RoleEntity>().AsNoTracking();

                var roles = user_role_query.Where(x => x.AdminUID == subject_id).Select(x => x.RoleUID);

                var res = await role_query
                    .Where(x => roles.Contains(x.UID))
                    .ToArrayAsync();

                return res.SelectMany(x => this.permissionSerializer.Deserialize(x.PermissionJson)).Distinct().ToArray();
            }

            if (this.__permissions__ == null)
            {
                var cache_key = this._keyManager.AdminPermission(subject_id);

                var data = await this._cache.GetOrSetAsync_(cache_key, __my_permission__, this._cache_timeout);

                this.__permissions__ = data ?? new string[] { };
            }

            return this.__permissions__;
        }
    }
}
