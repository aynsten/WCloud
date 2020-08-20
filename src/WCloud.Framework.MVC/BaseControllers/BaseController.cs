using Lib.extension;
using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WCloud.Framework.MVC.BaseController
{
    public abstract class BaseController : Controller
    {
        [NonAction]
        protected virtual int? CheckPage(int? page)
        {
            page ??= 1;
            return page < 1 ? 1 : page;
        }

        #region 返回结果

        [NonAction]
        public override JsonResult Json(object data)
        {
            return base.Json(data, JsonHelper._setting);
        }

        [NonAction]
        protected virtual ActionResult GetJson(object obj)
        {
            var res = new JsonResult(obj);
            return res;
        }

        [NonAction]
        protected virtual ActionResult GetJsonp(object obj, string callback = "callback")
        {
            var func = (string)this.HttpContext.Request.Query[callback];

            return Content($"{func}({obj.ToJson()})", "text/javascript");
        }

        [NonAction]
        protected virtual ActionResult SuccessJson(object data = null)
        {
            var res = data == null ?
                new _().SetSuccessData(new { }) :
                new _().SetSuccessData(data);

            return GetJson(res);
        }

        [NonAction]
        protected virtual ActionResult GetJsonRes(string errmsg,
            string code = default(string),
            object data = null)
        {
            return GetJson(new _()
            {
                Success = ValidateHelper.IsEmpty(errmsg),
                ErrorMsg = errmsg,
                ErrorCode = code,
                Data = data
            });
        }

        /// <summary>
        /// 返回json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [NonAction]
        protected virtual ActionResult StringAsJson(string json)
        {
            return Content(json, contentType: "application/json");
        }
        #endregion
    }
}
