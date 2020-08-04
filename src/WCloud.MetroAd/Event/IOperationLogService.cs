using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Event
{
    public interface IOperationLogService : IBasicService<OperationLogEntity>, IAutoRegistered
    {
    }
}
