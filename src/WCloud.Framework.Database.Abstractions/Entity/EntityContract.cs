
using System;

namespace WCloud.Framework.Database.Abstractions.Entity
{
    /// <summary>
    /// 标记删除
    /// </summary>
    public interface ILogicalDeletion
    {
        int IsDeleted { get; set; }
        //单独加一个流水表用来记录
        //DateTime? DeletedTimeUtc { get; set; }
    }
    /// <summary>
    /// 数据表中存在租户字段
    /// </summary>
    public interface IOrgRow //: Volo.Abp.MultiTenancy.IMultiTenant
    {
        string OrgUID { get; set; }
    }
    public interface IRowVersion
    {
        /// <summary>
        /// https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql/issues/351
        /// </summary>
        public DateTime RowVersion { get; set; }
    }
    public interface ICreateTime
    {
        DateTime CreateTimeUtc { get; set; }
    }
    public interface IUpdateTime
    {
        DateTime? UpdateTimeUtc { get; set; }
    }
    /// <summary>
    /// 自增id
    /// </summary>
    public interface IIncID
    {
        int IncID { get; set; }
    }
}
