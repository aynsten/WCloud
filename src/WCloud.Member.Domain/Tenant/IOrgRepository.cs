using Lib.helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Domain.Tenant
{
    public interface IOrgRepository : IMemberRepository<OrgEntity>
    {
        Task<PagerData<UserEntity>> QueryDeactiveMembers(string org_uid, string q, int page, int pagesize);
        Task<List<UserEntity>> AllActiveMembers(string org_uid);
        Task<List<UserEntity>> GetMembersByRole(string org_uid, string role_uid);
        Task<List<UserEntity>> LoadOrgRoles(string org_uid, List<UserEntity> list);
        Task UpdateOrgMemberCount(string org_uid);
        Task<IEnumerable<OrgEntity>> LoadOwners(IEnumerable<OrgEntity> list);
        Task<List<OrgMemberEntity>> GetMyOrgMap(string user_uid);
        Task<_<OrgMemberEntity>> AddMember(OrgMemberEntity model);
        Task<_<OrgMemberEntity>> RemoveOwner(string org_uid, string user_uid);
        Task UpdateMemberStatus(string org_uid, string user_uid, bool active);
        Task SaveOrgMemberRole(string org_uid, string user_uid, List<OrgMemberRoleEntity> model);
    }
}
