using Bogus;
using Lib.extension;
using Lib.helper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.Application;
using WCloud.Member.Application.Service;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.Initialization
{
    public static class MemberInitExtension
    {
        public const string default_org_uid = ConfigSet.Identity.DefaultOrgUID;
        public const string admin_uid = ConfigSet.Identity.DefaultAdminUID;
        public const string admin_username = ConfigSet.Identity.DefaultAdminUserName;
        public const string admin_role_uid = ConfigSet.Identity.DefaultAdminRoleUID;

        public static async Task InitAdminRoles(this IServiceProvider app)
        {
            using var s = app.CreateScope();

            var model = new AdminEntity()
            {
                UserName = admin_username,
                NickName = admin_username,
                PassWord = "123"
            }.InitEntity();

            var _login = s.ServiceProvider.Resolve_<ILoginService<AdminEntity>>();
            var res = await _login.AddAccount(model, specific_uid: admin_uid);
            res.ThrowIfNotSuccess();
        }

        /// <summary>
        /// 创建测试用户
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static async Task InitAdminUsers(this IServiceProvider app)
        {
            using var s = app.CreateScope();
            var roleRepository = s.ServiceProvider.Resolve_<IRoleRepository>();
            var _permission = s.ServiceProvider.Resolve_<IPermissionService>();

            var role = roleRepository.QueryOne(x => x.Id == admin_role_uid);
            if (role == null)
            {
                var admin_role = new RoleEntity()
                {
                    NodeName = "超级管理员",
                    RoleDescription = "具有所有权限的超级管理员"
                };

                admin_role.AsFirstLevel().InitEntity();
                admin_role.SetId(admin_role_uid);
                admin_role.PermissionJson = _permission.AllPermissions().ToJson();

                await roleRepository.InsertAsync(admin_role);
            }
            else
            {
                role.PermissionJson = _permission.AllPermissions().ToJson();
                await roleRepository.UpdateAsync(role);
            }

            await roleRepository.SetUserRoles(admin_uid, new[] { admin_role_uid });
        }

        public static async Task InitOrgInfo(this IServiceProvider app)
        {
            using var s = app.CreateScope();
            var org_repo = s.ServiceProvider.Resolve_<IOrgRepository>();

            //如果组织为空就创建假数据
            if (!org_repo.Exist(x => x.Id == default_org_uid))
            {
                //创建组织
                var org = new OrgEntity()
                {
                    OrgName = "体验水司",
                    UserUID = admin_uid,
                }.InitEntity();

                org.SetId(default_org_uid);

                //新建默认租户
                await org_repo.InsertAsync(org);
            }
        }

        static OrgMemberEntity FakeMap(UserEntity m)
        {
            return new OrgMemberEntity()
            {
                OrgUID = default_org_uid,
                UserUID = m.Id,
                MemberApproved = 1,
                IsOwner = 0
            }.InitEntity();
        }
        static UserEntity FakeUser(string password)
        {
            var faker = new Faker("zh_CN");
            return new UserEntity()
            {
                UserName = faker.Phone.PhoneNumber("###########"),
                NickName = faker.Name.FullName(),
                UserSex = (int)faker.Person.Gender,
                PassWord = password,
            }.InitEntity();
        }

        public static async Task AddRandomUserAndJoinOrg(this IServiceProvider app, int count = 10)
        {
            using var s = app.CreateScope();
            var password = s.ServiceProvider.Resolve_<IPasswordHelper>().Encrypt("123");

            var org_repo = s.ServiceProvider.Resolve_<IOrgRepository>();
            var user_repo = s.ServiceProvider.Resolve_<IUserRepository>();
            if (!(await org_repo.AllActiveMembers(default_org_uid)).Any())
            {
                var users = Com.Range(count).Select(x => FakeUser(password)).ToArray();
                users.Last().UserName = "123";
                var maps = users.Select(FakeMap).ToArray();

                foreach (var u in users)
                {
                    await user_repo.InsertAsync(u);
                    var map = FakeMap(u);
                    await org_repo.AddMember(map);
                }
            }
        }
    }
}
