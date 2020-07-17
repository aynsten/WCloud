using Lib.ioc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCloud.Member.Application.PermissionValidator
{
    /// <summary>
    /// 再租户内是否有权限
    /// </summary>
    public interface IOrgPermissionValidatorService : IAutoRegistered
    {
        Task<bool> HasOrgPermission(string org_uid, string subject_id, string permission_uid);

        Task<bool> HasAnyOrgPermission(string org_uid, string subject_id, IEnumerable<string> permission_uid);

        Task<bool> HasAllOrgPermission(string org_uid, string subject_id, IEnumerable<string> permission_uid);

        Task<string[]> LoadAllOrgPermission(string org_uid, string subject_id);
    }
}
