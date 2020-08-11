using FluentAssertions;
using IdentityModel.Client;
using Lib.helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Authentication.Filters;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("admin")]
    public class AdminAccountController : WCloudBaseController, IAdminController
    {
        private readonly ILoginService<AdminEntity> _login;
        private readonly IAuthTokenService authTokenService;
        private readonly IConfiguration config;
        private readonly HttpClient httpClient;
        private readonly OAuthConfig oAuthConfig;

        public AdminAccountController(
            ILoginService<AdminEntity> login,
            IAuthTokenService authTokenService,
            IConfiguration config,
            IHttpClientFactory factory,
            OAuthConfig oAuthConfig)
        {
            this._login = login;
            this.authTokenService = authTokenService;
            this.config = config;
            this.oAuthConfig = oAuthConfig;
            this.httpClient = factory.CreateClient("my_admin_oauth");
        }

        /// <summary>
        /// 管理员账号密码登陆，获取token
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> OauthPwdLogin([FromForm]string user_name, [FromForm]string password)
        {
            var disco = await this.httpClient.__disco__(this.config);
            var tokenResponse = await this.httpClient.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = ConfigSet.Identity.AdminPwdGrantType,

                ClientId = this.oAuthConfig.ClientId,
                ClientSecret = this.oAuthConfig.ClientSecret,

                Parameters =
                {
                    { "scope", this.oAuthConfig.Scope },
                    { "username",user_name},
                    { "password",password}
                }
            });

            var res = tokenResponse.ToTokenModel();
            res.ThrowIfNotSuccess();

            return SuccessJson(res.Data);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> RefreshToken([FromForm]string refresh_token)
        {
            var disco = await this.httpClient.__disco__(this.config);

            var tokenResponse = await this.httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest()
            {
                Address = disco.TokenEndpoint,
                Scope = this.oAuthConfig.Scope,
                RefreshToken = refresh_token,
                GrantType = "refresh_token",
                ClientId = this.oAuthConfig.ClientId,
                ClientSecret = this.oAuthConfig.ClientSecret
            });

            var res = tokenResponse.ToTokenModel();
            res.ThrowIfNotSuccess();

            return SuccessJson(res.Data);
        }

        /// <summary>
        /// 自己改自己的密码
        /// </summary>
        [HttpPost, ApiRoute, AuthAdmin]
        public async Task<IActionResult> ChangePwd([FromForm]string old_pwd, [FromForm]string pwd)
        {
            if (!ValidateHelper.IsAllNotEmpty(old_pwd, pwd))
            {
                throw new NoParamException();
            }

            var loginadmin = await this.GetLoginAdminAsync();
            var admin = await this._login.GetUserByUID(loginadmin.UserID);
            admin.Should().NotBeNull();

            if (admin.PassWord != this._login.EncryptPassword(old_pwd))
            {
                return GetJsonRes("旧密码不匹配");
            }

            await this._login.SetPassword(admin.UID, pwd);

            return SuccessJson();
        }

        /// <summary>
        /// 管理员重设用户密码
        /// </summary>
        [HttpPost, ApiRoute, AuthAdmin(Permission = "reset-pwd")]
        public async Task<IActionResult> ResetPwd([FromForm]string user_uid, [FromForm]string pwd)
        {
            if (!ValidateHelper.IsAllNotEmpty(user_uid, pwd))
            {
                throw new NoParamException();
            }

            await this._login.SetPassword(user_uid, pwd);

            return SuccessJson();
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute, System.Obsolete]
        public async Task<IActionResult> LogOutAction()
        {
            await this.HttpContext.SignOutAsync(ConfigSet.Identity.AdminLoginScheme);

            return SuccessJson();
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Logout()
        {
            //await this.HttpContext.SignOutAsync();
            await Task.CompletedTask;

            return SuccessJson();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> LoginAdminInfo()
        {
            var loginuser = await this.GetLoginAdminAsync();
            return SuccessJson(loginuser);
        }
    }
}
