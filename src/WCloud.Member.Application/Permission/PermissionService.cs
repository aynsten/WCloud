using System.Linq;
using Volo.Abp.Authorization.Permissions;

namespace WCloud.Member.Application.Service.impl
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionDefinitionManager permissionService;

        public PermissionService(IPermissionDefinitionManager permissionService)
        {
            this.permissionService = permissionService;
        }

        public IPermissionDefinitionManager AbpPermissionDefinition => this.permissionService;

        public string[] AllPermissions()
        {
            var res = this.permissionService.GetPermissions().Select(x => x.Name).ToArray();
            return res;
        }

    }
}
