using System.Threading.Tasks;

namespace WCloud.Framework.Storage
{
    public interface IUploadHelper
    {
        string DebugInfo();
        string StorageProvider { get; }

        /// <summary>
        /// 删除七牛的文件
        /// </summary>
        /// <param name="key"></param>
        Task Delete(string key);

        /// <summary>
        /// 删除七牛的文件
        /// </summary>
        /// <param name="key"></param>
        Task<bool> KeyExists(string key);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="saveKey"></param>
        /// <returns></returns>
        Task<string> Upload(byte[] bs, string key);

        /// <summary>
        /// 获取文件地址http://www.domain.com/file-qiniu-key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string ConcatUrl(string key);
    }
}
