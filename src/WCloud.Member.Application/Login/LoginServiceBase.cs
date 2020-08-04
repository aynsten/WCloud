using FluentAssertions;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.EntityFrameworkCore.Repository;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.Application.Login
{
    public abstract class LoginServiceBase<T> : ILoginService<T> where T : EntityBase, ILoginEntity, IMemberShipDBTable
    {
        protected readonly ILogger logger;
        protected readonly IPasswordHelper _passHelper;
        protected readonly IMobilePhoneFormatter _mobileFormatter;
        protected readonly IMSRepository<T> _userRepo;

        protected LoginServiceBase(
            IServiceProvider provider,
            IPasswordHelper passHelper,
            IMobilePhoneFormatter _mobileFormatter,
            IMSRepository<T> _userRepo)
        {
            this.logger = provider.Resolve_<ILogger<LoginServiceBase<T>>>();
            this._passHelper = passHelper;
            this._mobileFormatter = _mobileFormatter;
            this._userRepo = _userRepo;
        }

        public virtual async Task<_<T>> ValidUserPassword(string user_name, string password)
        {
            user_name.Should().NotBeNullOrEmpty();
            password.Should().NotBeNullOrEmpty();

            var data = new _<T>();

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

        public virtual async Task<_<T>> AddAccount(T model, string specific_uid = null)
        {
            model.Should().NotBeNull();
            model.UserName.Should().NotBeNullOrEmpty();

            var data = new _<T>();

            model.InitSelf();
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
                model.UID = specific_uid;
            }

            await this._userRepo.AddAsync(model);
            data.SetSuccessData(model);
            return data;
        }

        public virtual async Task SetPassword(string uid, string pwd)
        {
            uid.Should().NotBeNullOrEmpty();
            pwd.Should().NotBeNullOrEmpty();

            var user = await this._userRepo.GetFirstAsync(x => x.UID == uid);
            user.Should().NotBeNull("用户不存在，无法修改密码");

            user.PassWord = this._passHelper.Encrypt(pwd);
            user.Update();
            user.LastPasswordUpdateTimeUtc = user.UpdateTimeUtc;

            await this._userRepo.UpdateAsync(user);
        }

        public virtual async Task ActiveOrDeActiveUser(string uid, bool active)
        {
            uid.Should().NotBeNullOrEmpty();

            var uids = new[] { uid };

            if (active)
            {
                await this._userRepo.RecoverByUIDs(uids);
            }
            else
            {
                await this._userRepo.RemoveByUIDs(uids);
            }
        }

        public virtual async Task<T> GetUserByUID(string uid)
        {
            uid.Should().NotBeNullOrEmpty();

            var res = await this._userRepo.GetFirstAsNoTrackAsync(x => x.UID == uid);
            return res;
        }

        public virtual async Task<T> GetUserByUserName(string user_name)
        {
            user_name.Should().NotBeNullOrEmpty();

            user_name = this._mobileFormatter.Format(user_name);

            var res = await this._userRepo.NoTrackingQueryable.Where(x => x.UserName == user_name).Take(2).ToArrayAsync();
            if (res.Length > 1)
                throw new MsgException("找到多个用户，无法登陆");

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
            var query = this._userRepo.Database.Set<T>().IgnoreQueryFilters().AsNoTracking();
            var res = await query.AnyAsync(x => x.UserName == user_name);
            return res;
        }

        public async Task SetUserName(string uid, string user_name)
        {
            uid.Should().NotBeNullOrEmpty();
            user_name.Should().NotBeNullOrEmpty();

            var db = this._userRepo.Database;

            var set = db.Set<T>();

            var user = await set.FirstOrDefaultAsync(x => x.UID == uid);

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

            await db.SaveChangesAsync();
        }
    }
}
