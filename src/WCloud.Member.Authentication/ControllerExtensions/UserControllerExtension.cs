using Lib.helper;
using Lib.ioc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Model;
using WCloud.Member.Application.PermissionValidator;
using WCloud.Member.Authentication.OrgSelector;
using WCloud.Member.Authentication.UserContext;

namespace WCloud.Member.Authentication.ControllerExtensions
{
    public interface IUserController { }

    public static class UserControllerExtension
    {
        [Obsolete]
        public static string GetSelectedOrgUID<T>(this T controller) where T : ControllerBase, IUserController
        {
            var org_uid = controller.HttpContext.RequestServices.Resolve_<ICurrentOrgSelector>().GetSelectedOrgUID();

            if (ValidateHelper.IsEmpty(org_uid))
                throw new NoOrgException();

            return org_uid;
        }

        [Obsolete]
        public static void SetCookieOrgUID<T>(this T controller, string uid) where T : ControllerBase, IUserController
        {
            controller.HttpContext.RequestServices.Resolve_<ICurrentOrgSelector>().SetCookieOrgUID(uid);
        }

        /// <summary>
        /// 获取登录用户
        /// </summary>
        /// <returns></returns>
        public static async Task<WCloudUserInfo> GetLoginUserAsync<T>(this T controller) where T : ControllerBase, IUserController
        {
            var user_context = controller.HttpContext.RequestServices.Resolve_<ILoginContext<WCloudUserInfo>>();
            var loginuser = await user_context.GetLoginContextAsync();

            if (loginuser == null)
                //没有登录
                throw new NoLoginException();

            return loginuser;
        }

        public static async Task<WCloudUserInfo> ValidOrgMember<T>(this T controller, int? flag = null, string[] permissions = null) where T : ControllerBase, IUserController
        {
            //this.GetSelectedOrgUID();

            var loginuser = await GetLoginUserAsync(controller);

            if (loginuser.Org == null)
                throw new NotInCurrentOrgException();
            if (ValidateHelper.IsEmpty(loginuser.Org.UID))
                throw new NoOrgException();

            if (flag != null)
            {
                //组织所有人或者角色允许
                if (!(loginuser.Org.IsOwner || loginuser.HasRoleInOrg(flag.Value)))
                    throw new NoPermissionInOrgException();
            }

            if (ValidateHelper.IsNotEmpty(permissions))
            {
                var validator = controller.HttpContext.RequestServices.Resolve_<IOrgPermissionValidatorService>();
                if (!await validator.HasAllOrgPermission(loginuser.Org.UID, loginuser.UserID, permissions))
                    throw new NoPermissionInOrgException();
            }

            return loginuser;
        }
    }
}
