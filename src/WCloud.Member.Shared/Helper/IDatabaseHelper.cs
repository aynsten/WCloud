using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WCloud.Member.Shared.Helper
{
    public interface IDatabaseHelper : IAutoRegistered
    {
        Task CreateDatabase();
    }
}
