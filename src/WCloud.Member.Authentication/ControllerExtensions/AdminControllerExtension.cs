using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Model;
using WCloud.Member.Authentication.Admin;

namespace WCloud.Member.Authentication.ControllerExtensions
{
    public interface IAdminController { }

    public static class AdminControllerExtension
    {
        static async Task __validate_login_context__(ControllerBase controller, WCloudAdminInfo loginuser, string[] permissions)
        {
            if (!loginuser.IsAuthed())
            {
                //没有登录
                throw new NoLoginException();
            }

            if (ValidateHelper.IsNotEmpty(permissions))
            {
                var validator = controller.HttpContext.RequestServices.Resolve_<IAdminAuthService>();
                var my_permission = await validator.GetAdminPermission(loginuser.UserID);
                if (permissions.Except(my_permission).Any())
                {
                    throw new NoPermissionException();
                }
            }
        }

        public static async Task<WCloudAdminInfo> GetLoginAdminAsync<T>(this T controller, string[] permissions = null) where T : ControllerBase, IAdminController
        {
            var user_context = controller.HttpContext.RequestServices.Resolve_<IWCloudContext<T>>();
            var loginuser = user_context.CurrentAdminInfo;

            await __validate_login_context__(controller, loginuser, permissions);

            return loginuser;
        }
    }
}
