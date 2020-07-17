using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroLineController : BasicServiceController<IMetroLineService, MetroLineEntity>, IAdminController
    {
        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Save([FromForm] string data)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            this.AfterSave = async update =>
            {
                var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("线路管理", update).WithExtraData(new { data });
                await this.AddOperationLog(olog);
            };

            return await base.Save(data);
        }

        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Delete([FromForm] string uid)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            var olog = new OperationLogMessage(loginadmin).Delete("线路管理").WithExtraData(new { uid });
            await this.AddOperationLog(olog);

            return await base.Delete(uid);
        }
    }
}
