using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.Application.Service
{
    public interface IOrgRoleService : IAutoRegistered
    {
        Task<List<OrgRoleEntity>> GetOrgRoles(string org_uid);

        Task DeleteOrgRole(string role_uid);

        Task<_<OrgRoleEntity>> AddOrgRole(OrgRoleEntity model);

        //Task<_<OrgRoleEntity>> UpdateOrgRole(OrgRoleEntity model);

        Task SaveOrgRolePermission(string org_uid,string org_role_uid, string[] permissions);

        //Task SetMemberOrgRole(string org_uid, string user_uid, string[] org_role_uids);
    }
}
