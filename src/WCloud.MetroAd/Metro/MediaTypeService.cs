using System;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.MetroAd.Metro
{
    public class MediaTypeService : BasicService<MediaTypeEntity>, IMediaTypeService
    {
        public MediaTypeService(IServiceProvider provider, IMetroAdRepository<MediaTypeEntity> repo) : base(provider, repo)
        {
            //
        }
    }
}
