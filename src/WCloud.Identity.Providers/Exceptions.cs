using System;
using System.Collections.Generic;
using System.Text;
using Lib.core;

namespace WCloud.Identity.Providers
{
    public class GrantException : BaseException
    {
        public GrantException(string msg) : base(msg) { }
    }
}
