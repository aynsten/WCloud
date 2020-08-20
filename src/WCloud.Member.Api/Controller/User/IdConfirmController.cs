using FluentAssertions;
using WCloud.Core.Cache;
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Core.MessageBus;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Login;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Domain.Login;
using WCloud.Member.Shared.MessageBody;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("user")]
    public class UserIdConfirmController : WCloudBaseController, IUserController
    {
        private readonly ILogger _logger;
        private readonly ICacheKeyManager cacheKeyManager;
        private readonly ICacheProvider cacheProvider;
        private readonly IUserLoginService _login;
        private readonly IUserService userService;
        private readonly IConfiguration config;

        public UserIdConfirmController(
            ILogger<UserIdConfirmController> _logger,
            ICacheKeyManager cacheKeyManager,
            ICacheProvider cacheProvider,
            IUserLoginService _login,
            IUserService userService,
            IConfiguration config)
        {
            this._logger = _logger;
            this.cacheKeyManager = cacheKeyManager;
            this.cacheProvider = cacheProvider;
            this._login = _login;
            this.config = config;
            this.userService = userService;
        }

        /// <summary>
        /// 获取用户实名认证信息
        /// </summary>
        /// <param name="userService"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> UserIdCardInfo([FromServices]IUserService userService)
        {
            var loginuser = await this.GetLoginUserAsync();

            var user = await userService.GetUserByUID(loginuser.UserID);
            user.Should().NotBeNull();

            var res = new
            {
                user.IdCard,
                user.RealName,
                user.IdCardConfirmed,
            };

            return SuccessJson(res);
        }

        /// <summary>
        /// 检查实名认证是否通过
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> IsIdCardConfirmed()
        {
            var loginuser = await this.GetLoginUserAsync();

            var res = await this.userService.GetByUID(loginuser.UserID);
            res.Should().NotBeNull();

            var confirmed = res.IdCardConfirmed > 0;

            return SuccessJson(confirmed);
        }

        /// <summary>
        /// 提交实名认证信息
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="code"></param>
        /// <param name="idcard"></param>
        /// <param name="real_name"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> SetUserIdCard([FromServices]IUserService userService,
            [FromForm]string code, [FromForm]string idcard, [FromForm]string real_name)
        {
            code = "暂时不用";

            if (!ValidateHelper.IsAllNotEmpty(code, idcard, real_name))
                return GetJsonRes("请输入完整信息");

            if (!Regex.IsMatch(idcard, @"^(\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$", RegexOptions.IgnoreCase))
            {
                return GetJsonRes("请输入正确的身份证号码");
            }

            var loginuser = await this.GetLoginUserAsync();

            var data = await this._login.GetUserPhone(loginuser.UserID);
            var phone = data.FirstOrDefault()?.Phone;
            if (ValidateHelper.IsEmpty(phone))
                return GetJsonRes("用户未绑定手机，无法实名制");

            var after = DateTime.UtcNow.AddMinutes(-5);
            var code_model = await this._login.GetValidationCode(x =>
            x.UserUID == loginuser.UserID &&
            x.Phone == phone &&
            x.Code == code &&
            x.CreateTimeUtc > after &&
            x.CodeType == id_confirm_sms_type);

            if (code_model == null)
            {
                //return GetJsonRes("验证码错误");
            }

            var res = await userService.SetIdCard(loginuser.UserID, idcard, real_name);
            res.ThrowIfNotSuccess();

            var key = this.cacheKeyManager.UserLoginInfo(loginuser.UserID);
            await this.cacheProvider.RemoveAsync(key);

            return SuccessJson();
        }

        const string id_confirm_sms_type = "id_confirm";

        /// <summary>
        /// 发送实名认证验证码
        /// </summary>
        /// <param name="messagePublisher"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> SendOneTimeCode([FromServices]IMessagePublisher messagePublisher)
        {
            var loginuser = await this.GetLoginUserAsync();

            var data = await this._login.GetUserPhone(loginuser.UserID);
            var phone = data.FirstOrDefault()?.Phone;
            if (ValidateHelper.IsEmpty(phone))
                return GetJsonRes("用户未绑定手机，无法实名制");

            var ran = new Random((int)DateTime.UtcNow.Ticks);
            var code = string.Join(string.Empty, Com.Range(4).Select(x => ran.RealNext(9)).ToArray());

            //添加验证码
            await _login.AddVadlidationCode(new ValidationCodeEntity()
            {
                UserUID = loginuser.UserID,
                Phone = phone,
                Code = code,
                CodeType = id_confirm_sms_type
            });

            await messagePublisher.PublishAsync(new UserPhoneBindSmsMessage() { Phone = phone, Code = code });

            return SuccessJson();
        }
    }
}
