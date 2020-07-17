using System;

namespace WCloud.Member.Authentication.OrgSelector
{
    public interface ICurrentOrgSelector
    {
        string OrgCookieName { get; }

        string GetSelectedOrgUID(string specify = null);

        string GetSelectedOrgUIDOrThrow(string specify = null);

        void SetCookieOrgUID(string uid);
    }
}