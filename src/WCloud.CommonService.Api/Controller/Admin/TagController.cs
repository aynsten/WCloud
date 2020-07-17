using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using WCloud.CommonService.Application.Tag;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.CommonService.Api.Controller
{
    [CommonServiceRoute("admin")]
    public class TagController : WCloudBaseController, IAdminController
    {
        private readonly ILogger _logger;
        private readonly ITagService _tagService;

        public TagController(ILogger<TagController> logger, ITagService tagService)
        {
            this._logger = logger;
            this._tagService = tagService;
        }

        /// <summary>
        /// 查询所有
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> QueryAll()
        {
            var data = await this._tagService.QueryAll();

            var res = data.OrderByDescending(x => x.ReferenceCount).Select(x => new
            {
                x.UID,
                x.TagName,
                x.Desc,
                x.Icon,
                x.Image,
                x.ReferenceCount,
                x.Group
            });

            return SuccessJson(res);
        }

        /// <summary>
        /// 添加或者更新
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Save([FromForm]string data)
        {
            var model = this.JsonToEntity_<TagEntity>(data);

            if (ValidateHelper.IsNotEmpty(model.UID))
            {
                var res = await this._tagService.UpdateTag(model);
                res.ThrowIfNotSuccess();
            }
            else
            {
                var res = await this._tagService.AddTag(model);
                res.ThrowIfNotSuccess();
            }

            return SuccessJson();
        }

        /// <summary>
        /// 删除标签
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Delete(string uid)
        {
            await this._tagService.DeleteTag(uid);

            return SuccessJson();
        }

        /// <summary>
        /// 迁移标签
        /// </summary>
        /// <param name="from_uid"></param>
        /// <param name="to_uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute, System.Obsolete]
        public async Task<IActionResult> MigrateTag(string from_uid, string to_uid)
        {
            await this._tagService.MigrateTag(from_uid, to_uid);

            this._logger.LogInformation($"标签迁移from {from_uid} to {to_uid}");

            return SuccessJson();
        }
    }
}
