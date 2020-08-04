using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.CommonService.Application.Infrastructure
{
    public interface IQueueJobService:IBasicService<QueueJobEntity>,IAutoRegistered
    {
        //
    }
}
