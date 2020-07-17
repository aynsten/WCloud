using System;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.MetroAd.Event
{
    public class OperationLogService : BasicService<OperationLogEntity>, IOperationLogService
    {
        public OperationLogService(IServiceProvider provider, IMetroAdRepository<OperationLogEntity> repo) : base(provider, repo) { }
    }
}
