using Lib.ioc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Showcase
{
    public interface ICaseService : IBasicService<CaseEntity>, IAutoRegistered
    {
        Task<IEnumerable<CaseEntity>> PrepareCase(IEnumerable<CaseEntity> list);
        Task<IEnumerable<CaseEntity>> LoadAdWindows(IEnumerable<CaseEntity> list);
    }
}
