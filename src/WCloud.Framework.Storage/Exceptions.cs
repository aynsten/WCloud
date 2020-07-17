using System;

namespace WCloud.Framework.Storage.Qiniu_
{
    public class QiniuException : Exception
    {
        public QiniuException(string msg) : base(msg) { }
    }
}
