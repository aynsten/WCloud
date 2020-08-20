using Lib.core;
using System;

namespace WCloud.Core.Cache
{
    public class CacheException : BaseException
    {
        public CacheException(string msg, Exception e = null) : base(msg, e) { }
    }
}
