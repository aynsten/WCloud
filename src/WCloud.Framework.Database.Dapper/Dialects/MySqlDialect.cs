using System;
using System.Collections.Generic;
using System.Linq;

namespace WCloud.Framework.Database.Dapper.Dialects
{
    public class MySqlDialect : ISqlDialect
    {
        public string Compare(string field_1, string _operator, string field_2)
        {
            return $"`{field_1}` {_operator} @{field_1}";
        }

        public string Limit(int? skip, int? take)
        {
            throw new NotImplementedException();
        }

        public string OrderBy(IDictionary<string, string> sort)
        {
            var sort_params = sort.Select(x => $"{x.Key} {x.Value}");
            return $"ORDER BY {string.Join(",", sort_params)}";
        }

        public string SelectFields(string[] fields)
        {
            throw new NotImplementedException();
        }
    }
}
