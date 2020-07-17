using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace WCloud.Member.DataAccess.EF
{
    public static class MemberDataAccessExtension
    {
        public static string GetMemberShipConnectionStringOrThrow(this IConfiguration config)
        {
            var constr = config.GetConnectionString("db_member_ship");
            constr.Should().NotBeNullOrEmpty();
            return constr;
        }
    }
}
