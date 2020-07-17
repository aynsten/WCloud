using Lib.helper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using WCloud.Framework.MVC.Extension;

namespace WCloud.Member.Authentication.Cookies
{
    public class MyCookieManager : ICookieManager
    {
        /// <summary>
        /// 存储关键数据的key
        /// </summary>
        private readonly string _key;
        public MyCookieManager(string _key)
        {
            this._key = _key ?? throw new ArgumentNullException(nameof(_key));
        }

        public void AppendResponseCookie(HttpContext context, string key, string value, CookieOptions options)
        {
            context.Response.Cookies.SetCookie_(key, value, option: options);
        }

        public void DeleteCookie(HttpContext context, string key, CookieOptions options)
        {
            context.Response.Cookies.DeleteCookie_(key, option: options);
        }

        public string GetRequestCookie(HttpContext context, string key)
        {
            var value = context.Request.Cookies.GetCookie_(key, string.Empty);
            if (key == this._key && ValidateHelper.IsEmpty(value))
                return "这个字符串不能为空，如果为空就不会走到auth handler里的authentication方法了";

            return value;
        }
    }
}
