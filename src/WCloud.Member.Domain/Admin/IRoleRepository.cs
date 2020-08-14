using System.Threading.Tasks;

namespace WCloud.Member.Domain.Admin
{
    public interface IRoleRepository : IMemberRepository<RoleEntity>
    {
        Task SetUserRoles(string user_uid, string[] role_ids);
    }
}
