using Lib.core;
using System;

namespace Lib.cache
{
    public class CacheException : BaseException
    {
        public CacheException(string msg, Exception e = null) : base(msg, e) { }
    }
}
