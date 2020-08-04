using Microsoft.Extensions.DependencyInjection;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.CommonService.Application.Message
{
    public interface IUserMessageService : IBasicService<UserMessageEntity>, IAutoRegistered
    {
    }
}
