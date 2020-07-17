using System;
using System.Collections.Generic;
using System.Text;

namespace WCloud.Framework.Database.Dapper.Expressions
{
    public class ExpressionParsedResult
    {
        public string SQL { get; set; }

        public Dictionary<string, object> Params { get; set; }

        public List<string> TargetFields { get; set; }

        public Dictionary<string, string> Joins { get; set; }
    }
}
