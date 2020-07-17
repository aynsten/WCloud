using System.Collections.Generic;
using Lib.core;

namespace WCloud.Framework.MessageBus
{
    public class RepeatConsumeException : BaseException
    {
        public Dictionary<string, string[]> Errors { get; set; }

        public RepeatConsumeException(string msg) : base(msg) { }
    }
}
