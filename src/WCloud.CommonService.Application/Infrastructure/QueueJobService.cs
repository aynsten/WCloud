using FluentAssertions;
using System;
using WCloud.Framework.Database.EntityFrameworkCore.Service;

namespace WCloud.CommonService.Application.Infrastructure
{
    public class QueueJobService : BasicService<QueueJobEntity>, IQueueJobService
    {
        public QueueJobService(IServiceProvider provider, ICommonServiceRepository<QueueJobEntity> repo) : base(provider, repo)
        {
        }

        protected override object UpdateField(QueueJobEntity data)
        {
            return new
            {
                data.Status,
                data.StartTimeUtc,
                data.EndTimeUtc,
                data.ExceptionMessage,
                data.ExtraData
            };
        }

        protected override void ValidModel(QueueJobEntity model)
        {
            model.Should().NotBeNull("queue entity");
            model.JobKey.Should().NotBeNullOrEmpty("queue job key");
        }
    }
}
