using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("admin")]
    public class UserManagementController : BasicServiceController<IUserService, UserEntity>, IAdminController
    {
        /// <summary>
        /// 查询user
        /// </summary>
        /// <param name="q"></param>
        /// <param name="page"></param>
        /// <param name="isremove"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Query_([FromForm] string q, [FromForm] int? page, [FromForm]bool? isremove)
        {
            var admin = await this.GetLoginAdminAsync();

            page = this.CheckPage(page);

            var data = await this._service.QueryUserList(isremove: isremove, page: page.Value, pagesize: this.PageSize);

            data.DataList = await this._service.LoadUserPhone(data.DataList);

            var res = data.PagerDataMapper_(this.__parse__);

            return SuccessJson(res);
        }
    }
}
