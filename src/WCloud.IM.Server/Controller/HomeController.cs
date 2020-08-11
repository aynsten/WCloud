// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Authentication;

namespace WCloud.IM.Server.Controllers
{
    [AllowAnonymous]
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

        public IActionResult Index()
        {
            var x = this.HttpContext.__login_required__();
            return Content("ok");
        }

#if DEBUG
        public IActionResult log()
        {
            foreach (var m in Com.Range(1000))
            {
                this._logger.LogInformation(DateTime.Now.ToString());
            }
            this._logger.AddErrorLog("xx", new FileNotFoundException("/image/xx.png"));
            return Ok();
        }
#endif

    }
}