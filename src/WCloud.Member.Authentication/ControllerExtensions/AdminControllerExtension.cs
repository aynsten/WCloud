using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Model;
using WCloud.Member.Application.PermissionValidator;
using WCloud.Member.Authentication.UserContext;

namespace WCloud.Member.Authentication.ControllerExtensions
{
    public interface IAdminController { }

    public static class AdminControllerExtension
    {
        static async Task __validate_login_context__(ControllerBase controller, WCloudAdminInfo loginuser, string[] permissions)
        {
            if (loginuser == null)
                //没有登录
                throw new NoLoginException();

            if (ValidateHelper.IsNotEmpty(permissions))
            {
                var validator = controller.HttpContext.RequestServices.Resolve_<IPermissionValidatorService>();
                if (!await validator.HasAllPermission(loginuser.UserID, permissions))
                    throw new NoPermissionException();
            }
        }

        public static async Task<WCloudAdminInfo> GetLoginAdminAsync<T>(this T controller, string[] permissions = null) where T : ControllerBase, IAdminController
        {
            var user_context = controller.HttpContext.RequestServices.Resolve_<ILoginContext<WCloudAdminInfo>>();
            var loginuser = await user_context.GetLoginContextAsync();

            await __validate_login_context__(controller, loginuser, permissions);

            return loginuser;
        }
    }
}
