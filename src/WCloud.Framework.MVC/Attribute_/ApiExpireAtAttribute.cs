using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WCloud.Framework.MVC.Attribute_
{
    /// <summary>
    /// 标记接口过期时间
    /// </summary>
    public class ApiExpireAtAttribute : _ActionFilterBaseAttribute
    {
        private DateTime Date { get; set; }

        public ApiExpireAtAttribute(string date)
        {
            this.Date = DateTime.Parse(date);
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (DateTime.Now > this.Date)
            {
                context.Result = new JsonResult(new _() { }.SetErrorMsg("无法响应请求，请升级客户端"));
                return;
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
