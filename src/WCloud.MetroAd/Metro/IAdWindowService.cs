using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Metro
{
    public interface IAdWindowService : IBasicService<AdWindowEntity>, IAutoRegistered
    {
        Task<IEnumerable<AdWindowEntity>> QueryByStation(string station_uid);
    }
}
