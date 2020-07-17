using Lib.extension;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core;
using WCloud.Framework.Database.Abstractions.Extension;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Member.Application.Service;
using WCloud.Member.DataAccess.EF;
using WCloud.Member.Domain.Admin;

namespace WCloud.Member.Application.InitData
{
    public class InitDataHelper : IInitDataHelper
    {
        public const string admin_uid = ConfigSet.Identity.DefaultAdminUID;
        public const string admin_username = ConfigSet.Identity.DefaultAdminUserName;
        public const string admin_role_uid = ConfigSet.Identity.DefaultAdminRoleUID;

        private readonly IServiceProvider _provider;
        private readonly ILoginService<AdminEntity> _login;
        private readonly IMSRepository<AdminEntity> _repo;
        private readonly IPermissionService _permission;
        public InitDataHelper(IServiceProvider _provider, IMSRepository<AdminEntity> _repo, ILoginService<AdminEntity> _login, IPermissionService permission)
        {
            this._provider = _provider;
            this._repo = _repo;
            this._login = _login;
            this._permission = permission;
        }

        public void CreateAdmin()
        {
            var db = this._repo.Database;
            var user_set = db.Set<AdminEntity>();
            if (!user_set.AsNoTrackingQueryable().Any(x => x.UID == admin_uid))
            {
                Task.Run(async () =>
                {
                    var model = new AdminEntity()
                    {
                        UserName = admin_username,
                        NickName = admin_username,
                        PassWord = "123"
                    }.InitSelf("admin");

                    var res = await this._login.AddAccount(model, specific_uid: admin_uid);
                    res.ThrowIfNotSuccess();
                }).Wait();
            }
        }

        public void CreateAdminRole()
        {
            var db = this._repo.Database;

            var role_set = db.Set<RoleEntity>();

            role_set.RemoveRange(role_set.Where(x => x.UID == admin_role_uid));
            db.SaveChanges();

            var admin_role = new RoleEntity()
            {
                NodeName = "超级管理员",
                RoleDescription = "具有所有权限的超级管理员"
            };

            admin_role.AsFirstLevel().InitSelf("role");
            admin_role.UID = admin_role_uid;
            admin_role.PermissionJson = this._permission.AllPermissions().ToJson();

            role_set.Add(admin_role);

            db.SaveChanges();
        }

        public void SetAdminRoleForAdmin()
        {
            var db = this._repo.Database;
            var set = db.Set<AdminRoleEntity>();

            set.RemoveRange(set.Where(x => x.AdminUID == admin_uid && x.RoleUID == admin_role_uid));
            db.SaveChanges();

            var map = new AdminRoleEntity()
            {
                AdminUID = admin_uid,
                RoleUID = admin_role_uid,
            }.InitSelf("ur");

            set.Add(map);

            db.SaveChanges();
        }
    }
}
