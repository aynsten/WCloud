using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCloud.MetroAd.Statistic
{
    public interface IAdWindowStatisticService:IAutoRegistered
    {
        Task<IEnumerable<AdWindowUsageGroupByDay>> QueryByTime(
               DateTime start_utc, DateTime end_utc, string station_uid, string line_uid, int? station_type);
        Task<IEnumerable<AdWindowUsageGroupByDay>> LoadData(IEnumerable<AdWindowUsageGroupByDay> data);
    }
}
