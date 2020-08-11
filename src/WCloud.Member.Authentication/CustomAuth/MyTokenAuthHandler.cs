using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Cache;
using WCloud.Framework.MVC.Extension;
using WCloud.Member.Application;
using WCloud.Member.Application.Service;
using WCloud.Member.Domain;

namespace WCloud.Member.Authentication.CustomAuth
{
    /// <summary>
    /// signin-hanlder->signout-handler->auth-handler
    /// </summary>
    public class MyTokenAuthHanlder<LoginUserEntity> : SignInAuthenticationHandler<MyTokenAuthOption>,
       IAuthenticationHandler,
       IAuthenticationSignInHandler,
       IAuthenticationSignOutHandler
        where LoginUserEntity : class, ILoginEntity
    {
        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager _keyManager;
        private readonly IDataProtectionProvider _protectionProvider;

        private readonly IAuthTokenService _tokenService;
        private readonly ILoginService<LoginUserEntity> _loginService;

        public MyTokenAuthHanlder(
            ICacheProvider _cache,
            ICacheKeyManager _keyManager,
            IDataProtectionProvider _protectionProvider,

            IAuthTokenService _tokenService,
            ILoginService<LoginUserEntity> _loginService,

            IOptionsMonitor<MyTokenAuthOption> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            this._cache = _cache;
            this._keyManager = _keyManager;
            this._protectionProvider = _protectionProvider;

            this._tokenService = _tokenService;
            this._loginService = _loginService;
        }

        private ISecureDataFormat<string> _protector;
        protected override async Task InitializeHandlerAsync()
        {
            await base.InitializeHandlerAsync();

            var protector = this._protectionProvider.CreateProtector($"protector_for_custom_token_{this.Options.CookieOption.Name}_login");
            this._protector = new SecureDataFormat<string>(new StringSerializer(), protector);
        }

        async Task<LoginUserEntity> __find_user_by_token(string token)
        {
            async Task<LoginUserEntity> __load_user__()
            {
                var token_model = await this._tokenService.GetUserIdByTokenAsync(token);
                var user_uid = token_model?.UserUID;

                //多个账号体系公用一个token数据表，admin:123,user:123（不方便sql关联查询）
                user_uid = user_uid?.Split(':').LastOrDefault();

                if (ValidateHelper.IsEmpty(user_uid))
                    throw new MsgException("token无效");

                var user_model = await this._loginService.GetUserByUID(user_uid);

                return user_model;
            }

            var key = this._keyManager.AuthToken(token);
            var data = await this._cache.GetOrSetAsync_(key, __load_user__, TimeSpan.FromMinutes(500));

            if (data == null || ValidateHelper.IsEmpty(data.UID))
                throw new MsgException("fail to load user");

            return data;
        }

        string __unprotect_token__(string data)
        {
            try
            {
                var token = this._protector.Unprotect(data);

                if (ValidateHelper.IsEmpty(token))
                {
                    throw new MsgException("解密得到的token为空");
                }

                return token;
            }
            catch
            {
                throw new MsgException("解密token失败");
            }
        }

        string __get_token_from_httpcontext__()
        {
            var head_token = (string)this.Context.Request.Headers["token"];
            if (ValidateHelper.IsNotEmpty(head_token))
            {
                return head_token;
            }

            var cookie_encrypt_token = this.Context.Request.Cookies.GetCookie_(this.Options.CookieOption.Name);
            if (ValidateHelper.IsNotEmpty(cookie_encrypt_token))
            {
                var res = this.__unprotect_token__(cookie_encrypt_token);
                return res;
            }

            throw new MsgException("token not found");
        }

        /// <summary>
        /// 校验
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                var token = this.__get_token_from_httpcontext__();

                var user = await this.__find_user_by_token(token);

                var principal = user.ToPrincipal(this.Scheme.Name);

                var ticket = new AuthenticationTicket(principal, authenticationScheme: this.Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch (MsgException e)
            {
                return AuthenticateResult.Fail(e.Message);
            }
        }

        /// <summary>
        /// 登陆
        /// create token for user
        /// write to cookie
        /// </summary>
        protected override async Task HandleSignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            await Task.CompletedTask;

            var token = user.GetToken();
            token.Should().NotBeNullOrEmpty();

            token = this._protector.Protect(token);

            var option = this.Options.CookieOption.Build(this.Context);
            this.Context.Response.Cookies.SetCookie_(this.Options.CookieOption.Name, token, option);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected override async Task HandleSignOutAsync(AuthenticationProperties properties)
        {
            var option = this.Options.CookieOption.Build(this.Context);
            this.Context.Response.Cookies.DeleteCookie_(this.Options.CookieOption.Name, option);
            await Task.CompletedTask;
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            /*
            var res = new _() { ErrorCode = "-999" }.SetErrorMsg("未登陆");
            var json = res.ToJson();

            this.Context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
            this.Context.Response.ContentType = "application/json";
            await this.Context.Response.WriteAsync(json);
            */
            //await Task.CompletedTask;

            throw new NoLoginException();
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            //await Task.CompletedTask;

            throw new NoPermissionException();
        }
    }
}
