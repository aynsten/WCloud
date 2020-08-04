using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public class NotRegException : Exception
    {
        public NotRegException(string msg) : base(msg) { }
    }

    public class RepeatRegException : Exception
    {
        public RepeatRegException(string msg) : base(msg) { }

        public Dictionary<string, string[]> BadReg { get; set; }
    }
}
