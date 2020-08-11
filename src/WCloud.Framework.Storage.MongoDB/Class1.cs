#if DEBUG
using System;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace WCloud.Framework.Storage.MongoDB
{
    public class Class1
    {
        public Class1()
        {
            IGridFSBucket bucket = null;
            ObjectId id = new ObjectId();
            var bytes = bucket.DownloadAsBytes(id);
        }
    }
}

#endif