using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Http;
using System;
using WCloud.Core;
using WCloud.Framework.MVC.Extension;

namespace WCloud.Member.Authentication.OrgSelector
{
    public class MyCurrentOrgSelector : ICurrentOrgSelector
    {
        private readonly HttpContext _context;

        public MyCurrentOrgSelector(IServiceProvider provider)
        {
            this._context = provider.CurrentHttpContext();
        }

        public string OrgCookieName => "selected_org_uid";

        public string GetSelectedOrgUID(string specify = null)
        {
            var org_uid = new string[]
            {
                specify,
                this._context.Request.Headers[OrgCookieName],
                this._context.Request.Cookies.GetCookie_(OrgCookieName)
            }.FirstNotEmpty_();

            return org_uid;
        }

        public string GetSelectedOrgUIDOrThrow(string specify = null)
        {
            var org_uid = this.GetSelectedOrgUID(specify);

            if (ValidateHelper.IsEmpty(org_uid))
                throw new NoOrgException();

            return org_uid;
        }

        public void SetCookieOrgUID(string uid)
        {
            var option = new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow + TimeSpan.FromDays(300)
            };
            this._context.Response.Cookies.SetCookie_(OrgCookieName, uid, option);
        }
    }
}
