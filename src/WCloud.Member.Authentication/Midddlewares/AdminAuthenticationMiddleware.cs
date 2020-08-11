using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;
using WCloud.Framework.Middleware;
using WCloud.Member.Application.Login;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Authentication.Midddlewares
{
    public class AdminAuthenticationMiddleware : _BaseMiddleware
    {
        /// <summary>
        /// 不要修改结构
        /// </summary>
        class LoginDataWrapper
        {
            public AdminEntity User { get; set; }
        }
        public AdminAuthenticationMiddleware(RequestDelegate next) : base(next)
        {
        }

        async Task<LoginDataWrapper> __load_login_data__(IServiceProvider provider, string subject_id, DateTime login_time)
        {
            var res = new LoginDataWrapper();
            var userLoginService = provider.Resolve_<IAdminLoginService>();

            var user_model = await userLoginService.GetUserByUID(subject_id);
            if (user_model == null)
                throw new MsgException("no user found in database");

            if (user_model.LastPasswordUpdateTimeUtc != null && user_model.LastPasswordUpdateTimeUtc.Value > login_time)
                throw new MsgException("password has been changed,pls relogin");

            res.User = user_model;

            return res;
        }

        bool __login_required__(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint == null)
                return false;
            var allow_anonymous = endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>();
            if (allow_anonymous != null)
                return false;
            return true;
        }

        public override async Task Invoke(HttpContext context)
        {
            var provider = context.RequestServices;
            var logger = provider.Resolve_<ILogger<AdminAuthenticationMiddleware>>();
            try
            {
                if (!this.__login_required__(context))
                    throw new MsgException("不需要登陆");
                var claims = context.User?.Claims ?? new Claim[] { };
                var subject_id = claims.GetSubjectID();
                var login_type = claims.GetAccountType();
                var login_time = claims.GetCreateTimeUtc();

                if (ValidateHelper.IsEmpty(subject_id))
                    throw new MsgException("subject id is not found");
                if (login_type != "admin")
                    throw new MsgException("account type is not user");
                if (login_time == null)
                    throw new MsgException("login time is not availabe");

                var user = provider.Resolve_<WCloudAdminInfo>();
                var cacheProvider = provider.ResolveDistributedCache_();
                var cacheKeyPrvoder = provider.Resolve_<ICacheKeyManager>();

                var key = cacheKeyPrvoder.AdminLoginInfo(subject_id);

                var data = await cacheProvider.GetOrSetAsync_(key,
                    () => this.__load_login_data__(provider, subject_id, login_time.Value),
                    expire: TimeSpan.FromMinutes(10),
                    cache_when: x => x != null && x.User != null);

                data.Should().NotBeNull(nameof(LoginDataWrapper));

                var user_model = data.User;

                user.UserID = user_model.UID;
                user.NickName = user_model.NickName;
                user.UserName = user_model.NickName;
                user.UserImg = user_model.UserImg;
            }
            catch (MsgException e)
            {
#if DEBUG
                logger.LogDebug(e.Message);
#endif
            }
            catch (Exception e)
            {
                logger.AddErrorLog("在中间件中加载登陆用户抛出异常", e);
            }
            finally
            {
                //不管是否加载成功都放行
                await this._next.Invoke(context);
            }
        }
    }
}
