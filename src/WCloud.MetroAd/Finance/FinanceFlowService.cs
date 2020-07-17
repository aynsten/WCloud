using FluentAssertions;
using Lib.extension;
using Lib.helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.MetroAd.Finance
{
    public class FinanceFlowService : BasicService<FinanceFlowEntity>, IFinanceFlowService
    {
        public FinanceFlowService(IServiceProvider provider, IMetroAdRepository<FinanceFlowEntity> repo) : base(provider, repo)
        {
            //
        }

        public class YearMonthReport
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public decimal Sum { get; set; }
        }

        public async Task<IEnumerable<YearMonthReport>> QueryFlowReport(
            DateTime? start_time_utc, DateTime? end_time_utc)
        {
            var repo = this._repo;

            var query = repo.Table.AsNoTracking();

            query = query.WhereIf(start_time_utc != null, x => x.CreateTimeUtc >= start_time_utc);
            query = query.WhereIf(end_time_utc != null, x => x.CreateTimeUtc < end_time_utc);

            var data = await query
                .GroupBy(x => new { x.TimeYear, x.TimeMonth })
                .Select(x => new
                {
                    x.Key.TimeYear,
                    x.Key.TimeMonth,
                    Sum = x.Sum(d => d.PriceInCent)
                }).ToArrayAsync();

            var res = data.Select(x => new YearMonthReport()
            {
                Year = x.TimeYear,
                Month = x.TimeMonth,
                Sum = ((decimal)x.Sum) / 100
            }).ToArray();

            return res;
        }

        public async Task<PagerData<FinanceFlowEntity>> QueryFlowByConditions(DateTime? start_time_utc, DateTime? end_time_utc,
            int? pay_method,
            int page, int pagesize)
        {
            page.Should().BeGreaterOrEqualTo(1);
            pagesize.Should().BeInRange(1, 5000);

            var repo = this._repo;

            var query = repo.Table.AsNoTracking();

            query = query.WhereIf(pay_method != null, x => x.PayMethod == pay_method.Value);
            query = query.WhereIf(start_time_utc != null, x => x.CreateTimeUtc >= start_time_utc);
            query = query.WhereIf(end_time_utc != null, x => x.CreateTimeUtc < end_time_utc);

            var data = new PagerData<FinanceFlowEntity>()
            {
                Page = page,
                PageSize = pagesize
            };

            data.ItemCount = await query.CountAsync();
            data.DataList = await query
                .OrderByDescending(x => x.OrderCreateTimeUtc).ThenByDescending(x => x.Id)
                .QueryPage(page, pagesize).ToListAsync();

            return data;
        }
    }
}
