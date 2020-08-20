using WCloud.Core.Cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MVC.Middleware;
using WCloud.Member.Authentication.Admin;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.Authentication.Midddlewares
{
    public class AdminAuthenticationMiddleware : MiddlewareBase
    {
        public AdminAuthenticationMiddleware(RequestDelegate next) : base(next)
        {
            //
        }

        async Task<_<AdminDto>> __load_login_data__(IServiceProvider provider, string subject_id, DateTime login_time)
        {
            var res = new _<AdminDto>();
            var userLoginService = provider.Resolve_<IAdminAuthService>();

            var user_model = await userLoginService.GetAdminLoginInfoById(subject_id);
            if (user_model == null)
            {
                throw new MsgException("no user found in database");
            }

            if (user_model.LastPasswordUpdateTimeUtc != null && user_model.LastPasswordUpdateTimeUtc.Value > login_time)
            {
                throw new MsgException("password has been changed,pls relogin");
            }

            res.SetSuccessData(user_model);

            return res;
        }

        public override async Task Invoke(HttpContext context)
        {
            var provider = context.RequestServices;
            var __context = provider.Resolve_<IWCloudContext<AdminAuthenticationMiddleware>>();
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
                if (login_type != "admin")
                {
                    throw new MsgException("account type is not user");
                }
                if (login_time == null)
                {
                    throw new MsgException("login time is not availabe");
                }

                var key = __context.CacheKeyManager.AdminLoginInfo(subject_id);

                var data = await __context.CacheProvider.GetOrSetAsync_(key,
                    () => this.__load_login_data__(provider, subject_id, login_time.Value),
                    expire: TimeSpan.FromMinutes(10),
                    cache_when: x => x != null);

                if (data?.Data == null)
                {
                    throw new MsgException("缓存读取登录信息不存在");
                }

                var user_model = data.Data;

                __context.CurrentAdminInfo.UserID = user_model.Id;
                __context.CurrentAdminInfo.NickName = user_model.NickName;
                __context.CurrentAdminInfo.UserName = user_model.NickName;
                __context.CurrentAdminInfo.UserImg = user_model.UserImg;
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
