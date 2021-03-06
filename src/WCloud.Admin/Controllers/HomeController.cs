using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Framework.MVC.Extension;
using WCloud.Member.Authentication.ControllerExtensions;

namespace WCloud.Admin.Controllers
{
    [AllowAnonymous]
    [AdminServiceRoute]
    public class HomeController : WCloudBaseController, IAdminController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger _logger;

        public HomeController(
            IWebHostEnvironment environment,
            ILogger<HomeController> logger)
        {
            this._environment = environment;
            this._logger = logger;
        }

        [HttpGet, ApiRoute]
        public IActionResult Index() => Content("ok");

        [HttpGet, ApiRoute]
        public IActionResult session()
        {
            HttpContext.Session.SetObjectAsJson("xx", DateTime.UtcNow);
            return Content("ok");
        }

        [HttpGet, ApiRoute]
        public async Task<IActionResult> Apm()
        {
            var transaction = Elastic.Apm.Agent
           .Tracer.StartTransaction("MyTransaction", Elastic.Apm.Api.ApiConstants.TypeRequest);
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1));

                var span = transaction.StartSpan("query_permission", Elastic.Apm.Api.ApiConstants.TypeDb);

                await Task.Delay(TimeSpan.FromSeconds(2));

                span.End();
                //application code that is captured as a transaction
            }
            catch (Exception e)
            {
                transaction.CaptureException(e);
                throw;
            }
            finally
            {
                transaction.End();
            }
            return SuccessJson();
        }
    }
}