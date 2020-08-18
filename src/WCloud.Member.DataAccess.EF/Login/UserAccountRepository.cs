using FluentAssertions;
using Lib.extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;

namespace WCloud.Member.DataAccess.EF.Login
{
    public class UserAccountRepository : MemberShipRepository<UserEntity>, IUserAccountRepository
    {
        private readonly IWCloudContext _context;
        public UserAccountRepository(IWCloudContext<UserAccountRepository> _context) : base(_context.Provider)
        {
            this._context = _context;
        }

        public async Task AddVadlidationCode(ValidationCodeEntity model)
        {
            var db = this.Database;
            db.Set<ValidationCodeEntity>().Add(model);
            await db.SaveChangesAsync();
        }

        public async Task<ExternalLoginMapEntity> FindExternalLoginByOpenID(string provider, string openid)
        {
            var db = this.Database;
            var query = db.Set<ExternalLoginMapEntity>().Where(x => x.ProviderKey == provider && x.OpenID == openid);
            var res = await query.FirstOrDefaultAsync();
            return res;
        }

        public async Task<ExternalLoginMapEntity> FindExternalLoginByUserID(string provider, string user_uid)
        {
            var db = this.Database;
            var query = db.Set<ExternalLoginMapEntity>().Where(x => x.ProviderKey == provider && x.UserID == user_uid);
            var res = await query.FirstOrDefaultAsync();
            return res;
        }

        public async Task<UserEntity> GetUserByPhone(string phone)
        {
            var db = this.Database;

            var map = await db.Set<UserPhoneEntity>().Where(x => x.Phone == phone).FirstOrDefaultAsync();

            if (map == null)
            {
                return null;
            }

            var res = await db.Set<UserEntity>().Where(x => x.Id == map.UserUID).FirstOrDefaultAsync();
            return res;
        }

        public async Task<IEnumerable<UserPhoneEntity>> GetUserPhone(string user_uid)
        {
            var db = this.Database;

            var res = await db.Set<UserPhoneEntity>().Where(x => x.UserUID == user_uid).ToArrayAsync();

            return res;
        }

        public Task<ValidationCodeEntity> GetValidationCode(Expression<Func<ValidationCodeEntity, bool>> where)
        {
            var db = this.Database;

            var res = db.Set<ValidationCodeEntity>().Where(where).OrderByDescending(x => x.CreateTimeUtc).FirstOrDefaultAsync();

            return res;
        }

        public async Task<bool> IsPhoneExist(string phone)
        {
            var db = this.Database;

            var res = await db.Set<UserPhoneEntity>().IgnoreQueryFilters().Where(x => x.Phone == phone).AnyAsync();

            return res;
        }

        public async Task<bool> IsUserNameExist(string user_name)
        {
            var db = this.Database;

            var res = await this.NoTrackingQueryable.IgnoreQueryFilters().Where(x => x.UserName == user_name).AnyAsync();

            return res;
        }

        public async Task RecoverByIdAsync(string[] uids)
        {
            await RepositoryExtension.RecoverByIdAsync(this, uids);
        }

        public async Task RemoveByIdAsync(string[] uids)
        {
            await RepositoryExtension.RemoveByIdAsync(this, uids);
        }

        public async Task RemoveExternalLogin(string user_uid, string[] provider)
        {
            var db = this.Database;

            var set = db.Set<ExternalLoginMapEntity>();

            set.RemoveRange(set.Where(x => x.UserID == user_uid && provider.Contains(x.ProviderKey)));

            await db.SaveChangesAsync();
        }

        public async Task<_<ExternalLoginMapEntity>> SaveExternalProviderMapping(ExternalLoginMapEntity model)
        {
            var res = new _<ExternalLoginMapEntity>();

            model.InitEntity();
            if (!model.IsValid(out var msg))
            {
                return res.SetErrorMsg(msg);
            }

            var db = this.Database;
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

        public async Task<_<UserPhoneEntity>> SetPhone(string uid, string phone)
        {
            var res = new _<UserPhoneEntity>();

            if (!this.__valid_phone__(phone, out var msg))
            {
                return res.SetErrorMsg(msg);
            }

            if (await this.IsPhoneExist(phone))
            {
                return res.SetErrorMsg("手机号已存在");
            }

            var db = this.Database;

            var set = db.Set<UserPhoneEntity>();

            var data = await set.Where(x => x.UserUID == uid).ToArrayAsync();
            if (data.Any())
            {
                set.RemoveRange(data);
            }

            var map = new UserPhoneEntity()
            {
                UserUID = uid,
                Phone = phone
            };

            map.InitEntity();

            set.Add(map);

            await db.SaveChangesAsync();

            return res.SetSuccessData(map);
        }
    }
}
