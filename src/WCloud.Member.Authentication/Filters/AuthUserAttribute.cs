using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WCloud.Core;
using WCloud.Framework.MVC.Attribute_;

namespace WCloud.Member.Authentication.Filters
{
    public class AuthUserAttribute : _ActionFilterBaseAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var loginContext = context.HttpContext.RequestServices.Resolve_<IWCloudContext<AuthUserAttribute>>();

            var loginuser = loginContext.CurrentUserInfo;

            if (loginuser == null)
            {
                throw new NoLoginException();
            }

            //let it go
            await next.Invoke();
        }
    }
}
