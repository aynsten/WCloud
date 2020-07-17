using Lib.extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Finance;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroFinanceFlowController : BasicServiceController<IFinanceFlowService, FinanceFlowEntity>, IAdminController
    {
        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryReport(DateTime? start_time_utc, DateTime? end_time_utc)
        {
            var admin = await this.GetLoginAdminAsync();

            var data = await this._service.QueryFlowReport(start_time_utc, end_time_utc);

            var res = data.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToArray();

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryByConditions(DateTime? start_time_utc, DateTime? end_time_utc, int? pay_method, int? page)
        {
            var admin = await this.GetLoginAdminAsync();

            page ??= 1;

            var data = await this._service.QueryFlowByConditions(start_time_utc, end_time_utc, pay_method, page.Value, this.PageSize);

            var res = data.PagerDataMapper_(x => x);

            return SuccessJson(res);
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> PayMethod()
        {
            var admin = await this.GetLoginAdminAsync();

            var data = typeof(WCloud.MetroAd.Order.PayMethodEnum).GetEnumFieldsValues();

            var res = data.Select(x => new { x.Key, x.Value }).ToArray();

            return SuccessJson(res);
        }
    }
}
