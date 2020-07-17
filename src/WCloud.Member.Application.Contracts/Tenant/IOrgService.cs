using Lib.helper;
using Lib.ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Application.Service
{
    public interface IOrgService : IAutoRegistered
    {
        /// <summary>
        /// 离线计算成员数量
        /// </summary>
        /// <param name="org_uid"></param>
        /// <returns></returns>
        Task UpdateOrgMemberCount(string org_uid);

        Task<List<OrgMemberEntity>> GetMyOrgMap(string user_uid);

        Task<List<OrgEntity>> GetOrgListByUID(params string[] org_uids);

        /// <summary>
        /// 在职员工
        /// </summary>
        /// <param name="org_uid"></param>
        /// <returns></returns>
        Task<List<UserEntity>> AllActiveMembers(string org_uid);

        /// <summary>
        /// 查询离职员工
        /// </summary>
        /// <param name="org_uid"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<PagerData<UserEntity>> QueryDeactiveMembers(string org_uid, string q = null, int page = 1, int pagesize = 10);

        Task<_<OrgEntity>> AddOrg(OrgEntity model);

        Task DeleteOrg(params string[] org_uids);

        Task ActiveOrDeActiveOrg(string uid, bool active);

        Task<_<OrgEntity>> UpdateOrg(OrgEntity model);

        Task<_<OrgEntity>> ChangeOrgName(string org_uid, string name);

        Task<OrgEntity> GetOrgByUID(string org_uid);

        Task<PagerData<OrgEntity>> QueryOrgPager(string q = null, int page = 1, int pagesize = 10, int? isremove = null);

        Task<_<OrgMemberEntity>> AddMember(OrgMemberEntity model);

        Task<_<OrgMemberEntity>> RemoveOwner(string org_uid, string user_uid);

        /// <summary>
        /// 冻结成员账号
        /// </summary>
        /// <param name="org_uid"></param>
        /// <param name="user_uid"></param>
        /// <returns></returns>
        Task DeactiveMember(string org_uid, string user_uid);

        /// <summary>
        /// 激活成员账号
        /// </summary>
        /// <param name="org_uid"></param>
        /// <param name="user_uid"></param>
        /// <returns></returns>
        Task ActiveMember(string org_uid, string user_uid);

        Task<List<UserEntity>> LoadOrgRoles(string org_uid, List<UserEntity> list);

        Task<IEnumerable<OrgEntity>> LoadOwners(IEnumerable<OrgEntity> list);

        Task SaveOrgMemberRole(string org_uid, string user_uid, List<OrgMemberRoleEntity> model);

        Task<List<UserEntity>> GetMembersByRole(string org_uid, string role_uid);
    }
}
