using Microsoft.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore;
using WCloud.Framework.Database.EntityFrameworkCore.AbpDatabase;
using WCloud.MetroAd.Event;
using WCloud.MetroAd.Finance;
using WCloud.MetroAd.Metro;
using WCloud.MetroAd.Order;
using WCloud.MetroAd.Showcase;
using WCloud.MetroAd.Statistic;

namespace WCloud.MetroAd
{
    public class MetroAdDbContext : AbpDbContextBase<MetroAdDbContext>
    {
        public MetroAdDbContext(DbContextOptions<MetroAdDbContext> option) : base(option)
        {
            //
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.AutoApplyEntityConfigurationForMySQL<MetroAdDbContext, IMetroAdTable>(modelBuilder);
        }

        public virtual DbSet<OperationLogEntity> OperationLogEntity { get; set; }
        public virtual DbSet<AdWindowUsageEntity> AdWindowUsageEntity { get; set; }

        public virtual DbSet<FinanceFlowEntity> FinanceFlowEntity { get; set; }

        public virtual DbSet<AdWindowEntity> AdWindowEntity { get; set; }
        public virtual DbSet<MediaTypeEntity> MediaTypeEntity { get; set; }
        public virtual DbSet<MetroLineEntity> MetroLineEntity { get; set; }
        public virtual DbSet<MetroStationEntity> MetroStationEntity { get; set; }

        public virtual DbSet<OrderEntity> OrderEntity { get; set; }
        public virtual DbSet<OrderItemEntity> OrderItemEntity { get; set; }
        public virtual DbSet<DesignImageEntity> DesignImageEntity { get; set; }
        public virtual DbSet<DeployEntity> DeployEntity { get; set; }
        public virtual DbSet<OrderHistoryEntity> OrderHistoryEntity { get; set; }
        public virtual DbSet<PaymentNotificationEntity> PaymentNotificationEntity { get; set; }

        public virtual DbSet<CaseEntity> CaseEntity { get; set; }
    }
}
