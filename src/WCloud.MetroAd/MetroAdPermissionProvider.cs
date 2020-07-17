using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using WCloud.MetroAd.Localization;

namespace WCloud.MetroAd
{
    public class MetroAdPermissionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            LocalizableString __lang__(string name) => LocalizableString.Create<MetroadResource>(name);

            var group = context.AddGroup("metro-ad",__lang__("metro-ad"));

            var order_permission = group.AddPermission("metro-ad-order", __lang__("metro-ad-order"));
            order_permission.AddChild("metro-ad-order-view", __lang__("metro-ad-order-view"));
            order_permission.AddChild("metro-ad-order-edit", __lang__("metro-ad-order-edit"));

            var order_tf_permission = group.AddPermission("metro-ad-order-tf", __lang__("metro-ad-order-tf"));

            var ad_permission = group.AddPermission("metro-ad-manage", __lang__("metro-ad-manage"));
            ad_permission.AddChild("metro-ad-line", __lang__("metro-ad-line"));
            ad_permission.AddChild("metro-ad-station", __lang__("metro-ad-station"));
            ad_permission.AddChild("metro-ad-window", __lang__("metro-ad-window"));

            var showcase_permission = group.AddPermission("metro-ad-showcase", __lang__("metro-ad-showcase"));
            showcase_permission.AddChild("metro-ad-showcase-edit", __lang__("metro-ad-showcase-edit"));

            var finance_permission = group.AddPermission("metro-ad-finance", __lang__("metro-ad-finance"));
            finance_permission.AddChild("metro-ad-finance-view", __lang__("metro-ad-finance-view"));

            var operationlog_permission = group.AddPermission("metro-ad-operationlog", __lang__("metro-ad-operationlog"));
            operationlog_permission.AddChild("metro-ad-operationlog-view", __lang__("metro-ad-operationlog-view"));
        }
    }
}
