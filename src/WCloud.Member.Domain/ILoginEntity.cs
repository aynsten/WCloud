using System;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain
{
    public interface ILoginEntity : ILogicalDeletion, IUpdateTime
    {
        string Id { get; }
        string UserName { get; set; }
        string PassWord { get; set; }
        DateTime? LastPasswordUpdateTimeUtc { get; set; }
    }
}
