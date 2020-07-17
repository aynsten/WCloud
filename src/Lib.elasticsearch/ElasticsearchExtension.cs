using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Lib.extension;
using Lib.helper;
using Nest;

namespace Lib.elasticsearch
{
    public static class ElasticsearchExtension
    {
        /// <summary>
        /// 如果有错误就抛出异常
        /// </summary>
        public static T ThrowIfException<T>(this T response, string data = null) where T : IResponse
        {
            if (!response.IsValid)
            {
                var msg = new
                {
                    extra_data = data,
                    debug = response.DebugInformation,
                    error = response.ServerError
                }.ToJson();

                var inner = response.OriginalException ?? new ResponseException($"{nameof(response.OriginalException)} is empty");
                throw new ResponseException(msg, inner);
            }
            return response;
        }

        /// <summary>
        /// 如果有错误就抛出异常
        /// </summary>
        public static T BulkResponseThrowIfException<T>(this T response, int includeErrorCount = 30) where T : BulkResponse
        {
            if (response.ItemsWithErrors.Any())
            {
                var json = response.ItemsWithErrors.Take(includeErrorCount).ToJson();
                throw new BulkException($"部分数据未能成功更新到ES，这里最多展示{includeErrorCount}条：{json}");
            }
            return response;
        }

        /// <summary>
        /// 开启链接调试
        /// </summary>
        public static ConnectionSettings EnableDebug(this ConnectionSettings setting) =>
            setting.DisableDirectStreaming(true);

        /// <summary>
        /// 记录请求信息
        /// </summary>
        /// <param name="pool"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static ConnectionSettings LogRequestInfo(this ConnectionSettings pool, Action<IApiCallDetails> callback)
        {
            callback = callback ?? throw new ArgumentNullException(nameof(callback));

            return pool.OnRequestCompleted(callback);
        }

        public static bool IsValidLatitude(this double lat) => GeoLocation.IsValidLatitude(lat);

        public static bool IsValidLongitude(this double lng) => GeoLocation.IsValidLongitude(lng);

        /// <summary>
        /// 创建client
        /// </summary>
        /// <param name="pool"></param>
        /// <returns></returns>
        public static IElasticClient CreateClient(this ConnectionSettings pool) => new ElasticClient(pool);

        /// <summary>
        /// 唯一ID
        /// </summary>
        public static DocumentPath<T> ID<T>(this IElasticClient client, string indexName, string uid)
            where T : class, IESIndex
            => DocumentPath<T>.Id(uid).Index(indexName);


    }

}
