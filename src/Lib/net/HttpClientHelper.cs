using Lib.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Lib.net
{
    /// <summary>
    /// http请求
    /// </summary>
    public static class HttpClientHelper
    {
        private static readonly IReadOnlyDictionary<string, string> _extension_map = new Dictionary<string, string>()
        {
            ["jpg"] = ".jpg",
            ["jpeg"] = ".jpg",
            ["png"] = ".png",
            ["gif"] = ".gif",
            ["bmp"] = ".bmp",
            ["mp3"] = ".mp3",
            ["mp4"] = ".mp4",
            ["exe"] = ".exe",
        };

        /// <summary>
        /// 通过contentTYPE获取文件格式
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetExtByContentType(string extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            var t = extension.ToLower();

            foreach (var kv in _extension_map)
                if (t.Contains(kv.Key))
                    return kv.Value;

            return extension;
        }

        /// <summary>
        /// 返回描述本地计算机上的网络接口的对象(网络接口也称为网络适配器)。
        /// </summary>
        /// <returns></returns>
        public static NetworkInterface[] NetCardInfo() => NetworkInterface.GetAllNetworkInterfaces();

        ///<summary>
        /// 通过NetworkInterface读取网卡Mac
        ///</summary>
        ///<returns></returns>
        public static string[] GetMacByNetworkInterface() => NetCardInfo()
            .Select(x => x.GetPhysicalAddress()?.ToString())
            .Where(x => ValidateHelper.IsNotEmpty(x))
            .ToArray();

    }
}