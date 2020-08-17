using System.Threading.Tasks;

namespace WCloud.Member.Domain.Tenant
{
    public interface IOrgRoleRepository : IMemberRepository<OrgRoleEntity>
    {
        Task<string[]> QueryMyPermission(string org_uid, string subject_id);
    }
}
