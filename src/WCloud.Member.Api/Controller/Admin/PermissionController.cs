using Lib.extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.PermissionValidator;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("admin")]
    public class AdminPermissionController : WCloudBaseController, IAdminController
    {
        private readonly IPermissionService _perService;
        private readonly IStringLocalizerFactory stringLocalizerFactory;

        public AdminPermissionController(
            IStringLocalizerFactory stringLocalizerFactory,
            IPermissionService _perService)
        {
            this.stringLocalizerFactory = stringLocalizerFactory;
            this._perService = _perService;
        }

        /// <summary>
        /// 查询我的权限
        /// </summary>
        /// <param name="_validator"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> MyPermissions([FromServices]IAdminPermissionService _validator)
        {
            var loginuser = await this.GetLoginAdminAsync();

            var permissions = await _validator.LoadAllPermissionsName(loginuser.UserID);

            return SuccessJson(permissions);
        }

        [NonAction]
        string __lang_str__(ILocalizableString display_name) => display_name?.Localize(this.stringLocalizerFactory)?.Value;

        /// <summary>
        /// 查询所有权限
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryAllPermissions()
        {
            await this.GetLoginAdminAsync();

            var list = new List<string>();

            object __permission_definition__(PermissionDefinition m)
            {
                list.AddOnceOrThrow(m.Name);
                var DisplayName = this.__lang_str__(m.DisplayName) ?? m.Name;
                return new
                {
                    key = m.Name,
                    title = DisplayName,
                    raw_data = new
                    {
                        UID = m.Name,
                        DisplayName
                    },
                    children = m.Children?.Select(x => __permission_definition__(x)).ToArray() ?? new object[] { }
                };
            }

            object __group_definition__(PermissionGroupDefinition x)
            {
                list.AddOnceOrThrow(x.Name);
                var DisplayName = this.__lang_str__(x.DisplayName) ?? x.Name;
                return new
                {
                    key = x.Name,
                    title = DisplayName,
                    raw_data = new
                    {
                        UID = x.Name,
                        DisplayName
                    },
                    children = x.Permissions?.Select(m => __permission_definition__(m)).ToArray() ?? new object[] { },
                };
            }

            var groups = this._perService.AbpPermissionDefinition.GetGroups();

            var res = groups.Select(x => __group_definition__(x)).ToArray();

            return SuccessJson(res);
        }
    }
}