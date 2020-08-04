using Bogus;
using Lib.helper;
using Lib.ioc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using WCloud.Core;
using WCloud.Core.Authentication.Roles;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;
using WCloud.Member.Shared.Helper;

namespace WCloud.Member.Application.InitData
{
    public static class MemberInitExtension
    {
        public const string default_org_uid = ConfigSet.Identity.DefaultOrgUID;
        public const string admin_uid = ConfigSet.Identity.DefaultAdminUID;
        public const string admin_username = ConfigSet.Identity.DefaultAdminUserName;
        public const string admin_role_uid = ConfigSet.Identity.DefaultAdminRoleUID;

        public static IServiceProvider InitAdminRoles(this IServiceProvider app)
        {
            using (var s = app.CreateScope())
            {
                s.ServiceProvider.Resolve_<IInitDataHelper>().CreateAdminRole();
            }
            return app;
        }

        /// <summary>
        /// 创建测试用户
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IServiceProvider InitAdminUsers(this IServiceProvider app)
        {
            using (var s = app.CreateScope())
            {
                var dataHelper = s.ServiceProvider.Resolve_<IInitDataHelper>();

                dataHelper.CreateAdmin();
                dataHelper.SetAdminRoleForAdmin();
            }
            return app;
        }

        public static IServiceProvider InitOrgInfo(this IServiceProvider app)
        {
            using (var s = app.CreateScope())
            {
                var db = s.ServiceProvider.Resolve_<IMSRepository<UserEntity>>().Database;
                var org_set = db.Set<OrgEntity>();
                var org_member_set = db.Set<OrgMemberEntity>();
                //如果组织为空就创建假数据
                if (!org_set.Any(x => x.UID == default_org_uid))
                {
                    //创建组织
                    var org = new OrgEntity()
                    {
                        OrgName = "体验水司",
                        UserUID = admin_uid,
                    }.InitSelf();

                    org.UID = default_org_uid;

                    org_set.Add(org);
                    //新建默认租户
                    db.SaveChanges();
                }
                if (!org_member_set.Any(x => x.OrgUID == default_org_uid && x.UserUID == admin_uid))
                {
                    var map = new OrgMemberEntity()
                    {
                        IsOwner = 1,
                        OrgUID = default_org_uid,
                        UserUID = admin_uid,
                        MemberApproved = 1,
                        Flag = (int)MemberRoleEnum.管理员
                    }.InitSelf("auto-mb");
                    org_member_set.Add(map);
                    db.SaveChanges();
                }
            }
            return app;
        }

        public static IServiceProvider AddRandomUserAndJoinOrg(this IServiceProvider app, int count = 30)
        {
            OrgMemberEntity FakeMap(UserEntity m)
            {
                return new OrgMemberEntity()
                {
                    OrgUID = default_org_uid,
                    UserUID = m.UID,
                    MemberApproved = 1,
                    IsOwner = 0
                }.InitSelf("m");
            }

            using (var s = app.CreateScope())
            {
                var password = s.ServiceProvider.Resolve_<IPasswordHelper>().Encrypt("123");
                UserEntity FakeUser()
                {
                    var faker = new Faker("zh_CN");
                    return new UserEntity()
                    {
                        UserName = faker.Phone.PhoneNumber("###########"),
                        NickName = faker.Name.FullName(),
                        UserSex = (int)faker.Person.Gender,
                        PassWord = password,
                    }.InitSelf("user");
                }

                var db = s.ServiceProvider.Resolve_<IMSRepository<UserEntity>>().Database;
                var user_set = db.Set<UserEntity>();
                var map_set = db.Set<OrgMemberEntity>();
                if (map_set.AsNoTracking().Where(x => x.OrgUID == default_org_uid).Take(count).Count() < count)
                {
                    var users = Com.Range(count).Select(x => FakeUser()).ToArray();
                    users.Last().UserName = "123";
                    var maps = users.Select(FakeMap).ToArray();

                    user_set.AddRange(users);
                    map_set.AddRange(maps);

                    db.SaveChanges();
                }
            }
            return app;
        }
    }
}
