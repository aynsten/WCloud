using System;
using System.Text;

namespace Lib.core
{
    /// <summary>
    /// 加载当前上下文项目config文件中的配置
    /// </summary>
    [Serializable]
    public class ConfigHelper
    {
        static readonly ConfigHelper _ins = new ConfigHelper();

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static ConfigHelper Instance => _ins;

        public Encoding SystemEncoding => Encoding.UTF8;

        public Encoding EncodingOrDefault(Encoding encoding) => encoding ?? this.SystemEncoding;

        public bool LoadPlugin => false;
    }
}
