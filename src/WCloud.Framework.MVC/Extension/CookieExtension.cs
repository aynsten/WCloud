using Microsoft.AspNetCore.Http;

namespace WCloud.Framework.MVC.Extension
{
    public static class CookieExtension
    {
        public static string GetCookie_(this IRequestCookieCollection cookie, string key, string deft = default(string))
            => cookie[key] ?? deft;

        public static IResponseCookies SetCookie_(this IResponseCookies cookie, string key, string value,
            CookieOptions option = null)
        {
            if (option == null)
                cookie.Append(key, value);
            else
                cookie.Append(key, value, option);
            return cookie;
        }

        public static IResponseCookies DeleteCookie_(this IResponseCookies cookie, string key, CookieOptions option = null)
        {
            if (option == null)
                cookie.Delete(key);
            else
                cookie.Delete(key, option);
            return cookie;
        }
    }
}
