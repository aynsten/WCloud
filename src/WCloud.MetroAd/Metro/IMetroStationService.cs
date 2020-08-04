using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Metro
{
    public interface IMetroStationService : IBasicService<MetroStationEntity>, IAutoRegistered
    {
        Task<IEnumerable<MetroStationEntity>> QueryByLine(string line_uid);
    }
}
