
namespace WCloud.Core.Cache
{
    public interface ICacheKeyManager
    {
        string AdminPermission(string admin_uid);

        string AdminLoginInfo(string admin_uid);

        string AdminInfo(string admin_uid);

        string AuthHttpItemKey(string auth_type);

        string AuthToken(string token);

        string UserInfo(string user_uid);

        string UserLoginInfo(string user_uid);

        string UserOrgList(string user_uid);

        string UserOrgPermission(string org_uid, string user_uid);

        string AllSubSystem();

        string Html(string path);

        string AllStationsAdWindows();

        string ShowCase();

        string OrderCount(string user_uid);
    }
}
