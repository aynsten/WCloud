using FluentAssertions;
using Lib.core;
using Lib.extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WCloud.CommonService.Application.FileUpload;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.CommonService.Api.Controller
{
    /// <summary>
    /// 公用组件
    /// </summary>
    [CommonServiceRoute("user")]
    public class UserCommonController : WCloudBaseController, IUserController
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="_uploadService"></param>
        /// <param name="file"></param>
        /// <param name="catalog"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Upload([FromServices] IFileUploadService _uploadService,
            IFormFile file = null,
            [FromForm] string catalog = null)
        {
            var files = this.Request.Form.Files.AsEnumerable_<IFormFile>().ToArray();
            files.Should().HaveCount(1);

            var f = files.First();
            if (f.Length <= 0)
            {
                throw new MsgException("文件为空");
            }

            var data = await _uploadService.Upload(f.GetAllBytes(), f.FileName, catalog);
            data.ThrowIfNotSuccess();

            var res = data.Data;

            return SuccessJson(res);
        }
    }
}
