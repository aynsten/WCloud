using Lib.cache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Cache;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute]
    public class AdController : WCloudBaseController, IUserController
    {
        /// <summary>
        /// 所有广告席位
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> AllAdWindows(
            [FromServices]ICacheProvider _cache,
            [FromServices]ICacheKeyManager _cacheKeyManager,
            [FromServices]IMetroLineService _service)
        {
            var key = _cacheKeyManager.AllStationsAdWindows();
            var data = await _cache.GetOrSetAsync_(key, _service.AllStationAdWindows, TimeSpan.FromMinutes(10));
            data ??= new MetroLineEntity[] { };

            data = await _service.RemoveLinesWithoutAdWindow(data);

            var res = data.Select(x => x).ToArray();

            return SuccessJson(res);
        }
    }
}
