using Lib.cache;
using Lib.core;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.MVC.Extension;

namespace WCloud.Framework.MVC.Attribute_
{
    /// <summary>
    /// 防止重复提交
    /// </summary>
    public class AntiReSubmitAttribute : _ActionFilterBaseAttribute
    {
        public virtual int CacheSeconds { get; set; } = 5;

        public virtual string ErrorMessage { get; set; } = "重复提交，请稍后再试";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            CacheSeconds = Math.Abs(CacheSeconds);
            if (CacheSeconds == 0) { throw new Exception("缓存时间不能为0"); }

            var sessionID = context.HttpContext.Session.Id;
            var key = $"{nameof(AntiReSubmitAttribute)}:{sessionID}";

            var reqparams = context.HttpContext.Request.Form.ToDict();
            reqparams = reqparams.AddDict(context.HttpContext.Request.Query.ToDict());

            var dict = new SortedDictionary<string, string>(reqparams, new MyStringComparer());
            var submitData = dict.ToUrlParam();
            var (AreaName, ControllerName, ActionName) = context.RouteData.GetRouteInfo();
            submitData = $"{AreaName}/{ControllerName}/{ActionName}/:{submitData}";
            //读取缓存
            var cache = context.HttpContext.RequestServices.Resolve_<ICacheProvider>();
            {
                var data = await cache.GetAsync_<string>(key);
                if (data != null)
                {
                    if (data.Data == submitData)
                    {
                        var res = new _() { }.SetErrorMsg(this.ErrorMessage);
                        context.Result = new JsonResult(res);
                        return;
                    }
                }
                //10秒钟不能提交相同的数据
                await cache.SetAsync_(key, submitData, TimeSpan.FromSeconds(CacheSeconds));
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
