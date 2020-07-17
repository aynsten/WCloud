using Dapper;
using FluentAssertions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WCloud.Framework.Database.Abstractions.Model;

namespace WCloud.Framework.Database.Dapper
{
    public static class MySQLExtension
    {
        public static IEnumerable<MySQLTableStructureModel> __QueryMySqlTableStructure__(this IDbConnection con,
            string db_name, int max_count = 50000)
        {
            db_name.Should().NotBeNullOrEmpty();

            var sql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=@db_name LIMIT @max_count";

            var res = con.Query<MySQLTableStructureModel>(sql, new { db_name, max_count }).ToArray();

            return res;
        }
    }
}
