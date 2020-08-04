using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.InitData
{
    public static class MetroadInitDataExtension
    {
        public static IServiceProvider InitMediaTypes(this IServiceProvider app)
        {
            using (var s = app.CreateScope())
            {
                var repo = s.ServiceProvider.Resolve_<IMetroAdRepository<MediaTypeEntity>>();
                if (!repo.Exist(x => x.Id >= 0))
                {
                    var data = new string[] { "视频", "文字", "声音", "图文", "图片", "其他" }
                    .Select(x => new MediaTypeEntity() { Name = x }.InitSelf()).ToArray();
                    repo.AddBulk(data);
                }
            }
            return app;
        }

        public static IServiceProvider InitMetroLinesAndStations(this IServiceProvider app)
        {
            using (var s = app.CreateScope())
            {
                var repo = s.ServiceProvider.Resolve_<IMetroAdRepository<MetroLineEntity>>();
                if (!repo.Exist(x => x.Id >= 0))
                {
                    var data = new string[] { "一号线", "二号线", "三号线", "四号线", "五号线", "六号线" }
                    .Select(x => new MetroLineEntity() { Name = x }.InitSelf()).ToArray();
                    repo.AddBulk(data);

                    var station_repo = s.ServiceProvider.Resolve_<IMetroAdRepository<MetroStationEntity>>();
                    var metro_line_uid = data.FirstOrDefault().UID;
                    var stations = new string[] { "莘庄", "外环路", "莲花路", "锦江乐园", "上海南站" }
                    .Select(x => new MetroStationEntity() { Name = x, MetroLineUID = metro_line_uid }.InitSelf()).ToArray();
                    station_repo.AddBulk(stations);

                    var ad_window_repo = s.ServiceProvider.Resolve_<IMetroAdRepository<AdWindowEntity>>();
                    var station_uid = stations.FirstOrDefault().UID;
                    var adwindows = new string[] { "橱窗1", "橱窗2", "橱窗3", "橱窗4" }.Select(x => new AdWindowEntity()
                    {
                        Name = x,
                        Desc = x,
                        MetroLineUID = metro_line_uid,
                        MetroStationUID = station_uid
                    }.InitSelf()).ToArray();
                    adwindows[0].UID = "123";
                    ad_window_repo.AddBulk(adwindows);
                }
            }
            return app;
        }
    }
}
