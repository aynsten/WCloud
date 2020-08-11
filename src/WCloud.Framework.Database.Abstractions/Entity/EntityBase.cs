using Lib.data;
using Lib.helper;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace WCloud.Framework.Database.Abstractions.Entity
{
    public interface IDtoBase : IEntityDto { }

    public interface IDto : IEntityDto<string>
    {
        //
    }

    public abstract class DtoBase : IDto, IDtoBase
    {
        public string Id { get; set; }
    }

    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class EntityBase : Entity<string>, IDBTable
    {
        protected EntityBase() { }
        protected EntityBase(string id) : base(id) { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override string Id { get; protected set; }
        public virtual void SetId(string _id)
        {
            this.Id = _id;
        }

        public virtual DateTime CreateTimeUtc { get; set; }

        /// <summary>
        /// 第一次写库初始化
        /// </summary>
        [Obsolete]
        public virtual void Init(string flag = null, DateTime? utc_time = null)
        {
            var time = utc_time ?? DateTime.UtcNow;

            var uid = Com.GetUUID();
            this.Id = ValidateHelper.IsEmpty(flag) ? uid : $"{flag}-{uid}";

            this.CreateTimeUtc = time;
        }

        public override bool Equals(object obj)
        {
            var other = obj as EntityBase;
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

            return this.Id == other.Id && (this.Id != default);
        }

        public override int GetHashCode()
        {
            if (this.Id == null)
            {
                return default;
            }
            else
            {
                return this.Id.GetHashCode();
            }
        }

        public static bool operator ==(EntityBase x, EntityBase y)
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

        public static bool operator !=(EntityBase x, EntityBase y)
        {
            return !(x == y);
        }
    }
}
