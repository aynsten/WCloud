using WCloud.Core.Authentication.Roles;

namespace WCloud.Core
{
    /// <summary>
    /// 各种常量
    /// </summary>
    public static class ConfigSet
    {
        public static class Identity
        {
            /// <summary>
            /// 管理员登陆
            /// </summary>
            public const string AdminLoginScheme = "admin_login_scheme";

            /// <summary>
            /// 用户登陆
            /// </summary>
            public const string UserLoginScheme = "user_login_scheme";

            /// <summary>
            /// 外部登陆域
            /// </summary>
            public const string ExternalLoginScheme = "external_login_scheme";

            /// <summary>
            /// 微信外部登陆grant type
            /// </summary>
            public const string WechatGrantType = "wechat-mini-program";

            public const string AdminPwdGrantType = "admin-password";

            /// <summary>
            /// 初始化管理员
            /// </summary>
            public const string DefaultAdminUID = "default-admin-uid";

            /// <summary>
            /// 超级管理员
            /// </summary>
            public const string DefaultAdminRoleUID = "default-admin-role";

            /// <summary>
            /// 初始化管理员
            /// </summary>
            public const string DefaultAdminUserName = "admin-test";

            /// <summary>
            /// 初始化组织
            /// </summary>
            public const string DefaultOrgUID = "default-org-uid";
        }

        public static class MessageBus
        {
            public static class Queue
            {
                public const string Admin = "admin_queue";

                public const string User = "user_queue";

                public const string MetroAd = "metro_ad_queue";
            }
        }

        public enum Redis : int
        {
            缓存 = 1,
            加密KEY = 2,
            KV存储 = 3,
            分布式锁 = 4,
            发布订阅 = 5,
            消息队列 = 9,
        }

        public static class Roles
        {
            /// <summary>
            /// 管理员
            /// </summary>
            public static int ManagerRole => (int)(MemberRoleEnum.管理员);

            /// <summary>
            /// 管理员或者普通成员
            /// </summary>
            public static int MemberRole => (int)(MemberRoleEnum.管理员 | MemberRoleEnum.普通成员);

            /// <summary>
            /// 所有
            /// </summary>
            public static int AnyRole => (int)(MemberRoleEnum.管理员 | MemberRoleEnum.普通成员 | MemberRoleEnum.观察者);
        }
    }
}
