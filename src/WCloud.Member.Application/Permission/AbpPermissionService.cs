using Lib.extension;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace WCloud.Member.Application.Service.impl
{
    [Microsoft.Extensions.DependencyInjection.NotRegIoc]
    public class AbpPermissionService : IPermissionService
    {
        private readonly IPermissionDefinitionManager permissionService;
        private readonly IStringLocalizerFactory stringLocalizerFactory;

        public AbpPermissionService(
            IPermissionDefinitionManager permissionService,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            this.permissionService = permissionService;
            this.stringLocalizerFactory = stringLocalizerFactory;
        }

        public IPermissionDefinitionManager AbpPermissionDefinition => this.permissionService;

        public string[] AllPermissions()
        {
            var res = this.permissionService.GetPermissions().Select(x => x.Name).ToArray();
            return res;
        }

        string __lang_str__(ILocalizableString display_name) => display_name?.Localize(this.stringLocalizerFactory)?.Value;

        public async Task<object[]> BuildAntDesignTree()
        {
            await Task.CompletedTask;

            var list = new List<string>();
            object __permission_definition__(PermissionDefinition m)
            {
                list.AddOnceOrThrow(m.Name);
                var DisplayName = this.__lang_str__(m.DisplayName) ?? m.Name;
                return new
                {
                    key = m.Name,
                    title = DisplayName,
                    raw_data = new
                    {
                        UID = m.Name,
                        DisplayName
                    },
                    children = m.Children?.Select(x => __permission_definition__(x)).ToArray() ?? new object[] { }
                };
            }

            object __group_definition__(PermissionGroupDefinition x)
            {
                list.AddOnceOrThrow(x.Name);
                var DisplayName = this.__lang_str__(x.DisplayName) ?? x.Name;
                return new
                {
                    key = x.Name,
                    title = DisplayName,
                    raw_data = new
                    {
                        UID = x.Name,
                        DisplayName
                    },
                    children = x.Permissions?.Select(m => __permission_definition__(m)).ToArray() ?? new object[] { },
                };
            }

            var groups = this.AbpPermissionDefinition.GetGroups();

            var res = groups.Select(x => __group_definition__(x)).ToArray();

            return res;
        }
    }
}
