using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.MetroAd.Metro
{
    public interface IMediaTypeService : IBasicService<MediaTypeEntity>, IAutoRegistered
    {
    }
}
