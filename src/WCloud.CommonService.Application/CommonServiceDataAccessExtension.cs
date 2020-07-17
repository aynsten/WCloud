using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace WCloud.CommonService.Application
{
    public static class CommonServiceDataAccessExtension
    {

        public static string GetCommonServiceConnectionStringOrThrow(this IConfiguration config)
        {
            var constr = config.GetConnectionString("db_common_service");
            constr.Should().NotBeNullOrEmpty();
            return constr;
        }
    }
}
