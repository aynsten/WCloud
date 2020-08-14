using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.DataAccess.EF.Admin
{
    public class AdminRepository : MemberShipRepository<AdminEntity>, IAdminRepository
    {
        public AdminRepository(IServiceProvider provider) : base(provider)
        { }

        public async Task<IEnumerable<AdminEntity>> LoadRoles(IEnumerable<AdminEntity> list)
        {
            if (list.Any())
            {
                var db = this.Database;
                var uids = list.Select(x => x.Id).ToList();
                var maps = await db.Set<AdminRoleEntity>().AsNoTrackingQueryable()
                    .Where(x => uids.Contains(x.AdminUID)).Select(x => new { x.AdminUID, x.RoleUID }).ToArrayAsync();

                if (maps.Any())
                {
                    var role_uids = maps.Select(x => x.RoleUID).ToList();
                    var roles = await db.Set<RoleEntity>().AsNoTrackingQueryable().Where(x => role_uids.Contains(x.Id)).ToArrayAsync();

                    foreach (var m in list)
                    {
                        var user_uids = maps.Where(x => x.AdminUID == m.Id).Select(x => x.RoleUID).ToList();
                        m.Roles = roles.Where(x => user_uids.Contains(x.Id)).ToArray();
                    }
                }
            }
            foreach (var m in list)
            {
                m.Roles ??= new RoleEntity[] { };
            }
            return list;
        }

        public async Task<PagerData<AdminEntity>> QueryAdmin(QueryAdminParameter filter, int page, int pagesize)
        {
            var query = this.NoTrackingQueryable.IgnoreQueryFilters();

            query = query.WhereIf(filter.IsDeleted != null, x => x.IsDeleted == filter.IsDeleted);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(filter.Name), x => x.NickName == filter.Name);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(filter.Keyword), x => x.NickName.StartsWith(filter.Keyword));

            var data = await query.ToPagedListAsync(page, pagesize, x => x.CreateTimeUtc, desc: false);

            return data;
        }

        public async Task<List<AdminEntity>> QueryTopUser(string q, string[] role_uid, int size)
        {
            var db = this.Database;
            var user_query = db.Set<AdminEntity>().AsNoTrackingQueryable();

            user_query = user_query.WhereIf(ValidateHelper.IsNotEmpty(q),
                x => x.UserName.StartsWith(q) || x.NickName.StartsWith(q));

            if (ValidateHelper.IsNotEmpty(role_uid))
            {
                var role_map_query = db.Set<AdminRoleEntity>().AsNoTracking();

                var query = from user in user_query
                            join map in role_map_query.Where(x => role_uid.Contains(x.RoleUID))
                            on user.Id equals map.AdminUID
                            select user;

                user_query = query;
            }

            var data = await user_query.Take(size).ToListAsync();

            return data;
        }

        public async Task<PagerData<AdminEntity>> QueryUserList(string name = null, string email = null, string keyword = null, int? isremove = null, int page = 1, int pagesize = 20)
        {
            var query = this.NoTrackingQueryable.IgnoreQueryFilters();

            query = query.WhereIf(isremove != null, x => x.IsDeleted == isremove);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(name), x => x.NickName == name);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(keyword), x => x.NickName.StartsWith(keyword));

            var data = await query.ToPagedListAsync(page, pagesize, x => x.CreateTimeUtc, desc: false);

            return data;
        }
    }
}
