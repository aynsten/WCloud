using Microsoft.Extensions.Logging;
using WCloud.CommonService.Application.Infrastructure;
using WCloud.Framework.MVC.BaseController;

namespace WCloud.CommonService.Api.Controller
{
    [CommonServiceRoute("admin")]
    public class QueueController : BasicServiceController<IQueueJobService, QueueJobEntity>
    {
        private readonly ILogger _logger;
        public QueueController(ILogger<QueueController> logger)
        {
            this._logger = logger;
        }

        protected override object __parse__(QueueJobEntity x)
        {
            return new
            {
                x.UID,
                x.JobKey,
                x.Desc,
                x.Exchange,
                x.RoutingKey,
                x.Queue,
                x.ExtraData,
                x.ExceptionMessage,
                x.Status,
                x.StartTimeUtc,
                x.EndTimeUtc
            };
        }
    }
}
