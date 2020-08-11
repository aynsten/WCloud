using FluentAssertions;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.CommonService.Application;
using WCloud.Framework.Database.Abstractions.Entity;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Application.Entity;

namespace WCloud.Member.Application.Service.impl
{
    public class MenuService : IMenuService
    {
        protected readonly ICommonServiceRepository<MenuEntity> _menuRepo;

        public MenuService(
            ICommonServiceRepository<MenuEntity> _menuRepo)
        {
            this._menuRepo = _menuRepo;
        }

        public virtual async Task<_<MenuEntity>> AddMenu(MenuEntity model)
        {
            model.Should().NotBeNull();

            return await this._menuRepo.AddTreeNode(model);
        }

        public virtual async Task<List<MenuEntity>> QueryMenuList(string group_key, string parent = null)
        {
            group_key.Should().NotBeNullOrEmpty("query menu list group key");

            var query = this._menuRepo.NoTrackingQueryable;
            query = query.Where(x => x.GroupKey == group_key);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(parent), x => x.ParentUID == parent);

            var res = await query.OrderBy(x => x.Sort).Take(5000).ToListAsync();
            return res;
        }

        public virtual async Task UpdateMenu(MenuEntity model)
        {
            model.Should().NotBeNull("update menu model");
            model.Id.Should().NotBeNullOrEmpty("update menu uid");

            var data = new _<MenuEntity>();
            var menu = await this._menuRepo.QueryOneAsync(x => x.Id == model.Id);
            menu.Should().NotBeNull("菜单不存在");

            menu.NodeName = model.NodeName;
            menu.PermissionJson = model.PermissionJson;

            menu.SetUpdateTime();

            await this._menuRepo.UpdateAsync(menu);
        }

        public virtual async Task<bool> DeleteMenuWhenNoChildren(string uid)
        {
            uid.Should().NotBeNullOrEmpty("delete menu uid");

            var res = await this._menuRepo.DeleteSingleNodeWhenNoChildren_(uid);
            return res;
        }

        public virtual async Task<MenuEntity> GetMenuByUID(string uid)
        {
            uid.Should().NotBeNullOrEmpty("get menu by uid,uid");

            var res = await this._menuRepo.QueryOneAsync(x => x.Id == uid);
            return res;
        }
    }
}
