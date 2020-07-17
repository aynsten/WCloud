using FluentAssertions;
using Lib.helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Settings;
using WCloud.Core;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Authentication.CustomAuth;
using WCloud.Member.Authentication.Filters;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Shared.Localization;

namespace WCloud.Admin.Controllers
{
    public class AccountController : WCloudBaseController, IAdminController
    {
        private readonly ILoginService<AdminEntity> _login;
        private readonly IAuthTokenService authTokenService;

        public AccountController(
            ILoginService<AdminEntity> login,
            IAuthTokenService authTokenService)
        {
            this._login = login;
            this.authTokenService = authTokenService;
        }

        /// <summary>
        /// 密码登陆
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost, AdminRoute]
        public async Task<IActionResult> LoginViaPass([FromForm]string username, [FromForm]string password)
        {
            if (!ValidateHelper.IsAllNotEmpty(username, password))
            {
                return GetJsonRes("用户名密码不能为空");
            }

            var admin = await this._login.GetUserByUserName(username);
            if (admin == null)
                return GetJsonRes("用户不存在");
            if (admin.PassWord != this._login.EncryptPassword(password))
                return GetJsonRes("密码错误");

            var scheme = ConfigSet.Identity.AdminLoginScheme;
            var principal = admin.ToPrincipal(scheme, x => x.SetAccountType("admin").SetCreateTimeUtc());

            await this.HttpContext.SignInAsync(principal, new AuthenticationProperties() { });

            return SuccessJson();
        }

        /// <summary>
        /// 自己改自己的密码
        /// </summary>
        [HttpPost, AdminRoute, AuthAdmin]
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
        [HttpPost, AdminRoute, AuthAdmin(Permission = "reset-pwd")]
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
        [HttpPost, AdminRoute]
        public async Task<IActionResult> LogOutAction()
        {
            await this.HttpContext.SignOutAsync(ConfigSet.Identity.AdminLoginScheme);

            return SuccessJson();
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [HttpPost, AdminRoute]
        public async Task<IActionResult> GetLoginAdminInfo()
        {
            var loginuser = await this.GetLoginAdminAsync();
            return SuccessJson(loginuser);
        }

#if DEBUG
        [HttpGet, AdminRoute]
        public async Task<string> lang([FromServices]IStringLocalizer<MemberShipResource> l, [FromQuery]string key)
        {
            var ls = this.HttpContext.RequestServices.GetServices<IStringLocalizer<MemberShipResource>>().ToArray();

            var settings = this.HttpContext.RequestServices.GetServices<ISettingProvider>().ToArray();

            foreach (var s in settings)
            {
                var res = await s.GetAllAsync();
            }

            return l[key ?? "contact"] ?? "?";
        }

        async Task<IActionResult> __signin__(string user_uid)
        {
            var token = await this.authTokenService.CreateAccessTokenAsync($"user:{user_uid}");

            var identity = new ClaimsIdentity(authenticationType: ConfigSet.Identity.AdminLoginScheme);

            identity.SetToken(token.AccessToken);

            var principal = new ClaimsPrincipal(identity);

            await this.HttpContext.SignInAsync(principal);

            return SuccessJson(token);
        }
#endif

    }
}
