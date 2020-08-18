using FluentAssertions;
using Lib.core;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Application.Login;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.User;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.Application.Service.impl
{
    public class UserLoginService : IUserLoginService
    {
        private readonly IWCloudContext _context;
        protected readonly IPasswordHelper _passHelper;
        protected readonly IMobilePhoneFormatter _mobileFormatter;
        protected readonly IUserAccountRepository userAccountRepository;

        protected UserLoginService(
            IWCloudContext<UserLoginService> _context,
            IPasswordHelper passHelper,
            IMobilePhoneFormatter _mobileFormatter,
            IUserAccountRepository _userRepo)
        {
            this._context = _context;
            this._passHelper = passHelper;
            this._mobileFormatter = _mobileFormatter;
            this.userAccountRepository = _userRepo;
        }

        public virtual async Task<_<UserEntity>> ValidUserPassword(string user_name, string password)
        {
            user_name.Should().NotBeNullOrEmpty();
            password.Should().NotBeNullOrEmpty();

            var data = new _<UserEntity>();

            var user_model = await this.GetUserByUserName(user_name);
            if (user_model == null)
            {
                data.SetErrorMsg("用户不存在");
                return data;
            }
            if (user_model.PassWord != this._passHelper.Encrypt(password))
            {
                data.SetErrorMsg("密码错误");
                return data;
            }

            data.SetSuccessData(user_model);
            return data;
        }

        bool __valid_user_name__(string name, out string msg)
        {
            msg = string.Empty;

            name.Should().NotBeNullOrEmpty();

            var chars = Com.Range((int)'a', (int)'z').Concat(Com.Range((int)'A', (int)'Z')).Select(x => (char)x).ToList();
            chars.AddRange(Com.Range(0, 10).Select(x => x.ToString()[0]));
            chars.AddRange(new[] { '_', '-' });

            if (!name.All(x => chars.Contains(x)))
            {
                msg = "用户名包含非法字符";
                return false;
            }
            return true;
        }

        public virtual async Task<_<UserEntity>> AddAccount(UserEntity model, string specific_uid = null)
        {
            model.Should().NotBeNull();
            model.UserName.Should().NotBeNullOrEmpty();

            var data = new _<UserEntity>();

            model.InitEntity();
            if (ValidateHelper.IsNotEmpty(model.PassWord))
            {
                model.PassWord = this._passHelper.Encrypt(model.PassWord);
            }
            if (!this.__valid_user_name__(model.UserName, out var msg))
            {
                return data.SetErrorMsg(msg);
            }
            //检查用户名是否存在
            if (await this.IsUserNameExist(model.UserName))
            {
                return data.SetErrorMsg("用户名已存在");
            }

            if (ValidateHelper.IsNotEmpty(specific_uid))
            {
                model.SetId(specific_uid);
            }

            await this.userAccountRepository.InsertAsync(model);
            data.SetSuccessData(model);
            return data;
        }

        public virtual async Task SetPassword(string uid, string pwd)
        {
            uid.Should().NotBeNullOrEmpty();
            pwd.Should().NotBeNullOrEmpty();

            var user = await this.userAccountRepository.QueryOneAsync(x => x.Id == uid);
            user.Should().NotBeNull("用户不存在，无法修改密码");

            user.PassWord = this._passHelper.Encrypt(pwd);
            user.SetUpdateTime();
            user.LastPasswordUpdateTimeUtc = user.UpdateTimeUtc;

            await this.userAccountRepository.UpdateAsync(user);
        }

        public virtual async Task ActiveOrDeActiveUser(string uid, bool active)
        {
            uid.Should().NotBeNullOrEmpty();

            var uids = new[] { uid };

            if (active)
            {
                await this.userAccountRepository.RecoverByIdAsync(uids);
            }
            else
            {
                await this.userAccountRepository.RemoveByIdAsync(uids);
            }
        }

        public virtual async Task<UserEntity> GetUserByUID(string uid)
        {
            uid.Should().NotBeNullOrEmpty();

            var res = await this.userAccountRepository.QueryOneAsync(x => x.Id == uid);
            return res;
        }

        public virtual async Task<UserEntity> GetUserByUserName(string user_name)
        {
            user_name.Should().NotBeNullOrEmpty();

            user_name = this._mobileFormatter.Format(user_name);

            var res = await this.userAccountRepository.QueryManyAsync(x => x.UserName == user_name, 2);
            if (res.Count > 1)
            {
                throw new MsgException("找到多个用户，无法登陆");
            }

            return res.FirstOrDefault();
        }

        public virtual string EncryptPassword(string pwd)
        {
            var res = this._passHelper.Encrypt(pwd);
            return res;
        }

        public virtual async Task<bool> IsUserNameExist(string user_name)
        {
            user_name.Should().NotBeNullOrEmpty();
            var res = await this.userAccountRepository.IsUserNameExist(user_name);
            return res;
        }

        public async Task SetUserName(string uid, string user_name)
        {
            uid.Should().NotBeNullOrEmpty();
            user_name.Should().NotBeNullOrEmpty();


            var user = await this.userAccountRepository.QueryOneAsync(x => x.Id == uid);

            user.Should().NotBeNull();

            if (user.UserName == user_name)
            {
                return;
            }

            if (await this.IsUserNameExist(user_name))
            {
                throw new MsgException("用户名已存在");
            }

            user.UserName = user_name;
            await this.userAccountRepository.UpdateAsync(user);
        }

        public async Task<IEnumerable<UserPhoneEntity>> GetUserPhone(string user_uid)
        {
            user_uid.Should().NotBeNullOrEmpty("get user phone useruid");

            var res = await this.userAccountRepository.GetUserPhone(user_uid);

            return res;
        }

        public virtual async Task AddVadlidationCode(ValidationCodeEntity model)
        {
            model.Should().NotBeNull("add validation code model");

            model.InitEntity();

            await this.userAccountRepository.AddVadlidationCode(model);
        }

        public async Task<UserEntity> GetUserByPhone(string phone)
        {
            phone.Should().NotBeNullOrEmpty("get user by phone,phone");

            var res = await this.userAccountRepository.GetUserByPhone(phone);

            return res;
        }

        public async Task<ExternalLoginMapEntity> FindExternalLoginByOpenID(string provider, string openid)
        {
            provider.Should().NotBeNullOrEmpty("find external login by open id provider");
            openid.Should().NotBeNullOrEmpty("find external login by open id openid");

            /*
             var db = this.userAccountRepository.Database;

            var map = await db.Set<ExternalLoginMapEntity>().AsNoTracking()
                .Where(x => x.ProviderKey == provider && x.OpenID == openid)
                .OrderByDescending(x => x.CreateTimeUtc).Take(1).FirstOrDefaultAsync();
             */
            var map = await this.userAccountRepository.FindExternalLoginByOpenID(provider, openid);

            return map;
        }

        public async Task<_<ExternalLoginMapEntity>> SaveExternalProviderMapping(ExternalLoginMapEntity model)
        {
            model.Should().NotBeNull("save external provider mapping model");
            model.ProviderKey.Should().NotBeNullOrEmpty("save external provider mapping provder key");
            model.UserID.Should().NotBeNullOrEmpty("save external provider mapping user uid");

            var res = await this.userAccountRepository.SaveExternalProviderMapping(model);

            return res;
        }

        public async Task<ExternalLoginMapEntity> FindExternalLoginByUserID(string provider, string user_uid)
        {
            provider.Should().NotBeNullOrEmpty("find external login by user uid provider");
            user_uid.Should().NotBeNullOrEmpty("find external login by user uid user uid");

            /*
             var db = this.userAccountRepository.Database;

            var map = await db.Set<ExternalLoginMapEntity>().AsNoTracking()
                .Where(x => x.ProviderKey == provider && x.UserID == user_uid)
                .OrderByDescending(x => x.CreateTimeUtc)
                .Take(1).FirstOrDefaultAsync();
             */
            var map = await this.userAccountRepository.FindExternalLoginByUserID(provider, user_uid);

            return map;
        }

        public async Task RemoveExternalLogin(string user_uid, string[] provider)
        {
            user_uid.Should().NotBeNullOrEmpty("remove external login user uid");
            provider.Should().NotBeNullOrEmpty("remove external login provider");

            await this.userAccountRepository.RemoveExternalLogin(user_uid, provider);
        }

        public async Task<bool> IsPhoneExist(string phone)
        {
            phone.Should().NotBeNullOrEmpty("is phone exist phone");

            var res = await this.userAccountRepository.IsPhoneExist(phone);

            /*
            var query = this.userAccountRepository.Database.Set<UserPhoneEntity>().AsNoTracking();
            var res = await query.AnyAsync(x => x.Phone == phone);
             */

            return res;
        }

        public async Task<_<UserPhoneEntity>> SetPhone(string uid, string phone)
        {
            uid.Should().NotBeNullOrEmpty("set phone uid");
            phone.Should().NotBeNullOrEmpty("set phone phone");

            var res = await this.userAccountRepository.SetPhone(uid, phone);
            return res;
        }

        public async Task<ValidationCodeEntity> GetValidationCode(Expression<Func<ValidationCodeEntity, bool>> where)
        {
            var res = await this.userAccountRepository.GetValidationCode(where);
            return res;
        }
    }
}
