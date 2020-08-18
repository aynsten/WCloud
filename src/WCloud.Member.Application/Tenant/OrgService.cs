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
        }

        public virtual async Task<_<OrgMemberEntity>> AddMember(OrgMemberEntity model)
        {
            model.Should().NotBeNull();
            model.OrgUID.Should().NotBeNullOrEmpty();
            model.UserUID.Should().NotBeNullOrEmpty();

            var res = await this._orgRepo.AddMember(model);

            return res;
        }

        public virtual async Task<_<OrgMemberEntity>> RemoveOwner(string org_uid, string user_uid)
        {
            org_uid.Should().NotBeNullOrEmpty();
            user_uid.Should().NotBeNullOrEmpty();

            var res = await this._orgRepo.RemoveOwner(org_uid, user_uid);

            return res;
        }

        public async Task UpdateMemberStatus(string org_uid, string user_uid, bool active)
        {
            org_uid.Should().NotBeNullOrEmpty();
            user_uid.Should().NotBeNullOrEmpty();

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
        }

        public async Task<List<UserEntity>> GetMembersByRole(string org_uid, string role_uid)
        {
            var res = await this._orgRepo.GetMembersByRole(org_uid, role_uid);
            return res;
        }

        public async Task<List<UserEntity>> LoadOrgRoles(string org_uid, List<UserEntity> list)
        {
            org_uid.Should().NotBeNullOrEmpty();
            list.Should().NotBeNull();

            var res = await this._orgRepo.LoadOrgRoles(org_uid, list);

            return res;
        }

        public async Task UpdateOrgMemberCount(string org_uid)
        {
            org_uid.Should().NotBeNullOrEmpty();

            await this._orgRepo.UpdateOrgMemberCount(org_uid);
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

            await this._orgRepo.SaveOrgMemberRole(org_uid, user_uid, model);
        }

        public async Task<IEnumerable<OrgEntity>> LoadOwners(IEnumerable<OrgEntity> list)
        {
            list.Should().NotBeNull();

            var res = await this._orgRepo.LoadOwners(list);

            return res;
        }
    }
}
