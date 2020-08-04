using Lib.extension;
using System;

namespace WCloud.Framework.Zookeeper.ServiceManager
{
    public static class ServiceManageHelper
    {
        /*
        public static Policy RetryAsyncPolicy() =>
            Policy.Handle<Exception>().WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(i * 100));
            */

        public static string ParseServiceName<T>() => ParseServiceName(typeof(T));

        public static string ParseServiceName(Type t) => $"{t.FullName}".RemoveWhitespace();

        public static string EndpointNodeName(string node_id) => $"node_{node_id}";
    }
}
