using Lib.core;
using System;

namespace Lib.akka
{
    public class AlreadyExistException : BaseException
    {
        public AlreadyExistException(string msg, Exception e = null) : base(msg, e) { }
    }
}
