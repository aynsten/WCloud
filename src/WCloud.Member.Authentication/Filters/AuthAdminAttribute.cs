using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Model;
using WCloud.Framework.MVC.Attribute_;
using WCloud.Framework.MVC.Extension;
using WCloud.Member.Application.PermissionValidator;
using WCloud.Member.Authentication.UserContext;

namespace WCloud.Member.Authentication.Filters
{
    public class AuthAdminAttribute : _ActionFilterBaseAttribute
    {
        /// <summary>
        /// 权限，逗号隔开
        /// </summary>
        public string Permission { get; set; }

        public bool ValidUrl { get; set; } = false;

        public AuthAdminAttribute() { }

        public AuthAdminAttribute(string permission)
        {
            this.Permission = permission;
        }

        string[] GetPermissions(ActionExecutingContext context)
        {
            var pers = new List<string>();
            if (this.ValidUrl)
            {
                if (ValidateHelper.IsNotEmpty(this.Permission))
                    throw new NotSupportedException("当校验url时不支持自定义permission");

                if (context.RouteData != null)
                    throw new Exception("获取不到route");

                var route = context.RouteData.GetRouteInfo();
                var action_as_permission = string.Join("/", new[] { route.area, route.controller, route.action });
                action_as_permission = action_as_permission.ToLower();

                pers.Add(action_as_permission);
            }
            else
            {
                this.Permission = this.Permission ?? string.Empty;
                pers.AddRange(this.Permission.Split(','));
            }

            var res = pers.WhereNotEmpty().ToArray();

            return res;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var loginContext = context.HttpContext.RequestServices.Resolve_<ILoginContext<WCloudAdminInfo>>();
            var loginuser = await loginContext.GetLoginContextAsync();

            //检查登录
            if (loginuser == null)
            {
                throw new NoLoginException();
            }

            //检查权限
            var pers = this.GetPermissions(context);

            if (ValidateHelper.IsNotEmpty(pers))
            {
                var permissionService = context.HttpContext.RequestServices.Resolve_<IPermissionValidatorService>();
                //有全部权限
                if (!await permissionService.HasAllPermission(loginuser.UserID, pers))
                {
                    throw new NoPermissionException();
                }
            }

            //let it go
            await next.Invoke();
        }
    }
}
