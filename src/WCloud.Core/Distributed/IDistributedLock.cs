using System;
using System.Threading.Tasks;

namespace WCloud.Core.Distributed
{
    public interface IDistributedLock : IDisposable
    {
        Task LockOrThrow();
        Task ReleaseLock();
    }
}
