using Lib.helper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.MVC;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Service;
using WCloud.Member.Authentication.ControllerExtensions;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Api.Controller
{
    [MemberServiceRoute("admin")]
    public class AdminDeptController : WCloudBaseController, IAdminController
    {
        private readonly IDeptService _deptService;

        public AdminDeptController(IDeptService _deptService)
        {
            this._deptService = _deptService;
        }

        [NonAction]
        private object Parse(DepartmentEntity x)
        {
            return new
            {
                x.Id,
                x.NodeName,
                x.Description,
                x.ParentUID,
                x.Level,
            };
        }

        /// <summary>
        /// admin 部门查询
        /// </summary>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Query()
        {
            var data = await this._deptService.Query();

            var res = data.BuildAntTreeStructure(x => x.NodeName, x => this.Parse(x));

            return SuccessJson(res);
        }

        /// <summary>
        /// 添加或者修改部门
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Save([FromForm] string data)
        {
            var model = this.JsonToEntity_<DepartmentEntity>(data);

            if (ValidateHelper.IsNotEmpty(model.Id))
            {
                model.AsFirstLevelIfParentIsNotValid();
                var res = await this._deptService.Update(model);
                res.ThrowIfNotSuccess();
            }
            else
            {
                var res = await this._deptService.Add(model);
                res.ThrowIfNotSuccess();
            }

            return SuccessJson();
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        [HttpPost, ApiRoute]
        public async Task<IActionResult> Delete(string uid)
        {
            await this._deptService.DeleteWhenNoChildren(uid);

            return SuccessJson();
        }
    }
}
