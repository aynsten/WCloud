using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.CommonService.Application.Message;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.CommonService.Api.Controller
{
    [CommonServiceRoute("admin")]
    public class MessageController : BasicServiceController<IUserMessageService, UserMessageEntity>, IAdminController
    {
        private readonly ILogger _logger;
        public MessageController(ILogger<MessageController> _logger)
        {
            this._logger = _logger;
        }

        protected override object __parse__(UserMessageEntity x)
        {
            return new
            {
                x.FromUID,
                x.UserUID,
                x.Message,
                x.AlreadyRead,
                x.CreateTimeUtc
            };
        }

        [NonAction]
        public override Task<IActionResult> QueryAll()
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public override Task<IActionResult> Save([FromForm] string data)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public override Task<IActionResult> Query(string q, int? page)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public override Task<IActionResult> GetByUID(string uid)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除消息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Delete([FromForm]string uid)
        {
            var loginuser = await this.GetLoginAdminAsync();

            var model = await this._service.GetByUID(uid);

            model.Should().NotBeNull();

            model.UserUID.Should().Be(loginuser.UserID);

            return await base.Delete(uid);
        }
    }
}
