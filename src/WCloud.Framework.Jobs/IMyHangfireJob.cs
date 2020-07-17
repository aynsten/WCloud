using System.Threading.Tasks;

namespace WCloud.Framework.Jobs
{
    public interface IMyHangfireJob
    {
        Task ExecuteAsync();
    }
}
