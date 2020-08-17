using FluentAssertions;
using Lib.extension;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.Abstractions.Service;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Application.Service.impl
{
    public class UserService : BasicService<UserEntity>, IUserService
    {
        private readonly IWCloudContext _context;
        private readonly IUserRepository _userRepo;

        public UserService(
            IServiceProvider provider,
            IUserRepository _userRepo) : base(provider, _userRepo)
        {
            this._userRepo = _userRepo;
        }

        public async Task<IEnumerable<UserEntity>> LoadUserPhone(IEnumerable<UserEntity> data)
        {
            data.Should().NotBeNull();
            if (data.Any())
            {
                var uids = data.Select(x => x.Id).ToArray();

                var phones = this._userRepo.UserPhoneQueryable.Where(x => uids.Contains(x.UserUID)).ToArray();

                foreach (var m in data)
                {
                    m.UserPhone = phones.FirstOrDefault(x => x.UserUID == m.Id);
                }
            }
            return await Task.FromResult(data);
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

        async Task<IQueryable<UserEntity>> __build_query__(IQueryable<UserEntity> query, string keyword)
        {
            var uids_ = this._userRepo.UserPhoneQueryable
                .Where(x => x.Phone == keyword).Select(x => x.UserUID).Take(1).ToArray();

            query = query.Where(x => x.UserName == keyword || x.NickName == keyword || uids_.Contains(x.Id));

            await Task.CompletedTask;

            return query;
        }

        public async Task<IEnumerable<UserEntity>> GetTopMatchedUsers(string keyword, int max_count)
        {
            keyword.Should().NotBeNullOrEmpty();

            var query = this._userRepo.Queryable;

            query = await this.__build_query__(query, keyword);

            var res = query.Take(max_count).ToArray();

            return res;
        }

        public async Task<PagerData<UserEntity>> QueryUserList(
            string keyword = null,
            bool? isremove = null, int page = 1, int pagesize = 20)
        {
            page.Should().BeGreaterOrEqualTo(1);
            pagesize.Should().BeInRange(1, 100);

            var query = this._userRepo.Queryable;

            if (ValidateHelper.IsNotEmpty(keyword))
            {
                query = await this.__build_query__(query, keyword);
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
