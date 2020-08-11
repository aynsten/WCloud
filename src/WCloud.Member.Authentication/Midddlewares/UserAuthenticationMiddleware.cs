using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;
using WCloud.Framework.Middleware;
using WCloud.Member.Application.Login;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.OrgSelector;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Authentication.Midddlewares
{
    /// <summary>
    /// 预加载登陆信息和租户信息
    /// </summary>
    public class UserAuthenticationMiddleware : _BaseMiddleware
    {
        /// <summary>
        /// 不要修改结构
        /// </summary>
        class LoginDataWrapper
        {
            public UserEntity User { get; set; }
            public OrgMemberEntity OrgMember { get; set; }
        }

        public UserAuthenticationMiddleware(RequestDelegate next) : base(next) { }

        async Task<LoginDataWrapper> __load_login_data__(IServiceProvider provider, string subject_id, DateTime login_time)
        {
            var res = new LoginDataWrapper();
            var userLoginService = provider.Resolve_<IUserLoginService>();

            var user_model = await userLoginService.GetUserByUID(subject_id);
            if (user_model == null)
                throw new MsgException("no user found in database");

            if (user_model.LastPasswordUpdateTimeUtc != null && user_model.LastPasswordUpdateTimeUtc.Value > login_time)
                throw new MsgException("password has been changed,pls relogin");

            res.User = user_model;

            var org_selector = provider.Resolve_<ICurrentOrgSelector>();
            var org_service = provider.Resolve_<IOrgService>();

            var my_orgs = await org_service.GetMyOrgMap(subject_id);

            var selected_org_uid = org_selector.GetSelectedOrgUID();
            var selected_org = my_orgs.OrderByDescending(x => x.OrgUID == selected_org_uid ? 1 : 0).FirstOrDefault();
            res.OrgMember = selected_org;

            return res;
        }

        public override async Task Invoke(HttpContext context)
        {
            var provider = context.RequestServices;
            var logger = provider.Resolve_<ILogger<UserAuthenticationMiddleware>>();
            try
            {
                if (!context.__login_required__())
                    throw new MsgException("不需要登陆");
                var claims = context.User?.Claims ?? new Claim[] { };
                var subject_id = claims.GetSubjectID();
                var login_type = claims.GetAccountType();
                var login_time = claims.GetCreateTimeUtc();

                if (ValidateHelper.IsEmpty(subject_id))
                    throw new MsgException("subject id is not found");
                if (login_type != "user")
                    throw new MsgException("account type is not user");
                if (login_time == null)
                    throw new MsgException("login time is not availabe");

                var user = provider.Resolve_<WCloudUserInfo>();
                var cacheProvider = provider.ResolveDistributedCache_();
                var cacheKeyPrvoder = provider.Resolve_<ICacheKeyManager>();

                var key = cacheKeyPrvoder.UserLoginInfo(subject_id);

                var data = await cacheProvider.GetOrSetAsync_(key,
                    () => this.__load_login_data__(provider, subject_id, login_time.Value),
                    expire: TimeSpan.FromMinutes(10),
                    cache_when: x => x != null && x.User != null);

                data.Should().NotBeNull(nameof(LoginDataWrapper));
                data.User.Should().NotBeNull(nameof(UserEntity));

                var user_model = data.User;

                user.UserID = user_model.Id;
                user.NickName = user_model.NickName;
                user.UserName = user_model.NickName;
                user.UserImg = user_model.UserImg;

                var selected_org = data.OrgMember;
                if (selected_org != null)
                {
                    user.Org ??= new OrgInfo();
                    user.Org.Id = selected_org.OrgUID;
                    user.Org.IsOwner = selected_org.IsOwner > 0;
                }
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
