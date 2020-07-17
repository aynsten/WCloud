using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Event;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroOperationLogController : BasicServiceController<IOperationLogService, OperationLogEntity>, IAdminController
    {
        //
    }
}
