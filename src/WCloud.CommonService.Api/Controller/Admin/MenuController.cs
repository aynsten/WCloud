using FluentAssertions;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Entity;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.CommonService.Api.Controller
{
    [CommonServiceRoute("admin")]
    public class MenuController : WCloudBaseController, IAdminController
    {
        private readonly IMenuService menuService;

        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService;
        }

        /// <summary>
        /// 查询菜单
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryMenuTree(string group)
        {
            group.Should().NotBeNullOrEmpty();
            var admin = await this.GetLoginAdminAsync();

            var data = await this.menuService.QueryMenuList(group);

            var res = data.BuildAntTreeStructure(x => x.NodeName);

            return SuccessJson(res);
        }

        /// <summary>
        /// 添加或者更新菜单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> SaveMenu(string data)
        {
            var model = this.JsonToEntity_<MenuEntity>(data);
            var admin = await this.GetLoginAdminAsync();

            if (ValidateHelper.IsEmpty(model.Id))
            {
                var res = await this.menuService.AddMenu(model);
                res.ThrowIfNotSuccess();
            }
            else
            {
                await this.menuService.UpdateMenu(model);
            }

            return SuccessJson();
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> DeleteMenu(string uid)
        {
            uid.Should().NotBeNullOrEmpty();
            var admin = await this.GetLoginAdminAsync();
            var res = await this.menuService.DeleteMenuWhenNoChildren(uid);
            if (!res)
            {
                return GetJsonRes("删除失败");
            }
            return SuccessJson();
        }
    }
}
