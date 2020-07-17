using Lib.cache;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Admin.Message;
using WCloud.Core.Cache;
using WCloud.Core.Helper;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.MetroAd.Showcase;

namespace WCloud.MetroAd.Api.Controller
{
    [MetroAdServiceRoute("admin")]
    public class AdminCaseController : BasicServiceController<ICaseService, CaseEntity>, IAdminController
    {
        private readonly IStringArraySerializer stringArraySerializer;
        private readonly ICacheProvider _cache;
        private readonly ICacheKeyManager cacheKeyManager;

        public AdminCaseController(IStringArraySerializer stringArraySerializer, ICacheProvider _cache, ICacheKeyManager cacheKeyManager)
        {
            this.stringArraySerializer = stringArraySerializer;
            this._cache = _cache;
            this.cacheKeyManager = cacheKeyManager;
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryShowCase()
        {
            var data = await this._service.QueryTop(10);
            data = await this._service.PrepareCase(data);
            data = await this._service.LoadAdWindows(data);

            var res = data.Select(x => x).ToArray();

            return SuccessJson(res);
        }

        protected override Task<CaseEntity> PrepareInsertData(CaseEntity model)
        {
            model.ImageList ??= new string[] { };
            model.AdWindowUIDList ??= new string[] { };

            model.ImageJson = this.stringArraySerializer.Serialize(model.ImageList);
            model.AdWindowUIDJson = this.stringArraySerializer.Serialize(model.AdWindowUIDList);

            return base.PrepareInsertData(model);
        }

        protected override Task<CaseEntity> PrepareUpdateData(CaseEntity model)
        {
            model.ImageList ??= new string[] { };
            model.AdWindowUIDList ??= new string[] { };

            model.ImageJson = this.stringArraySerializer.Serialize(model.ImageList);
            model.AdWindowUIDJson = this.stringArraySerializer.Serialize(model.AdWindowUIDList);

            return base.PrepareUpdateData(model);
        }

        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Save([FromForm] string data)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            this.AfterSave = async update =>
            {
                var olog = new OperationLogMessage(loginadmin).PageAction("案例管理", update ? "更新" : "新建").WithExtraData(new { data });
                await this.AddOperationLog(olog);
            };

            var res = await base.Save(data);

            var key = this.cacheKeyManager.ShowCase();
            await this._cache.RemoveAsync(key);

            return res;
        }

        [HttpPost, ApiRoute]
        public override async Task<IActionResult> Delete([FromForm] string uid)
        {
            var loginadmin = await this.GetLoginAdminAsync();

            var olog = new OperationLogMessage(loginadmin).PageAction("橱窗管理", "删除").WithExtraData(new { uid });
            await this.AddOperationLog(olog);

            return await base.Delete(uid);
        }
    }
}
