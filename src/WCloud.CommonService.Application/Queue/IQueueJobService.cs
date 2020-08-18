using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.CommonService.Application.Queue
{
    public interface IQueueJobService:IBasicService<QueueJobEntity>,IAutoRegistered
    {
        //
    }
}
