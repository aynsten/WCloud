using Lib.data;
using Lib.helper;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace WCloud.Framework.Database.Abstractions.Entity
{
    public interface IBaseDto : IEntityDto { }

    public interface IDto : IEntityDto<int>
    {
        //
    }

    public abstract class BaseDto : IDto, IBaseDto
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class BaseEntity : Entity<int>, IDBTable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override int Id { get; protected set; } = default;

        public virtual string UID { get; set; }

        public virtual DateTime CreateTimeUtc { get; set; }

        /// <summary>
        /// 第一次写库初始化
        /// </summary>
        [Obsolete]
        public virtual void Init(string flag = null, DateTime? utc_time = null)
        {
            var time = utc_time ?? DateTime.UtcNow;

            var uid = Com.GetUUID();
            this.UID = ValidateHelper.IsEmpty(flag) ? uid : $"{flag}-{uid}";

            this.CreateTimeUtc = time;
        }

        public override bool Equals(object obj)
        {
            var other = obj as BaseEntity;
            if (other is null)
            {
                return false;
            }
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            var typeOfThis = this.GetType();
            var typeOfOther = other.GetType();
            if (!(typeOfThis.IsAssignableFrom(typeOfOther) || typeOfOther.IsAssignableFrom(typeOfThis)))
            {
                return false;
            }

            return this.UID == other.UID && (this.UID != default);
        }

        public override int GetHashCode()
        {
            if (this.UID == null)
            {
                return default;
            }
            else
            {
                return this.UID.GetHashCode();
            }
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            if (object.Equals(x, null))
            {
                return object.Equals(y, null);
            }
            else
            {
                return x.Equals(y);
            }
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }
    }
}
