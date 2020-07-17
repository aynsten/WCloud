using Microsoft.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore.AbpDatabase;
using WCloud.Member.Domain;
using WCloud.Member.Domain.Admin;
using WCloud.Member.Domain.Employee;
using WCloud.Member.Domain.Login;
using WCloud.Member.Domain.Permission;
using WCloud.Member.Domain.Tenant;
using WCloud.Member.Domain.User;
using WCloud.Member.Domain.Vendor;

namespace WCloud.Member.DataAccess.EF
{
    /// <summary>
    /// 账号体系数据库
    /// </summary>
    public class MemberShipDbContext : AbpDbContextBase<MemberShipDbContext>
    {
        /*
        private readonly IConfiguration _config;
        public MemberShipDbContext(IConfiguration config)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
        }*/

        public MemberShipDbContext(DbContextOptions<MemberShipDbContext> option) : base(option)
        {
            //
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            /*
            var _connection_string = this._config.GetMemberShipConnectionStringOrThrow();
            if (ValidateHelper.IsEmpty(_connection_string))
                throw new ArgumentException("未指定账号体系数据库连接字符串");

            optionsBuilder.UseMySql(_connection_string, option =>
            {
                option.CommandTimeout((int)TimeSpan.FromSeconds(3).TotalSeconds);
            });*/
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.AutoApplyEntityConfigurationForMySQL<MemberShipDbContext, IMemberShipDBTable>(modelBuilder);
        }

        //-----------------------------------后台管理账号
        public virtual DbSet<AdminEntity> AdminEntity { get; set; }

        //-----------------------------------角色权限
        public virtual DbSet<DepartmentEntity> DepartmentEntity { get; set; }

        public virtual DbSet<RoleEntity> RoleEntity { get; set; }
        public virtual DbSet<AdminRoleEntity> UserRoleEntity { get; set; }

        //-----------------------------------商户/员工

        public virtual DbSet<VendorEntity> VendorEntity { get; set; }
        public virtual DbSet<EmployeeEntity> EmployeeEntity { get; set; }

        //-----------------------------------用户/登陆

        public virtual DbSet<UserEntity> UserEntity { get; set; }
        public virtual DbSet<ValidationCodeEntity> ValidationCodeEntity { get; set; }
        public virtual DbSet<ExternalLoginMapEntity> ExternalLoginMapEntity { get; set; }
        public virtual DbSet<AuthTokenEntity> AuthTokenEntity { get; set; }
        public virtual DbSet<UserPhoneEntity> UserPhoneEntity { get; set; }

        //-----------------------------------组织/租户

        public virtual DbSet<OrgEntity> OrgEntity { get; set; }
        public virtual DbSet<OrgMemberEntity> OrgMemberEntity { get; set; }
        public virtual DbSet<OrgMemberRoleEntity> OrgMemberRoleEntity { get; set; }
        public virtual DbSet<OrgRoleEntity> OrgRoleEntity { get; set; }

        public virtual DbSet<PermissionEntity> PermissionEntity { get; set; }
    }
}
