using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using WCloud.Member.Shared.Localization;

namespace WCloud.Member.Shared
{
    public class MemberShipPermissionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            LocalizableString __lang__(string name) => LocalizableString.Create<MemberShipResource>(name);

            var group = context.AddGroup("member-ship", __lang__("member-ship"));

            var admin_permission = group.AddPermission("manage-admin", __lang__("manage-admin"));
            admin_permission.AddChild("add-admin", __lang__("add-admin"));
            admin_permission.AddChild("delete-admin", __lang__("delete-admin"));

            var user_permission = group.AddPermission("manage-user", __lang__("manage-user"));
            user_permission.AddChild("add-user", __lang__("add-user"));
            user_permission.AddChild("delete-user", __lang__("delete-user"));
            user_permission.AddChild("view-user", __lang__("view-user"));
            user_permission.AddChild("reset-pwd", __lang__("reset-pwd"));

            var dept_permission = group.AddPermission("manage-dept", __lang__("manage-dept"));

            var permission_permission = group.AddPermission("manage-permission", __lang__("manage-permission"));

            var role_permission = group.AddPermission("manage-role", __lang__("manage-role"));
            role_permission.AddChild("role-edit", __lang__("role-edit"));

            var org_permission = group.AddPermission("manage-org", __lang__("manage-org"));
            org_permission.AddChild("org-edit");
        }
    }
}
