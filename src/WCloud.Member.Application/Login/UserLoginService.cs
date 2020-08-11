using FluentAssertions;
using Lib.extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Application.Login;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.Application.Service.impl
{
    public class UserLoginService : LoginServiceBase<UserEntity>, IUserLoginService
    {
        public UserLoginService(IServiceProvider provider,
            IPasswordHelper passHelper,
            IMobilePhoneFormatter _mobileFormatter,
            IMSRepository<UserEntity> _userRepo) :
            base(provider, passHelper, _mobileFormatter, _userRepo)
        {
            //
        }

        public async Task<IEnumerable<UserPhoneEntity>> GetUserPhone(string user_uid)
        {
            user_uid.Should().NotBeNullOrEmpty("get user phone useruid");

            var res = await this._userRepo.Database.Set<UserPhoneEntity>().AsNoTracking()
                .Where(x => x.UserUID == user_uid)
                .OrderByDescending(x => x.CreateTimeUtc)
                .ToArrayAsync();

            return res;
        }

        public virtual async Task AddVadlidationCode(ValidationCodeEntity model)
        {
            model.Should().NotBeNull("add validation code model");

            model.InitSelf();

            var db = this._userRepo.Database;

            db.Set<ValidationCodeEntity>().Add(model);

            await db.SaveChangesAsync();
        }

        bool __valid_phone__(string phone, out string msg)
        {
            msg = string.Empty;

            phone.Should().NotBeNullOrEmpty("validate phone phone");

            if (phone.Length != 11)
            {
                msg = "手机号长度必须为11位";
                return false;
            }

            return true;
        }

        public async Task<UserEntity> GetUserByPhone(string phone)
        {
            phone.Should().NotBeNullOrEmpty("get user by phone,phone");

            var db = this._userRepo.Database;

            var map = await db.Set<UserPhoneEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Phone == phone);

            if (map == null)
            {
                return null;
            }

            var res = await this.GetUserByUID(map.UserUID);

            return res;
        }

        public async Task<ExternalLoginMapEntity> FindExternalLoginByOpenID(string provider, string openid)
        {
            provider.Should().NotBeNullOrEmpty("find external login by open id provider");
            openid.Should().NotBeNullOrEmpty("find external login by open id openid");

            var db = this._userRepo.Database;

            var map = await db.Set<ExternalLoginMapEntity>().AsNoTracking()
                .Where(x => x.ProviderKey == provider && x.OpenID == openid)
                .OrderByDescending(x => x.CreateTimeUtc).Take(1).FirstOrDefaultAsync();

            return map;
        }

        public async Task<_<ExternalLoginMapEntity>> SaveExternalProviderMapping(ExternalLoginMapEntity model)
        {
            model.Should().NotBeNull("save external provider mapping model");
            model.ProviderKey.Should().NotBeNullOrEmpty("save external provider mapping provder key");
            model.UserID.Should().NotBeNullOrEmpty("save external provider mapping user uid");

            var res = new _<ExternalLoginMapEntity>();

            model.InitSelf();
            if (!model.IsValid(out var msg))
            {
                return res.SetErrorMsg(msg);
            }

            var db = this._userRepo.Database;
            var table = db.Set<ExternalLoginMapEntity>();

            //删除之前的绑定
            var old = await table.Where(x =>
            x.ProviderKey == model.ProviderKey &&
            x.UserID == model.UserID &&
            x.Id != model.Id).ToArrayAsync();

            if (old.Any())
            {
                table.RemoveRange(old);
            }
            table.Add(model);

            await db.SaveChangesAsync();

            return res.SetSuccessData(model);
        }

        public async Task<ExternalLoginMapEntity> FindExternalLoginByUserID(string provider, string user_uid)
        {
            provider.Should().NotBeNullOrEmpty("find external login by user uid provider");
            user_uid.Should().NotBeNullOrEmpty("find external login by user uid user uid");

            var db = this._userRepo.Database;

            var map = await db.Set<ExternalLoginMapEntity>().AsNoTracking()
                .Where(x => x.ProviderKey == provider && x.UserID == user_uid)
                .OrderByDescending(x => x.CreateTimeUtc)
                .Take(1).FirstOrDefaultAsync();

            return map;
        }

        public async Task RemoveExternalLogin(string user_uid, string[] provider)
        {
            user_uid.Should().NotBeNullOrEmpty("remove external login user uid");
            provider.Should().NotBeNullOrEmpty("remove external login provider");

            var db = this._userRepo.Database;

            var set = db.Set<ExternalLoginMapEntity>();

            set.RemoveRange(set.Where(x => x.UserID == user_uid && provider.Contains(x.ProviderKey)));

            await db.SaveChangesAsync();
        }

        public async Task<bool> IsPhoneExist(string phone)
        {
            phone.Should().NotBeNullOrEmpty("is phone exist phone");
            var query = this._userRepo.Database.Set<UserPhoneEntity>().AsNoTracking();
            var res = await query.AnyAsync(x => x.Phone == phone);
            return res;
        }

        public async Task<_<UserPhoneEntity>> SetPhone(string uid, string phone)
        {
            uid.Should().NotBeNullOrEmpty("set phone uid");
            phone.Should().NotBeNullOrEmpty("set phone phone");

            var res = new _<UserPhoneEntity>();

            if (!this.__valid_phone__(phone, out var msg))
            {
                return res.SetErrorMsg(msg);
            }

            if (await this.IsPhoneExist(phone))
            {
                return res.SetErrorMsg("手机号已存在");
            }

            var db = this._userRepo.Database;

            var set = db.Set<UserPhoneEntity>();

            var data = await set.Where(x => x.UserUID == uid).ToArrayAsync();
            if (data.Any())
                set.RemoveRange(data);

            var map = new UserPhoneEntity()
            {
                UserUID = uid,
                Phone = phone
            };

            map.InitSelf();

            set.Add(map);

            await db.SaveChangesAsync();

            return res.SetSuccessData(map);
        }

        public async Task<ValidationCodeEntity> GetValidationCode(Expression<Func<ValidationCodeEntity, bool>> where)
        {
            var db = this._userRepo.Database;

            var data = await db.Set<ValidationCodeEntity>().AsNoTracking()
                .Where(where)
                .OrderByDescending(x => x.CreateTimeUtc)
                .FirstOrDefaultAsync();

            return data;
        }
    }
}
