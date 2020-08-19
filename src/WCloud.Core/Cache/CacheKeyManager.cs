using System;
using System.Text;
using Lib.cache;
using Microsoft.Extensions.Configuration;

namespace WCloud.Core.Cache
{
    /// <summary>
    /// 缓存key的统一管理
    /// </summary>
    public class CacheKeyManager : ICacheKeyManager
    {
        private readonly IConfiguration _config;
        private readonly string _prefix;
        public CacheKeyManager(IConfiguration config)
        {
            this._config = config;
            this._prefix = this._config["cache_key_prefix"] ?? "_";
        }

        public string AuthHttpItemKey(string auth_type)
        {
            var res = $"http.item.auth.{auth_type}".WithPrefix(this._prefix);
            return res;
        }

        public string AuthToken(string token)
        {
            var res = $"auth.token.{token}".WithPrefix(this._prefix);
            return res;
        }

        public string UserProfile(string user_uid)
        {
            var res = $"user.info.{user_uid}".WithPrefix(this._prefix);
            return res;
        }

        public string AdminPermission(string admin_uid)
        {
            var res = $"admin.permission.{admin_uid}".WithPrefix(this._prefix);
            return res;
        }

        public string UserOrgList(string user_uid)
        {
            var res = $"user.org_list.{user_uid}".WithPrefix(this._prefix);
            return res;
        }

        public string AllSubSystem()
        {
            var res = $"all.sub.system.list".WithPrefix(this._prefix);
            return res;
        }

        public string Html(string path)
        {
            var bs = Encoding.UTF8.GetBytes(path);
            var path_fingle_print = Convert.ToBase64String(bs);
            return $"html.path.{path_fingle_print}".WithPrefix(this._prefix);
        }

        public string AdminProfile(string admin_uid)
        {
            var res = $"admin.info.{admin_uid}".WithPrefix(this._prefix);
            return res;
        }

        public string UserOrgPermission(string org_uid, string user_uid)
        {
            var res = $"user.{user_uid}.org.{org_uid}.permission".WithPrefix(this._prefix);
            return res;
        }

        public string AllStationsAdWindows()
        {
            var res = $"metroad.allstation.and.adwindows".WithPrefix(this._prefix);
            return res;
        }

        public string ShowCase()
        {
            var res = $"metroad.showcase".WithPrefix(this._prefix);
            return res;
        }

        public string AdminLoginInfo(string admin_uid)
        {
            var res = $"login.admin.{admin_uid}".WithPrefix(this._prefix);
            return res;
        }

        public string UserLoginInfo(string user_uid)
        {
            var res = $"login.user.{user_uid}".WithPrefix(this._prefix);
            return res;
        }

        public string OrderCount(string user_uid)
        {
            var res = $"metro.ad.order.count.{user_uid}".WithPrefix(this._prefix);
            return res;
        }
    }
}
