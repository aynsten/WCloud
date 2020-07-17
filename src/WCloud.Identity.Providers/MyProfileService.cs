using IdentityServer4.Models;
using IdentityServer4.Services;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication;

namespace WCloud.Identity.Providers
{
    /// <summary>
    ///  Profile就是用户资料，ids 4里面定义了一个IProfileService的接口用来获取用户的一些信息
    ///  ，主要是为当前的认证上下文绑定claims。我们可以实现IProfileService从外部创建claim扩展到ids4里面。
    ///  然后返回
    /// </summary>
    public class MyProfileService : IProfileService
    {
        private readonly ILogger _logger;
        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager _keyManager;
        private readonly IUserService _userService;

        public MyProfileService(
            ILogger<MyProfileService> logger,
            ICacheProvider cache,
            ICacheKeyManager keyManager,
            IUserService userService)
        {
            this._logger = logger;
            this._cache = cache;
            this._keyManager = keyManager;
            this._userService = userService;
        }

        /// <summary>
        /// 获取用户Claims
        /// 用户请求userinfo endpoint时会触发该方法
        /// http://localhost:5003/connect/userinfo
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await Task.CompletedTask;
            try
            {
                //depending on the scope accessing the user data.
                var claims = context.Subject.Claims.ToList();

                var account_type = claims.GetAccountType();

                //set issued claims to return
                context.IssuedClaims = claims.ToList();
            }
            catch (Exception e)
            {
                this._logger.AddErrorLog(e.Message, e);
            }
        }

        /// <summary>
        /// 判断用户是否可用
        /// Identity Server会确定用户是否有效
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            try
            {
                var user_uid = context.Subject?.GetSubjectID();
                if (ValidateHelper.IsEmpty(user_uid))
                {
                    throw new MsgException("subject找不到");
                }

                context.IsActive = true;
                await Task.CompletedTask;
            }
            catch (MsgException)
            {
                context.IsActive = false;
            }
            catch (Exception e)
            {
                this._logger.AddErrorLog(e.Message, e);
                context.IsActive = false;
            }
        }
    }
}
