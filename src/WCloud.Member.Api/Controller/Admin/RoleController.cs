using FluentAssertions;
using WCloud.Core.Cache;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Core.DataSerializer;
using WCloud.Core.MessageBus;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Authentication.Filters;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Shared.MessageBody;

namespace WCloud.Member.Api.Controller
{
    /// <summary>
    /// 角色
    /// </summary>
    [MemberServiceRoute("admin")]
    public class AdminRoleController : WCloudBaseController, IAdminController
    {
        const string per = "manage-role";

        private readonly ICacheProvider _cache;
        private readonly IRoleService _roleService;
        private readonly ICacheKeyManager _cacheKey;
        private readonly IMessagePublisher _publisher;
        private readonly IDataSerializer permissionSerializer;

        public AdminRoleController(
            ICacheProvider cache,
            IRoleService _roleService,
            ICacheKeyManager cacheKey,
            IMessagePublisher publisher,
            IDataSerializer permissionSerializer)
        {
            this._cache = cache;
            this._roleService = _roleService;
            this._cacheKey = cacheKey;
            this._publisher = publisher;
            this.permissionSerializer = permissionSerializer;
        }

        [NonAction]
        object ParseRole(RoleEntity x)
        {
            return new
            {
                x.Id,
                x.NodeName,
                x.RoleDescription,
                x.ParentUID,
                x.Level,
                PermissionUIDs = this.permissionSerializer.DeserializeArray(x.PermissionJson)
            };
        }

#if DEBUG
        [HttpPost, ApiRoute, AuthAdmin]
        public async Task<IActionResult> query_test()
        {
            var list = await this._roleService.QueryRoleList();
            var node = list.Where(x => x.Level == 1).FirstOrDefault();
            node.Should().NotBeNull();

            var deep_res = list.FindNodeChildrenRecursivelyDeepFirst__(node).Select(x => x.Key.NodeName).ToArray();
            var wide_res = list.FindNodeChildrenRecursivelyWideFirst__(node).Select(x => x.Key.NodeName).ToArray();

            var res = new
            {
                deep_res,
                wide_res
            };

            return SuccessJson(res);
        }
#endif

        /// <summary>
        /// ant design的树结构
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> QueryAntTree()
        {
            var list = await this._roleService.QueryRoleList();

            var res = list.BuildAntTreeStructure(x => x.NodeName, x => this.ParseRole(x));

            return SuccessJson(res);
        }

        /// <summary>
        /// 查询所有角色
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> QueryAll()
        {
            var list = await this._roleService.QueryRoleList();

            var res = list.Select(x => this.ParseRole(x));

            return SuccessJson(res);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> Delete([FromForm] string uid)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            await this._roleService.DeleteRoleWhenNoChildren(uid);

            return SuccessJson();
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> Save([FromForm] string data)
        {
            var model = this.JsonToEntity_<RoleEntity>(data);

            var loginadmin = await this.GetLoginAdminAsync();

            if (ValidateHelper.IsNotEmpty(model.Id))
            {
                var res = await this._roleService.UpdateRole(model);
                res.ThrowIfNotSuccess();
            }
            else
            {
                model.AsFirstLevelIfParentIsNotValid();
                var res = await this._roleService.AddRole(model);
                res.ThrowIfNotSuccess();
            }

            return SuccessJson();
        }

        /// <summary>
        /// 给用户设置角色
        /// </summary>
        /// <param name="user_uid"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> SetUserRole([FromForm] string user_uid, [FromForm] string role)
        {
            user_uid.Should().NotBeNullOrEmpty();
            var roles = this.JsonToEntity_<string[]>(role);

            var map = roles.Select(x => new AdminRoleEntity()
            {
                AdminUID = user_uid,
                RoleUID = x
            }).ToList();

            var loginadmin = await this.GetLoginAdminAsync();

            await this._roleService.SetUserRoles(user_uid, map);

            await this._publisher.PublishAsync(new UserRoleChangedMessage()
            {
                UserUID = user_uid
            });

            return SuccessJson();
        }

        /// <summary>
        /// 给角色设置权限
        /// </summary>
        /// <param name="role_uid"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> SetRolePermission([FromForm] string role_uid, [FromForm] string permission)
        {
            role_uid.Should().NotBeNullOrEmpty();

            var permission_data = this.JsonToEntity_<string[]>(permission);

            var loginadmin = await this.GetLoginAdminAsync();

            await this._roleService.SetRolePermissions(role_uid, permission_data);

            await this._publisher.PublishAsync(new RolePermissionUpdatedMessage() { RoleUID = role_uid });

            return SuccessJson();
        }

    }
}