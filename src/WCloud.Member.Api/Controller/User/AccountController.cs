using FluentAssertions;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Framework.Wechat.Login;
using WCloud.Member.Application.Login;
using WCloud.Member.Authentication;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("user")]
    public class UserAccountController : WCloudBaseController, IUserController
    {
        private readonly ILogger _logger;
        private readonly IUserWxLoginService userWxLoginService;
        private readonly IUserLoginService _login;
        private readonly IConfiguration config;
        private readonly HttpClient httpClient;
        private readonly OAuthConfig oAuthConfig;

        public UserAccountController(
            ILogger<UserAccountController> _logger,
            IUserWxLoginService userWxLoginService,
            IUserLoginService _login,
            IConfiguration config,
            IHttpClientFactory factory,
            OAuthConfig oAuthConfig)
        {
            this._logger = _logger;
            this.userWxLoginService = userWxLoginService;
            this._login = _login;
            this.config = config;
            this.oAuthConfig = oAuthConfig;
            this.httpClient = factory.CreateClient("wx_login_");
        }

        /// <summary>
        /// 微信第三方登陆
        /// </summary>
        /// <param name="code"></param>
        /// <param name="nick_name"></param>
        /// <param name="avatar_url"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> WxOauthLogin(string code, string nick_name, string avatar_url)
        {
            var disco = await this.httpClient.__disco__(this.config);
            var tokenResponse = await this.httpClient.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = ConfigSet.Identity.WechatGrantType,

                ClientId = this.oAuthConfig.ClientId,
                ClientSecret = this.oAuthConfig.ClientSecret,

                Parameters =
                {
                    { "scope", this.oAuthConfig.Scope },
                    { "code", code },
                    { "nick_name", nick_name },
                    { "avatar_url", avatar_url }
                }
            });

            var res = tokenResponse.ToTokenModel();
            res.ThrowIfNotSuccess();

            return SuccessJson(res.Data);
        }

        /// <summary>
        /// 密码授权模式
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> OauthPwdLogin(string user_name, string password)
        {
            var disco = await this.httpClient.__disco__(this.config);
            var tokenResponse = await this.httpClient.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = "password",

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
        public async Task<IActionResult> RefreshToken([FromForm] string refresh_token)
        {
            var disco = await this.httpClient.__disco__(this.config);

            var tokenResponse = await this.httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest()
            {
                Address = disco.TokenEndpoint,
                Scope = this.oAuthConfig.Scope,
                RefreshToken = refresh_token,
                GrantType = "refresh_token",
                ClientId = this.oAuthConfig.ClientId,
                ClientSecret = this.oAuthConfig.ClientSecret,
            });

            var res = tokenResponse.ToTokenModel();
            res.ThrowIfNotSuccess();

            return SuccessJson(res.Data);
        }

        /// <summary>
        /// 微信设置用户登陆手机号
        /// </summary>
        /// <param name="code"></param>
        /// <param name="iv"></param>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> SetUserPhone(string code, string iv, string encryptedData)
        {
            code.Should().NotBeNullOrEmpty();
            iv.Should().NotBeNullOrEmpty();
            encryptedData.Should().NotBeNullOrEmpty();

            var loginuser = await this.GetLoginUserAsync();

            var openid = await this.userWxLoginService.__get_wx_openid__(code);

            var map = await this._login.FindExternalLoginByOpenID(this.userWxLoginService.LoginProvider, openid.openid);
            if (map == null || map.UserID != loginuser.UserID)
                return GetJsonRes("当前微信账号未正确绑定");

            var mobile = this.userWxLoginService.__extract_mobile_or_throw__(encryptedData, iv, openid.session_key);

            var res = await this._login.SetPhone(loginuser.UserID, mobile);
            res.ThrowIfNotSuccess();

            return SuccessJson();
        }

        /// <summary>
        /// 检查用户是否绑定手机
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> CheckUserPhone()
        {
            var loginuser = await this.GetLoginUserAsync();

            var data = await this._login.GetUserPhone(loginuser.UserID);

            var res = data.Any();

            return SuccessJson(res);
        }

        /// <summary>
        /// 读取用户绑定的手机号
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> GetUserPhone()
        {
            var loginuser = await this.GetLoginUserAsync();

            var data = await this._login.GetUserPhone(loginuser.UserID);

            var phone = data.FirstOrDefault();

            var res = new
            {
                phone?.Phone
            };

            return SuccessJson(res);
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
        /// 登陆用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> LoginUserInfo()
        {
            var loginuser = await this.GetLoginUserAsync();
            return SuccessJson(loginuser);
        }

#if DEBUG
        /// <summary>
        /// 生成jwt
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public IActionResult test_jwt()
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Subject,"wer")
            };

            //这个secret key也可以通过jwks来生成，不需要保密。只要保持一致就可以了
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("123456789123456789123456789"));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: sign);

            return SuccessJson(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> test_jwt_xx([FromServices] IHttpClientFactory factory)
        {
            var claims = new[]
            {
                new Claim(JwtClaimTypes.Subject,"wer")
            };
            var client = factory.CreateClient("jwks");
            using var response = await client.GetAsync("https://agilewater.cn:5001/.well-known/openid-configuration/jwks");
            var data = await response.Content.ReadAsStringAsync();
            var keys = JObject.Parse(data)["keys"];
            var k = keys[0].ToString();

            //这个secret key也可以通过jwks来生成，不需要保密。只要保持一致就可以了
            var key = new JsonWebKey(k);
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5000",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: sign);

            return SuccessJson(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
#endif
    }
}
