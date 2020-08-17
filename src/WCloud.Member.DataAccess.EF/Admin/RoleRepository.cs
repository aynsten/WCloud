using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.DataAccess.EF.Admin
{
    public class RoleRepository : MemberShipRepository<RoleEntity>, IRoleRepository
    {
        private readonly IWCloudContext _context;
        public RoleRepository(IWCloudContext<RoleRepository> _context) : base(_context.Provider)
        {
            this._context = _context;
        }

        public async Task SetUserRoles(string user_uid, string[] role_ids)
        {
            var db = this.Database;
            var set = db.Set<AdminRoleEntity>();

            set.RemoveRange(set.Where(x => x.AdminUID == user_uid));
            await db.SaveChangesAsync();

            if (role_ids.Any())
            {
                var mapping = role_ids.Select(x => new AdminRoleEntity()
                {
                    AdminUID = user_uid,
                    RoleUID = x
                }.InitEntity()).ToArray();
                set.AddRange(mapping);
                await db.SaveChangesAsync();
            }
        }

        public async Task<string[]> QueryMyPermission(string subject_id)
        {
            var db = this.Database;

            var user_role_query = db.Set<AdminRoleEntity>().AsNoTracking();
            var role_query = db.Set<RoleEntity>().AsNoTracking();

            var roles = user_role_query.Where(x => x.AdminUID == subject_id).Select(x => x.RoleUID);

            var res = await role_query
                .Where(x => roles.Contains(x.Id))
                .ToArrayAsync();

            return res.SelectMany(x => this._context.StringArraySerializer.Deserialize(x.PermissionJson)).Distinct().ToArray();
        }
    }
}
