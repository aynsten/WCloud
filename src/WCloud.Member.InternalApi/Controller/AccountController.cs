using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.MVC.BaseController;
using WCloud.Member.Application.Login;
using WCloud.Member.Application.PermissionValidator;
using WCloud.Member.Application.Service;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.InternalApi.Controller
{
    [InternalMemberServiceRoute("account")]
    public class AccountController : WCloudBaseController
    {
        private readonly IWCloudContext _context;
        private readonly IAdminPermissionService adminPermissionService;
        private readonly IOrgPermissionService orgPermissionService;
        private readonly IAdminLoginService adminLoginService;
        private readonly IUserLoginService userLoginService;
        private readonly IOrgService orgService;
        public AccountController(IWCloudContext<AccountController> _context,
            IAdminPermissionService adminPermissionService,
            IOrgPermissionService orgPermissionService,
            IAdminLoginService adminLoginService,
            IUserLoginService userLoginService,
            IOrgService orgService)
        {
            this._context = _context;
            this.adminPermissionService = adminPermissionService;
            this.orgPermissionService = orgPermissionService;
            this.adminLoginService = adminLoginService;
            this.userLoginService = userLoginService;
            this.orgService = orgService;
        }

        [HttpPost("get-admin-permission")]
        public async Task<IActionResult> get_admin_permission([FromBody] string admin_id)
        {
            admin_id.Should().NotBeNullOrEmpty();

            var res = await this.adminPermissionService.LoadAllPermissionsName(admin_id);

            return SuccessJson(res);
        }

        [HttpPost("get-admin-login-info")]
        public async Task<IActionResult> get_admin_login_info([FromBody] string admin_id)
        {
            admin_id.Should().NotBeNullOrEmpty();

            var res = await this.adminLoginService.GetUserByUID(admin_id);

            var data = this._context.ObjectMapper.Map<AdminEntity, AdminDto>(res);

            return SuccessJson(res);
        }

        [HttpPost("get-user-org-mapping")]
        public async Task<IActionResult> get_user_org_mapping([FromBody] string user_id)
        {
            user_id.Should().NotBeNullOrEmpty();

            var res = await this.orgService.GetMyOrgMap(user_id);

            return SuccessJson(res);
        }

        [HttpPost("get-org-user-permission")]
        public async Task<IActionResult> get_org_user_permission([FromBody] string org_id, [FromBody] string user_id)
        {
            org_id.Should().NotBeNullOrEmpty();
            user_id.Should().NotBeNullOrEmpty();

            var res = await this.orgPermissionService.LoadAllOrgPermission(org_id, user_id);

            return SuccessJson(res);
        }

        [HttpPost("get-user-login-info")]
        public async Task<IActionResult> get_user_login_info([FromBody] string user_id)
        {
            user_id.Should().NotBeNullOrEmpty();

            var res = await this.userLoginService.GetUserByUID(user_id);

            return SuccessJson(res);
        }
    }
}
