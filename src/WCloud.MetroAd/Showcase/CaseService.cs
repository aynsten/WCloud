using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Helper;
using WCloud.Framework.Database.EntityFrameworkCore.Service;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Showcase
{
    public class CaseService : BasicService<CaseEntity>, ICaseService
    {
        private readonly IStringArraySerializer stringArraySerializer;
        public CaseService(IServiceProvider provider, IMetroAdRepository<CaseEntity> repo, IStringArraySerializer stringArraySerializer) : base(provider, repo)
        {
            this.stringArraySerializer = stringArraySerializer;
        }

        public async Task<IEnumerable<CaseEntity>> LoadAdWindows(IEnumerable<CaseEntity> list)
        {
            list.Should().NotBeNull();
            list.Any(x => x.AdWindowUIDList == null).Should().BeFalse();

            if (list.Any())
            {
                var adwindow_uids = list.SelectMany(x => x.AdWindowUIDList).Distinct().ToArray();

                var db = this._repo.Database;

                var data = adwindow_uids.Any() ?
                await db.Set<AdWindowEntity>().AsNoTracking().Where(x => adwindow_uids.Contains(x.UID)).ToArrayAsync() :
                new AdWindowEntity[] { };

                foreach (var m in data)
                {
                    m.ImageList = this.stringArraySerializer.Deserialize(m.ImageListJson);
                }

                foreach (var m in list)
                {
                    m.AdWindows = data.Where(x => m.AdWindowUIDList.Contains(x.UID)).ToArray();
                }
            }
            return list;
        }

        public async Task<IEnumerable<CaseEntity>> PrepareCase(IEnumerable<CaseEntity> list)
        {
            list.Should().NotBeNull();
            if (list.Any())
            {
                foreach (var m in list)
                {
                    m.AdWindowUIDList = this.stringArraySerializer.Deserialize(m.AdWindowUIDJson);
                    m.ImageList = this.stringArraySerializer.Deserialize(m.ImageJson);
                }
            }
            await Task.CompletedTask;
            return list;
        }

        protected override object UpdateField(CaseEntity data)
        {
            return new
            {
                data.Name,
                data.Desc,
                data.AdWindowUIDJson,
                data.ImageJson,
                data.IsActive
            };
        }
    }
}
