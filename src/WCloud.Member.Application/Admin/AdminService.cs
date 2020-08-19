using FluentAssertions;
using Lib.helper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Shared.Admin;

namespace WCloud.Member.Application.Service.impl
{
    public class AdminService : IAdminService
    {
        private readonly IWCloudContext<AdminService> _context;
        private readonly IAdminRepository adminRepository;

        public AdminService(
            IWCloudContext<AdminService> _context,
            IAdminRepository adminRepository)
        {
            this._context = _context;
            this.adminRepository = adminRepository;
        }

        public virtual async Task UpdateUser(AdminEntity model)
        {
            model.Should().NotBeNull("admin service model");
            model.Id.Should().NotBeNullOrEmpty("admin service model uid");

            var data = new _<AdminEntity>();

            var user = await this.adminRepository.QueryOneAsync(x => x.Id == model.Id);
            user.Should().NotBeNull($"admin不存在:{model.Id}");

            user.SetField(new
            {
                model.UserImg,
                model.NickName,
                model.Sex,
                model.ContactPhone,
                model.ContactEmail
            });

            user.SetUpdateTime();

            await this.adminRepository.UpdateAsync(user);
        }

        public async Task<AdminEntity> GetUserByUID(string uid)
        {
            uid.Should().NotBeNullOrEmpty("admin service getuserbyuid");

            var res = await this.adminRepository.QueryOneAsync(x => x.Id == uid);
            return res;
        }

        public async Task<AdminDto> GetAdminById(string uid)
        {
            uid.Should().NotBeNullOrEmpty("admin service getuserbyuid");

            var data = await this.adminRepository.QueryOneAsync(x => x.Id == uid);
            data.Should().NotBeNull();

            var res = this._context.ObjectMapper.Map<AdminEntity, AdminDto>(data);

            return res;
        }

        public async Task<PagerData<AdminDto>> QueryAdmin(QueryAdminParameter filter, int page, int pagesize)
        {
            page.Should().BeGreaterOrEqualTo(1, "admin service page");
            pagesize.Should().BeInRange(1, 100, "admin service pagesize");

            var data = await this.adminRepository.QueryAdmin(filter, page, pagesize);

            var res = data.MapPagerData(x => this._context.ObjectMapper.Map<AdminEntity, AdminDto>(x));

            return res;
        }

        public async Task<PagerData<AdminEntity>> QueryUserList(string name = null, string email = null, string keyword = null, int? isremove = null, int page = 1, int pagesize = 20)
        {
            page.Should().BeGreaterOrEqualTo(1, "admin service page");
            pagesize.Should().BeInRange(1, 100, "admin service pagesize");

            var data = await this.adminRepository.QueryUserList(name, email, keyword, isremove, page, pagesize);

            return data;
        }

        public async Task<AdminEntity> GetUserByUserName(string name)
        {
            name.Should().NotBeNullOrEmpty("adminservice getuserbyusername");

            var res = await this.adminRepository.QueryOneAsync(x => x.UserName == name);
            return res;
        }

        public async Task<IEnumerable<AdminEntity>> LoadRoles(IEnumerable<AdminEntity> list)
        {
            list.Should().NotBeNull("load roles list");
            var data = await this.adminRepository.LoadRoles(list);

            var res = data;

            return res;
        }

        public async Task<_<AdminEntity>> AddAdmin(AdminEntity model)
        {
            model.Should().NotBeNull("adminservice add model");
            model.UserName.Should().NotBeNullOrEmpty("adminservice add model username");

            var res = new _<AdminEntity>();

            model.InitEntity();
            if (await this.adminRepository.ExistAsync(x => x.UserName == model.UserName))
            {
                return res.SetErrorMsg("用户名已存在");
            }

            await this.adminRepository.InsertAsync(model);

            return res.SetSuccessData(model);
        }

        public async Task<List<AdminEntity>> QueryTopUser(string q = null, string[] role_uid = null, int size = 20)
        {
            size.Should().BeInRange(1, 100, "query top user size");

            var data = await this.adminRepository.QueryTopUser(q, role_uid, size);

            var res = data;

            return res;
        }
    }
}
