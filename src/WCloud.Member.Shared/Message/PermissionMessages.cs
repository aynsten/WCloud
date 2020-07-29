using WCloud.Core;
using WCloud.Core.MessageBus;

namespace WCloud.Member.Shared.MessageBody
{
    /// <summary>
    /// 角色权限绑定修改
    /// </summary>
    [QueueConfig(ConfigSet.MessageBus.Queue.Admin)]
    public class RolePermissionUpdatedMessage: IMessageBody
    {
        public string RoleUID { get; set; }
    }

    /// <summary>
    /// 用户角色绑定修改
    /// </summary>
    [QueueConfig(ConfigSet.MessageBus.Queue.Admin)]
    public class UserRoleChangedMessage: IMessageBody
    {
        public string UserUID { get; set; }
    }
}
