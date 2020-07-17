using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using WCloud.Framework.MVC.Extension;

namespace WCloud.Framework.MVC.Attribute_
{
    public class OnlyDevAttribute : _ActionFilterBaseAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var env = context.HttpContext.RequestServices.ResolveHostingEnvironment_();

            if (env.IsDevelopment())
                await next.Invoke();
            else
            {
                context.Result = new NoContentResult();
            }
        }
    }
}
