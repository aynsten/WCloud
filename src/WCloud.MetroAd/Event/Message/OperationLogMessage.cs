using FluentAssertions;
using Lib.extension;
using WCloud.Core.Authentication.Model;
using WCloud.Core.MessageBus;
using WCloud.Member.Shared;
using WCloud.MetroAd.Event;

namespace WCloud.Admin.Message
{
    public class OperationLogMessage : OperationLogEntity, IMessageBody
    {
        public OperationLogMessage() { }

        public OperationLogMessage(WCloudAdminInfo loginuser)
        {
            this.UserUID = loginuser.UserID;
            this.UserName = string.Join("-", new[] { loginuser.UserName, loginuser.NickName });
            this.AccountType = (int)AccountTypeEnum.Admin;
        }

        public OperationLogMessage(WCloudUserInfo loginuser)
        {
            this.UserUID = loginuser.UserID;
            this.UserName = string.Join("-", new[] { loginuser.UserName, loginuser.NickName });
            this.AccountType = (int)AccountTypeEnum.User;
        }

        public OperationLogMessage PageAction(string page, string action = null)
        {
            this.Page = page;
            this.Action = action ?? string.Empty;
            return this;
        }

        public OperationLogMessage UpdateOrAdd(string page, bool update)
        {
            return this.PageAction(page, update ? "更新" : "添加");
        }

        public OperationLogMessage Delete(string page)
        {
            return this.PageAction(page, "删除");
        }

        public OperationLogMessage WithMessage(string msg)
        {
            this.Message = msg;
            return this;
        }

        public OperationLogMessage WithExtraData(object data)
        {
            data.Should().NotBeNull();

            this.ExtraDataJson = data.ToJson();
            return this;
        }
    }
}
