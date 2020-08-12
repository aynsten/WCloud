using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Linq;
using System.Threading.Tasks;
using WCloud.Core.Storage;
using WCloud.Framework.Database.MongoDB;

namespace WCloud.Framework.Storage.MongoDB
{
    /*
    Files uploaded to GridFS are identified either by Id or by Filename.
    Each uploaded file is assigned a unique Id of type ObjectId. 
    If multiple files are uploaded to GridFS with the same Filename,
    they are considered to be “revisions” of the same file, 
    and the UploadDateTime is used to decide whether one revision is newer than another.
     */

    /// <summary>
    /// https://mongodb.github.io/mongo-csharp-driver/2.10/reference/gridfs/uploadingfiles/#uploading-files
    /// object id 和 文件名都用来确定文件，多个同名文件讲被视为不同版本
    /// </summary>
    public class MongoDBUploadProvider : IUploadHelper
    {
        public string StorageProvider => "mongodb";

        private readonly IGridFSBucket bucket;
        public MongoDBUploadProvider(MongoConnectionWrapper wrapper, MongoDBStorageOption option)
        {
            option.Should().NotBeNull();
            var db = wrapper.Client.GetDatabase(wrapper.DatabaseName);
            var setting = new GridFSBucketOptions
            {
                BucketName = option.Bucket,
                //ChunkSizeBytes = 1048576, // 1MB
                WriteConcern = WriteConcern.WMajority,
                ReadPreference = ReadPreference.Secondary
            };
            this.bucket = new GridFSBucket(db, setting);
        }

        public string ConcatUrl(string key)
        {
            var res = $"{key}";
            return res;
        }

        public string DebugInfo()
        {
            return new
            {

            }.ToJson();
        }

        public async Task Delete(string key)
        {
            key.Should().NotBeNullOrEmpty();

            var filter = Builders<GridFSFileInfo>.Filter.Where(x => x.Filename == key);

            while (true)
            {
                var find_option = new GridFSFindOptions()
                {
                    Skip = 0,
                    Limit = 100,
                };
                using var res = await this.bucket.FindAsync(filter, find_option);
                var list = await res.ToListAsync();
                if (!list.Any())
                    break;

                foreach (var m in list)
                    await this.bucket.DeleteAsync(m.Id);
            }
        }

        public async Task<bool> KeyExists(string key)
        {
            key.Should().NotBeNullOrEmpty();
            var filter = (FilterDefinition<GridFSFileInfo>)(x => x.Filename == key);
            var find_option = new GridFSFindOptions()
            {
                Skip = 0,
                Limit = 1,
                Sort = Builders<GridFSFileInfo>.Sort.Descending(x => x.UploadDateTime)
            };
            using var res = await this.bucket.FindAsync(filter, find_option);
            return res.Any();
        }

        public async Task<string> Upload(byte[] bs, string key)
        {
            bs.Should().NotBeNullOrEmpty();
            key.Should().NotBeNullOrEmpty();

            var id = await this.bucket.UploadFromBytesAsync(key, bs);

            return this.ConcatUrl(key);
        }
    }
}
