using FluentAssertions;
using Lib.cache;
using Lib.core;
using Lib.extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Core.Cache;
using WCloud.Core.Helper;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Metro;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminMetroAdWindowController : BasicServiceController<IAdWindowService, AdWindowEntity>, IAdminController
    {
        private readonly IStringArraySerializer stringArraySerializer;
        private readonly IMessagePublisher messagePublisher;
        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager cacheKeyManager;

        public AdminMetroAdWindowController(IStringArraySerializer stringArraySerializer, IMessagePublisher messagePublisher,
            ICacheProvider _cache, ICacheKeyManager cacheKeyManager)
        {
            this.stringArraySerializer = stringArraySerializer;
            this.messagePublisher = messagePublisher;
            this._cache = _cache;
            this.cacheKeyManager = cacheKeyManager;
        }

        protected override object __parse__(AdWindowEntity x)
        {
            x.ImageListJson ??= "[]";
            x.ImageList = x.ImageListJson.JsonToEntity<string[]>(throwIfException: false) ?? new string[] { };

            return new
            {
                x.UID,
                x.Name,
                x.Desc,
                x.MediaTypeUID,
                x.MetroStationUID,
                x.ImageList,
                x.IsActive,
                x.Height,
                x.Width,
                x.PriceInCent,
                x.Price
            };
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryByStation([FromForm]string station_uid)
        {
            station_uid.Should().NotBeNullOrEmpty();

            var data = await this._service.QueryByStation(station_uid);

            var res = data.Select(this.__parse__).ToArray();

            return SuccessJson(res);
        }

        protected override async Task<AdWindowEntity> PrepareInsertData(AdWindowEntity model)
        {
            model.ImageList ??= new string[] { };
            model.ImageListJson = this.stringArraySerializer.Serialize(model.ImageList);

            var repo = this.HttpContext.RequestServices.Resolve_<IMetroAdRepository<MetroStationEntity>>();
            var station = await repo.GetFirstAsNoTrackAsync(x => x.UID == model.MetroStationUID);

            if (station == null)
                throw new MsgException("线路不存在");

            model.MetroLineUID = station.MetroLineUID;

            return model;
        }

        protected override async Task<AdWindowEntity> PrepareUpdateData(AdWindowEntity model)
        {
            model.ImageList ??= new string[] { };
            model.ImageListJson = this.stringArraySerializer.Serialize(model.ImageList);

            await Task.CompletedTask;

            return model;
        }

        async Task __remove_cache__()
        {
            var key = this.cacheKeyManager.AllStationsAdWindows();
            await this._cache.RemoveAsync(key);
        }

        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Save([FromForm] string data)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            this.AfterSave = async update =>
            {
                var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("橱窗管理", update).WithExtraData(new { data });
                await this.AddOperationLog(olog);
            };

            var res = await base.Save(data);

            await this.__remove_cache__();

            return res;
        }

        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Delete([FromForm] string uid)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            this.AfterSave = async update =>
            {
                var olog = new OperationLogMessage(loginadmin).UpdateOrAdd("橱窗管理", update).WithExtraData(new { uid });
                await this.AddOperationLog(olog);
            };

            var res = await base.Delete(uid);

            await this.__remove_cache__();

            return res;
        }

        /// <summary>
        /// 所有广告席位
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> AllAdWindows([FromServices]IMetroLineService _service)
        {
            var data = await _service.AllStationAdWindows();

            data = await _service.RemoveLinesWithoutAdWindow(data);

            var res = data.Select(x => x).ToArray();

            return SuccessJson(res);
        }
    }
}
