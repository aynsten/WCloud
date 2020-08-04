using Lib.ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Application.Service
{
    public interface IRoleService : IAutoRegistered
    {
        Task<List<RoleEntity>> QueryRoleList(string parent = null);

        Task<_<RoleEntity>> AddRole(RoleEntity role);

        Task DeleteRoleWhenNoChildren(string uid);

        Task<_<RoleEntity>> UpdateRole(RoleEntity model);

        Task SetUserRoles(string user_uid, List<AdminRoleEntity> roles);

        Task SetRolePermissions(string role_uid, string[] permissions);
    }
}
