using FluentAssertions;
using Lib.cache;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Core.Validator;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Authentication.Filters;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("admin")]
    public class AdminInfoController : WCloudBaseController, IAdminController
    {
        const string per = "manage-admin";

        private readonly IAdminService _adminService;
        private readonly ILoginService<AdminEntity> _login;
        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager _keyManager;

        public AdminInfoController(
            IAdminService _userService,
            ILoginService<AdminEntity> _login,
            ICacheProvider _cache,
            ICacheKeyManager _keyManager)
        {
            this._adminService = _userService;
            this._login = _login;
            this._cache = _cache;
            this._keyManager = _keyManager;
        }

        [NonAction]
        object Parse(AdminEntity data)
        {
            return new
            {
                data.Id,
                data.UserName,
                data.NickName,
                data.UserImg,
                data.Sex,
                data.IsDeleted
            };
        }

        /// <summary>
        /// 用户名查询用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> GetUserByName([FromForm]string name)
        {
            var data = await this._adminService.GetUserByUserName(name);
            if (data != null)
            {
                var res = this.Parse(data);

                return SuccessJson(res);
            }
            return GetJsonRes("未找到用户");
        }

        /// <summary>
        /// 下拉框搜索用户
        /// </summary>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> SelectUser([FromForm]string q, [FromForm]string role_uid, [FromForm]string department_uid)
        {
            var data = await this._adminService.QueryTopUser(
                q: q,
                role_uid: ValidateHelper.IsNotEmpty(role_uid) ? new string[] { role_uid } : null,
                size: 20);

            var res = data.Select(this.Parse);

            return SuccessJson(res);
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> Query([FromForm]string name, [FromForm]string email, [FromForm]string q,
            [FromForm]bool? isremove, [FromForm]int? page)
        {
            page = CheckPage(page);

            var data = await this._adminService.QueryUserList(
                name: name,
                email: email,
                keyword: q,
                isremove: (isremove ?? false).ToBoolInt(),
                page: page.Value,
                pagesize: this.PageSize);

            data.DataList = await this._adminService.LoadRoles(data.DataList);

            var res = new
            {
                data.Page,
                data.PageCount,
                data.PageSize,
                data.ItemCount,
                Data = data.DataList.Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.NickName,
                    x.UserImg,
                    x.IsDeleted,
                    Roles = x.Roles.Select(m => new
                    {
                        m.Id,
                        m.NodeName
                    })
                })
            };

            return SuccessJson(res);
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> AddAdmin(
            [FromServices]IEntityValidationHelper<AdminEntity> validator, [FromForm] string data)
        {
            var model = this.JsonToEntity_<AdminEntity>(data);
            model.PassWord = "123";

            if (!validator.IsValid(model, out var msg))
                return GetJsonRes(msg);

            var res = await this._login.AddAccount(model);
            res.ThrowIfNotSuccess();
            return SuccessJson();
        }

        /// <summary>
        /// 禁用管理员
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> DeleteAdmin([FromForm]string uid)
        {
            await this._login.ActiveOrDeActiveUser(uid, false);
            return SuccessJson();
        }

        /// <summary>
        /// 恢复管理员
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = per)]
        public async Task<IActionResult> ActiveAdmin([FromForm]string uid)
        {
            await this._login.ActiveOrDeActiveUser(uid, true);
            return SuccessJson();
        }

        /// <summary>
        /// 个人资料
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Profile()
        {
            var loginuser = await this.GetLoginAdminAsync();

            var data = await this._adminService.GetUserByUID(loginuser.UserID);

            data.Should().NotBeNull();

            var res = this.Parse(data);

            return SuccessJson(res);
        }

        /// <summary>
        /// 更新个人资料
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> UpdateProfile([FromForm]string data)
        {
            var model = this.JsonToEntity_<AdminEntity>(data);

            var loginuser = await this.GetLoginAdminAsync();

            model.SetId(loginuser.UserID);

            await this._adminService.UpdateUser(model);

            var key = this._keyManager.AdminInfo(model.Id);
            await this._cache.RemoveAsync(key);

            return SuccessJson();
        }
    }
}