using FluentAssertions;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Statistic
{
    public class AdWindowUsageGroupByDay
    {
        public string LineName { get; set; }
        public string StationName { get; set; }
        public string AdWindowName { get; set; }
        public int? StationType { get; set; }

        public string AdWindowUID { get; set; }
        public decimal Price { get; set; }
        public int Days { get; set; }
    }

    public class AdWindowStatisticService : IAdWindowStatisticService
    {
        private readonly IMetroAdRepository<AdWindowUsageEntity> repo;
        private readonly IMetroAdDbFactory _dbFactory;

        public AdWindowStatisticService(
            IMetroAdRepository<AdWindowUsageEntity> repo,
            IMetroAdDbFactory metroAdDbFactory)
        {
            this.repo = repo;
            this._dbFactory = metroAdDbFactory;
        }

        public async Task<IEnumerable<AdWindowUsageGroupByDay>> QueryByTime(
            DateTime start_utc, DateTime end_utc, string station_uid, string line_uid, int? station_type)
        {
            (start_utc < end_utc).Should().BeTrue();

            var query = this.repo.Table.AsNoTracking();

            query = query.Where(x => x.DateUtc >= start_utc && x.DateUtc < end_utc);

            query = query.WhereIf(ValidateHelper.IsNotEmpty(line_uid), x => x.LineUID == line_uid);
            query = query.WhereIf(station_type != null, x => x.StationType == station_type.Value);
            query = query.WhereIf(ValidateHelper.IsNotEmpty(station_uid), x => x.StationUID == station_uid);

            var data = await query.GroupBy(x => x.AdWindowUID).Select(x => new
            {
                x.Key,
                PriceSum = x.Sum(d => d.PriceInCent),
                Count = x.Count()
            }).ToArrayAsync();

            var res = data.Select(x => new AdWindowUsageGroupByDay()
            {
                AdWindowUID = x.Key,
                Days = x.Count,
                Price = (decimal)((double)x.PriceSum / 100.0)
            }).ToArray();

            return res;
        }

        public async Task<IEnumerable<AdWindowUsageGroupByDay>> LoadData(IEnumerable<AdWindowUsageGroupByDay> data)
        {
            data.Should().NotBeNull();
            if (data.Any())
            {
                var db = this.repo.Database;

                var adwindow_uids = data.Select(x => x.AdWindowUID).ToArray();

                var adwindows = await db.Set<AdWindowEntity>().AsNoTracking().Where(x => adwindow_uids.Contains(x.UID)).ToArrayAsync();

                var line_uids = adwindows.Select(x => x.MetroLineUID).ToArray();
                var station_uids = adwindows.Select(x => x.MetroStationUID).ToArray();
                var media_uids = adwindows.Select(x => x.MediaTypeUID).ToArray();

                var lines = await db.Set<MetroLineEntity>().AsNoTracking().Where(x => line_uids.Contains(x.UID)).ToArrayAsync();
                var stations = await db.Set<MetroStationEntity>().AsNoTracking().Where(x => station_uids.Contains(x.UID)).ToArrayAsync();

                foreach (var m in data)
                {
                    var window = adwindows.FirstOrDefault(x => x.UID == m.AdWindowUID);
                    if (window == null)
                        continue;
                    m.LineName = lines.FirstOrDefault(x => x.UID == window.MetroLineUID)?.Name;
                    var station = stations.FirstOrDefault(x => x.UID == window.MetroStationUID);
                    m.StationName = station?.Name;
                    m.StationType = station?.StationType;
                    m.AdWindowName = window.Name;
                }

            }
            return data;
        }
    }
}
