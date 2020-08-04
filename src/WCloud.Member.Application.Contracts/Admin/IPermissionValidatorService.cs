using WCloud.Core.Apm;
using Lib.ioc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCloud.Member.Application.PermissionValidator
{
    /// <summary>
    /// 检查主体是否具有权限
    /// </summary>
    public interface IPermissionValidatorService : IAutoRegistered
    {
        /// <summary>
        /// 拥有权限
        /// </summary>
        /// <param name="subject_id"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [Apm]
        Task<bool> HasPermission(string subject_id, string permission);

        /// <summary>
        /// 拥有部分权限
        /// </summary>
        /// <param name="subject_id"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [Apm]
        Task<bool> HasAnyPermission(string subject_id, IEnumerable<string> permissions);

        /// <summary>
        /// 拥有全部权限
        /// </summary>
        /// <param name="subject_id"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        [Apm]
        Task<bool> HasAllPermission(string subject_id, IEnumerable<string> permissions);

        /// <summary>
        /// 为主体加载全部权限
        /// </summary>
        /// <param name="subject_id"></param>
        /// <returns></returns>
        [Apm]
        Task<string[]> LoadAllPermissionsName(string subject_id);
    }
}
