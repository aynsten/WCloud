using System.Threading.Tasks;

namespace WCloud.Member.Domain.Admin
{
    public interface IRoleRepository : IMemberRepository<RoleEntity>
    {
        Task SetUserRoles(string user_uid, string[] role_ids);
        /// <summary>
        /// 用户->角色->权限
        /// </summary>
        Task<string[]> QueryMyPermission(string subject_id);
    }
}
