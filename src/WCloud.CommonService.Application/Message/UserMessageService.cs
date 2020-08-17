using FluentAssertions;
using System;
using System.Linq;
using WCloud.Framework.Database.Abstractions.Service;

namespace WCloud.CommonService.Application.Message
{
    public class UserMessageService : BasicService<UserMessageEntity>, IUserMessageService
    {
        public UserMessageService(IServiceProvider provider, ICommonServiceRepository<UserMessageEntity> repo) : base(provider, repo) { }

        protected override IQueryable<UserMessageEntity> SearchKeyword(IQueryable<UserMessageEntity> query, string q)
        {
            return query;
        }

        protected override object UpdateField(UserMessageEntity data)
        {
            return new
            {
                data.AlreadyRead,
                data.ReadTimeUtc
            };
        }

        protected override void ValidModel(UserMessageEntity model)
        {
            model.Should().NotBeNull("user message model");
            model.FromUID.Should().NotBeNullOrEmpty("user message from uid");
            model.UserUID.Should().NotBeNullOrEmpty("user message user uid");
            model.Message.Should().NotBeNullOrEmpty("user message meassage");
        }
    }
}
