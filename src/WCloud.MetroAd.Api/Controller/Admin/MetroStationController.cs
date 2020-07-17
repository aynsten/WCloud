using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroStationController : BasicServiceController<IMetroStationService, MetroStationEntity>, IAdminController
    {
        private readonly IMessagePublisher messagePublisher;

        public AdminMetroStationController(IMessagePublisher messagePublisher)
        {
            this.messagePublisher = messagePublisher;
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryByLine([FromForm]string line_uid)
        {
            line_uid.Should().NotBeNullOrEmpty();

            var data = await this._service.QueryByLine(line_uid);

            var res = data.Select(this.__parse__).ToArray();

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Save([FromForm] string data)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            this.AfterSave = async update =>
            {
                var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("站点管理", update).WithExtraData(new { data });
                await this.AddOperationLog(olog);
            };

            var res = await base.Save(data);

            return res;
        }

        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Delete([FromForm] string uid)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            var olog = new OperationLogMessage(loginadmin).Delete("站点管理").WithExtraData(new { uid });
            await this.AddOperationLog(olog);

            return await base.Delete(uid);
        }
    }
}
