using FluentAssertions;
using Lib.extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.Application.Service.impl
{
    public class OrgRoleService : IOrgRoleService
    {
        private readonly IWCloudContext _context;
        private readonly IOrgRoleRepository _orgRoleRepo;

        public OrgRoleService(
            IWCloudContext<OrgRoleService> _context,
            IOrgRoleRepository _orgRoleRepo)
        {
            this._context = _context;
            this._orgRoleRepo = _orgRoleRepo;
        }

        public virtual async Task<List<OrgRoleEntity>> GetOrgRoles(string org_uid)
        {
            org_uid.Should().NotBeNullOrEmpty("get org roles org uid");

            var query = this._orgRoleRepo.Queryable;
            query = query.Where(x => x.OrgUID == org_uid);

            var list = query.OrderByDescending(x => x.CreateTimeUtc).Take(5000).ToList();

            await Task.CompletedTask;

            return list;
        }

        public virtual async Task<_<OrgRoleEntity>> AddOrgRole(OrgRoleEntity orgrole)
        {
            orgrole.Should().NotBeNull("add org role model");
            orgrole.RoleName.Should().NotBeNullOrEmpty("add org role name");
            orgrole.OrgUID.Should().NotBeNullOrEmpty("add org role org uid");

            if (await this._orgRoleRepo.ExistAsync(x => x.OrgUID == orgrole.OrgUID && x.RoleName == orgrole.RoleName))
            {
                return new _<OrgRoleEntity>().SetErrorMsg("角色名重复");
            }

            var res = await this._orgRoleRepo.InsertAsync(orgrole);
            return new _<OrgRoleEntity>().SetSuccessData(orgrole);
        }

        public virtual async Task DeleteOrgRole(string role_uid)
        {
            role_uid.Should().NotBeNullOrEmpty("delete org role role uid");

            await this._orgRoleRepo.DeleteWhereAsync(x => x.Id == role_uid);
        }

        public virtual async Task SaveOrgRolePermission(string org_uid, string org_role_uid, string[] permissions)
        {
            org_uid.Should().NotBeNullOrEmpty("save org role permission org_uid");
            org_role_uid.Should().NotBeNullOrEmpty("save org role permission org_role_uid");
            permissions.Should().NotBeNull("save org role permission permissions");

            var model = await this._orgRoleRepo.QueryOneAsync(x => x.Id == org_role_uid && x.OrgUID == org_uid);
            model.Should().NotBeNull("org role");

            if (!this._context.DataSerializer.DeserializeArray(model.PermissionJson).AllEqual(permissions))
            {
                model.PermissionJson = permissions.ToJson();
                await this._orgRoleRepo.UpdateAsync(model);
            }
        }
    }
}
