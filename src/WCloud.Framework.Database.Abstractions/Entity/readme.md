
``` c#
    /// <summary>
    /// 搜索邮箱有资料
    /// 【EF使用乐观锁，并发token&行版本】并发标记
    /// https://docs.microsoft.com/zh-cn/ef/core/modeling/concurrency
    /// </summary>
    [Obsolete("不要使用")]
    class RowVersionTest : BaseEntity
    {
        /// <summary>
        /// where iid=@id and name=@name
        /// </summary>
        [ConcurrencyCheck]
        public virtual string Name { get; set; }

        /// <summary>
        /// update xx set rowversion=@newrowversion where iid=@id and name=@name
        /// 
        /// sqlserver 通常使用byte[]
        /// 但是不一定要使用byte[]，使用自动生成字段+ConcurrencyCheck也行
        /// </summary>
        [Timestamp]
        public virtual byte[] RowVersion { get; set; }
    }
```

``` c#
    /// <summary>
    /// 搜索邮箱：
    /// 使用rrule
    /// 【终极解决方案】calendar周期性事件，可以高效查询的数据库设计和存储方案
    /// </summary>
    [Obsolete("使用rrule")]
    class CalendarEventTimeEntity : BaseEntity
    {
        public virtual string RRule { get; set; }

        public virtual DateTime Start { get; set; }

        public virtual DateTime? End { get; set; }
    }
```
