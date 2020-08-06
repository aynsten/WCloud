using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Lib.core;
using Lib.extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WCloud.CommonService.Application.FileUpload;
using WCloud.Core.MessageBus;
using WCloud.Framework.MessageBus;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Framework.Redis;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.CommonService.Api.Controller
{
    /// <summary>
    /// 公用组件
    /// </summary>
    [CommonServiceRoute("admin")]
    public class CommonController : WCloudBaseController, IAdminController
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

        [HttpPost, ApiRoute]
        public async Task<IActionResult> test_time_consumer([FromServices] IMessagePublisher publisher, [FromForm] int? count)
        {
            count ??= 1;

            foreach (var m in Lib.helper.Com.Range(count.Value))
            {
                await publisher.PublishAsync(new TimeMessage()
                {
                    TimeUtc = DateTime.UtcNow
                });
            }

            return SuccessJson();
        }

        [HttpPost, ApiRoute]
        public async Task<IActionResult> redis_test([FromServices] RedisConnectionWrapper con)
        {
            var db = con.Connection.GetDatabase((int)WCloud.Core.ConfigSet.Redis.KV存储);

            var now = DateTime.UtcNow;

            db.SetAdd("set", "dd" + now);

            db.SortedSetAdd("sorted_set", "d" + now, now.Ticks);
            db.SortedSetAdd("sorted_set", "d3" + now, now.Ticks);

            db.HashSet("hash", "a" + now, now.ToString());

            //db.GeoAdd("position", 43, 54, "userid");
            //db.GeoRadius("position", "userid", 4, count: 100);
            //var res = db.GeoRadius("position", 4, 5, 7);

            /*
             redis计算“附近的人”。
             把地图分为很多的小方格，每个方格一个key。并且知道每个方格的中心点坐标。

                用我当前坐标查出我附近的方格。
                然后每个方格里查出离我最近的人。
                最后多个数据集聚合得出数据。

            数据上报的时候也是先查出我附近的方格。然后把我的位置写到附近的所有方格里

             */


            await Task.CompletedTask;

            return SuccessJson();
        }

    }
}
