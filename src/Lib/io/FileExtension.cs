using Lib.core;
using Lib.helper;
using System.IO;

namespace Lib.io
{
    public static class FileExtension
    {
        /// <summary>
        /// 读取字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);
            var bs = new byte[s.Length];
            s.Read(bs, 0, bs.Length);

            return bs;
        }

        /// <summary>
        /// 获取文件MD5
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetMD5(this FileInfo file)
        {
            if (!file.Exists)
                throw new SourceNotExistException($"无法读取MD5，文件{file.FullName}不存在");

            return SecureHelper.GetFileMD5(file.FullName);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="file"></param>
        public static void DeleteIfExist(this FileInfo file)
        {
            if (file.Exists)
                file.Delete();
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dir"></param>
        public static void CreateIfNotExist(this DirectoryInfo dir)
        {
            if (!dir.Exists)
                dir.Create();
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        public static void DeleteIfExist(this DirectoryInfo dir, bool recursive = true)
        {
            if (dir.Exists)
                dir.Delete(recursive: recursive);
        }

    }
}
