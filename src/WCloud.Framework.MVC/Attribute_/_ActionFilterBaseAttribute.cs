using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WCloud.Framework.MVC.Attribute_
{
    public abstract class _ActionFilterBaseAttribute : ActionFilterAttribute
    {
        protected IActionResult GetJson(object data) => new JsonResult(data, serializerSettings: Lib.helper.JsonHelper._setting);
    }
}
