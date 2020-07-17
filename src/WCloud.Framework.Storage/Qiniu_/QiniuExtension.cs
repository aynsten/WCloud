using Lib.extension;
using Qiniu.Http;
using Qiniu.IO.Model;
using Qiniu.Util;
using System;

namespace WCloud.Framework.Storage.Qiniu_
{
    public static class QiniuExtension
    {
        /// <summary>
        /// 有异常就抛出
        /// </summary>
        /// <param name="res"></param>
        public static T ThrowIfException<T>(this T res) where T : HttpResult
        {
            if (res.Code != 200)
            {
                throw new QiniuException($"七牛服务器错误，返回数据：{res.ToJson()}");
            }
            return res;
        }

        public static string CreateUploadToken(this Mac mac, string bucket)
        {
            // 上传策略
            var putPolicy = new PutPolicy();
            // 设置要上传的目标空间
            putPolicy.Scope = bucket ?? throw new ArgumentNullException(nameof(bucket));
            putPolicy.SetExpires(3600);

            // 生成上传凭证
            var uploadToken = new Qiniu.Util.Auth(mac).CreateUploadToken(putPolicy.ToJsonString());
            return uploadToken;
        }
    }
}
