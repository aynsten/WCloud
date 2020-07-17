using Lib.ioc;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Event
{
    public interface IOperationLogService : IBasicService<OperationLogEntity>, IAutoRegistered
    {
    }
}
