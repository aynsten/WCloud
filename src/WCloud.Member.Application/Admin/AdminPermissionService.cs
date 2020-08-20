using FluentAssertions;
using WCloud.Core.Cache;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Application.PermissionValidator
{
    [ScopedInstance]
    public class AdminPermissionService : IAdminPermissionService
    {
        private readonly TimeSpan _cache_timeout = TimeSpan.FromMinutes(10);

        private readonly IWCloudContext _context;
        private readonly IRoleRepository _repo;

        public AdminPermissionService(
            IWCloudContext<AdminPermissionService> _context,
            IRoleRepository _repo)
        {
            this._context = _context;
            this._repo = _repo;
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
            if (this.__permissions__ == null)
            {
                var cache_key = this._context.CacheKeyManager.AdminPermission(subject_id);

                var data = await this._context.CacheProvider.GetOrSetAsync_(cache_key,
                    () => this._repo.QueryMyPermission(subject_id),
                    this._cache_timeout);

                this.__permissions__ = data ?? new string[] { };
            }

            return this.__permissions__;
        }
    }
}
