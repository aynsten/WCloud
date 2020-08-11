using FluentAssertions;
using Lib.cache;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Core.Authentication.Roles;
using WCloud.Core.Cache;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Authentication.Filters;
using WCloud.Member.Authentication.OrgSelector;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Api.Controller
{
    /// <summary>
    /// 组织
    /// 以github为例，epc就是github，途虎在github中就等同于epc的客户。
    /// 途虎就是组织，组织下面是成员（这里的成员就是点检人员/保安）
    /// </summary>
    [MemberServiceRoute("admin")]
    public class AdminOrgController : WCloudBaseController, IAdminController
    {
        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager _keyManager;
        private readonly IOrgService _orgService;
        private readonly ICurrentOrgSelector _orgSelector;
        private readonly ILoginService<UserEntity> _userLogin;


        public AdminOrgController(ICacheProvider cache, ICacheKeyManager keyManager,
            IOrgService _orgService,
            ICurrentOrgSelector _orgSelector,
            ILoginService<UserEntity> _userLogin)
        {
            this._cache = cache;
            this._keyManager = keyManager;
            this._orgService = _orgService;
            this._orgSelector = _orgSelector;
            this._userLogin = _userLogin;

        }

        /// <summary>
        /// 查询组织
        /// </summary>
        [HttpPost, ApiRoute, AuthAdmin(Permission = "manage.org")]
        public async Task<IActionResult> Query(string q, int? page, bool? isremove)
        {
            page = CheckPage(page);

            var pager = await this._orgService.QueryOrgPager(q, page.Value, this.PageSize, isremove: (isremove ?? false).ToBoolInt());

            pager.DataList = await this._orgService.LoadOwners(pager.DataList);

            var data = new
            {
                pageNo = pager.Page,
                pageSize = pager.PageSize,
                totalCount = pager.ItemCount,
                totalPage = pager.PageCount,//Math.Ceiling((double)pager.ItemCount / pager.PageSize),
                data = pager.DataList.Select(x => new
                {
                    x.Id,
                    x.OrgName,
                    x.OrgDescription,
                    x.OrgWebSite,
                    x.Phone,
                    x.MemeberCount,
                    x.ExpiredTimeUtc,
                    x.CreateTimeUtc,
                    Owner = x.Owners.Select(m => new
                    {
                        m.Id,
                        m.UserName,
                        m.NickName,
                        m.UserImg
                    })
                })
            };

            var res = new _().SetSuccessData(data);
            return GetJson(res);
        }

        /// <summary>
        /// 添加或者更新组织
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = "manage.org")]
        public async Task<IActionResult> Save([FromForm]string data)
        {
            var model = this.JsonToEntity_<OrgEntity>(data);

            var loginuser = await this.GetLoginAdminAsync();

            model.UserUID = loginuser.UserID;

            if (ValidateHelper.IsNotEmpty(model.Id))
            {
                var res = await this._orgService.UpdateOrg(model);

                res.ThrowIfNotSuccess();
            }
            else
            {
                var res = await this._orgService.AddOrg(model);

                res.ThrowIfNotSuccess();
            }

            return SuccessJson();
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, AuthAdmin(Permission = "manage.org")]
        public async Task<IActionResult> Delete(string uid)
        {
            var admin = await this.GetLoginAdminAsync();

            await this._orgService.ActiveOrDeActiveOrg(uid, false);

            return SuccessJson();
        }

        /// <summary>
        /// 获取所有角色和响应的值
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        [AuthAdmin]
        public async Task<IActionResult> QueryMemberRoles()
        {
            await Task.CompletedTask;

            var data = MemberRoleHelper.GetRoles();

            return GetJson(new _()
            {
                Success = true,
                Data = data.Select(x => new { x.Key, x.Value }).ToList()
            });
        }

        /// <summary>
        /// 添加默认管理员
        /// </summary>
        [HttpPost, ApiRoute, AuthAdmin]
        public async Task<IActionResult> AddAdminMember(string phone, string org_uid)
        {
            var org = await this._orgService.GetOrgByUID(org_uid);
            org.Should().NotBeNull();

            var map = new UserEntity()
            {
                UserName = phone + "@" + org.OrgName,
                PassWord = "123"
            };
            var reg_res = await this._userLogin.AddAccount(map);
            reg_res.ThrowIfNotSuccess();
            var user = reg_res.Data;
            var map_orgmember = new OrgMemberEntity()
            {
                OrgUID = org_uid,
                UserUID = user.Id,
                IsOwner = 1,
                MemberApproved = 1,
            };
            var res = await this._orgService.AddMember(map_orgmember);
            res.ThrowIfNotSuccess();

            return SuccessJson();
        }

        /// <summary>
        /// 移除管理员
        /// </summary>
        [HttpPost, ApiRoute, AuthAdmin]
        public async Task<IActionResult> RemoveOwner(string org_uid, string user_uid)
        {
            if (!ValidateHelper.IsAllNotEmpty(org_uid, user_uid))
                throw new NoParamException();

            var loginuser = await this.GetLoginAdminAsync();

            await this._orgService.RemoveOwner(org_uid, user_uid);

            return SuccessJson();
        }
    }
}