using Lib.ioc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Metro
{
    public interface IMetroLineService : IBasicService<MetroLineEntity>, IAutoRegistered
    {
        Task<IEnumerable<MetroLineEntity>> AllStationAdWindows();
        Task<IEnumerable<MetroLineEntity>> RemoveLinesWithoutAdWindow(IEnumerable<MetroLineEntity> list);
    }
}
