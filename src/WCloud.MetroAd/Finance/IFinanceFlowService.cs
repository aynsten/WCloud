using Lib.helper;
using Lib.ioc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Service;
using static WCloud.MetroAd.Finance.FinanceFlowService;

namespace WCloud.MetroAd.Finance
{
    public interface IFinanceFlowService : IBasicService<FinanceFlowEntity>, IAutoRegistered
    {
        Task<IEnumerable<YearMonthReport>> QueryFlowReport(
               DateTime? start_time_utc, DateTime? end_time_utc);

        Task<PagerData<FinanceFlowEntity>> QueryFlowByConditions(
               DateTime? start_time_utc, DateTime? end_time_utc, int? pay_method, int page, int pagesize);
    }
}
