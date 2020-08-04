using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Lib.helper;
using Lib.ioc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Model;
using WCloud.Core.Cache;
using WCloud.Framework.MVC.Helper;
using WCloud.Member.Application;
using WCloud.Member.Application.Service;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Authentication.Cookies
{
    public class MyCookieAuthentication : CookieAuthenticationHandler
    {
        public MyCookieAuthentication(
            IOptionsMonitor<CookieAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            //
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return base.HandleAuthenticateAsync();
        }
    }

    /// <summary>
    /// #TODO 把loginuser放到一个请求上下文的context中
    /// </summary>
    public class MyCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromMinutes(20);

        /// <summary>
        /// 找到token
        /// </summary>
        /// <param name="key_manager"></param>
        /// <param name="_cache"></param>
        /// <param name="_context"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        async Task<_<TokenModel>> FindTokenOrThrow(
            ICacheKeyManager key_manager, ICacheProvider _cache,
            HttpContext _context, string token)
        {
            var key = key_manager.AuthToken(token);
            var data = await _cache.GetOrSetAsync_(key, async () =>
            {
                var res = new _<TokenModel>();
                var _token = _context.RequestServices.Resolve_<IAuthTokenService>();

                var token_model = await _token.GetUserIdByTokenAsync(token);
                if (token_model == null)
                    return res.SetErrorMsg("token不存在");
                if (ValidateHelper.IsEmpty(token_model.UserUID))
                    return res.SetErrorMsg("用户ID为空");

                return res.SetSuccessData(token_model);
            }, TimeSpan.FromMinutes(10));

            if (data == null)
                throw new ArgumentNullException("读取token缓存错误");
            if (data.Error)
                throw new MsgException(data.ErrorMsg);
            return data;
        }

        /// <summary>
        /// 找到用户
        /// </summary>
        /// <param name="key_manager"></param>
        /// <param name="_cache"></param>
        /// <param name="_context"></param>
        /// <param name="user_uid"></param>
        /// <returns></returns>
        async Task<_<WCloudUserInfo>> FindUserOrThrow(
            ICacheKeyManager key_manager, ICacheProvider _cache,
            HttpContext _context, string user_uid)
        {
            var key = key_manager.UserInfo(user_uid);
            var data = await _cache.GetOrSetAsync_(key, async () =>
            {
                var res = new _<WCloudUserInfo>();
                var _user = _context.RequestServices.Resolve_<ILoginService<UserEntity>>();

                var model = await _user.GetUserByUID(user_uid);
                if (model == null)
                    return res.SetErrorMsg("用户不存在");
                var principal = model.ToPrincipal(ConfigSet.Identity.UserLoginScheme);
                //var loginuser = principal.ToWCloudUserInfo();
                var loginuser = new WCloudUserInfo() { };
                loginuser.UserID.Should().NotBeNullOrEmpty("没有填充上面的数据");

                return res.SetSuccessData(loginuser);
            }, TimeSpan.FromMinutes(10));

            if (data == null)
                throw new ArgumentNullException("读取用户缓存错误");
            if (data.Error)
                throw new MsgException(data.ErrorMsg);
            return data;
        }

        /// <summary>
        /// 提取token，反推出用户信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var auth_type = context.Scheme.Name;
            var _context = context.HttpContext;
            var _principal = context.Principal ?? new ClaimsPrincipal();

            try
            {
                var token = new string[] {
                    _context.Request.Headers["auth_token"],
                    _principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                }.FirstNotEmpty_();

                if (ValidateHelper.IsEmpty(token))
                    throw new MsgException("token不存在");

                //缓存相关
                var key_manager = _context.RequestServices.Resolve_<ICacheKeyManager>();
                var _cache = _context.RequestServices.ResolveDistributedCache_();

                //把登录数据缓存到http上下文
                var cache_key = key_manager.AuthHttpItemKey(auth_type);
                var loginuser_json = await _context.CacheInHttpContextAsync(cache_key, async () =>
                {
                    var token_data = await this.FindTokenOrThrow(key_manager, _cache, _context, token);
                    var user_data = await this.FindUserOrThrow(key_manager, _cache, _context, token_data.Data.UserUID);

                    return user_data.Data.ToJson();
                });
                if (ValidateHelper.IsEmpty(loginuser_json))
                    throw new MsgException(nameof(loginuser_json));

                //生成identity
                var identities = _principal.Identities.Where(x => x.AuthenticationType != auth_type).ToList();

                var identity = new ClaimsIdentity(authenticationType: auth_type);
                identity.AddClaim_(ClaimTypes.UserData, loginuser_json);

                identities.Add(identity);
                var loginuser = new ClaimsPrincipal(identities: identities);

                context.ReplacePrincipal(loginuser);
            }
            catch (MsgException)
            {
                context.RejectPrincipal();
            }
            catch (Exception e)
            {
                context.HttpContext.RequestServices.ResolveLogger<MyCookieAuthenticationEvents>().AddErrorLog(e.Message, e);

                context.RejectPrincipal();
            }
            await base.ValidatePrincipal(context);
        }

        /// <summary>
        /// 抹掉大部分信息，只留token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task SigningIn(CookieSigningInContext context)
        {
            var _context = context.HttpContext;
            try
            {
                var auth_type = context.Scheme.Name;

                var json = context.Principal?.FindFirst(ClaimTypes.UserData)?.Value;
                if (ValidateHelper.IsEmpty(json))
                    throw new ArgumentException("未找到用户数据");

                var loginuser = json.JsonToEntity<WCloudUserInfo>() ?? throw new ArgumentException("数据格式错误");
                var token = loginuser.ExtraData["token"];

                if (ValidateHelper.IsEmpty(token))
                    throw new ArgumentException("token不存在");

                var identity = new ClaimsIdentity(authenticationType: auth_type);
                identity.AddClaim_(ClaimTypes.NameIdentifier, token);
                context.Principal = new ClaimsPrincipal(identity);

                await base.SigningIn(context);

                //缓存相关
                var key_manager = _context.RequestServices.Resolve_<ICacheKeyManager>();
                var _cache = _context.RequestServices.ResolveDistributedCache_();

                //删除用户缓存
                var key = key_manager.UserInfo(loginuser.UserID);
                await _cache.RemoveAsync(key);
                //删除token缓存
                key = key_manager.AuthToken(token);
                await _cache.RemoveAsync(key);
            }
            catch (Exception e)
            {
                throw new LoginException("登录异常", e);
            }
        }
    }
}
