using Lib.cache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.MetroAd.Showcase;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute]
    public class CaseController : BasicServiceController<ICaseService, CaseEntity>
    {
        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryShowCase(
            [FromServices]ICacheProvider _cache,
            [FromServices]ICacheKeyManager cacheKeyManager)
        {
            var key = cacheKeyManager.ShowCase();

            var data = await _cache.GetOrSetAsync_(key, async () =>
            {
                var list = await this._service.QueryTop(10);
                list = await this._service.PrepareCase(list);
                list = await this._service.LoadAdWindows(list);

                return list;
            }, TimeSpan.FromMinutes(90));

            data ??= new CaseEntity[] { };

            var res = data.Where(x => x.IsActive > 0).Select(x => x).ToArray();

            return SuccessJson(res);
        }
    }
}
