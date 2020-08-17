using FluentAssertions;
using Lib.extension;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Application.Service.impl
{
    public class OrgService : IOrgService
    {
        private readonly IOrgRepository _orgRepo;

        public OrgService(IOrgRepository _orgRepo)
        {
            this._orgRepo = _orgRepo;
        }


        public virtual async Task<_<OrgEntity>> AddOrg(OrgEntity model)
        {
            model.Should().NotBeNull();
            model.OrgName.Should().NotBeNullOrEmpty();

            var res = new _<OrgEntity>();

            model.InitEntity();
            if (!model.IsValid(out var msg))
            {
                res.SetErrorMsg(msg);
                return res;
            }

            if (await this._orgRepo.ExistAsync(x => x.OrgName == model.OrgName))
            {
                return res.SetErrorMsg("名称已经存在");
            }

            await this._orgRepo.InsertAsync(model);

            res.SetSuccessData(model);
            return res;
        }

        public virtual async Task DeleteOrg(string[] org_uids)
        {
            org_uids.Should().NotBeNull();
            org_uids.Should().NotBeNullOrEmpty();
            foreach (var m in org_uids)
                m.Should().NotBeNullOrEmpty();

            await this._orgRepo.DeleteByIds(org_uids);
        }

        public virtual async Task ActiveOrDeActiveOrg(string uid, bool active)
        {
            uid.Should().NotBeNullOrEmpty();

            var org = await this._orgRepo.QueryOneAsync(x => x.Id == uid);
            org.Should().NotBeNull($"客户不存在:{uid}");

            org.IsDeleted = (!active).ToBoolInt();
            org.SetUpdateTime();

            await this._orgRepo.UpdateAsync(org);
        }
        public virtual async Task<_<OrgEntity>> UpdateOrg(OrgEntity model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            var res = new _<OrgEntity>();

            var entity = await this._orgRepo.QueryOneAsync(x => x.Id == model.Id);

            entity.Should().NotBeNull("组织不存在");

            entity.OrgDescription = model.OrgDescription;
            entity.OrgImage = model.OrgImage;
            entity.OrgWebSite = model.OrgWebSite;
            entity.Phone = model.Phone;

            entity.SetUpdateTime();
            if (!entity.IsValid(out var msg))
            {
                res.SetErrorMsg(msg);
                return res;
            }

            await this._orgRepo.UpdateAsync(entity);

            res.SetSuccessData(entity);
            return res;
        }

        public virtual async Task<OrgEntity> GetOrgByUID(string org_uid)
        {
            org_uid.Should().NotBeNullOrEmpty();

            var res = await this._orgRepo.QueryOneAsync(x => x.Id == org_uid);
            return res;
        }

        public virtual async Task<PagerData<OrgEntity>> QueryOrgPager(string q = null, int page = 1, int pagesize = 10, int? isremove = null)
        {
            page.Should().BeGreaterOrEqualTo(1);
            pagesize.Should().BeInRange(1, 100);

            var query = this._orgRepo.Queryable;
            query = query.WhereIf(isremove != null, x => x.IsDeleted == isremove);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(q), x => x.OrgName.StartsWith(q));

            var data = query.ToPagedList(page, pagesize, x => x.CreateTimeUtc);

            await Task.CompletedTask;

            return data;
        }

        public async Task<List<OrgMemberEntity>> GetMyOrgMap(string user_uid)
        {
            user_uid.Should().NotBeNullOrEmpty();
            var res = await this._orgRepo.GetMyOrgMap(user_uid);
            /*
             
            var res = await this._orgMemberRepo.QueryManyAsync(x => x.MemberApproved > 0 && x.UserUID == user_uid);
             */
            return res;
        }

        public async Task<List<OrgEntity>> GetOrgListByUID(params string[] org_uids)
        {
            org_uids.Should().NotBeNull();
            org_uids.Should().NotBeNullOrEmpty();

            var res = await this._orgRepo.QueryManyAsync(x => org_uids.Contains(x.Id));
            return res;
        }

        public async Task<PagerData<UserEntity>> QueryDeactiveMembers(string org_uid, string q = null, int page = 1, int pagesize = 10)
        {
            org_uid.Should().NotBeNullOrEmpty();
            page.Should().BeGreaterOrEqualTo(1);
            pagesize.Should().BeInRange(1, 100);

            var res = await this._orgRepo.QueryDeactiveMembers(org_uid, q, page, pagesize);

            return res;
            /*
             
            var db = this._orgRepo.Database;

            var map_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
            var user_query = db.Set<UserEntity>().AsNoTrackingQueryable().IgnoreQueryFilters();

            var query = from map in map_query.Where(x => x.OrgUID == org_uid)
                        join user in user_query.Where(x => x.IsDeleted == 1)
                        on map.UserUID equals user.Id
                        select user;

            query = query.WhereIf(ValidateHelper.IsNotEmpty(q), x => x.UserName.StartsWith(q) || x.NickName.StartsWith(q));

            var paged = await query.ToPagedListAsync(page, pagesize, x => x.CreateTimeUtc);

            return paged;
             */
        }

        public virtual async Task<_<OrgMemberEntity>> AddMember(OrgMemberEntity model)
        {
            model.Should().NotBeNull();
            model.OrgUID.Should().NotBeNullOrEmpty();
            model.UserUID.Should().NotBeNullOrEmpty();

            var res = await this._orgRepo.AddMember(model);

            return res;

            /*
            var org = await this._orgRepo.QueryOneAsync(x => x.Id == model.OrgUID);
            org.Should().NotBeNull();

            if (org.MemeberCount >= 3000)
            {
                return new _<OrgMemberEntity>().SetErrorMsg("成员数达到上限");
            }

            await this._orgMemberRepo.DeleteWhereAsync(x => x.UserUID == model.UserUID && x.OrgUID == model.OrgUID);

            var res = await this._orgMemberRepo.InsertAsync(model);

            {
                //更新数量
                await this.UpdateOrgMemberCount(model.OrgUID);
            }

            return new _<OrgMemberEntity>().SetSuccessData(model);
            */
        }

        public virtual async Task<_<OrgMemberEntity>> RemoveOwner(string org_uid, string user_uid)
        {
            org_uid.Should().NotBeNullOrEmpty();
            user_uid.Should().NotBeNullOrEmpty();

            var res = await this._orgRepo.RemoveOwner(org_uid, user_uid);

            return res;

            /*
            var res = new _<OrgMemberEntity>();

            var entity = await this._orgMemberRepo.QueryOneAsync(x => x.OrgUID == org_uid && x.UserUID == user_uid);

            entity.Should().NotBeNull("管理员不存在");

            entity.IsOwner = 0;

            if (!entity.IsValid(out var msg))
            {
                res.SetErrorMsg(msg);
                return res;
            }

            await this._orgMemberRepo.UpdateAsync(entity);

            res.SetSuccessData(entity);
            return res;
            */
        }

        public async Task UpdateMemberStatus(string org_uid, string user_uid, bool active)
        {
            org_uid.Should().NotBeNullOrEmpty();
            user_uid.Should().NotBeNullOrEmpty();

            /*
             
            var user_in_org = await this._orgMemberRepo.ExistAsync(x => x.OrgUID == org_uid && x.UserUID == user_uid);
            user_in_org.Should().Be(true);

            var db = this._orgMemberRepo.Database;
            var user = await db.Set<UserEntity>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == user_uid);
            user.Should().NotBeNull();

            user.IsDeleted = active ? 0 : 1;
            user.SetUpdateTime();

            await db.SaveChangesAsync();

            await this.UpdateOrgMemberCount(org_uid);
             */
            await this._orgRepo.UpdateMemberStatus(org_uid, user_uid, active);
        }

        public virtual async Task DeactiveMember(string org_uid, string user_uid)
        {
            await this.UpdateMemberStatus(org_uid, user_uid, active: false);
        }

        public async Task ActiveMember(string org_uid, string user_uid)
        {
            await this.UpdateMemberStatus(org_uid, user_uid, active: true);
        }

        public async Task<List<UserEntity>> AllActiveMembers(string org_uid)
        {
            org_uid.Should().NotBeNullOrEmpty();
            var res = await this._orgRepo.AllActiveMembers(org_uid);
            return res;
            /*
             
            var db = this._orgRepo.Database;

            var map_query = db.Set<OrgMemberEntity>().AsNoTrackingQueryable();
            var user_query = db.Set<UserEntity>().AsNoTrackingQueryable();

            var query = from map in map_query.Where(x => x.OrgUID == org_uid)
                        orderby map.CreateTimeUtc ascending
                        join user in user_query
                        on map.UserUID equals user.Id
                        select user;

            var list = await query.Take(3000).ToListAsync();

            return list;
             */
        }

        public async Task<List<UserEntity>> GetMembersByRole(string org_uid, string role_uid)
        {
            var res = await this._orgRepo.GetMembersByRole(org_uid, role_uid);
            return res;
            /*
             
            var db = this._orgRepo.Database;

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
             */
        }

        public async Task<List<UserEntity>> LoadOrgRoles(string org_uid, List<UserEntity> list)
        {
            org_uid.Should().NotBeNullOrEmpty();
            list.Should().NotBeNull();

            var res = await this._orgRepo.LoadOrgRoles(org_uid, list);

            return res;

            /*
             
            if (ValidateHelper.IsNotEmpty(list))
            {
                var db = this._orgRepo.Database;
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
             */
        }

        public async Task UpdateOrgMemberCount(string org_uid)
        {
            org_uid.Should().NotBeNullOrEmpty();

            await this._orgRepo.UpdateOrgMemberCount(org_uid);

            /*
            var db = this._orgRepo.Database;

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
            */
        }

        public async Task<_<OrgEntity>> ChangeOrgName(string org_uid, string name)
        {
            org_uid.Should().NotBeNullOrEmpty();
            name.Should().NotBeNullOrEmpty();

            var res = new _<OrgEntity>();

            var org = await this._orgRepo.QueryOneAsync(x => x.Id == org_uid);
            org.Should().NotBeNull();

            if (!await this._orgRepo.ExistAsync(x => x.OrgName == name && x.Id != org.Id))
            {
                return res.SetErrorMsg("名称已经存在");
            }

            org.OrgName = name;
            org.SetUpdateTime();

            await this._orgRepo.UpdateAsync(org);

            return res.SetSuccessData(org);
        }

        public virtual async Task SaveOrgMemberRole(string org_uid, string user_uid, List<OrgMemberRoleEntity> model)
        {
            org_uid.Should().NotBeNullOrEmpty();
            user_uid.Should().NotBeNullOrEmpty();
            model.ForEach(x =>
            {
                x.UserUID.Should().Be(user_uid);
                x.OrgUID.Should().Be(org_uid);
            });
            /*
             
            //删除旧的角色
            await this._orgMemberRoleRepo.DeleteWhereAsync(x => x.OrgUID == org_uid && x.UserUID == user_uid);

            await this._orgMemberRoleRepo.InsertBulkAsync(model.Select(x => x.InitEntity()));
             */
            await this._orgRepo.SaveOrgMemberRole(org_uid, user_uid, model);
        }

        public async Task<IEnumerable<OrgEntity>> LoadOwners(IEnumerable<OrgEntity> list)
        {
            list.Should().NotBeNull();

            var res = await this._orgRepo.LoadOwners(list);

            return res;
            /*
             
            if (ValidateHelper.IsNotEmpty(list))
            {
                var org_uids = list.Select(x => x.Id).ToArray();

                var db = this._orgRepo.Database;

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
             */
        }
    }
}
