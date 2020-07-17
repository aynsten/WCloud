using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCloud.CommonService.Application.KVStore;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Shared;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroSettingController : WCloudBaseController, IAdminController
    {
        private readonly IKVStoreService kVStoreService;
        public AdminMetroSettingController(IKVStoreService kVStoreService)
        {
            this.kVStoreService = kVStoreService;
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> GetOrderPeroid()
        {
            var data = await this.kVStoreService.GetValue<OrderPeroid[]>(OrderPeroid.order_peroid_key);
            data ??= new OrderPeroid[] { };

            data = this.__distinct__(data);

            var res = data.OrderBy(x => x.Peroid).Select(x => x).ToArray();

            return SuccessJson(res);
        }

        OrderPeroid[] __distinct__(OrderPeroid[] model)
        {
            model = model.Select(x => x.Peroid).Distinct().Select(x => new OrderPeroid() { Peroid = x }).ToArray();
            return model;
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> SetOrderPeroid(string data)
        {
            var admin = await this.GetLoginAdminAsync();

            var model = this.JsonToEntity_<OrderPeroid[]>(data);

            model = this.__distinct__(model);

            await this.kVStoreService.SetValue(OrderPeroid.order_peroid_key, model);

            return SuccessJson();
        }
    }
}
