using FluentAssertions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.MetroAd.Metro
{
    public class MetroStationService : BasicService<MetroStationEntity>, IMetroStationService
    {
        public MetroStationService(IServiceProvider provider, IMetroAdRepository<MetroStationEntity> repo) : base(provider, repo)
        {
            //
        }

        public async Task<IEnumerable<MetroStationEntity>> QueryByLine(string line_uid)
        {
            line_uid.Should().NotBeNullOrEmpty();

            var res = await this._repo.QueryManyAsync(x => x.MetroLineUID == line_uid, count: 5000);

            return res;
        }

        protected override object UpdateField(MetroStationEntity data)
        {
            return new
            {
                data.Name,
                data.Desc,
                data.StationType
            };
        }
    }
}
