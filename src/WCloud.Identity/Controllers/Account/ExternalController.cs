using IdentityModel;
using Lib.helper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Login;
using WCloud.Member.Authentication.ControllerExtensions;

namespace IdentityServer4.Quickstart.UI
{
    [AllowAnonymous]
    public class ExternalController : WCloudBaseController, IUserController
    {
        private readonly IUserLoginService _login;

        public ExternalController(IUserLoginService login)
        {
            this._login = login;
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Challenge(string provider, string returnUrl)
        {
            await Task.CompletedTask;

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "~/";

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
            };

            return Challenge(props, provider);
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            var external_cookie_login = ConfigSet.Identity.ExternalLoginScheme;

            // read external identity from the temporary cookie
            var res = await HttpContext.AuthenticateAsync(external_cookie_login);
            if (res?.Succeeded != true || res.Principal == null)
            {
                throw new Exception("External authentication error");
            }
            var provider = res.Properties.Items["scheme"];
            var principal = res.Principal;

            var openid =
                principal.FindFirst(JwtClaimTypes.Subject)?.Value ??
                principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ValidateHelper.IsEmpty(openid))
                return Content("未知openid");

            var map = await this._login.FindExternalLoginByOpenID(provider, openid);

            if (map == null)
                return Content("此外部账号未关联任何用户");

            var user = await this._login.GetUserByUID(map.UserID);

            if (user == null)
                return Content("用户不存在或者被禁用");

            var identity = new IdentityServer4.IdentityServerUser(user.UID)
            {
                DisplayName = user.UserName
            };

            await this.HttpContext.SignInAsync(identity);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(external_cookie_login);

            // retrieve return URL
            var returnUrl = res.Properties.Items["returnUrl"] ?? "~/";
            return Redirect(returnUrl);
        }
    }
}
