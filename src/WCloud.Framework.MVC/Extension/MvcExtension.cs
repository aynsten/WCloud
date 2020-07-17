using Lib.core;
using Lib.extension;
using Lib.helper;
using Lib.io;
using Lib.ioc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WCloud.Framework.MVC.Extension
{
    public static class MvcExtension
    {
        public static IWebHostEnvironment ResolveHostingEnvironment_(this IServiceProvider provider) =>
            provider.Resolve_<IWebHostEnvironment>();

        public static HttpContext CurrentHttpContext(this IServiceProvider provider) =>
            provider.Resolve_<IHttpContextAccessor>().HttpContext ?? throw new ArgumentException(nameof(HttpContext));

        public static bool IsInHttpContext(this IServiceProvider provider)
        {
            try
            {
                var context = provider.ResolveOptional_<IHttpContextAccessor>()?.HttpContext;
                return context != null;
            }
            catch
            {
                return false;
            }
        }

        public static string GetPagerHtml<T, EXT>(this PagerData<T, EXT> pager, Controller controller, string pageKey, int currentPage, int pageSize)
        {
            var p = new Dictionary<string, string>();
            var _context = controller.HttpContext;

            var kv = _context.Request.Query.ToDict().Where(x => ValidateHelper.IsNotEmpty(x.Key) && x.Key.ToLower() != pageKey.ToLower()).ToDictionary(x => x.Key, x => ConvertHelper.GetString(x.Value));

            p.AddDict(kv);

            return PagerHelper.GetPagerHtmlByData(
                url: _context.Request.PathBase,
                pageKey: pageKey,
                urlParams: p,
                itemCount: pager.ItemCount,
                page: currentPage,
                pageSize: pageSize);
        }

        /// <summary>
        /// 获取Area Controller Action
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public static (string area, string controller, string action) GetRouteInfo(this RouteData route)
        {
            var AreaName = ConvertHelper.GetString(route.Values["Area"]);
            var ControllerName = ConvertHelper.GetString(route.Values["Controller"]);
            var ActionName = ConvertHelper.GetString(route.Values["Action"]);
            return (AreaName, ControllerName, ActionName);
        }

        /// <summary>
        /// 设置请求ID
        /// </summary>
        public static void SetNewRequestID(this HttpContext context) =>
            context.Items["req_guid"] = Com.GetUUID();

        /// <summary>
        /// 获取请求ID
        /// </summary>
        /// <returns></returns>
        public static string GetRequestID(this HttpContext context) =>
            ConvertHelper.GetString(context.Items["req_guid"]);

        /// <summary>
        /// 获取类似/home/index的url
        /// </summary>
        public static string ActionUrl(this RouteData route)
        {
            var data = route.GetRouteInfo();
            var sp = new string[] { data.area, data.controller, data.action }.Where(x => ValidateHelper.IsNotEmpty(x));
            if (ValidateHelper.IsEmpty(sp))
                throw new MsgException("无法获取action访问路径");

            return $"/{string.Join("/", sp)}";
        }

        /// <summary>
        /// 获取上传文件的字节数组
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this IFormFile file)
        {
            using (var s = file.OpenReadStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                var bs = s.GetBytes();
                return bs;
            }
        }

        /// <summary>
        /// post和get数据的合并
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Dictionary<string, string> PostAndGet(this HttpContext context) =>
            context.QueryStringToDict().AddDict(context.PostToDict());

        /// <summary>
        /// get数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Dictionary<string, string> QueryStringToDict(this HttpContext context) =>
            context.Request.Query.ToDict();

        /// <summary>
        /// post数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Dictionary<string, string> PostToDict(this HttpContext context) =>
            context.Request.Form.ToDict();

        public static Dictionary<string, string> ToDict(this IEnumerable<KeyValuePair<string, StringValues>> data) =>
            data.ToDictionary(x => x.Key, x => (string)x.Value);
    }
}
