using FluentAssertions;
using Lib.extension;
using Polly;
using Qiniu.IO;
using Qiniu.RS;
using Qiniu.RS.Model;
using Qiniu.RSF;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WCloud.Framework.Storage.Qiniu_
{
    /// <summary>
    /// http://developer.qiniu.com/resource/official.html#sdk
    /// https://developer.qiniu.com/kodo/api/3928/error-responses
    /// </summary>
    public class QiniuHelper : IQiniuHelper
    {
        private readonly Mac _mac;
        private readonly QiniuOption _option;

        public QiniuOption Option => this._option;

        public string StorageProvider => "qiniu";

        public string DebugInfo()
        {
            return new
            {
                this.Option.QiniuBucketName,
                this.Option.QiniuBaseUrl
            }.ToJson();
        }

        public QiniuHelper(QiniuOption option)
        {
            option.Should().NotBeNull();
            option.QiniuAccessKey.Should().NotBeNullOrEmpty();
            option.QiniuSecretKey.Should().NotBeNullOrEmpty();
            option.QiniuBaseUrl.Should().NotBeNullOrEmpty();
            option.QiniuBucketName.Should().NotBeNullOrEmpty();

            this._option = option;
            this._mac = new Mac(this._option.QiniuAccessKey, this._option.QiniuSecretKey);
        }

        void CheckSaveKey(string key)
        {
            key.Should().NotBeNullOrEmpty();
            key.Trim(' ', '/', '\\', '?', '!', '@', '*', '$').Length.Should().Be(key.Length, "包含非法字符");
        }

        public virtual async Task<StatResult> FindEntry(string key)
        {
            this.CheckSaveKey(key);

            var bm = new BucketManager(this._mac);
            // 返回结果存储在result中
            var res = await bm.StatAsync(this._option.QiniuBucketName, key);
            //这里不要throw if exception。因为如果文件不存在success就是false
            return res;
        }

        public virtual async Task Delete(string key)
        {
            this.CheckSaveKey(key);

            var bm = new BucketManager(this._mac);
            // 返回结果存储在result中
            var res = await bm.DeleteAsync(this._option.QiniuBucketName, key);
            res.ThrowIfException();
        }

        public virtual async Task<string> Upload(string localFile, string key)
        {
            this.CheckSaveKey(key);

            // 开始上传文件
            var um = new UploadManager();
            var token = this._mac.CreateUploadToken(this._option.QiniuBucketName);

            var res = await um.UploadFileAsync(localFile, key, token);
            res.ThrowIfException();

            return this.ConcatUrl(key);
        }

        public virtual async Task<string> Upload(byte[] bs, string key)
        {
            this.CheckSaveKey(key);

            // 开始上传文件
            var um = new UploadManager();
            var token = this._mac.CreateUploadToken(this._option.QiniuBucketName);

            var res = await um.UploadDataAsync(bs, key, token);
            res.ThrowIfException();

            return this.ConcatUrl(key);
        }

        public virtual string ConcatUrl(string key)
        {
            this.CheckSaveKey(key);

            var res = $"{this._option.QiniuBaseUrl.TrimEnd('/', '\\')}/{key}";
            return res;
        }

        public virtual string WrapAccessToken(string url, TimeSpan expired)
        {
            url.Should().NotBeNullOrEmpty();
            new string[] { "http://", "https://" }.Any(x => url.ToLower().StartsWith(x)).Should().BeTrue();

            var accUrl = DownloadManager.CreateSignedUrl(this._mac, url, (int)expired.TotalSeconds);

            return accUrl;
        }

        public virtual string RemoveExif(string key)
        {
            this.CheckSaveKey(key);

            var manager = new OperationManager(this._mac);

            var res = manager.Pfop(this._option.QiniuBucketName,
                key,
                "imageMogr2/auto-orient/strip|saveas/" + Base64.UrlSafeBase64Encode($"{this._option.QiniuBucketName}:{key}"),
                null, null, true);
            res.ThrowIfException();

            return key;
        }

        public virtual IEnumerable<IEnumerable<FileDesc>> ListFiles(Func<bool> stop, uint retry_times = 1)
        {
            stop.Should().NotBeNull();
            retry_times.Should().BeGreaterOrEqualTo(1);

            // 这个示例单独使用了一个Settings类，其中包含AccessKey和SecretKey
            // 实际应用中，请自行设置您的AccessKey和SecretKey
            string marker = string.Empty; // 首次请求时marker必须为空
            string prefix = null; // 按文件名前缀保留搜索结果
            string delimiter = null; // 目录分割字符(比如"/")
            var limit = 512; // 单次列举数量限制(最大值为1000)
            var bm = new BucketManager(this._mac);
            var retry = Policy.Handle<Exception>().Retry((int)retry_times);
            do
            {
                var data = retry.Execute(() =>
                {
                    var res = bm.ListFiles(this._option.QiniuBucketName, prefix, marker, limit, delimiter);

                    res.ThrowIfException();

                    //获取新的游标
                    marker = res.Result.Marker;

                    return res.Result.Items;
                });

                if (data?.Any() ?? false)
                    yield return data;

            } while (!string.IsNullOrEmpty(marker) && !stop.Invoke());
        }

        public async Task<bool> KeyExists(string key)
        {
            var res = await this.FindEntry(key);

            return res.Code == 200;
        }
    }
}
