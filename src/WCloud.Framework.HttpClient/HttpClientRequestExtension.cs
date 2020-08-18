using Lib.helper;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WCloud.Framework.HttpClient_
{
    public static class HttpClientRequestExtension
    {
        /// <summary>
        /// 提交post请求
        /// </summary>
        public static async Task<string> PostAsync(this HttpClient client,
            string url, IDictionary<string, string> param,
            TimeSpan? timeout = null)
        {
            var u = new Uri(url);
            if (timeout != null)
                client.Timeout = timeout.Value;
            using (var formContent = new MultipartFormDataContent())
            {
                if (ValidateHelper.IsNotEmpty(param))
                {
                    formContent.AddParam_(param);
                }
                using (var response = await client.PostAsync(u, formContent))
                    return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<string> GetAsync(this HttpClient client,
            string url,
            TimeSpan? timeout = null)
        {
            var u = new Uri(url);
            if (timeout != null)
                client.Timeout = timeout.Value;
            using (var response = await client.GetAsync(u))
                return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PostAsJsonAsync_<T>(this HttpClient client,
            string url, T data)
        {
            var u = new Uri(url);
            using (var response = await client.PostAsync(u, new JsonContent<T>(data)))
                return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<byte[]> DownloadBytes(this HttpClient client,
            string url)
        {
            using (var res = await client.GetAsync(url))
            {
                return await res.Content.ReadAsByteArrayAsync();
            }
        }
    }
}
