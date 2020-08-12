using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore.Service;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Application.Service.impl
{
    public class UserService : BasicService<UserEntity>, IUserService
    {
        private readonly IMSRepository<UserEntity> _userRepo;
        private readonly IMSRepository<AdminRoleEntity> _userRoleRepo;
        private readonly IMSRepository<RoleEntity> _roleRepo;

        public UserService(
            IServiceProvider provider,
            IMSRepository<UserEntity> _userRepo,
            IMSRepository<AdminRoleEntity> _userRoleRepo,
            IMSRepository<RoleEntity> _roleRepo) : base(provider, _userRepo)
        {
            this._userRepo = _userRepo;
            this._userRoleRepo = _userRoleRepo;
            this._roleRepo = _roleRepo;
        }

        public async Task<IEnumerable<UserEntity>> LoadUserPhone(IEnumerable<UserEntity> data)
        {
            data.Should().NotBeNull();
            if (data.Any())
            {
                var uids = data.Select(x => x.Id).ToArray();

                var phones = await this._userRepo.Database.Set<UserPhoneEntity>().AsNoTracking().Where(x => uids.Contains(x.UserUID)).ToArrayAsync();

                foreach (var m in data)
                {
                    m.UserPhone = phones.FirstOrDefault(x => x.UserUID == m.Id);
                }
            }
            return data;
        }

        public virtual async Task<_<UserEntity>> UpdateUser(UserEntity model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            var data = new _<UserEntity>();

            var user = await this._userRepo.QueryOneAsync(x => x.Id == model.Id);
            user.Should().NotBeNull($"用户不存在:{model.Id}");

            user.SetField(new
            {
                model.UserImg,
                model.UserSex,
                model.NickName
            });

            user.SetUpdateTime();

            if (!user.IsValid(out var msg))
            {
                data.SetErrorMsg(msg);
                return data;
            }

            await this._userRepo.UpdateAsync(user);
            data.SetSuccessData(user);
            return data;
        }

        public virtual async Task<_<UserEntity>> SetIdCard(string user_uid, string idcard, string real_name)
        {
            user_uid.Should().NotBeNullOrEmpty();
            idcard.Should().NotBeNullOrEmpty();
            real_name.Should().NotBeNullOrEmpty();

            var data = new _<UserEntity>();

            var user = await this._userRepo.QueryOneAsync(x => x.Id == user_uid);
            user.Should().NotBeNull($"用户不存在:{user_uid}");

            user.IdCard = idcard;
            user.RealName = real_name;
            user.IdCardConfirmed = 1;

            user.SetUpdateTime();

            await this._userRepo.UpdateAsync(user);
            data.SetSuccessData(user);
            return data;
        }

        public async Task<_<UserEntity>> UpdateUserAvatar(string user_uid, string avatar_url)
        {
            user_uid.Should().NotBeNullOrEmpty();

            var data = new _<UserEntity>();

            var user = await this._userRepo.QueryOneAsync(x => x.Id == user_uid);
            user.Should().NotBeNull($"用户不存在:{user_uid}");

            user.UserImg = avatar_url;

            user.SetUpdateTime();

            await this._userRepo.UpdateAsync(user);
            data.SetSuccessData(user);
            return data;
        }

        public async Task<UserEntity> GetUserByUID(string uid)
        {
            uid.Should().NotBeNullOrEmpty();

            var res = await this._userRepo.QueryOneAsync(x => x.Id == uid);
            return res;
        }

        async Task<IQueryable<UserEntity>> __build_query__(IQueryable<UserEntity> query, string keyword, DbContext db)
        {
            var uids_ = await db.Set<UserPhoneEntity>().AsNoTracking()
                .Where(x => x.Phone == keyword).Select(x => x.UserUID).Take(1).ToArrayAsync();

            query = query.Where(x => x.UserName == keyword || x.NickName == keyword || uids_.Contains(x.Id));

            return query;
        }

        public async Task<IEnumerable<UserEntity>> GetTopMatchedUsers(string keyword, int max_count)
        {
            keyword.Should().NotBeNullOrEmpty();
            var db = this._userRepo.Database;

            var query = db.Set<UserEntity>().AsNoTracking();

            query = await this.__build_query__(query, keyword, db);

            var res = await query.Take(max_count).ToArrayAsync();

            return res;
        }

        public async Task<PagerData<UserEntity>> QueryUserList(
            string keyword = null,
            bool? isremove = null, int page = 1, int pagesize = 20)
        {
            page.Should().BeGreaterOrEqualTo(1);
            pagesize.Should().BeInRange(1, 100);

            var db = this._userRepo.Database;

            var query = db.Set<UserEntity>().IgnoreQueryFilters().AsNoTracking();

            if (ValidateHelper.IsNotEmpty(keyword))
            {
                query = await this.__build_query__(query, keyword, db);
            }

            if (isremove != null)
            {
                if (isremove.Value)
                {
                    query = query.Where(x => x.IsDeleted > 0);
                }
                else
                {
                    query = query.Where(x => x.IsDeleted <= 0);
                }
            }

            var data = await query.ToPagedListAsync(page, pagesize, x => x.CreateTimeUtc, desc: false);

            return data;
        }

        public async Task<UserEntity> GetUserByUserName(string name)
        {
            name.Should().NotBeNullOrEmpty();

            var res = await this._userRepo.QueryOneAsync(x => x.UserName == name);
            return res;
        }

        protected override object UpdateField(UserEntity data)
        {
            return new
            {
                data.UserSex,
                data.NickName,
                data.UserImg,
            };
        }
    }
}
