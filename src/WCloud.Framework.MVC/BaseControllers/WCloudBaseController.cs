using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WCloud.Framework.Filters;

namespace WCloud.Framework.MVC.BaseController
{
    public abstract class WCloudBaseController : BaseController
    {
        /// <summary>
        /// 默认分页
        /// </summary>
        protected virtual int PageSize => 10;

        [NonAction, Obsolete("用中间件或者exception handler拦截异常")]
        protected async Task<ActionResult> RunActionAsync(Func<Task<ActionResult>> GetActionFunc)
        {
            try
            {
                var data = await GetActionFunc.Invoke();

                return data;
            }
#if !xx
            catch (Exception e)
            {
                var err_handler = this.HttpContext.RequestServices.Resolve_<ICommonExceptionHandler>();
                var response = err_handler.Handle(e);
                return GetJson(response);
            }
#else
            catch (MsgException e)
            {
                return GetJsonRes(e.Message);
            }
            catch (NoParamException e)
            {
                return GetJson(new _()
                {
                    Success = false,
                    ErrorMsg = "参数错误",
                    ErrorCode = "-100",
                    Data = e.ResponseTemplate
                });
            }
            catch (NoOrgException)
            {
                return GetJson(new _()
                {
                    Success = false,
                    ErrorMsg = "请选择组织",
                    ErrorCode = "-111"
                });
            }
            catch (NoLoginException)
            {
                return GetJson(new _()
                {
                    Success = false,
                    ErrorMsg = "没有登录",
                    ErrorCode = "-401"
                });
            }
            catch (NoPermissionInOrgException)
            {
                return GetJson(new _()
                {
                    Success = false,
                    ErrorMsg = "没有权限",
                    ErrorCode = "-403"
                });
            }
            catch (NotImplementedException)
            {
                return GetJson(new _()
                {
                    Success = false,
                    ErrorMsg = "功能没有实现",
                    ErrorCode = "-1111"
                });
            }
            catch (Exception e)
            {
                e.AddErrorLog();
                return GetJsonRes("error");
            }
#endif
        }
    }
}
