using WCloud.Core.Cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Model;
using WCloud.Framework.MVC.Middleware;
using WCloud.Member.Authentication.Org;
using WCloud.Member.Authentication.OrgSelector;
using WCloud.Member.Authentication.User;
using WCloud.Member.Shared.Org;
using WCloud.Member.Shared.User;

namespace WCloud.Member.Authentication.Midddlewares
{
    /// <summary>
    /// 预加载登陆信息和租户信息
    /// </summary>
    public class UserAuthenticationMiddleware : MiddlewareBase
    {
        /// <summary>
        /// 不要修改结构
        /// </summary>
        class LoginDataWrapper
        {
            public UserDto User { get; set; }
            public OrgMemberDto OrgMember { get; set; }
        }

        public UserAuthenticationMiddleware(RequestDelegate next) : base(next) { }

        async Task<LoginDataWrapper> __load_login_data__(IServiceProvider provider, string subject_id, DateTime login_time)
        {
            var res = new LoginDataWrapper();
            var userLoginService = provider.Resolve_<IUserAuthService>();

            var user_model = await userLoginService.GetUserByUID(subject_id);
            if (user_model == null)
            {
                throw new MsgException("no user found in database");
            }

            if (user_model.LastPasswordUpdateTimeUtc != null && user_model.LastPasswordUpdateTimeUtc.Value > login_time)
            {
                throw new MsgException("password has been changed,pls relogin");
            }

            res.User = user_model;

            var org_selector = provider.Resolve_<ICurrentOrgSelector>();
            var org_service = provider.Resolve_<IOrgAuthService>();

            var my_orgs = await org_service.GetMyOrgMap(subject_id);

            var selected_org_uid = org_selector.GetSelectedOrgUID();
            var selected_org = my_orgs
                .OrderByDescending(x => x.OrgUID == selected_org_uid ? 1 : 0)
                .ThenByDescending(x => x.CreateTimeUtc).FirstOrDefault();
            res.OrgMember = selected_org;

            return res;
        }

        public override async Task Invoke(HttpContext context)
        {
            var provider = context.RequestServices;
            var __context = provider.Resolve_<IWCloudContext<UserAuthenticationMiddleware>>();
            try
            {
                if (!context.__login_required__())
                {
                    throw new MsgException("不需要登陆");
                }
                var claims = context.User?.Claims ?? new Claim[] { };
                var subject_id = claims.GetSubjectID();
                var login_type = claims.GetAccountType();
                var login_time = claims.GetCreateTimeUtc(__context.DataSerializer);

                if (ValidateHelper.IsEmpty(subject_id))
                {
                    throw new MsgException("subject id is not found");
                }
                if (login_type != "user")
                {
                    throw new MsgException("account type is not user");
                }
                if (login_time == null)
                {
                    throw new MsgException("login time is not availabe");
                }

                var key = __context.CacheKeyManager.UserLoginInfo(subject_id);

                var data = await __context.CacheProvider.GetOrSetAsync_(key,
                    () => this.__load_login_data__(provider, subject_id, login_time.Value),
                    expire: TimeSpan.FromMinutes(10),
                    cache_when: x => x != null);

                if (data?.User == null)
                    throw new MsgException("缓存读取登录信息不存在");

                var user_model = data.User;

                __context.CurrentUserInfo.UserID = user_model.Id;
                __context.CurrentUserInfo.NickName = user_model.NickName;
                __context.CurrentUserInfo.UserName = user_model.NickName;
                __context.CurrentUserInfo.UserImg = user_model.UserImg;

                var selected_org = data.OrgMember;
                if (selected_org != null)
                {
                    __context.CurrentUserInfo.Org ??= new OrgInfo();
                    __context.CurrentUserInfo.Org.Id = selected_org.OrgUID;
                    __context.CurrentUserInfo.Org.IsOwner = selected_org.IsOwner > 0;
                }
            }
            catch (MsgException e)
            {
#if DEBUG
                __context.Logger.LogDebug(e.Message);
#endif
            }
            catch (Exception e)
            {
                __context.Logger.AddErrorLog("在中间件中加载登陆用户抛出异常", e);
            }
            finally
            {
                //不管是否加载成功都放行
                await this._next.Invoke(context);
            }
        }
    }
}
