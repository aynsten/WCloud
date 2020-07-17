using WCloud.Member.Application.Entity;
using Lib.ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCloud.Member.Application.Service
{
    public interface IMenuService : IAutoRegistered
    {
        Task<_<MenuEntity>> AddMenu(MenuEntity model);

        Task<MenuEntity> GetMenuByUID(string uid);

        Task<bool> DeleteMenuWhenNoChildren(string uid);

        Task<List<MenuEntity>> QueryMenuList(string group_key, string parent = null);

        Task UpdateMenu(MenuEntity model);
    }
}
