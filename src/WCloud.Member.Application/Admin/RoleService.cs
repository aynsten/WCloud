using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Helper;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Application.Service.impl
{
    public class RoleService : IRoleService
    {
        private readonly IMSRepository<RoleEntity> _roleRepo;
        private readonly IMSRepository<AdminRoleEntity> _userRoleRepo;
        private readonly IStringArraySerializer permissionSerializer;

        public RoleService(
            IMSRepository<RoleEntity> _roleRepo,
            IMSRepository<AdminRoleEntity> _userRoleRepo,
            IStringArraySerializer permissionSerializer)
        {
            this._roleRepo = _roleRepo;
            this._userRoleRepo = _userRoleRepo;
            this.permissionSerializer = permissionSerializer;
        }

        public virtual async Task<List<RoleEntity>> QueryRoleList(string parent = null)
        {
            var query = this._roleRepo.NoTrackingQueryable;

            query = query.WhereIf(ValidateHelper.IsNotEmpty(parent), x => x.ParentUID == parent);

            var list = await query.OrderByDescending(x => x.Id).Take(5000).ToListAsync();

            return list;
        }

        public virtual async Task<_<RoleEntity>> AddRole(RoleEntity role)
        {
            role.Should().NotBeNull("add role role");
            role.NodeName.Should().NotBeNullOrEmpty("add role role name");

            var data = new _<RoleEntity>();

            if (await this._roleRepo.ExistAsync(x => x.NodeName == role.NodeName))
            {
                return data.SetErrorMsg("角色名重复");
            }

            var res = await this._roleRepo.AddTreeNode(role, "role");
            return res;
        }

        public virtual async Task<_<RoleEntity>> UpdateRole(RoleEntity model)
        {
            model.Should().NotBeNull("update role model");
            model.UID.Should().NotBeNullOrEmpty("update role uid");

            var data = new _<RoleEntity>();

            var role = await this._roleRepo.QueryOneAsync(x => x.UID == model.UID);
            role.Should().NotBeNull($"角色不存在：{model.UID}");

            role.NodeName = model.NodeName;
            role.RoleDescription = model.RoleDescription;

            if (!role.IsValid(out var msg))
            {
                data.SetErrorMsg(msg);
                return data;
            }

            await this._roleRepo.UpdateAsync(role);

            return data.SetSuccessData(role);
        }

        public virtual async Task SetUserRoles(string user_uid, List<AdminRoleEntity> roles)
        {
            user_uid.Should().NotBeNullOrEmpty("set user role user uid");
            roles.Should().NotBeNull("set user roles roles");
            roles.ForEach(x => x.AdminUID.Should().Be(user_uid, "set user roles user uid equal"));

            await this._userRoleRepo.DeleteWhereAsync(x => x.AdminUID == user_uid);
            if (roles.Any())
            {
                roles = roles.Select(x => x.InitSelf()).ToList();

                await this._userRoleRepo.InsertBulkAsync(roles);
            }
        }

        public virtual async Task SetRolePermissions(string role_uid, string[] permissions)
        {
            role_uid.Should().NotBeNullOrEmpty("set role permission role uid");
            permissions.Should().NotBeNull("set role permission permissions");

            var model = await this._roleRepo.QueryOneAsync(x => x.UID == role_uid);
            model.Should().NotBeNull("set role permission");

            if (!this.permissionSerializer.Deserialize(model.PermissionJson).AllEqual(permissions))
            {
                model.PermissionJson = permissions.ToJson();
                await this._roleRepo.UpdateAsync(model);
            }
        }

        public async Task DeleteRoleWhenNoChildren(string uid)
        {
            uid.Should().NotBeNullOrEmpty("delete role uid");

            await this._roleRepo.DeleteSingleNodeWhenNoChildren_(uid);
        }
    }
}
