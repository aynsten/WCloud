using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Domain.User;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("user")]
    public class UserInfoController : WCloudBaseController, IUserController
    {
        private readonly IUserService userService;

        public UserInfoController(IUserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// 更新个人信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> UpdateProfile(string data)
        {
            var model = this.JsonToEntity_<UserEntity>(data);

            var loginuser = await this.GetLoginUserAsync();

            model.UID = loginuser.UserID;

            var res = await this.userService.UpdateUser(model);

            res.ThrowIfNotSuccess();

            return SuccessJson();
        }

        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="avatar_url"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> UpdateAvatar(string avatar_url)
        {
            var loginuser = await this.GetLoginUserAsync();

            var res = await this.userService.UpdateUserAvatar(loginuser.UserID, avatar_url);

            res.ThrowIfNotSuccess();

            return SuccessJson();
        }
    }
}
