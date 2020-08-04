using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Model;
using WCloud.Framework.MVC.Attribute_;
using WCloud.Member.Authentication.UserContext;

namespace WCloud.Member.Authentication.Filters
{
    public class AuthUserAttribute : _ActionFilterBaseAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var loginContext = context.HttpContext.RequestServices.Resolve_<ILoginContext<WCloudUserInfo>>();

            var loginuser = await loginContext.GetLoginContextAsync();

            if (loginuser == null)
            {
                throw new NoLoginException();
            }

            //let it go
            await next.Invoke();
        }
    }
}
