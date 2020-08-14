using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.DataAccess.EF.Admin
{
    public class RoleRepository : MemberShipRepository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(IServiceProvider provider) : base(provider)
        {
            //
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
    }
}
