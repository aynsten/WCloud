using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Member.Domain.Tenant;

namespace WCloud.Member.DataAccess.EF.Tenant
{
    public class OrgRoleRepository : MemberShipRepository<OrgRoleEntity>, IOrgRoleRepository
    {
        private readonly IWCloudContext _context;
        public OrgRoleRepository(IWCloudContext<OrgRoleRepository> _context) : base(_context.Provider)
        {
            this._context = _context;
        }

        public async Task<string[]> QueryMyPermission(string org_uid, string subject_id)
        {
            var db = this.Database;

            var user_role_map = db.Set<OrgMemberRoleEntity>().AsNoTracking();
            var role_query = db.Set<OrgRoleEntity>().AsNoTracking();

            var roles = from user_role in user_role_map.Where(x => x.OrgUID == org_uid && x.UserUID == subject_id)

                        join role_permission in role_query
                        on user_role.RoleUID equals role_permission.Id

                        select role_permission;

            var roles_data = await roles.ToArrayAsync();

            var res = roles_data.SelectMany(x => this._context.StringArraySerializer.Deserialize(x.PermissionJson)).Distinct().ToArray();

            return res;
        }
    }
}
