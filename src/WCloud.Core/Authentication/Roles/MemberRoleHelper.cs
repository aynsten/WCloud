using System.Collections.Generic;
using System.Linq;
using Lib.extension;
using Lib.helper;

namespace WCloud.Core.Authentication.Roles
{
    /// <summary>
    /// 验证角色
    /// </summary>
    public static class MemberRoleHelper
    {
        public static bool IsValid(int flag) =>
            PermissionHelper.HasAnyPermission(flag, GetRoles().Select(x => x.Value).ToArray());

        public static Dictionary<string, int> GetRoles() =>
            typeof(MemberRoleEnum)
            .GetEnumFieldsValues()
            .ToDictionary(x => x.Key, x => (int)x.Value);

        public static List<string> ParseRoleNames(int flag, Dictionary<string, int> all_roles = null)
        {
            all_roles = all_roles ?? GetRoles();

            return all_roles.Where(x => PermissionHelper.HasPermission(flag, x.Value)).Select(x => x.Key).ToList();
        }
    }
}
