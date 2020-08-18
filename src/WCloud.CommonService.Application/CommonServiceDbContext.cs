using Microsoft.EntityFrameworkCore;
using WCloud.CommonService.Application.FileUpload;
using WCloud.CommonService.Application.KVStore;
using WCloud.CommonService.Application.Message;
using WCloud.CommonService.Application.Queue;
using WCloud.CommonService.Application.Tag;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore.AbpDatabase;
using WCloud.Member.Application.Entity;

namespace WCloud.CommonService.Application
{
    public class CommonServiceDbContext : AbpDbContextBase<CommonServiceDbContext>
    {
        public CommonServiceDbContext(DbContextOptions<CommonServiceDbContext> option) : base(option) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //自动为下面的dbset注册base entity的字段
            //然后查找当前程序集的所有mapper，依次注册
            this.AutoApplyEntityConfigurationForMySQL<CommonServiceDbContext, ICommonServiceEntity>(modelBuilder);
        }

        public virtual DbSet<UserMessageEntity> UserMessageEntity { get; set; }

        public virtual DbSet<QueueJobEntity> QueueJobEntity { get; set; }

        public virtual DbSet<FileUploadEntity> FileUploadEntity { get; set; }
        public virtual DbSet<FileOwnerEntity> FileOwnerEntity { get; set; }

        public virtual DbSet<TagEntity> TagEntity { get; set; }
        public virtual DbSet<TagMapEntity> TagMapEntity { get; set; }

        public virtual DbSet<MenuEntity> MenuEntity { get; set; }

        public virtual DbSet<KVStoreEntity> KVStoreEntity { get; set; }
    }
}
