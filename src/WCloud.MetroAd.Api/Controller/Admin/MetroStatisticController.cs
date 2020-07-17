using FluentAssertions;
using Lib.cache;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Statistic;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroStatisticController : WCloudBaseController, IUserController
    {
        private readonly IAdWindowStatisticService adWindowStatisticService;
        private readonly ICacheProvider cacheProvider;

        public AdminMetroStatisticController(IAdWindowStatisticService adWindowStatisticService, ICacheProvider cacheProvider)
        {
            this.adWindowStatisticService = adWindowStatisticService;
            this.cacheProvider = cacheProvider;
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryAdWindowMonthlyUsage(string month, string line_uid, int? station_type, string station_uid, int? timespan)
        {
            timespan.Should().NotBeNull();

            if (ValidateHelper.IsEmpty(month))
                return GetJsonRes("请选择日期");
            if (!DateTime.TryParse(month, out var date))
                return GetJsonRes("日期解析错误");

            var start_utc = date.AddHours(-timespan.Value);
            var end_utc = start_utc.AddMonths(1);

            var data = await this.adWindowStatisticService.QueryByTime(start_utc, end_utc, station_uid, line_uid, station_type);

            data = await this.adWindowStatisticService.LoadData(data);

            var total_days = (end_utc - start_utc).TotalDays;
            total_days.Should().BeGreaterThan(0);

            var list = data.Select(x => new
            {
                x.AdWindowUID,
                x.AdWindowName,
                x.LineName,
                x.StationType,
                x.StationName,
                x.Price,
                x.Days,
                Rate = ((double)x.Days) / total_days
            }).ToArray();

            var res = new
            {
                days = total_days,
                data = list
            };

            return SuccessJson(res);
        }

    }
}
