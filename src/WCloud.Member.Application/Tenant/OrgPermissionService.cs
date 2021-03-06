﻿using FluentAssertions;
using WCloud.Core.Cache;
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
    public class OrgPermissionService : IOrgPermissionService
    {
        private readonly TimeSpan _cache_timeout = TimeSpan.FromMinutes(10);

        private readonly IWCloudContext _context;
        private readonly IOrgRoleRepository _repo;

        public OrgPermissionService(
            IWCloudContext<OrgPermissionService> _context,
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
