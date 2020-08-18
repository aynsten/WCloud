using System.Threading.Tasks;

namespace WCloud.Core.Helper
{
    public interface IDataInitializeHelper : Microsoft.Extensions.DependencyInjection.IAutoRegistered
    {
        Task CreateDatabase();

        Task InitSeedData();
    }
}
