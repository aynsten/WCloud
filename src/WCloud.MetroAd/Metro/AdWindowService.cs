using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.MetroAd.Metro
{
    public class AdWindowService : BasicService<AdWindowEntity>, IAdWindowService
    {
        public AdWindowService(IServiceProvider provider, IMetroAdRepository<AdWindowEntity> repo) : base(provider, repo)
        {
            //
        }

        public async Task<IEnumerable<AdWindowEntity>> QueryByStation(string station_uid)
        {
            station_uid.Should().NotBeNullOrEmpty();

            var res = await this._repo.QueryManyAsync(x => x.MetroStationUID == station_uid, count: 5000);

            return res;
        }

        protected override object UpdateField(AdWindowEntity data)
        {
            return new
            {
                data.Name,
                data.Desc,
                data.Height,
                data.Width,
                data.PriceInCent,
                data.MediaTypeUID,
                data.ImageListJson,
                data.IsActive,
            };
        }
    }
}
