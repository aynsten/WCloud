using System;
using WCloud.Framework.Database.Abstractions.Entity;

namespace WCloud.Member.Domain
{
    public interface ILoginEntity : ILogicalDeletion, IUpdateTime
    {
        string UID { get; set; }
        string UserName { get; set; }
        string PassWord { get; set; }
        DateTime? LastPasswordUpdateTimeUtc { get; set; }
    }
}
