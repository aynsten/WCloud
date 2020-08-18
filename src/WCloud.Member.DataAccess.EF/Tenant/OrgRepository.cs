using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Roles;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;

namespace WCloud.Member.DataAccess.EF.Tenant
{
    public class OrgRepository : MemberShipRepository<OrgEntity>, IOrgRepository
    {
        private readonly IWCloudContext _context;
        public OrgRepository(IWCloudContext<OrgRepository> _context) : base(_context.Provider)
        {
            this._context = _context;
        }

        public async Task<_<OrgMemberEntity>> AddMember(OrgMemberEntity model)
        {
            var org = await this.QueryOneAsync(x => x.Id == model.OrgUID);
            org.Should().NotBeNull();

            if (org.MemeberCount >= 3000)
            {
                return new _<OrgMemberEntity>().SetErrorMsg("成员数达到上限");
            }

            var db = this.Database;

            var org_member_set = db.Set<OrgMemberEntity>();
            org_member_set.RemoveRange(org_member_set.Where(x => x.UserUID == model.UserUID && x.OrgUID == model.OrgUID));
            org_member_set.Add(model);

            await db.SaveChangesAsync();

            {
                //更新数量
                await this.UpdateOrgMemberCount(model.OrgUID);
            }

            return new _<OrgMemberEntity>().SetSuccessData(model);
        }

        public async Task<List<UserEntity>> AllActiveMembers(string org_uid)
        {
            var db = this.Database;

            var map_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
            var user_query = db.Set<UserEntity>().AsNoTrackingQueryable();

            var query = from map in map_query.Where(x => x.OrgUID == org_uid)
                        orderby map.CreateTimeUtc ascending
                        join user in user_query
                        on map.UserUID equals user.Id
                        select user;

            var list = await query.Take(3000).ToListAsync();

            return list;
        }

        public async Task<List<UserEntity>> GetMembersByRole(string org_uid, string role_uid)
        {
            var db = this.Database;

            var member_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
            var role_query = db.Set<OrgMemberRoleEntity>().AsNoTrackingQueryable();
            var user_query = db.Set<UserEntity>().AsNoTrackingQueryable();

            var query = from role in role_query.Where(x => x.OrgUID == org_uid && x.RoleUID == role_uid)

                        join user in user_query
                        on role.UserUID equals user.Id

                        join member in member_query.Where(x => x.OrgUID == org_uid)
                        on user.Id equals member.UserUID

                        select user;

            var res = await query.Take(5000).ToListAsync();

            return res;
        }

        public async Task<List<OrgMemberEntity>> GetMyOrgMap(string user_uid)
        {
            var query = this.Database.Set<OrgMemberEntity>().IgnoreQueryFilters().Where(x => x.MemberApproved > 0 && x.UserUID == user_uid);
            var res = await query.ToListAsync();
            return res;
        }

        public async Task<List<UserEntity>> LoadOrgRoles(string org_uid, List<UserEntity> list)
        {
            if (ValidateHelper.IsNotEmpty(list))
            {
                var db = this.Database;
                var uids = list.Select(x => x.Id).ToArray();

                var map_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
                var role_query = db.Set<OrgMemberRoleEntity>().AsNoTrackingQueryable();

                var org_map = await map_query
                    .Where(x => x.OrgUID == org_uid && uids.Contains(x.UserUID))
                    .Select(x => new { x.UserUID, x.IsOwner }).ToArrayAsync();

                var role_map = await role_query
                    .Where(x => x.OrgUID == org_uid && uids.Contains(x.UserUID))
                    .Select(x => new { x.UserUID, x.RoleUID }).ToArrayAsync();


                var roles = MemberRoleHelper.GetRoles();
                foreach (var m in list)
                {
                    var map = org_map.FirstOrDefault(x => x.UserUID == m.Id);
                    if (map != null)
                    {
                        //mapping
                        m.IsOrgOwner = map.IsOwner > 0;
                        //m.OrgFlag = map.Flag;
                        //m.OrgFlagName = string.Join(",", MemberRoleHelper.ParseRoleNames(map.Flag, roles).Take(3).ToList());
                    }
                    //角色
                    m.OrgRoleUIDs = role_map.Where(x => x.UserUID == m.Id).Select(x => x.RoleUID).ToArray();
                }
            }

            return list;
        }

