using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Application.Service.impl
{
    public class RoleService : IRoleService
    {
        private readonly IWCloudContext _context;
        private readonly IRoleRepository roleRepository;

        public RoleService(
            IWCloudContext<RoleService> _context,
            IRoleRepository roleRepository)
        {
            this._context = _context;
            this.roleRepository = roleRepository;
        }

        public virtual async Task<List<RoleEntity>> QueryRoleList(string parent = null)
        {
            var query = this.roleRepository.Queryable;

            query = query.WhereIf(ValidateHelper.IsNotEmpty(parent), x => x.ParentUID == parent);

            var list = query.OrderByDescending(x => x.CreateTimeUtc).Take(5000).ToList();

            await Task.CompletedTask;

            return list;
        }

        public virtual async Task<_<RoleEntity>> AddRole(RoleEntity role)
        {
            role.Should().NotBeNull("add role role");
            role.NodeName.Should().NotBeNullOrEmpty("add role role name");

            var data = new _<RoleEntity>();

            if (await this.roleRepository.ExistAsync(x => x.NodeName == role.NodeName))
            {
                return data.SetErrorMsg("角色名重复");
            }

            var res = await this.roleRepository.AddTreeNode(role, "role");
            this._context.Logger.LogInformation("添加新角色");
            return res;
        }

        public virtual async Task<_<RoleEntity>> UpdateRole(RoleEntity model)
        {
            model.Should().NotBeNull("update role model");
            model.Id.Should().NotBeNullOrEmpty("update role uid");

            var data = new _<RoleEntity>();

            var role = await this.roleRepository.QueryOneAsync(x => x.Id == model.Id);
            role.Should().NotBeNull($"角色不存在：{model.Id}");

            role.NodeName = model.NodeName;
            role.RoleDescription = model.RoleDescription;

            if (!role.IsValid(out var msg))
            {
                data.SetErrorMsg(msg);
                return data;
            }

            await this.roleRepository.UpdateAsync(role);

            return data.SetSuccessData(role);
        }

        public virtual async Task SetUserRoles(string user_uid, List<AdminRoleEntity> roles)
        {
            user_uid.Should().NotBeNullOrEmpty("set user role user uid");
            roles.Should().NotBeNull("set user roles roles");
            roles.ForEach(x => x.AdminUID.Should().Be(user_uid, "set user roles user uid equal"));

            await this.roleRepository.SetUserRoles(user_uid, roles.Select(x => x.RoleUID).ToArray());
        }

        public virtual async Task SetRolePermissions(string role_uid, string[] permissions)
        {
            role_uid.Should().NotBeNullOrEmpty("set role permission role uid");
            permissions.Should().NotBeNull("set role permission permissions");

            var model = await this.roleRepository.QueryOneAsync(x => x.Id == role_uid);
            model.Should().NotBeNull("set role permission");

            if (!this._context.DataSerializer.DeserializeArray(model.PermissionJson).AllEqual(permissions))
            {
                model.PermissionJson = permissions.ToJson();
                await this.roleRepository.UpdateAsync(model);
            }
        }

        public async Task DeleteRoleWhenNoChildren(string uid)
        {
            uid.Should().NotBeNullOrEmpty("delete role uid");

            await this.roleRepository.DeleteSingleNodeWhenNoChildren_(uid);
        }
    }
}
