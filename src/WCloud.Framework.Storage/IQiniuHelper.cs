using Qiniu.RS.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCloud.Framework.Storage.Qiniu_
{
    /// <summary>
    /// https://github.com/FoundatioFx/Foundatio
    /// </summary>
    public interface IQiniuHelper:IUploadHelper
    {
        QiniuOption Option { get; }

        /// <summary>
        /// 获取七牛的文件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<StatResult> FindEntry(string key);

        /// <summary>
        /// 上传文件到qiniu，返回访问链接
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Task<string> Upload(string localFile, string key);

        /// <summary>
        /// 为私有文件添加访问token
        /// </summary>
        /// <param name="saveKey"></param>
        /// <returns></returns>
        string WrapAccessToken(string url, TimeSpan expired);

        /// <summary>
        /// 去除图片exif信息，比如[edited by photoshop]
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string RemoveExif(string key);

        /// <summary>
        /// 通过游标的方式历遍七牛所有文件
        /// </summary>
        /// <param name="stop"></param>
        /// <param name="retry_times"></param>
        /// <returns></returns>
        IEnumerable<IEnumerable<FileDesc>> ListFiles(Func<bool> stop, uint retry_times = 1);
    }
}
