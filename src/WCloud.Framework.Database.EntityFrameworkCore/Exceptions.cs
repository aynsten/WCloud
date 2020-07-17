using System;
using System.Collections.Generic;
using System.Text;
using Lib.core;

namespace WCloud.Framework.Database.EntityFrameworkCore
{
    public class UnSubmitChangesException : MsgException
    {
        public UnSubmitChangesException(string msg) : base(msg) { }
    }

    public class DataNotFoundException : MsgException
    {
        public DataNotFoundException(string msg) : base(msg) { }
    }
}
