
namespace WCloud.Framework.Storage.Qiniu_
{
    public class QiniuOption
    {
        public string QiniuAccessKey { get; set; }

        public string QiniuSecretKey { get; set; }

        public string QiniuBucketName { get; set; }

        public string QiniuBaseUrl { get; set; }

        /// <summary>
        /// 存储位置的唯一key
        /// </summary>
        public string ProviderKey { get; set; }
    }
}
