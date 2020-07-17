using Lib.extension;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.MetroAd.Metro
{
    public class MetroLineService : BasicService<MetroLineEntity>, IMetroLineService
    {
        public MetroLineService(IServiceProvider provider, IMetroAdRepository<MetroLineEntity> repo) : base(provider, repo)
        {
            //
        }

        public async Task<IEnumerable<MetroLineEntity>> RemoveLinesWithoutAdWindow(IEnumerable<MetroLineEntity> list)
        {
            bool line_has_ad(MetroLineEntity line) => line.Stations.SelectMany(x => x.AdWindows).Any();
            bool station_has_ad(MetroStationEntity station) => station.AdWindows.Any();

            var res = list.Where(x => line_has_ad(x)).ToArray();
            foreach (var m in res)
            {
                m.Stations = m.Stations.Where(x => station_has_ad(x)).ToArray();
            }

            await Task.CompletedTask;
            return res;
        }

        public async Task<IEnumerable<MetroLineEntity>> AllStationAdWindows()
        {
            var db = this._repo.Database;

            var all_lines = await db.Set<MetroLineEntity>().AsNoTrackingQueryable().ToArrayAsync();

            var all_stations = await db.Set<MetroStationEntity>().AsNoTrackingQueryable().ToArrayAsync();

            var all_windows = await db.Set<AdWindowEntity>().AsNoTrackingQueryable().Where(x => x.IsActive > 0).ToArrayAsync();

            var all_media_types = await db.Set<MediaTypeEntity>().AsNoTracking().ToArrayAsync();

            foreach (var window in all_windows)
            {
                window.MediaType = all_media_types.FirstOrDefault(x => x.UID == window.MediaTypeUID);

                window.ImageListJson ??= "[]";
                window.ImageList = window.ImageListJson.JsonToEntity<string[]>(throwIfException: false) ?? new string[] { };
            }

            foreach (var station in all_stations)
            {
                station.AdWindows = all_windows.Where(x => x.MetroStationUID == station.UID).ToArray();
            }

            foreach (var line in all_lines)
            {
                line.Stations = all_stations.Where(x => x.MetroLineUID == line.UID).ToArray();
            }

            return all_lines;
        }

        protected override object UpdateField(MetroLineEntity data)
        {
            return new
            {
                data.Name,
                data.Desc
            };
        }
    }
}