        public async Task<IEnumerable<OrgEntity>> LoadOwners(IEnumerable<OrgEntity> list)
        {
            if (ValidateHelper.IsNotEmpty(list))
            {
                var org_uids = list.Select(x => x.Id).ToArray();

                var db = this.Database;

                var map_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
                var user_query = db.Set<UserEntity>().AsNoTrackingQueryable();

                var maps = await map_query
                    .Where(x => org_uids.Contains(x.OrgUID) && x.IsOwner == 1)
                    .Select(x => new { x.UserUID, x.OrgUID }).ToArrayAsync();

                var user_uids = maps.Select(x => x.UserUID).ToArray();

                if (user_uids.Any())
                {
                    var users = await user_query.Where(x => user_uids.Contains(x.Id)).ToArrayAsync();

                    foreach (var m in list)
                    {
                        var uids = maps.Where(x => x.OrgUID == m.Id).Select(x => x.UserUID).ToArray();
                        m.Owners = users.Where(x => uids.Contains(x.Id)).ToList();
                    }
                }
            }

            return list;
        }

        public async Task<PagerData<UserEntity>> QueryDeactiveMembers(string org_uid, string q, int page, int pagesize)
        {
            var db = this.Database;

            var map_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
            var user_query = db.Set<UserEntity>().AsNoTrackingQueryable().IgnoreQueryFilters();

            var query = from map in map_query.Where(x => x.OrgUID == org_uid)
                        join user in user_query.Where(x => x.IsDeleted == 1)
                        on map.UserUID equals user.Id
                        select user;

            query = query.WhereIf(ValidateHelper.IsNotEmpty(q), x => x.UserName.StartsWith(q) || x.NickName.StartsWith(q));

            var paged = await query.ToPagedListAsync(page, pagesize, x => x.CreateTimeUtc);

            return paged;
        }

        public async Task<_<OrgMemberEntity>> RemoveOwner(string org_uid, string user_uid)
        {
            var res = new _<OrgMemberEntity>();

            var db = this.Database;
            var set = db.Set<OrgMemberEntity>();

            var entity = await set.FirstOrDefaultAsync(x => x.OrgUID == org_uid && x.UserUID == user_uid);

            entity.Should().NotBeNull("管理员不存在");

            entity.IsOwner = 0;

            if (!entity.IsValid(out var msg))
            {
                res.SetErrorMsg(msg);
                return res;
            }

            await db.SaveChangesAsync();

            res.SetSuccessData(entity);
            return res;
        }

        public async Task SaveOrgMemberRole(string org_uid, string user_uid, List<OrgMemberRoleEntity> model)
        {
            var db = this.Database;
            var set = db.Set<OrgMemberRoleEntity>();

            //删除旧的角色
            set.RemoveRange(set.Where(x => x.OrgUID == org_uid && x.UserUID == user_uid));
            await db.SaveChangesAsync();

            set.AddRange(model.Select(x => x.InitEntity()));
            await db.SaveChangesAsync();
        }

        public async Task UpdateMemberStatus(string org_uid, string user_uid, bool active)
        {
            var db = this.Database;
            var user_in_org = await db.Set<OrgMemberEntity>().Where(x => x.OrgUID == org_uid && x.UserUID == user_uid).AnyAsync();
            user_in_org.Should().Be(true);

            var user = await db.Set<UserEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == user_uid);
            user.Should().NotBeNull();

            user.IsDeleted = active ? 0 : 1;
            user.SetUpdateTime();

            await db.SaveChangesAsync();

            await this.UpdateOrgMemberCount(org_uid);
        }

        public async Task UpdateOrgMemberCount(string org_uid)
        {
            var db = this.Database;

            var org = await db.Set<OrgEntity>().FirstOrDefaultAsync(x => x.Id == org_uid);
            org.Should().NotBeNull();

            var map_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
            var user_query = db.Set<UserEntity>().AsNoTrackingQueryable();

            var query = from map in map_query.Where(x => x.OrgUID == org_uid)
                        join user in user_query
                        on map.UserUID equals user.Id
                        select user.Id;

            org.MemeberCount = await query.CountAsync();

            org.SetUpdateTime();

            await db.SaveChangesAsync();
        }
    }
}
