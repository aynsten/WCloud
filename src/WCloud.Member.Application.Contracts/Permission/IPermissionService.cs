using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Authorization.Permissions;

namespace WCloud.Member.Application.Service
{
    public interface IPermissionService : IAutoRegistered
    {
        IPermissionDefinitionManager AbpPermissionDefinition { get; }
        string[] AllPermissions();
    }
}
