using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Quickstart.UI;
using Lib.helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WCloud.Core;
using WCloud.Core.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application;
using WCloud.Member.Authentication;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Domain.User;

namespace WCloud.Identity.MemberShipControllers
{
    public partial class AccountController : WCloudBaseController, IUserController
    {
        private readonly ILoginService<UserEntity> _login;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IMessagePublisher _publisher;

        public AccountController(
            ILoginService<UserEntity> login,
            IAuthenticationSchemeProvider schemeProvider,
            IMessagePublisher publisher)
        {
            this._login = login;

            this._schemeProvider = schemeProvider;
            this._publisher = publisher;
        }

        /// <summary>
        /// 登陆表单
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            returnUrl = returnUrl?.Trim();
            returnUrl = ValidateHelper.IsEmpty(returnUrl) ? "/" : returnUrl;

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToArray();

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalProviders = providers
            };

            return View(model);
        }

        /// <summary>
        /// 密码登陆
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoginViaPass(string username, string password)
        {
            if (!ValidateHelper.IsAllNotEmpty(username, password))
            {
                return GetJsonRes("用户名密码不能为空");
            }
            var res = await this._login.ValidUserPassword(username, password);
            res.ThrowIfNotSuccess();

            var user = res.Data;

            var identity = new IdentityServer4.IdentityServerUser(user.UID)
            {
                DisplayName = user.UserName
            };
            identity.AdditionalClaims = new Claim[]
            {
                new Claim(AuthExtensions.claims_account_type_key,"user")
            };

            await this.HttpContext.SignInAsync(identity);

            return GetJson(new _().SetSuccessData(string.Empty));
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LogOutAction()
        {
            //ids
            await this.HttpContext.SignOutAsync(ConfigSet.Identity.UserLoginScheme);
            //三方
            await this.HttpContext.SignOutAsync(ConfigSet.Identity.ExternalLoginScheme);

            return GetJson(new _().SetSuccessData(string.Empty));
        }

        /// <summary>
        /// 退出登陆
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await this.LogOutAction();

            if (ValidateHelper.IsNotEmpty(returnUrl))
                return Redirect(returnUrl);
            else
                return Redirect("/");
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetLoginUserInfo()
        {
            var loginuser = await this.GetLoginUserAsync();

            var res = new _().SetSuccessData(loginuser);

            return GetJson(res);
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="data">用户的json数据</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUser(string data)
        {
            var user = this.JsonToEntity_<UserEntity>(data);

            var res = await this._login.AddAccount(user);
            res.ThrowIfNotSuccess();

            return GetJsonRes(string.Empty);
        }
    }
}
